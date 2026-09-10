"""Attach the Sovereign leaf-blade spear (canon weapon plate) to Aedan V7's hand — CMU rig, holds through walk+idle."""
import bpy, bmesh, math
from mathutils import Vector, Matrix
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-V7-MO.glb")
arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
for a in bpy.data.actions:
    for base in ('idle', 'walk'):
        if a.name.startswith(base) and a.name != base: a.name = base
print(f"IMPORTED arm={arm.name} body_faces={len(body.data.polygons)} actions={[a.name for a in bpy.data.actions]}", flush=True)
zs = [v.co.z for v in body.data.vertices]; H = max(zs) - min(zs)
# ---- build spear (canon: bronze leaf head, collar, cord-wrapped haft, gate-rune ring) ----
L = 1.15 * H
mat_br = bpy.data.materials.new('BronzeDark'); mat_br.use_nodes = True
mat_br.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value = (0.28, 0.19, 0.10, 1)
mat_br.node_tree.nodes['Principled BSDF'].inputs['Metallic'].default_value = 0.85
mat_br.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value = 0.45
mat_haft = bpy.data.materials.new('HaftWood'); mat_haft.use_nodes = True
mat_haft.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value = (0.12, 0.08, 0.05, 1)
mat_haft.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value = 0.85
spear = bpy.data.objects.new('SovereignSpear', bpy.data.meshes.new('SpearMesh'))
bpy.context.collection.objects.link(spear)
bm = bmesh.new()
# haft: cylinder r=0.011H, from 0 to L*0.78
haft_top = L*0.78
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.011*H, radius2=0.011*H, depth=haft_top, matrix=Matrix.Translation(Vector((0, 0, haft_top/2))))
# leaf blade: flattened cone pair
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=4, radius1=0.030*H, radius2=0.002*H, depth=L*0.20, matrix=Matrix.Translation(Vector((0, 0, haft_top + L*0.10))))
# collar ring
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.017*H, radius2=0.017*H, depth=0.02*H, matrix=Matrix.Translation(Vector((0, 0, haft_top + 0.01*H))))
# cord wraps: 4 thin torus rings on lower haft
for frac in (0.30, 0.35, 0.40, 0.45):
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.013*H, radius2=0.013*H, depth=0.008*H, matrix=Matrix.Translation(Vector((0, 0, L*frac))))
# gate-rune band under collar
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.014*H, radius2=0.014*H, depth=0.012*H, matrix=Matrix.Translation(Vector((0, 0, haft_top - 0.02*H))))
bm.to_mesh(spear.data); bm.free()
# materials per part by z order
zmax = max((v.co.z for v in spear.data.vertices))
polys = sorted(spear.data.polygons, key=lambda p: spear.data.vertices[p.vertices[0]].co.z)
for p in spear.data.polygons:
    zc = sum(spear.data.vertices[vi].co.z for vi in p.vertices)/len(p.vertices)
    if zc > haft_top - 0.005*H:
        p.material_index = 0  # blade+collar bronze
    elif zc < haft_top*0.5 and abs(zc - L*0.30) < L*0.20:
        p.material_index = 2
    else:
        p.material_index = 1
spear.data.materials.append(mat_br); spear.data.materials.append(mat_haft); spear.data.materials.append(mat_haft)
spear.data.polygons.foreach_set('use_smooth', [True]*len(spear.data.polygons))
print("SPEAR_BUILT", flush=True)
# ---- attach to RightHand bone ----
sc = bpy.context.scene
sc.frame_set(1)
hand = arm.pose.bones.get('RightHand')
if hand is None:
    hand = next((b for b in arm.pose.bones if 'Hand' in b.name and 'Right' in b.name), None)
print(f"HAND={hand.name if hand else 'NONE'}", flush=True)
arm.update_tag()
bpy.context.view_layer.update()
bone_world = arm.matrix_world @ hand.matrix
spear.parent = arm; spear.parent_type = 'BONE'; spear.parent_bone = hand.name
# desired world pose at frame 1: grip near balance point, spear angled 18deg forward of vertical, blade up
ang = math.radians(18)
desired = Matrix.Translation(bone_world.to_translation()) @ Matrix.Rotation(ang, 4, 'Y')
# grip at 40% from butt
desired = desired @ Matrix.Translation(Vector((0, 0, -L*0.40)))
spear.matrix_parent_inverse = (arm.matrix_world @ hand.matrix).inverted() @ desired
print("SPEAR_ATTACHED", flush=True)
# verify through walk: check frame 60 spear transform sane
sc.frame_set(60)
arm.update_tag(); bpy.context.view_layer.update()
loc = spear.matrix_world.to_translation()
print(f"WALK_FRAME60 spear_world=({loc.x:.2f},{loc.y:.2f},{loc.z:.2f})", flush=True)
# ---- QC renders ----
def qc(name, frame, cam, rot=35):
    sc.frame_set(frame)
    c_data = bpy.data.cameras.new('qc'); camo = bpy.data.objects.new(f'c{name}', c_data)
    camo.location = Vector(cam); camo.rotation_euler = (R(90), 0, R(rot))
    sc.collection.objects.link(camo); sc.camera = camo
    sc.render.resolution_x = 640; sc.render.resolution_y = 960
    sc.render.filepath = f"/tmp/qc_v7armed_{name}.png"
    bpy.ops.render.render(write_still=True)
    bpy.data.objects.remove(camo, do_unlink=True)
qc('walk', 60, (1.1*H, 1.1*H, 0.55*H))
qc('idle', 31, (0.8*H, 1.3*H, 0.55*H))
print("QC_DONE", flush=True)
# ---- export ARMED ----
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-V7-MO-ARMED.glb", use_selection=False,
                          export_animations=True, export_nla_strips=True, export_skins=True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-V7-MO-ARMED.fbx",
                         add_leaf_bones=False, bake_anim=True, path_mode='COPY')
print("ARMED_EXPORTED", flush=True)
