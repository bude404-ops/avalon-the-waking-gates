"""AEDAN V7 — PIPELINE v2 EXECUTION: silhouette-exact mesh (weld+decimate, NO smoothing/voxel)
+ bake normal/AO from the original 468K sculpt + skeleton refit to mesh joints
+ REAL CMU MOCAP walk (07_01) retargeted by direct skeleton fit + chest-led idle."""
import bpy, bmesh, math
from mathutils import Vector, Euler
import numpy as np
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-STORMCROWN-404GEN-MESH-V1.glb")
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
body.name = 'AedanLo'
mesh = body.data
print(f"HI faces={len(mesh.polygons)}", flush=True)
# joint solver from rest mesh
verts = [body.matrix_world @ v.co for v in mesh.vertices]
zs = [v.z for v in verts]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
def slice_x(zc, band=0.02):
    lo_, hi_ = zmin + (zc-band)*H, zmin + (zc+band)*H
    xs = [abs(v.x) for v in verts if lo_ <= v.z <= hi_]
    return sum(xs)/len(xs) if xs else 0.0
sh_x = max(slice_x(0.83), 0.06*H); el_x = max(slice_x(0.61), 0.05*H)
wr_x = max(slice_x(0.46), 0.05*H); hip_x = max(slice_x(0.47), 0.05*H)
kn_x = max(slice_x(0.26), 0.05*H); an_x = max(slice_x(0.055), 0.04*H)
def P(x, h): return Vector((x, 0.0, zmin + h*H))
# HI copy (bake source)
hi_obj = body.copy(); hi_obj.data = body.data.copy(); hi_obj.name = 'AedanHi'
bpy.context.collection.objects.link(hi_obj)
print(f"HI copy faces={len(hi_obj.data.polygons)}", flush=True)
# LO: weld + decimate, NO smooth
bpy.context.view_layer.objects.active = body
w = body.modifiers.new('Weld', 'WELD'); w.merge_threshold = 0.0018
bpy.ops.object.modifier_apply(modifier='Weld')
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.delete_loose(use_verts=True, use_edges=True, use_faces=False)
bpy.ops.object.mode_set(mode='OBJECT')
d = body.modifiers.new('Dec', 'DECIMATE'); d.ratio = max(0.18, 100000/len(body.data.polygons))
bpy.ops.object.modifier_apply(modifier='Dec')
body.data.polygons.foreach_set('use_smooth', [True]*len(body.data.polygons))
print(f"LO faces={len(body.data.polygons)} uv={len(body.data.uv_layers)}", flush=True)
# ---- BAKE normal + AO from HI to LO ----
bpy.context.scene.render.engine = 'CYCLES'
bpy.context.scene.cycles.samples = 32
bpy.context.scene.cycles.device = 'CPU'
mat = body.data.materials[0]
nt = mat.node_tree
# normal map node chain
if not any(n.type == 'NORMAL_MAP' for n in nt.nodes):
    nm = nt.nodes.new('ShaderNodeNormalMap'); nm.name = 'BakedNormal'
    texn = nt.nodes.new('ShaderNodeTexImage'); texn.name = 'BakedNormalTex'
    nm.inputs['Color'].links.remove(nm.inputs['Color'].links[0]) if nm.inputs['Color'].links else None
    nt.links.new(texn.outputs['Color'], nm.inputs['Color'])
    bsdf = next(n for n in nt.nodes if n.type == 'BSDF_PRINCIPLED')
    nt.links.new(nm.outputs['Normal'], bsdf.inputs['Normal'])
img_n = bpy.data.images.new('bake_normal', 2048, 2048, float_buffer=True)
nt.nodes['BakedNormalTex'].image = img_n
# AO multiply into base color
texao = nt.nodes.new('ShaderNodeTexImage'); texao.name = 'BakedAOTex'
mix = nt.nodes.new('ShaderNodeMixRGB'); mix.blend_type = 'MULTIPLY'; mix.inputs['Fac'].default_value = 1.0
bsdf = next(n for n in nt.nodes if n.type == 'BSDF_PRINCIPLED')
base_src = bsdf.inputs['Base Color'].links[0].from_socket if bsdf.inputs['Base Color'].links else None
if base_src:
    nt.links.new(base_src, mix.inputs['Color1'])
    nt.links.new(texao.outputs['Color'], mix.inputs['Color2'])
    nt.links.new(mix.outputs['Color'], bsdf.inputs['Base Color'])
img_ao = bpy.data.images.new('bake_ao', 2048, 2048, float_buffer=True)
texao.image = img_ao
nt.nodes.active = nt.nodes['BakedNormalTex']
bpy.ops.object.select_all(action='DESELECT')
hi_obj.select_set(True); body.select_set(True); bpy.context.view_layer.objects.active = body
import time; t0 = time.time()
bpy.ops.object.bake(type='NORMAL', use_selected_to_active=True, margin=16, max_ray_distance=0.05, normal_space='TANGENT')
print(f"BAKE_NORMAL {time.time()-t0:.0f}s", flush=True)
nt.nodes.active = texao
bpy.ops.object.bake(type='AO', use_selected_to_active=True, margin=16, max_ray_distance=0.05, use_clear=True)
print(f"BAKE_AO {time.time()-t0:.0f}s", flush=True)
img_n.filepath_raw = "/tmp/bake_normal.png"; img_n.file_format='PNG'; bpy.ops.image.save_as(filepath="/tmp/bake_normal.png") if False else img_n.save()
img_ao.filepath_raw = "/tmp/bake_ao.png"; img_ao.file_format='PNG'; img_ao.save()
print("BAKES_SAVED", flush=True)
# hide HI (not exported)
hi_obj.hide_render = True; hi_obj.hide_viewport = True
# ---- CMU BVH import + skeleton fit ----
bpy.ops.import_anim.bvh(filepath="/tmp/mocap/07_01.bvh", use_fps_scale=True, update_scene_fps=False)
arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
arm.name = 'AedanRig'
walk_act = [a for a in bpy.data.actions if a.name.startswith(('07_01','07','BVH','Walk','walk'))]
print(f"ACTIONS after import: {[a.name for a in bpy.data.actions]}", flush=True)
# uniform scale to hips height + move
hips_z_mesh = zmin + 0.47*H
bpy.context.view_layer.objects.active = arm
bpy.ops.object.mode_set(mode='EDIT')
eb = arm.data.edit_bones
hb = eb.get('Hips')
if hb is None: hb = eb.get('hips')
hips_z_bvh = hb.head.z
s = hips_z_mesh / hips_z_bvh
for b in eb:
    b.head = Vector((b.head.x*s, b.head.y*s, b.head.z*s))
    b.tail = Vector((b.tail.x*s, b.tail.tail.y*s if False else b.tail.y*s, b.tail.z*s))
arm.data.edit_bones.update()
bpy.ops.object.mode_set(mode='OBJECT')
arm.location = (0, 0, 0)
# refit joints to mesh
bpy.ops.object.mode_set(mode='EDIT')
def setbone(n, head, tail, connect_parent=False):
    b = eb.get(n)
    if b is None: return None
    b.head, b.tail = Vector(head), Vector(tail)
    return b
setbone('Hips', P(0,0.47), P(0,0.52))
setbone('LowerBack', P(0,0.52), P(0,0.58))
setbone('Chest', P(0,0.58), P(0,0.66))
setbone('Chest2', P(0,0.66), P(0,0.74))
setbone('Chest3', P(0,0.74), P(0,0.80))
setbone('Neck', P(0,0.80), P(0,0.84))
setbone('Neck1', P(0,0.84), P(0,0.88))
setbone('Head', P(0,0.88), P(0,0.99))
for S, sx in (('Left',1),('Right',-1)):
    hx = sx*hip_x
    setbone(f'{S}HipJoint', P(hx*0.6,0.47), P(hx,0.47))
    setbone(f'{S}UpLeg', P(hx,0.47), P(sx*kn_x,0.26))
    setbone(f'{S}Leg', P(sx*kn_x,0.26), P(sx*an_x,0.055))
    setbone(f'{S}Foot', P(sx*an_x,0.055), P(sx*an_x*0.9,0.005))
    setbone(f'{S}ToeBase', P(sx*an_x*0.9,0.005), P(sx*an_x*0.7,0.002))
    setbone(f'{S}Shoulder', P(sx*sh_x*0.7,0.80), P(sx*sh_x,0.80))
    setbone(f'{S}Arm', P(sx*sh_x,0.80), P(sx*el_x,0.61))
    setbone(f'{S}ForeArm', P(sx*el_x,0.61), P(sx*wr_x,0.46))
    setbone(f'{S}Hand', P(sx*wr_x,0.46), P(sx*wr_x*0.85,0.38))
    for f in ('Thumb','FingerBase','HandIndex1'):
        setbone(f'{S}{f}', P(sx*wr_x*0.85,0.38), P(sx*wr_x*0.7,0.34))
bpy.ops.object.mode_set(mode='OBJECT')
print("SKELETON_REFIT", flush=True)
# ---- skin: nearest-segment weights on the fitted skeleton ----
names = [b.name for b in arm.data.bones]
seg = {}
for n in names:
    pb = arm.pose.bones.get(n)
    seg[n] = (np.array(pb.head), np.array(pb.tail))
V = np.array([[v.x, v.y, v.z] for v in body.data.vertices]); nV = len(V)
W = np.zeros((nV, len(names)))
for bi, n in enumerate(names):
    a, b = seg[n]; ab = b - a; ab2 = float(ab @ ab) if (b-a).any() else 1.0
    if ab2 < 1e-10:
        W[:, bi] = np.sum((V - a)**2, axis=1); continue
    t = np.clip(((V - a) @ ab) / ab2, 0.0, 1.0)
    proj = a + t[:, None] * ab
    W[:, bi] = np.sum((V - proj)**2, axis=1)
# damp tiny finger/thumb bones (merge into hand)
for n in names:
    if any(k in n for k in ('Thumb','Finger','HandIndex')):
        W[:, names.index(n)] += (0.02*H)**2
k = 4
top = np.argpartition(W, k, axis=1)[:, :k]
wtop = np.take_along_axis(W, top, axis=1)
wtop = 1.0 / (wtop + 1e-6); wtop /= wtop.sum(axis=1, keepdims=True)
for n in names: body.data.vertex_groups.new(name=n)
vg = [body.data.vertex_groups[n] for n in names]
for vi in range(0, nV, 5000):
    for r in range(5000):
        idx = vi + r
        if idx >= nV: break
        for bi in range(k):
            vg[top[idx, bi]].add([idx], float(wtop[idx, bi]), 'REPLACE')
am = body.modifiers.new('Armature', 'ARMATURE'); am.object = arm
body.parent = arm
print(f"SKINNED {nV} verts x {len(names)} bones", flush=True)
# ---- idle action (chest-led) ----
sc = bpy.context.scene
sc.render.fps = 24
walk = bpy.data.actions[0]
# walk: strip X/Y translation for in-place loop
for fc in list(walk.fcurves):
    d = fc.data_path
    if 'location' in d and 'Hips' in d and fc.array_index in (0, 1):
        walk.fcurves.remove(fc)
print("WALK IN-PLACE", flush=True)
idle = bpy.data.actions.new('idle')
rig_ad = arm.animation_data_create(); rig_ad.action = idle
pb = {b.name: b for b in arm.pose.bones}
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
for f, amp in ((1,0), (12,0.55), (31,1.0), (50,0.55), (61,0)):
    kf('Chest2', f, rot=(R(3.5)*amp, 0, 0))
    kf('Chest3', f, rot=(R(2.5)*amp, 0, 0))
    kf('Neck1', f, rot=(R(-1.5)*amp, R(1.2)*amp, 0))
    kf('LeftArm', f, rot=(0, 0, R(-2.5)*amp))
    kf('RightArm', f, rot=(0, 0, R(2.5)*amp))
    kf('Hips', f, loc=(0, 0, 0.002*H*amp))
print("IDLE_KEYED", flush=True)
# NLA
rig_ad.action = None
tracks = {'idle': (1, 61), 'walk': (1, 317)}
for nm, (s, e) in tracks.items():
    a = bpy.data.actions.get(nm)
    if a is None: continue
    tr = rig_ad.nla_tracks.new(); tr.name = nm
    tr.strips.new(nm, start=1, action=a)
print(f"NLA: {[t.name for t in rig_ad.nla_tracks]}", flush=True)
# ---- QC renders ----
def qc(name, frame, cam_loc, rot_deg=35):
    sc.frame_set(frame)
    cam_data = bpy.data.cameras.new('qc'); cam = bpy.data.objects.new(f'cam_{name}', cam_data)
    cam.location = Vector(cam_loc); cam.rotation_euler = (R(90), 0, R(rot_deg))
    sc.collection.objects.link(cam); sc.camera = cam
    sc.render.resolution_x = 640; sc.render.resolution_y = 960
    sc.render.filepath = f"/tmp/qc_{name}.png"
    bpy.ops.render.render(write_still=True)
    bpy.data.objects.remove(cam, do_unlink=True)
qc('walkA', 60, (0.9*H, 0.9*H, 0.55*H))
qc('walkB', 180, (0.9*H, 0.9*H, 0.55*H))
qc('idle', 30, (0.7*H, 1.1*H, 0.55*H))
print("QC_RENDERED", flush=True)
# ---- export ----
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-V7-MOCAP.glb", use_selection=False,
                          export_animations=True, export_nla_strips=True, export_skins=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-V7-MOCAP.fbx",
                         add_leaf_bones=False, bake_anim=True, path_mode='COPY')
print("V7_EXPORTED", flush=True)
