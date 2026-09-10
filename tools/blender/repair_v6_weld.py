"""V6 REPAIR: NO voxel remesh this time. Work the ORIGINAL 404-GEN mesh directly:
remove loose junk -> weld by distance (seals the 94K open seams) -> smooth pass ->
decimate preserving shape -> keep original UVs/texture -> save for rig."""
import bpy, bmesh, math
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-STORMCROWN-404GEN-MESH-V1.glb")
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
mesh = body.data
print(f"ORIG faces={len(mesh.polygons)} verts={len(mesh.vertices)} uv_layers={len(mesh.uv_layers)}", flush=True)
# 1. remove loose geometry + degenerate faces
bpy.context.view_layer.objects.active = body
bpy.ops.object.mode_set(mode='EDIT')
bm = bmesh.from_edit_mesh(mesh)
# delete loose verts (unconnected) and degenerate faces
bpy.ops.mesh.select_all(action='DESELECT')
bpy.ops.object.mode_set(mode='OBJECT')
import numpy as np
ms = body.modifiers.new('Weld', 'WELD')
ms.merge_threshold = 0.0018
dg = bpy.context.evaluated_depsgraph_get()
eval_obj = body.evaluated_get(dg)
n_faces = len(eval_obj.data.polygons)
print(f"WELD -> {n_faces} faces", flush=True)
# apply weld into a new mesh
bpy.ops.object.modifier_apply(modifier='Weld')
print(f"APPLIED faces={len(body.data.polygons)}", flush=True)
# 2. remove loose/degenerate after weld
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.delete_loose(use_verts=True, use_edges=True, use_faces=False)
bpy.ops.mesh.select_all(action='DESELECT')
bm = bmesh.from_edit_mesh(body.data)
deg = [f for f in bm.faces if f.calc_area() < 1e-9]
for f in deg: f.select_set(True)
bpy.ops.mesh.delete(type='FACE')
print(f"LOOSE removed, faces={len(body.data.polygons)}", flush=True)
# 3. smooth pass: Laplacian smooth, preserve shape (low repeat)
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.vertices_smooth(laplacian=False, factor=0.3, repeat=2)
print(f"SMOOTH pass faces={len(body.data.polygons)}", flush=True)
bpy.ops.object.mode_set(mode='OBJECT')
# 4. shape stats + normals
bpy.ops.object.shade_smooth()
if not body.data.uv_layers.active:
    body.data.uv_layers.new('UV')
print(f"V6_MESH faces={len(body.data.polygons)} verts={len(body.data.vertices)} uv={len(body.data.uv_layers)}", flush=True)
bpy.ops.wm.save_as_mainfile(filepath="/tmp/aedan_v6_welded.blend")
print("V6_STAGE_A_DONE", flush=True)
