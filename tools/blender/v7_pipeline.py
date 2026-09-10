"""V7 PIPELINE v2: silhouette-exact retopo (weld+decimate, NO smoothing) ->
bake normal+AO from the 468K original -> CMU mocap skeleton fit -> real walk clip
-> chest-led idle on the same rig -> export."""
import bpy, bmesh, math
from mathutils import Vector, Matrix, Euler
import numpy as np
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-STORMCROWN-404GEN-MESH-V1.glb")
src = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print(f"ORIG faces={len(src.data.polygons)}", flush=True)
bpy.context.view_layer.objects.active = src
# duplicate -> hi (caster)
hi = src.copy(); hi.data = src.data.copy()
bpy.context.collection.objects.link(hi)
lo = src
# weld + decimate on lo (NO smoothing - silhouette exact)
w = lo.modifiers.new('Weld', 'WELD'); w.merge_threshold = 0.0018
bpy.ops.object.modifier_apply(modifier='Weld')
d = lo.modifiers.new('Dec', 'DECIMATE'); d.ratio = 100000 / len(lo.data.polygons)
bpy.ops.object.modifier_apply(modifier='Dec')
lo.name = 'AedanBody'
print(f"LO faces={len(lo.data.polygons)} verts={len(lo.data.vertices)}", flush=True)
zs = [v.co.z for v in lo.data.vertices]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
# ---------- bake normal + AO ----------
bpy.context.scene.render.engine = 'CYCLES'
bpy.context.scene.cycles.samples = 24
bpy.context.scene.cycles.device = 'CPU'
bpy.ops.object.select_all(action='DESELECT')
hi.select_set(True); lo.select_set(True)
bpy.context.view_layer.objects.active = lo
import os
os.makedirs('/tmp/v7bake', exist_ok=True)
img_n = bpy.data.images.new('BakeNormal', 2048, 2048, alpha=False, float_buffer=False)
img_n.filepath_raw = '/tmp/v7bake/normal.png'; img_n.file_format = 'PNG'
bpy.ops.object.bake(type='NORMAL', use_selected_to_active=True, margin=12, use_clear=True)
print("BAKE_NORMAL_DONE", flush=True)
img_ao = bpy.data.images.new('BakeAO', 2048, 2048, alpha=False, float_buffer=False)
img_ao.filepath_raw = '/tmp/v7bake/ao.png'; img_ao.file_format = 'PNG'
bpy.ops.object.bake(type='AO', use_selected_to_active=True, margin=12, use_clear=True)
print("BAKE_AO_DONE", flush=True)
img_n.save(); img_ao.save()
# plug into material
mat0 = lo.data.materials[0]
nt = mat0.node_tree
nmap = nt.nodes.new('ShaderNodeNormalMap')
ntex = nt.nodes.new('ShaderNodeTexImage'); ntex.image = img_n
nt.links.new(ntex.outputs['Color'], nmap.inputs['Color'])
nt.links.new(nmap.outputs['Normal'], nt.nodes['Principled BSDF'].inputs['Normal'])
aotex = nt.nodes.new('ShaderNodeTexImage'); aotex.image = img_ao
mix = nt.nodes.new('ShaderNodeMixRGB'); mix.blend_type = 'MULTIPLY'; mix.inputs['Fac'].default_value = 0.35
alb = next(n for n in nt.nodes if n.type == 'TEX_IMAGE' and n.image and n.image.name != 'BakeNormal' and n.image.name != 'BakeAO')
nt.links.new(alb.outputs['Color'], mix.inputs['Color1'])
nt.links.new(aotex.outputs['Color'], mix.inputs['Color2'])
nt.links.new(mix.outputs['Color'], nt.nodes['Principled BSDF'].inputs['Base Color'])
# UVs for the bake images: images bake into lo's active UV automatically
# ---------- import CMU walk ----------
bpy.ops.import_anim.bvh(filepath="/tmp/mocap/07_01.bvh", target='ARMATURE')
arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
walk_act = bpy.data.actions[-1] if bpy.data.actions else None
print(f"BVH armature={arm.name} actions={[a.name for a in bpy.data.actions]}", flush=True)
# fit skeleton: uniform scale + hips alignment
bpy.context.view_layer.objects.active = arm
bpy.ops.object.mode_set(mode='EDIT')
eb = arm.data.edit_bones
def bn(name, default=None):
    return eb.get(name) or eb.get(default)
hips = bn('Hips')
# measure skeleton extents in rest pose
zlo = min(b.head.z for b in eb); zhi = max(b.tail.z for b in eb)
sH = zhi - zlo
s = H / sH
for b in eb:
    b.head *= s; b.tail *= s
# mesh hips height ~0.47H
off = Vector((0, 0, zmin + 0.47*H)) - hips.head
for b in eb:
    b.head += off; b.tail += off
# A-pose the arm chains (CMU rest is T-pose; mesh arms ~50 deg down)
for side, sgn in (('Left', 1), ('Right', -1)):
    sh = bn(f'{side}Shoulder')
    if sh is None: continue
    pivot = sh.head.copy()
    ang = math.radians(50) * sgn * -1  # rotate arm downward toward body
    rot = Matrix.Rotation(ang if side == 'Right' else -ang, 4, 'Y')
    rot = rot if side == 'Right' else Matrix.Rotation(math.radians(-50), 4, 'Y')
    # L: arms point +x, rotate about Y by -50 deg brings them down? test both
    for name in (f'{side}Shoulder', f'{side}Arm', f'{side}ForeArm', f'{side}Hand',
                 f'{side}Arm', f'{side}ForeArm', f'{side}Hand', ''):
        pass
    chain = [b for b in eb if b.name in (f'{side}Shoulder', f'{side}Arm', f'{side}ForeArm', f'{side}Hand')]
    sub = [b for b in eb if b.parent in chain and b not in chain]  # fingers etc
    allb = chain + sub
    for b in allb:
        b.head = pivot + rot @ (b.head - pivot)
        b.tail = pivot + rot @ (b.tail - pivot)
bpy.ops.object.mode_set(mode='OBJECT')
print("SKELETON_FITTED", flush=True)
# ---------- skin lo to CMU armature (nearest-segment) ----------
MAIN = ['Hips','LowerBack','Chest','Chest2','Chest3','Neck','Neck1','Head',
        'LeftUpLeg','LeftLeg','LeftFoot','LeftToeBase',
        'RightUpLeg','RightLeg','RightFoot','RightToeBase',
        'LeftShoulder','LeftArm','LeftForeArm','LeftHand',
        'RightShoulder','RightArm','RightForeArm','RightHand']
bone_names = [b.name for b in arm.data.bones]
names = [n for n in MAIN if n in bone_names]
pbmap = {b.name: b for b in arm.pose.bones}
arm_mat = arm.matrix_world
seg = {}
for n in names:
    b = arm.data.bones[n]
    a = arm_mat @ b.head_local; t = arm_mat @ b.tail_local
    seg[n] = (np.array([a.x,a.y,a.z]), np.array([t.x,t.y,t.z]))
V = np.array([[v.co.x, v.co.y, v.co.z] for v in lo.data.vertices]); nV = len(V)
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
for n in names: lo.vertex_groups.new(name=n)
vg = [lo.vertex_groups[n] for n in names]
for vi in range(0, nV, 5000):
    for r in range(5000):
        idx = vi + r
        if idx >= nV: break
        for bi in range(k):
            vg[top[idx, bi]].add([idx], float(wtop[idx, bi]), 'REPLACE')
am = lo.modifiers.new('ArmatureMod', 'ARMATURE'); am.object = arm
print(f"SKINNED {nV} verts onto {len(names)} bones", flush=True)
# ---------- idle on CMU rig (chest-led breathing) ----------
arm.animation_data_create()
idle = bpy.data.actions.new('idle')
arm.animation_data.action = idle
pb = pbmap
def kf(bn_, f, rot=None):
    b = pb[bn_]; b.rotation_mode = 'XYZ'
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
chest = next((n for n in ('Chest3','Chest2','Chest','LowerBack') if n in pbmap), None)
neckb = next((n for n in ('Neck1','Neck') if n in pbmap), None)
kf(chest, 1, rot=(0,0,0)); kf(neckb, 1, rot=(0,0,0))
for f, amp in ((12, 0.55), (31, 1.0), (50, 0.55), (61, 0.0)):
    kf(chest, f, rot=(R(4.5)*amp, 0, 0))
    kf(neckb, f, rot=(R(-1.5)*amp, 0, 0))
# ---------- NLA: walk (CMU action) + idle ----------
arm.animation_data.action = None
ad = arm.animation_data
walk = next((a for a in bpy.data.actions if '07_01' in a.name or 'walk' in a.name.lower()), None)
if walk: walk.name = 'walk'
for clip in ('idle', 'walk'):
    a = bpy.data.actions.get(clip)
    if a is None: continue
    tr = ad.nla_tracks.new(); tr.name = clip
    tr.strips.new(clip, start=1, action=a)
print(f"NLA: {[t.name for t in ad.nla_tracks]}", flush=True)
# ---------- export ----------
sc = bpy.context.scene
sc.frame_start, sc.frame_end = 1, 61
bpy.ops.object.select_all(action='DESELECT')
arm.select_set(True); lo.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-V7-MO.glb", use_selection=True,
                          export_animations=True, export_nla_strips=True, export_skins=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-V7-MO.fbx", use_selection=True,
                         add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
print("V7_EXPORTED", flush=True)
