"""Open V7, attach 3D spear to RightHand (per canon plate), QC render walk+idle, export ARMED + separate spear prop."""
import bpy, bmesh, math
from mathutils import Vector, Euler, Matrix
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-V7-MO.glb")
arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max((o for o in bpy.context.scene.objects if o.type == 'MESH'), key=lambda o: len(o.data.polygons))
zs = [v.co.z for v in body.data.vertices]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
sc = bpy.context.scene
sc.render.fps = 24
# ---- spear (matches canon plate: leaf blade, collar, cord wraps) ----
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
haft = list(bm.faces)
bmesh.ops.create_uvsphere(bm, u_segments=18, v_segments=10, radius=1.0)
for v in [v for v in bm.verts if v.co.length <= 1.0001]:
    v.co = Vector((v.co.x*0.019*H, v.co.y*0.078*H + 0.40*H, v.co.z*0.0075*H))
blade = [f for f in bm.faces if f not in haft]
bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0095*H, radius2=0.0095*H,
                      depth=0.022*H, matrix=Matrix.Translation((0, 0.335*H, 0)) @ Euler((R(-90),0,0)).to_matrix().to_4x4())
collar = [f for f in bm.faces if f not in haft and f not in blade]
for f in haft: f.material_index = 0
for f in blade: f.material_index = 1
for f in collar: f.material_index = 2
for gy in (-0.18*H, -0.10*H):
    pre = set(bm.faces)
    bmesh.ops.create_cone(bm, cap_ends=True, segments=12, radius1=0.0080*H, radius2=0.0080*H,
                          depth=0.014*H, matrix=Matrix.Translation((0, gy, 0)) @ Euler((R(-90),0,0)).to_matrix().to_4x4())
    for f in set(bm.faces) - pre: f.material_index = 3
bm.to_mesh(spear.data); bm.free()
for m in (m_wood, m_bronze, m_iron, m_cord): spear.data.materials.append(m)
# parent to hand bone
spear.parent = arm; spear.parent_type = 'BONE'
hand_bone = next((n for n in ('RightHand','Right_Hand','hand.R') if n in [b.name for b in arm.pose.bones]), None)
spear.parent_bone = hand_bone
spear.rotation_euler = (math.pi, 0, 0)
spear.location = (0, 0.12*H, 0)
print(f"SPEAR_ON {hand_bone}", flush=True)
# ---- QC renders ----
light_data = bpy.data.lights.new('sun', 'SUN')
light = bpy.data.objects.new('sun', light_data)
light.rotation_euler = (R(50), 0, R(30)); light_data.energy = 3
sc.collection.objects.link(light)
def qc(name, frame, loc, rot):
    sc.frame_set(frame)
    cam = bpy.data.objects.new(f'c{name}', bpy.data.cameras.new(f'c{name}'))
    cam.location = Vector(loc); cam.rotation_euler = rot
    sc.collection.objects.link(cam); sc.camera = cam
    sc.render.resolution_x = 560; sc.render.resolution_y = 840
    sc.render.film_transparent = False
    sc.render.filepath = f"/tmp/qc_{name}.png"
    bpy.ops.render.render(write_still=True)
    bpy.data.objects.remove(cam, do_unlink=True)
for t in arm.animation_data.nla_tracks:
    t.mute = True
walk_tr = next((t for t in arm.animation_data.nla_tracks if t.name == 'walk'), None)
idle_tr = next((t for t in arm.animation_data.nla_tracks if t.name == 'idle'), None)
sc.frame_start, sc.frame_end = 1, 200
if walk_tr:
    walk_tr.mute = False
    qc('walkA', 40, (1.0*H, -1.2*H, 0.55*H), (R(90), 0, R(-40)))
    qc('walkB', 120, (1.0*H, -1.2*H, 0.55*H), (R(90), 0, R(-40)))
    walk_tr.mute = True
if idle_tr:
    idle_tr.mute = False
    qc('idle', 30, (0.6*H, -1.4*H, 0.55*H), (R(90), 0, R(-20)))
    idle_tr.mute = True
print("QC_DONE", flush=True)
# ---- exports ----
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-V7-ARMED.glb", use_selection=False,
                          export_animations=True, export_nla_strips=True, export_skins=True)
# separate spear prop for Unity attach
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-V7-ARMED.glb")
for o in list(bpy.context.scene.objects):
    if o.type == 'MESH' and 'Spear' not in o.name:
        bpy.data.objects.remove(o, do_unlink=True)
bpy.ops.export_scene.gltf(filepath="models/generated/SPEAR-SOVEREIGN-PROP.glb", use_selection=False)
print("ARMED_AND_PROP_EXPORTED", flush=True)
