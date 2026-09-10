"""AEDAN V7.5 — clean final assembly:
- v7.1 body (vertex colors already baked in COLOR_0 lineage) + walk travel to lock
- DELETE the broken imported spear (CMU-world garbage transforms)
- Import the CLEAN spear prop, detect blade end via its bronze material,
  orient blade-up vertical, grip at the right hand bone's bind position,
  parent to the bone (static grip — hand animation carries it)
- Purge QC* junk, lock Hips location fcurves on all actions, export.
"""
import bpy
from mathutils import Vector, Matrix

SRC = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb'
SPEAR = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/SPEAR-SOVEREIGN-PROP.glb'
OUT = '/tmp/AEDAN-V7-5.glb'

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('armature:', arm.name, '| body:', body.name, flush=True)

# purge junk + delete broken spear
for o in list(bpy.context.scene.objects):
    if o.name.startswith('QC') or (o.type == 'MESH' and o != body and 'Spear' in o.name):
        print('removing:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)

# bind pose: use REST position (armature space, no pose dependency)
sc = bpy.context.scene
sc.frame_set(1)
bpy.context.view_layer.update()
hand = next(b for b in arm.pose.bones if 'Hand' in b.name and 'Left' not in b.name)
arm.update_tag(); bpy.context.view_layer.update()
hand_world = arm.matrix_world @ hand.matrix
hand_pos = hand_world.translation
print('hand bone:', hand.name, '| bind world pos:', tuple(round(v,3) for v in hand_pos), flush=True)

# import clean spear prop
bpy.ops.import_scene.gltf(filepath=SPEAR)
spear = next(o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body)
print('clean spear imported:', spear.name, flush=True)

# find blade end: verts of polys carrying the bronze material slot
spear_mesh = spear.data
mat_names = [m.name if m else '' for m in spear_mesh.materials]
print('spear material slots:', mat_names, flush=True)
blade_idx = next((i for i, n in enumerate(mat_names) if 'ronze' in n or 'lade' in n), 0)
blade_v, butt_v = [], []
for p in spear_mesh.polygons:
    (blade_v if p.material_index == blade_idx else butt_v).extend(p.vertices)
def avg_local(vids):
    vs = [spear_mesh.vertices[i].co for i in vids]
    return sum(vs, Vector()) / len(vs)
blade_c, butt_c = avg_local(blade_v), avg_local(butt_v)
v_local = (blade_c - butt_c).normalized()
L = (blade_c - butt_c).length
print(f'spear local: butt {tuple(round(v,3) for v in butt_c)} blade {tuple(round(v,3) for v in blade_c)} L={L:.3f}', flush=True)

# orient: local blade axis -> world +Z
q = v_local.rotation_difference(Vector((0, 0, 1)))
R = q.to_matrix().to_4x4()
# grip: point 35% up the haft from butt sits at the hand (+ small outward offset)
grip_local_point = butt_c + (blade_c - butt_c) * 0.35
pos = hand_pos + Vector((0.05, 0.02, 0.0)) - (R @ grip_local_point) + Vector((0, 0, 0))
M_wanted = Matrix.Translation(hand_pos + Vector((0.05, 0.02, 0))) @ R @ Matrix.Translation(-grip_local_point)

# parent to hand bone
spear.parent = arm
spear.parent_type = 'BONE'
spear.parent_bone = hand.name
spear.matrix_parent_inverse = Matrix()
spear.matrix_basis = hand_world.inverted() @ M_wanted
bpy.context.view_layer.update()
print('spear world pos:', tuple(round(v,3) for v in spear.matrix_world.translation), flush=True)
print('spear world blade z (should be >0 above hand):', flush=True)
mw = spear.matrix_world
wz = (mw @ blade_c).z - (mw @ butt_c).z
print('  blade-above-butt z span:', round(wz, 3), flush=True)

# lock Hips location fcurves on all actions
locked = 0
for act in bpy.data.actions:
    for f in act.fcurves:
        if f.data_path.startswith('pose.bones') and 'Hips' in f.data_path and 'location' in f.data_path:
            for kp in f.keyframe_points:
                kp.co[1] = 0.0
            locked += 1
print('Hips location fcurves locked:', locked, flush=True)

bpy.ops.export_scene.gltf(
    filepath=OUT, export_format='GLB',
    export_animations=True, export_animation_mode='ACTIONS',
    export_anim_single_armature=True, export_skins=True, export_apply=False)
print('EXPORT_DONE', flush=True)
