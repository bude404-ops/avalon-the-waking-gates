"""Rig with MANUAL distance-based vertex binding (bone-heat auto-weights fail silently on non-manifold AI meshes) + amplified idle + export. Probes actual deformation."""
import bpy, math, time, os
from mathutils import Vector
GLB = "models/generated/AEDAN-GAMEREADY-V1.glb"
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=GLB)
cands = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(cands, key=lambda o: len(o.data.polygons))
for o in cands:
    if o != body and len(o.data.polygons) < 400: bpy.data.objects.remove(o, do_unlink=True)
verts = [body.matrix_world @ v.co for v in body.data.vertices]
zs = [v.z for v in verts]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
def slice_x(zc, band=0.02):
    lo, hi = zmin + (zc-band)*H, zmin + (zc+band)*H
    xs = [abs(v.x) for v in verts if lo <= v.z <= hi]
    return sum(xs)/len(xs) if xs else 0.0
def P(x, h): return Vector((x, 0.0, zmin + h*H))
sh_x = max(slice_x(0.80), 0.08*H); el_x = max(slice_x(0.61), 0.07*H)
wr_x = max(slice_x(0.46), 0.06*H); hip_x = max(slice_x(0.44), 0.06*H)
kn_x = max(slice_x(0.26), 0.06*H); an_x = max(slice_x(0.055), 0.05*H)
arm = bpy.data.armatures.new('AedanRig')
rig = bpy.data.objects.new('AedanRig', arm)
bpy.context.collection.objects.link(rig)
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='EDIT')
eb = arm.edit_bones
BONES = {}
def bone(name, head, tail, parent, connect=True):
    b = eb.new(name); b.head, b.tail = head, tail
    b.parent = eb.get(parent) if parent else None
    b.use_connect = connect and parent is not None
    BONES[name] = (Vector(head), Vector(tail))
bone('hips', P(0,0.47), P(0,0.60), None, False)
bone('spine', P(0,0.60), P(0,0.68), 'hips')
bone('spine1', P(0,0.68), P(0,0.76), 'spine')
bone('spine2', P(0,0.76), P(0,0.83), 'spine1')
bone('neck', P(0,0.83), P(0,0.88), 'spine2')
bone('head', P(0,0.88), P(0,0.97), 'neck')
for s, sx in (('L',1),('R',-1)):
    bone(f'shoulder.{s}', P(sx*sh_x,0.83), P(sx*sh_x,0.80), 'spine2', False)
    bone(f'upper_arm.{s}', P(sx*sh_x,0.80), P(sx*el_x,0.61), f'shoulder.{s}')
    bone(f'forearm.{s}', P(sx*el_x,0.61), P(sx*wr_x,0.46), f'upper_arm.{s}')
    bone(f'hand.{s}', P(sx*wr_x,0.46), P(sx*wr_x,0.37), f'forearm.{s}')
    bone(f'upper_leg.{s}', P(sx*hip_x,0.47), P(sx*kn_x,0.26), 'hips', False)
    bone(f'lower_leg.{s}', P(sx*kn_x,0.26), P(sx*an_x,0.055), f'upper_leg.{s}')
    bone(f'foot.{s}', P(sx*an_x,0.055), P(sx*an_x*0.8,0.005), f'lower_leg.{s}')
bpy.ops.object.mode_set(mode='OBJECT')
# ---- MANUAL DISTANCE BINDING ----
import numpy as np
names = list(BONES.keys())
seg = {n: (np.array(BONES[n][0]), np.array(BONES[n][1])) for n in names}
# long bones deserve wider influence; use per-bone radius
V = np.array([[v.x, v.y, v.z] for v in verts])
nV = len(V)
W = np.zeros((nV, len(names)))
for bi, n in enumerate(names):
    a, b = seg[n]
    ab = b - a
    ab2 = float(ab @ ab)
    t = np.clip(((V - a) @ ab) / ab2, 0.0, 1.0)
    proj = a + t[:, None] * ab
    d2 = np.sum((V - proj)**2, axis=1)
    W[:, bi] = d2
# top-4 nearest bones per vertex, inverse-square weights
k = 4
top = np.argpartition(W, k, axis=1)[:, :k]
wtop = np.take_along_axis(W, top, axis=1)
wtop = 1.0 / (wtop + 1e-6)
wtop /= wtop.sum(axis=1, keepdims=True)
for bi in range(len(names)):
    vg = body.vertex_groups.new(name=names[bi])
vg_list = [body.vertex_groups[n] for n in names]
for vi in range(0, nV, 5000):
    for r in range(5000):
        idx = vi + r
        if idx >= nV: break
        for bi in range(k):
            vg_list[top[idx, bi]].add([idx], float(wtop[idx, bi]), 'REPLACE')
print(f"MANUAL_WEIGHTS: {nV} verts bound to {len(names)} bones (top-4 inv-sq)")
# ---- ARMATURE MODIFIER (no auto weights) ----
am = body.modifiers.new('Armature', 'ARMATURE')
am.object = rig
body.parent = rig
# ---- AMPLIFIED IDLE ----
sc = bpy.context.scene
sc.frame_start, sc.frame_end = 1, 61
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]
    b.rotation_mode = 'XYZ'
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
for f, amp in ((1,0), (31,1), (61,0)):
    kf('spine2', f, rot=(R(4)*amp, 0, 0))
    kf('spine1', f, rot=(R(2)*amp, 0, 0))
    kf('head', f, rot=(R(-1.5)*amp, R(2)*amp, 0))
    kf('upper_arm.L', f, rot=(0, 0, R(-6)*amp))
    kf('upper_arm.R', f, rot=(0, 0, R(6)*amp))
    kf('hips', f, loc=(0, 0, -0.012*H*amp))
bpy.ops.object.mode_set(mode='OBJECT')
# ---- DEFORM PROBE (evaluated mesh, weighted vertex) ----
mw = body.matrix_world
vgs = {g.name: g for g in body.vertex_groups}
vi = 0
for v in body.data.vertices:
    for g in v.groups:
        if body.vertex_groups[g.group].name == "upper_arm.L" and g.weight > 0.5: vi = v.index; break
    else: continue
    break
def eval_co(idx):
    dg = bpy.context.evaluated_depsgraph_get()
    return mw @ body.evaluated_get(dg).data.vertices[idx].co
sc.frame_set(1); c1 = eval_co(vi)
sc.frame_set(31); c31 = eval_co(vi)
print(f"PROBE arm vert rest=({c1.x:.4f},{c1.y:.4f},{c1.z:.4f}) f31=({c31.x:.4f},{c31.y:.4f},{c31.z:.4f}) delta={abs((c31-c1).length):.5f}")
print("RIG_IDLE_BAKED_MANUAL_WEIGHTS")
# ---- EXPORT ----
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.context.scene.objects:
    if o.type in ('MESH','ARMATURE'): o.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-RIGGED-V3.glb", use_selection=True, export_animations=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-RIGGED-V3.fbx", use_selection=True, add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
print("EXPORT_V3_DONE")
