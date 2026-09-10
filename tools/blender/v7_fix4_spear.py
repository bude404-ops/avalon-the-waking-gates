"""AEDAN V7.4 — FINAL ASSEMBLY FIX:
The spear survived round-trips as a ROOT node with baked CMU-world TRS keys.
Locking the Hips to bind stranded it 16 units up / 31 away -> viewer auto-frame
zoomed out -> 'model not showing'.

Fix: measure the spear's idle world transform (which was correct), express it
as a grip matrix relative to the right-hand bone, PARENT the spear to the hand
(static grip, no baked keys — the hand's animation carries it), purge QC junk
objects, lock Hips location fcurves on both actions, export.
"""
import bpy, math
from mathutils import Matrix

SRC = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb'
OUT = '/tmp/AEDAN-V7-4.glb'

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
spear = next(o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body and 'Spear' in o.name)
print('armature:', arm.name, '| body:', body.name, '| spear:', spear.name, flush=True)

# purge QC junk
for junk in [o for o in bpy.context.scene.objects if o.name.startswith('QC')]:
    print('purging junk:', junk.name, flush=True)
    bpy.data.objects.remove(junk, do_unlink=True)

# frame 1 = idle bind; measure current (baked) spear world transform + hand bone world matrix
sc = bpy.context.scene
sc.frame_set(1)
arm.update_tag(); bpy.context.view_layer.update()
pb = arm.pose.bones
hand = next(b for b in pb if 'Hand' in b.name and 'Left' not in b.name)
print('hand bone:', hand.name, flush=True)

spear_world = spear.matrix_world.copy()
hand_world = arm.matrix_world @ hand.matrix   # bone world matrix
grip = hand_world.inverted() @ spear_world    # spear transform relative to hand

# re-parent spear under the hand bone with the measured grip
spear.parent = arm
spear.parent_type = 'BONE'
spear.parent_bone = hand.name
spear.matrix_parent_inverse = Matrix()
# set local matrix so world matches the measured idle placement
# (for BONE parenting, matrix_basis drives it relative to the bone)
spear.matrix_basis = grip
arm.update_tag(); bpy.context.view_layer.update()

# strip the spear's own animation (baked CMU keys are garbage now)
if spear.animation_data:
    spear.animation_data.action = None
    spear.animation_data_clear()
    print('spear animation cleared', flush=True)
sc.frame_set(1)
arm.update_tag(); bpy.context.view_layer.update()
print('post-parent spear world:', tuple(round(v,3) for v in spear.matrix_world.translation), flush=True)
print('hand world:', tuple(round(v,3) for v in (arm.matrix_world @ hand.matrix).translation), flush=True)

# lock Hips location fcurves on all actions (walk in place, no launch)
for act in bpy.data.actions:
    fc = [f for f in act.fcurves if f.data_path.endswith('Hips') and f.data_path.startswith('pose.bones') and 'location' in f.data_path]
    for f in fc:
        for kp in f.keyframe_points:
            kp.co[1] = 0.0
    if fc:
        print('locked', act.name, 'Hips fcurves:', len(fc), flush=True)

# export
bpy.ops.export_scene.gltf(
    filepath=OUT, export_format='GLB',
    export_animations=True, export_animation_mode='ACTIONS',
    export_anim_single_armature=True, export_skins=True, export_apply=False)
print('EXPORT_DONE', flush=True)
