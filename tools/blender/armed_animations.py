"""ARMED BUILD V1: V5 + leaf-blade spear prop + WALK / SPEAR THRUST / HIT actions -> armed GLB/FBX + standalone spear GLB/FBX."""
import bpy, bmesh, math
from mathutils import Vector, Euler, Matrix
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-RIGGED-V5.glb")
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
zs = [v.co.z for v in body.data.vertices]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
rig = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
sc = bpy.context.scene
print(f"V5_IN verts={len(body.data.vertices)} H={H:.3f}", flush=True)

# ---------- materials ----------
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

# ---------- spear: single mesh, 4 material slots ----------
spear = bpy.data.objects.new('SpearLeafblade', bpy.data.meshes.new('SpearMesh'))
sc.collection.objects.link(spear)
bm = bmesh.new()
# 0 wood haft: cone along +Y
bmesh.ops.create_cone(bm, cap_ends=True, segments=16, radius1=0.0065*H, radius2=0.0052*H,
                      depth=0.84*H, matrix=Matrix.Translation((0, 0, 0)) @ Euler((R(-90), 0, 0)).to_matrix().to_4x4())
haft_faces = [f for f in bm.faces]
for f in haft_faces: f.material_index = 0
# 1 bronze leaf blade: flattened stretched sphere centered at y=+0.42H
bmesh.ops.create_uvsphere(bm, u_segments=18, v_segments=10, radius=1.0)
blade_verts = [v for v in bm.verts if v.co.length <= 1.0001 and v.co != Vector((0,0,1))]
for v in blade_verts:
    v.co = Vector((v.co.x * 0.019*H, v.co.y * 0.078*H + 0.40*H, v.co.z * 0.0075*H))
blade_faces = [f for f in bm.faces if f not in haft_faces]
for f in blade_faces: f.material_index = 1
# 2 iron socket collar: torus at y=+0.335H
bmesh.ops.create_icosphere(bm, subdivisions=2, radius=1.0)  # placeholder replaced below
# remove placeholder verts
icov = [v for v in bm.verts if abs(v.co.length - 1.0) < 1e-4 and v.co.y < -0.5]
for v in icov: bm.verts.remove(v)
bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0095*H, radius2=0.0095*H,
                      depth=0.022*H, matrix=Matrix.Translation((0, 0.335*H, 0)) @ Euler((R(-90), 0, 0)).to_matrix().to_4x4())
collar_faces = [f for f in bm.faces if f not in haft_faces and f not in blade_faces]
for f in collar_faces: f.material_index = 2
# 3 cord grip wraps: two thin cylinders at grip zone y in [-0.18H, -0.10H]
for gy in (-0.18*H, -0.10*H):
    bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0080*H, radius2=0.0080*H,
                          depth=0.014*H, matrix=Matrix.Translation((0, gy, 0)) @ Euler((R(-90), 0, 0)).to_matrix().to_4x4())
    for f in [f for f in bm.faces if f not in haft_faces and f not in blade_faces and f not in collar_faces]:
        f.material_index = 3
bm.verts.index_update()
bm.to_mesh(spear.data); bm.free()
for m in (m_wood, m_bronze, m_iron, m_cord):
    spear.data.materials.append(m)
print(f"SPEAR faces={len(spear.data.polygons)}", flush=True)

# ---------- attach spear to hand.R ----------
spear.parent = rig
spear.parent_type = 'BONE'
spear.parent_bone = 'hand.R'
spear.rotation_euler = (math.pi, 0, 0)   # +Y head flips to world up
spear.location = (0, 0.12*H, 0)          # grip point at the hand bone
bpy.context.view_layer.update()
print("SPEAR_ATTACHED", flush=True)

# ---------- animation helper ----------
def act(name):
    a = bpy.data.actions.new(name)
    rig.animation_data_create()
    rig.animation_data.action = a
    return a
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]; b.rotation_mode = 'XYZ'
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}

# ---------- WALK (41 frames, 2 steps) ----------
a = act('walk')
# frame 1: L contact
kf('hips', 1, rot=(0, R(6), 0), loc=(0, 0, -0.004*H))
kf('upper_leg.L', 1, rot=(R(24), 0, 0)); kf('lower_leg.L', 1, rot=(R(6), 0, 0)); kf('foot.L', 1, rot=(R(8), 0, 0))
kf('upper_leg.R', 1, rot=(R(-18), 0, 0)); kf('lower_leg.R', 1, rot=(R(32), 0, 0)); kf('foot.R', 1, rot=(R(-12), 0, 0))
kf('upper_arm.L', 1, rot=(R(-18), 0, 0)); kf('forearm.L', 1, rot=(R(10), 0, 0))
kf('upper_arm.R', 1, rot=(R(6), 0, 0));  kf('forearm.R', 1, rot=(R(6), 0, 0))
kf('spine', 1, rot=(0, 0, R(2)))
# frame 11: L passing / R swings through
kf('hips', 11, rot=(0, R(3), 0), loc=(0, 0, 0))
kf('upper_leg.L', 11, rot=(R(8), 0, 0)); kf('lower_leg.L', 11, rot=(R(4), 0, 0))
kf('upper_leg.R', 11, rot=(R(10), 0, 0)); kf('lower_leg.R', 11, rot=(R(14), 0, 0)); kf('foot.R', 11, rot=(R(6), 0, 0))
kf('upper_arm.L', 11, rot=(R(-6), 0, 0)); kf('upper_arm.R', 11, rot=(R(-4), 0, 0))
kf('spine', 11, rot=(0, 0, R(1)))
# frame 21: R contact (mirror of 1)
kf('hips', 21, rot=(0, R(-6), 0), loc=(0, 0, -0.004*H))
kf('upper_leg.R', 21, rot=(R(24), 0, 0)); kf('lower_leg.R', 21, rot=(R(6), 0, 0)); kf('foot.R', 21, rot=(R(8), 0, 0))
kf('upper_leg.L', 21, rot=(R(-18), 0, 0)); kf('lower_leg.L', 21, rot=(R(32), 0, 0)); kf('foot.L', 21, rot=(R(-12), 0, 0))
kf('upper_arm.R', 21, rot=(R(-18), 0, 0)); kf('forearm.R', 21, rot=(R(10), 0, 0))
kf('upper_arm.L', 21, rot=(R(6), 0, 0));  kf('forearm.L', 21, rot=(R(6), 0, 0))
kf('spine', 21, rot=(0, 0, R(-2)))
# frame 31: R passing
kf('hips', 31, rot=(0, R(-3), 0), loc=(0, 0, 0))
kf('upper_leg.R', 31, rot=(R(8), 0, 0)); kf('lower_leg.R', 31, rot=(R(4), 0, 0))
kf('upper_leg.L', 31, rot=(R(10), 0, 0)); kf('lower_leg.L', 31, rot=(R(14), 0, 0)); kf('foot.L', 31, rot=(R(6), 0, 0))
kf('upper_arm.R', 31, rot=(R(-6), 0, 0)); kf('upper_arm.L', 31, rot=(R(-4), 0, 0))
kf('spine', 31, rot=(0, 0, R(-1)))
# frame 41 = frame 1
for bn, rot in (('hips',(0,R(6),0)),): pass
kf('hips', 41, rot=(0, R(6), 0), loc=(0, 0, -0.004*H))
kf('upper_leg.L', 41, rot=(R(24), 0, 0)); kf('lower_leg.L', 41, rot=(R(6), 0, 0)); kf('foot.L', 41, rot=(R(8), 0, 0))
kf('upper_leg.R', 41, rot=(R(-18), 0, 0)); kf('lower_leg.R', 41, rot=(R(32), 0, 0)); kf('foot.R', 41, rot=(R(-12), 0, 0))
kf('upper_arm.L', 41, rot=(R(-18), 0, 0)); kf('forearm.L', 41, rot=(R(10), 0, 0))
kf('upper_arm.R', 41, rot=(R(6), 0, 0));  kf('forearm.R', 41, rot=(R(6), 0, 0))
kf('spine', 41, rot=(0, 0, R(2)))
print("WALK keyframed", flush=True)

# ---------- SPEAR THRUST (31 frames) ----------
a = act('thrust')
# 1 idle-ish
kf('upper_arm.R', 1, rot=(0, 0, 0)); kf('spine2', 1, rot=(0, 0, 0)); kf('head', 1, rot=(0, 0, 0))
# 8 windup: coil, pull spear back
kf('spine2', 8, rot=(0, R(12), 0)); kf('head', 8, rot=(0, R(-6), 0))
kf('upper_arm.R', 8, rot=(R(-38), R(20), R(15))); kf('forearm.R', 8, rot=(R(25), 0, 0))
kf('upper_arm.L', 8, rot=(R(12), 0, R(-8))); kf('hips', 8, rot=(0, R(5), 0))
# 15 STRIKE
kf('spine2', 15, rot=(0, R(-16), 0)); kf('head', 15, rot=(R(-4), R(6), 0))
kf('upper_arm.R', 15, rot=(R(52), R(-15), R(10))); kf('forearm.R', 15, rot=(R(-8), 0, 0))
kf('upper_arm.L', 15, rot=(R(-25), 0, R(-12))); kf('hips', 15, rot=(0, R(-8), 0), loc=(0, 0.02*H, 0))
# 22 hold impact
kf('spine2', 22, rot=(0, R(-14), 0)); kf('upper_arm.R', 22, rot=(R(48), R(-12), R(10))); kf('forearm.R', 22, rot=(R(-4), 0, 0))
# 31 recover to idle
kf('spine2', 31, rot=(0, 0, 0)); kf('head', 31, rot=(0, 0, 0))
kf('upper_arm.R', 31, rot=(0, 0, 0)); kf('forearm.R', 31, rot=(0, 0, 0))
kf('upper_arm.L', 31, rot=(0, 0, 0)); kf('hips', 31, rot=(0, 0, 0), loc=(0, 0, 0))
print("THRUST keyframed", flush=True)

# ---------- HIT REACTION (17 frames) ----------
a = act('hit')
kf('spine2', 1, rot=(0, 0, 0)); kf('head', 1, rot=(0, 0, 0)); kf('hips', 1, loc=(0, 0, 0))
kf('spine2', 5, rot=(R(-9), 0, R(-3))); kf('head', 5, rot=(R(11), 0, R(4))); kf('hips', 5, loc=(0, 0.03*H, -0.01*H))
kf('upper_arm.L', 5, rot=(R(14), 0, 0)); kf('upper_arm.R', 5, rot=(R(14), 0, 0))
kf('spine2', 9, rot=(R(-4), 0, R(-1))); kf('head', 9, rot=(R(5), 0, R(2))); kf('hips', 9, loc=(0, 0.015*H, -0.004*H))
kf('spine2', 17, rot=(0, 0, 0)); kf('head', 17, rot=(0, 0, 0)); kf('hips', 17, loc=(0, 0, 0))
kf('upper_arm.L', 17, rot=(0, 0, 0)); kf('upper_arm.R', 17, rot=(0, 0, 0))
print("HIT keyframed", flush=True)

# ---------- NLA tracks ----------
rig.animation_data.action = None
ad = rig.animation_data
for src_name, clip in (('walk', 'walk'), ('thrust', 'thrust'), ('hit', 'hit')):
    a = bpy.data.actions.get(clip)
    if a is None: continue
    tr = ad.nla_tracks.new()
    tr.name = clip
    tr.strips.new(clip, start=1, action=a)
# include the imported idle: find its action
for a in bpy.data.actions:
    if a.name not in ('walk', 'thrust', 'hit') and 'idle' in a.name.lower():
        tr = ad.nla_tracks.new(); tr.name = 'idle'
        tr.strips.new('idle', start=1, action=a)
print(f"NLA tracks: {[t.name for t in ad.nla_tracks]}", flush=True)

# ---------- export ----------
bpy.ops.object.mode_set(mode='OBJECT')
sc.frame_start, sc.frame_end = 1, 41
bpy.ops.object.select_all(action='DESELECT')
rig.select_set(True); body.select_set(True); spear.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-ARMED-V1.glb", use_selection=True,
                          export_animations=True, export_nla_strips=True, export_skins=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-ARMED-V1.fbx", use_selection=True,
                         add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
print("ARMED_EXPORTED", flush=True)

# ---------- standalone spear prop ----------
bpy.ops.object.select_all(action='DESELECT')
spear.select_set(True)
spear_dup = spear.copy(); spear_dup.data = spear.data.copy()
sc.collection.objects.link(spear_dup)
spear_dup.parent = None; spear_dup.parent_type = 'OBJECT'
spear_dup.rotation_euler = (R(90), 0, R(20))  # lay it diagonal for a nice prop rest
spear_dup.location = (0, 0, 0.06*H)
bpy.ops.object.select_all(action='DESELECT')
spear_dup.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/SPEAR-LEAFBLADE-V1.glb", use_selection=True, export_animations=False)
bpy.ops.export_scene.fbx(filepath="models/generated/SPEAR-LEAFBLADE-V1.fbx", use_selection=True, add_leaf_bones=False, bake_anim=False, embed_textures=True, path_mode='COPY')
print("SPEAR_EXPORTED", flush=True)
print("ALL_DONE", flush=True)
