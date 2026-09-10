#!/usr/bin/env python3
"""AVALON FORGE — CMU mocap -> Unity FBX converter.

Doctrine (CMU MOCAP ROOT LOCK, Sept 10): CMU clips carry capture-world Hips
translation (~15.7 units vertical + 63 units forward). Lock the Hips location
fcurves to the rig bind (0,0,0 on all three axes) BEFORE export — the walk
reads through the 31 bone rotation channels; root locked to bind, limbs
carry the motion.

Usage:
    blender --background --python convert_cmu.py -- <input.bvh> <output.fbx> [scale_cm_to_m]

Input : CMU BVH (mocap.cs.cmu.edu) — the CMU skeleton uses standard bone names
        (Hips, Spine, Spine1, LeftArm/LeftForeArm/LeftHand, LeftUpLeg/...)
Output: Unity-ready FBX (Y-up, Z-forward, armature + baked animation)
"""
import bpy
import sys

argv = sys.argv[sys.argv.index("--") + 1:]
if len(argv) < 2:
    print("usage: convert_cmu.py <input.bvh> <output.fbx>")
    sys.exit(1)
src, out = argv[0], argv[1]
scale = float(argv[2]) if len(argv) > 2 else 0.45  # CMU cm -> ~m for a 0.9m figure

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_anim.bvh(filepath=src, axis_forward='-Z', axis_up='Y',
                        target='ARMATURE', scale=scale, global_scale=1.0,
                        frame_start=1, use_fps_scale=True)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')

# ---- CMU MOCAP ROOT LOCK: zero ALL Hips location channels -------------------
locked = 0
for act in bpy.data.actions:
    for f in list(act.fcurves):
        if 'location' in f.data_path and 'Hips' in f.data_path:
            for kp in f.keyframe_points:
                kp.co[1] = 0.0
            locked += 1
print(f"Hips location fcurves locked: {locked}")

bpy.context.view_layer.update()

bpy.ops.export_scene.fbx(filepath=out,
                         object_types={'ARMATURE'},
                         add_leaf_bones=False,
                         bake_anim=True, bake_anim_use_nla_strips=False,
                         bake_anim_use_all_actions=True,
                         axis_forward='Z', axis_up='Y',
                         apply_scale_options='FBX_SCALE_NONE',
                         use_mesh_modifiers=True)
print(f"CONVERTED {out}")
