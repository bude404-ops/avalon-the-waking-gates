"""ARMED V2: welded high-detail mesh -> rig -> chest-led idle -> WALK v2 / THRUST v2 / HIT v2 (bezier, overlap, timing)
-> spear attached -> NLA -> export."""
import bpy, bmesh, math
from mathutils import Vector, Euler, Matrix
import numpy as np
R = math.radians
bpy.ops.wm.open_mainfile(filepath="/tmp/aedan_v6_welded.blend")
body = None
for o in bpy.context.scene.objects:
    if o.type == 'MESH': body = o; break
mesh = body.data
print(f"MESH faces={len(mesh.polygons)} verts={len(mesh.vertices)} uv={len(mesh.uv_layers)}", flush=True)
verts = [body.matrix_world @ v.co for v in mesh.vertices]
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
for n in names: mesh.vertex_groups.new(name=n)
vg_list = [mesh.vertex_groups[n] for n in names]
for vi in range(0, nV, 5000):
    for r in range(5000):
        idx = vi + r
        if idx >= nV: break
        for bi in range(k):
            vg_list[top[idx, bi]].add([idx], float(wtop[idx, bi]), 'REPLACE')
print(f"WEIGHTS {nV} verts", flush=True)
am = body.modifiers.new('Armature', 'ARMATURE'); am.object = rig
body.parent = rig
sc = bpy.context.scene
# ---------- spear ----------
def mat(name, col, met, rough):
    m = bpy.data.materials.new(name); m.use_nodes = True
    bs = m.node_tree.nodes.get('Principled BSDF')
    bs.inputs['Base Color'].default_value = (*col, 1)
    bs.inputs['Metallic'].default_value = met
    bs.inputs['Roughness'].default_value = rough
    return m
m_bronze = mat('BronzeDark', (0.32, 0.22, 0.10), 1.0, 0.42)
m_iron   = mat('IronDark',   (0.10, 0.10, 0.11), 1.0, 0.55)
m_wood   = mat('HaftWood',   (0.24, 0.17, 0.10), 0.0, 0.85)
m_cord   = mat('CordWrap',   (0.13, 0.11, 0.09), 0.0, 0.95)
spear = bpy.data.objects.new('SpearLeafblade', bpy.data.meshes.new('SpearMesh'))
sc.collection.objects.link(spear)
bm = bmesh.new()
bmesh.ops.create_cone(bm, cap_ends=True, segments=16, radius1=0.0065*H, radius2=0.0052*H,
                      depth=0.84*H, matrix=Matrix.Translation((0,0,0)) @ Euler((R(-90),0,0)).to_matrix().to_4x4())
haft_faces = list(bm.faces)
bmesh.ops.create_uvsphere(bm, u_segments=18, v_segments=10, radius=1.0)
for v in [v for v in bm.verts if v.co.length <= 1.0001]:
    v.co = Vector((v.co.x*0.019*H, v.co.y*0.078*H + 0.40*H, v.co.z*0.0075*H))
blade_faces = [f for f in bm.faces if f not in haft_faces]
bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0095*H, radius2=0.0095*H,
                      depth=0.022*H, matrix=Matrix.Translation((0, 0.335*H, 0)) @ Euler((R(-90),0,0)).to_matrix().to_4x4())
collar_faces = [f for f in bm.faces if f not in haft_faces and f not in blade_faces]
for f in haft_faces: f.material_index = 0
for f in blade_faces: f.material_index = 1
for f in collar_faces: f.material_index = 2
for gy in (-0.18*H, -0.10*H):
    pre = set(bm.faces)
    bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0080*H, radius2=0.0080*H,
                          depth=0.014*H, matrix=Matrix.Translation((0, gy, 0)) @ Euler((R(-90),0,0)).to_matrix().to_4x4())
    for f in set(bm.faces) - pre: f.material_index = 3
bm.to_mesh(spear.data); bm.free()
for m in (m_wood, m_bronze, m_iron, m_cord): spear.data.materials.append(m)
spear.parent = rig; spear.parent_type = 'BONE'; spear.parent_bone = 'hand.R'
spear.rotation_euler = (math.pi, 0, 0); spear.location = (0, 0.12*H, 0)
print("SPEAR_ON", flush=True)
# ---------- animations v2 ----------
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}
def act(name):
    a = bpy.data.actions.new(name)
    rig.animation_data_create(); rig.animation_data.action = a
    return a
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]; b.rotation_mode = 'XYZ'
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
# IDLE — chest-led breath (same as V5 doctrine)
a = act('idle')
for f, amp in ((1,0), (12,0.55), (31,1.0), (50,0.55), (61,0)):
    kf('spine2', f, rot=(R(4.5)*amp, 0, 0))
    kf('spine1', f, rot=(R(1.2)*amp, 0, 0))
    kf('head',   f, rot=(R(-1.5)*amp, R(1.2)*amp, 0))
    kf('upper_arm.L', f, rot=(0, 0, R(-3)*amp))
    kf('upper_arm.R', f, rot=(0, 0, R(3)*amp))
    kf('hips', f, loc=(0, 0, 0.002*H*amp))
# WALK v2 — 41f cycle, contact/passing with weight + overlap
a = act('walk')
# f1 L-contact: hips LOW, L leg extended fwd, R trailing toe-off
kf('hips', 1, rot=(0, R(7), 0), loc=(0, 0, -0.006*H))
kf('spine', 1, rot=(R(2), R(-4), R(3)))
kf('spine2', 1, rot=(R(1), R(-5), R(2)))
kf('head', 1, rot=(R(-1), R(4), R(-2)))
kf('upper_leg.L', 1, rot=(R(27), 0, 0)); kf('lower_leg.L', 1, rot=(R(4), 0, 0)); kf('foot.L', 1, rot=(R(6), 0, 0))
kf('upper_leg.R', 1, rot=(R(-14), 0, 0)); kf('lower_leg.R', 1, rot=(R(28), 0, 0)); kf('foot.R', 1, rot=(R(-28), 0, 0))
kf('upper_arm.L', 1, rot=(R(-22), 0, 0)); kf('forearm.L', 1, rot=(R(14), 0, 0))
kf('upper_arm.R', 1, rot=(R(8), 0, 0)); kf('forearm.R', 1, rot=(R(4), 0, 0))
# f6 L-weight (hips rising)
kf('hips', 6, rot=(0, R(4), 0), loc=(0, 0, -0.003*H))
kf('upper_leg.L', 6, rot=(R(18), 0, 0)); kf('lower_leg.L', 6, rot=(R(16), 0, 0))
# f11 passing R swings through: hips HIGH
kf('hips', 11, rot=(0, R(0), 0), loc=(0, 0, 0))
kf('spine', 11, rot=(R(1), R(-2), R(1)))
kf('upper_leg.L', 11, rot=(R(6), 0, 0)); kf('lower_leg.L', 11, rot=(R(8), 0, 0))
kf('upper_leg.R', 11, rot=(R(14), 0, 0)); kf('lower_leg.R', 11, rot=(R(22), 0, 0)); kf('foot.R', 11, rot=(R(8), 0, 0))
kf('upper_arm.L', 11, rot=(R(-8), 0, 0)); kf('forearm.L', 11, rot=(R(8), 0, 0))
kf('upper_arm.R', 11, rot=(R(-6), 0, 0)); kf('forearm.R', 11, rot=(R(10), 0, 0))
# f16 R-swap
kf('hips', 16, rot=(0, R(-4), 0), loc=(0, 0, -0.003*H))
kf('upper_leg.R', 16, rot=(R(18), 0, 0)); kf('lower_leg.R', 16, rot=(R(16), 0, 0))
# f21 R-contact (mirror of 1)
kf('hips', 21, rot=(0, R(-7), 0), loc=(0, 0, -0.006*H))
kf('spine', 21, rot=(R(2), R(4), R(-3)))
kf('spine2', 21, rot=(R(1), R(5), R(-2)))
kf('head', 21, rot=(R(-1), R(-4), R(2)))
kf('upper_leg.R', 21, rot=(R(27), 0, 0)); kf('lower_leg.R', 21, rot=(R(4), 0, 0)); kf('foot.R', 21, rot=(R(6), 0, 0))
kf('upper_leg.L', 21, rot=(R(-14), 0, 0)); kf('lower_leg.L', 21, rot=(R(28), 0, 0)); kf('foot.L', 21, rot=(R(-28), 0, 0))
kf('upper_arm.R', 21, rot=(R(-22), 0, 0)); kf('forearm.R', 21, rot=(R(14), 0, 0))
kf('upper_arm.L', 21, rot=(R(8), 0, 0)); kf('forearm.L', 21, rot=(R(4), 0, 0))
# f26
kf('hips', 26, rot=(0, R(-4), 0), loc=(0, 0, -0.003*H))
kf('upper_leg.R', 26, rot=(R(18), 0, 0)); kf('lower_leg.R', 26, rot=(R(16), 0, 0))
# f31 passing L swings through
kf('hips', 31, rot=(0, R(0), 0), loc=(0, 0, 0))
kf('spine', 31, rot=(R(1), R(2), R(-1)))
kf('upper_leg.R', 31, rot=(R(6), 0, 0)); kf('lower_leg.R', 31, rot=(R(8), 0, 0))
kf('upper_leg.L', 31, rot=(R(14), 0, 0)); kf('lower_leg.L', 31, rot=(R(22), 0, 0)); kf('foot.L', 31, rot=(R(8), 0, 0))
kf('upper_arm.R', 31, rot=(R(-8), 0, 0)); kf('forearm.R', 31, rot=(R(8), 0, 0))
kf('upper_arm.L', 31, rot=(R(-6), 0, 0)); kf('forearm.L', 31, rot=(R(10), 0, 0))
# f36
kf('hips', 36, rot=(0, R(4), 0), loc=(0, 0, -0.003*H))
kf('upper_leg.L', 36, rot=(R(18), 0, 0)); kf('lower_leg.L', 36, rot=(R(16), 0, 0))
# f41 = f1
kf('hips', 41, rot=(0, R(7), 0), loc=(0, 0, -0.006*H))
kf('spine', 41, rot=(R(2), R(-4), R(3)))
kf('spine2', 41, rot=(R(1), R(-5), R(2)))
kf('upper_leg.L', 41, rot=(R(27), 0, 0)); kf('lower_leg.L', 41, rot=(R(4), 0, 0)); kf('foot.L', 41, rot=(R(6), 0, 0))
kf('upper_leg.R', 41, rot=(R(-14), 0, 0)); kf('lower_leg.R', 41, rot=(R(28), 0, 0)); kf('foot.R', 41, rot=(R(-28), 0, 0))
kf('upper_arm.L', 41, rot=(R(-22), 0, 0)); kf('forearm.L', 41, rot=(R(14), 0, 0))
kf('upper_arm.R', 41, rot=(R(8), 0, 0)); kf('forearm.R', 41, rot=(R(4), 0, 0))
print("WALK_V2", flush=True)
# THRUST v2 — stagger stance, big anticipation, explosive strike, overshoot settle
a = act('thrust')
kf('upper_arm.R', 1, rot=(0,0,0)); kf('forearm.R', 1, rot=(0,0,0))
kf('spine2', 1, rot=(0,0,0)); kf('hips', 1, rot=(0,0,0), loc=(0,0,0)); kf('head', 1, rot=(0,0,0))
# f7 anticipation: coil back, weight shifts back foot
kf('spine2', 7, rot=(R(-4), R(16), 0)); kf('spine', 7, rot=(0, R(6), 0))
kf('head', 7, rot=(R(3), R(-8), 0))
kf('hips', 7, rot=(0, R(10), 0), loc=(0, -0.025*H, -0.008*H))
kf('upper_arm.R', 7, rot=(R(-42), R(24), R(18))); kf('forearm.R', 7, rot=(R(35), 0, 0))
kf('upper_arm.L', 7, rot=(R(18), 0, R(-10))); kf('forearm.L', 7, rot=(R(20), 0, 0))
kf('upper_leg.L', 7, rot=(R(12), 0, 0)); kf('upper_leg.R', 7, rot=(R(-16), 0, 0)); kf('lower_leg.R', 7, rot=(R(18), 0, 0))
# f13 STRIKE: explode forward, weight onto front foot
kf('spine2', 13, rot=(R(3), R(-20), 0)); kf('spine', 13, rot=(0, R(-9), 0))
kf('head', 13, rot=(R(-6), R(10), 0))
kf('hips', 13, rot=(0, R(-12), 0), loc=(0, 0.05*H, -0.012*H))
kf('upper_arm.R', 13, rot=(R(58), R(-18), R(6))); kf('forearm.R', 13, rot=(R(-6), 0, 0))
kf('upper_arm.L', 13, rot=(R(-30), 0, R(-14))); kf('forearm.L', 13, rot=(R(35), 0, 0))
kf('upper_leg.L', 13, rot=(R(-8), 0, 0)); kf('upper_leg.R', 13, rot=(R(18), 0, 0))
kf('lower_leg.L', 13, rot=(R(10), 0, 0))
# f17 overshoot (momentum carries)
kf('spine2', 17, rot=(R(5), R(-23), 0)); kf('hips', 17, loc=(0, 0.055*H, -0.012*H))
kf('upper_arm.R', 17, rot=(R(62), R(-20), R(4)))
# f24 settle
kf('spine2', 24, rot=(R(1), R(-14), 0)); kf('head', 24, rot=(R(-2), R(6), 0))
kf('hips', 24, rot=(0, R(-8), 0), loc=(0, 0.035*H, -0.008*H))
kf('upper_arm.R', 24, rot=(R(46), R(-14), R(8))); kf('forearm.R', 24, rot=(R(8), 0, 0))
# f31 recover
kf('spine2', 31, rot=(0,0,0)); kf('spine', 31, rot=(0,0,0)); kf('head', 31, rot=(0,0,0))
kf('hips', 31, rot=(0,0,0), loc=(0,0,0))
kf('upper_arm.R', 31, rot=(0,0,0)); kf('forearm.R', 31, rot=(0,0,0))
kf('upper_arm.L', 31, rot=(0,0,0)); kf('forearm.L', 31, rot=(0,0,0))
kf('upper_leg.L', 31, rot=(0,0,0)); kf('upper_leg.R', 31, rot=(0,0,0)); kf('lower_leg.L', 31, rot=(0,0,0)); kf('lower_leg.R', 31, rot=(0,0,0))
print("THRUST_V2", flush=True)
# HIT v2
a = act('hit')
kf('spine2', 1, rot=(0,0,0)); kf('head', 1, rot=(0,0,0)); kf('hips', 1, loc=(0,0,0))
kf('upper_arm.L', 1, rot=(0,0,0)); kf('upper_arm.R', 1, rot=(0,0,0))
kf('spine2', 4, rot=(R(-12), 0, R(-5))); kf('head', 4, rot=(R(14), 0, R(6)))
kf('hips', 4, loc=(0, 0.035*H, -0.015*H)); kf('spine', 4, rot=(R(-4), 0, 0))
kf('upper_arm.L', 4, rot=(R(22), 0, R(-8))); kf('upper_arm.R', 4, rot=(R(18), 0, R(8)))
kf('spine2', 8, rot=(R(-5), 0, R(-2))); kf('head', 8, rot=(R(6), 0, R(3))); kf('hips', 8, loc=(0, 0.018*H, -0.006*H))
kf('spine2', 14, rot=(0,0,0)); kf('head', 14, rot=(0,0,0)); kf('hips', 14, loc=(0,0,0))
kf('upper_arm.L', 14, rot=(0,0,0)); kf('upper_arm.R', 14, rot=(0,0,0))
print("HIT_V2", flush=True)
# NLA
rig.animation_data.action = None
ad = rig.animation_data
for clip in ('idle', 'walk', 'thrust', 'hit'):
    a = bpy.data.actions.get(clip)
    if a is None: continue
    tr = ad.nla_tracks.new(); tr.name = clip
    tr.strips.new(clip, start=1, action=a)
print(f"NLA: {[t.name for t in ad.nla_tracks]}", flush=True)
bpy.ops.object.mode_set(mode='OBJECT')
sc.frame_start, sc.frame_end = 1, 61
bpy.ops.object.select_all(action='DESELECT')
rig.select_set(True); body.select_set(True); spear.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-ARMED-V2.glb", use_selection=True,
                          export_animations=True, export_nla_strips=True, export_skins=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-ARMED-V2.fbx", use_selection=True,
                         add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
print("ARMED_V2_EXPORTED", flush=True)
