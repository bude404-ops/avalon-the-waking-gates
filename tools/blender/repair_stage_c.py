"""STAGE C: open stage-B blend (solid 110K, smart UV, baked texture) -> rig + chest-led idle -> export V5."""
import bpy, math
from mathutils import Vector
import numpy as np
R = math.radians
bpy.ops.wm.open_mainfile(filepath="/tmp/aedan_v4_stageB.blend")
solid = None
for o in bpy.context.scene.objects:
    if o.type == 'MESH' and o.name != 'ORIG_UV_CARRIER':
        solid = o if solid is None or len(o.data.polygons) > len(solid.data.polygons) else solid
print(f"SOLID faces={len(solid.data.polygons)} uv_layers={len(solid.data.uv_layers)} mats={len(solid.data.materials)}", flush=True)
verts = [solid.matrix_world @ v.co for v in solid.data.vertices]
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
names = list(BONES.keys())
seg = {n: (np.array(BONES[n][0]), np.array(BONES[n][1])) for n in names}
V = np.array([[v.x, v.y, v.z] for v in verts]); nV = len(V)
W = np.zeros((nV, len(names)))
for bi, n in enumerate(names):
    a, b = seg[n]; ab = b - a; ab2 = float(ab @ ab)
    t = np.clip(((V - a) @ ab) / ab2, 0.0, 1.0)
    proj = a + t[:, None] * ab
    W[:, bi] = np.sum((V - proj)**2, axis=1)
k = 4
top = np.argpartition(W, k, axis=1)[:, :k]
wtop = np.take_along_axis(W, top, axis=1)
wtop = 1.0 / (wtop + 1e-6); wtop /= wtop.sum(axis=1, keepdims=True)
for n in names: solid.vertex_groups.new(name=n)
vg_list = [solid.vertex_groups[n] for n in names]
for vi in range(0, nV, 5000):
    for r in range(5000):
        idx = vi + r
        if idx >= nV: break
        for bi in range(k):
            vg_list[top[idx, bi]].add([idx], float(wtop[idx, bi]), 'REPLACE')
print(f"MANUAL_WEIGHTS: {nV} verts", flush=True)
am = solid.modifiers.new('Armature', 'ARMATURE'); am.object = rig
solid.parent = rig
# CHEST-LED IDLE (V4 doctrine)
sc = bpy.context.scene
sc.frame_start, sc.frame_end = 1, 61
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]; b.rotation_mode = 'XYZ'
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
for f, amp in ((1,0), (31,1), (61,0)):
    kf('spine2', f, rot=(R(4.5)*amp, 0, 0))
    kf('spine1', f, rot=(R(1.2)*amp, 0, 0))
    kf('spine',  f, rot=(R(0)*amp, 0, 0))
    kf('hips',   f, rot=(0,0,0), loc=(0,0,0))
    kf('head',   f, rot=(R(-1.5)*amp, R(1.2)*amp, 0))
    kf('upper_arm.L', f, rot=(0, 0, R(-3)*amp))
    kf('upper_arm.R', f, rot=(0, 0, R(3)*amp))
bpy.ops.object.mode_set(mode='OBJECT')
print("IDLE chest-led applied", flush=True)
# probes: chest vert vs belly vert vs groin vert
mw = solid.matrix_world
dg = bpy.context.evaluated_depsgraph_get()
def eval_co(idx):
    sc.frame_set(sc.frame_current) if False else None
    dg = bpy.context.evaluated_depsgraph_get()
    return mw @ solid.evaluated_get(dg).data.vertices[idx].co
vgs = solid.vertex_groups
def find_vert(group_name):
    gi = vgs[group_name].index
    for v in solid.data.vertices:
        for g in v.groups:
            if g.group == gi and g.weight > 0.5:
                if v.co.z > zmin + 0.70*H if group_name=='spine2' else (v.co.z > zmin + 0.58*H if group_name=='spine' else True):
                    return v.index
    return None
chest = find_vert('spine2'); belly = find_vert('spine'); groin = find_vert('hips')
for nm, vi in (('CHEST', chest), ('BELLY', belly), ('GROIN', groin)):
    if vi is None: print(f"PROBE {nm} n/a"); continue
    sc.frame_set(1); c1 = eval_co(vi)
    sc.frame_set(31); c31 = eval_co(vi)
    print(f"PROBE {nm} delta={abs((c31-c1).length):.5f}", flush=True)
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.context.scene.objects:
    if o.type in ('MESH','ARMATURE') and o.name != 'ORIG_UV_CARRIER': o.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-RIGGED-V5.glb", use_selection=True, export_animations=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-RIGGED-V5.fbx", use_selection=True, add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
print("EXPORT_V5_DONE", flush=True)
