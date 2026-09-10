# STAGE A: import full-detail cleaned mesh -> purge floaters -> VOXEL REMESH (watertight) -> decimate -> smart UV -> save WIP blend
import bpy
SRC = "models/generated/AEDAN-CLEANED-V1.glb"
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)
cands = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(cands, key=lambda o: len(o.data.polygons))
removed = 0
for o in cands:
    if o != body and len(o.data.polygons) < 400:
        bpy.data.objects.remove(o, do_unlink=True); removed += 1
print(f"FLOATERS_PURGED: {removed}")
# duplicate original (UV+texture carrier, kept hidden for bake)
bpy.ops.object.select_all(action='DESELECT')
body.select_set(True)
bpy.ops.object.duplicate()
orig = bpy.context.active_object
orig.name = 'ORIG_UV_CARRIER'
orig.hide_render = False
orig.hide_viewport = False
# remesh the ORIGINAL body (active object)
bpy.context.view_layer.objects.active = body
body.select_set(True)
orig.select_set(False)
import numpy as np
zs = [v.co.z for v in body.data.vertices]
H = max(zs) - min(zs)
vox = H / 500.0
print(f"REMESHING voxel_size={vox:.5f} (H={H:.3f})")
bpy.context.scene.tool_settings.voxel_size = vox
bpy.ops.object.voxel_remesh()
bm_stat = len(body.data.polygons)
print(f"REMESH_DONE faces={bm_stat}")
# audit watertightness
import bmesh
bm = bmesh.new(); bm.from_mesh(body.data)
boundary = [e for e in bm.edges if e.is_boundary]
print(f"AUDIT boundary_edges_after={len(boundary)}")
bm.free()
# decimate to ~110K
target_ratio = min(1.0, 110000.0 / max(1, bm_stat))
dm = body.modifiers.new('Decim', 'DECIMATE')
dm.ratio = target_ratio
bpy.context.view_layer.objects.active = body
bpy.ops.object.modifier_apply(modifier='Decim')
print(f"DECIMATED faces={len(body.data.polygons)}")
# smart UV
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.uv.smart_project(angle_limit=1.1519, island_margin=0.02)
bpy.ops.object.mode_set(mode='OBJECT')
print("SMART_UV_DONE")
# copy material from orig carrier to body
if orig.data.materials and body.data.materials:
    body.data.materials[0] = orig.data.materials[0]
bpy.ops.wm.save_as_mainfile(filepath="/tmp/aedan_v4_stageA.blend")
print("STAGE_A_DONE")
