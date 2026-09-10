# STAGE B: decimate solid -> smart UV -> bake original texture onto new surface -> save WIP
import bpy, time
t0 = time.time()
bpy.ops.wm.read_factory_settings(use_empty=True)
# 1) import solid
bpy.ops.wm.obj_import(filepath="/tmp/aedan-solid-v4.obj")
solid = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
for o in [o for o in bpy.context.scene.objects if o.type == 'MESH' and o != solid]:
    bpy.data.objects.remove(o, do_unlink=True)
print(f"SOLID_IN faces={len(solid.data.polygons)}", flush=True)
# 2) import textured original carrier
bpy.ops.import_scene.gltf(filepath="models/generated/AEDAN-CLEANED-V1.glb")
cands = [o for o in bpy.context.scene.objects if o.type == 'MESH' and o != solid]
carrier = max(cands, key=lambda o: len(o.data.polygons))
for o in cands:
    if o != carrier: bpy.data.objects.remove(o, do_unlink=True)
carrier.name = 'ORIG_UV_CARRIER'
print(f"CARRIER_IN faces={len(carrier.data.polygons)}", flush=True)
# 3) decimate solid to ~110K
bpy.context.view_layer.objects.active = solid
target = 110000.0 / max(1, len(solid.data.polygons))
dm = solid.modifiers.new('Decim', 'DECIMATE')
dm.ratio = target
bpy.ops.object.modifier_apply(modifier='Decim')
print(f"DECIMATED faces={len(solid.data.polygons)}", flush=True)
# 4) smart UV on solid
bpy.context.view_layer.objects.active = solid
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.uv.smart_project(angle_limit=1.1519, island_margin=0.02)
bpy.ops.object.mode_set(mode='OBJECT')
print("SMART_UV_DONE", flush=True)
# 5) bake texture: carrier -> solid
bake_img = bpy.data.images.new('AEDAN_BAKE_V4', width=2048, height=2048)
sc = bpy.context.scene
sc.render.engine = 'CYCLES'
sc.cycles.samples = 1
sc.cycles.device = 'CPU'
sc.render.bake.margin = 16
mat = bpy.data.materials.new('AedanV4')
if len(solid.data.materials) == 0:
    solid.data.materials.append(mat)
else:
    solid.data.materials[0] = mat
mat.use_nodes = True
nt = mat.node_tree; nt.nodes.clear()
out = nt.nodes.new('ShaderNodeOutputMaterial'); bsdf = nt.nodes.new('ShaderNodeBsdfPrincipled')
tex = nt.nodes.new('ShaderNodeTexImage'); tex.image = bake_img
nt.links.new(bsdf.outputs['BSDF'], out.inputs['Surface'])
nt.links.new(tex.outputs['Color'], bsdf.inputs['Base Color'])
# ensure the solid is active + carrier selected for selected-to-active
bpy.ops.object.select_all(action='DESELECT')
carrier.select_set(True)
solid.select_set(True)
bpy.context.view_layer.objects.active = solid
sc.cycles.bake_type = 'DIFFUSE'
sc.render.bake.use_clear = True
sc.render.bake.use_pass_color = True
sc.render.bake.use_pass_direct = False
sc.render.bake.use_pass_indirect = False
sc.render.bake.use_selected_to_active = True
sc.render.bake.max_ray_distance = 0.03
sc.render.bake.cage_extrusion = 0.004 if hasattr(sc.render.bake, 'cage_extrusion') else None
print("BAKING...", flush=True)
bpy.ops.object.bake()
bake_img.filepath_raw = "/tmp/aedan_bake_v4.png"
bake_img.file_format = 'PNG'
bake_img.save()
print(f"BAKE_DONE in {time.time()-t0:.0f}s", flush=True)
# hide carrier for next stage
carrier.hide_render = True
carrier.hide_viewport = True
bpy.ops.wm.save_as_mainfile(filepath="/tmp/aedan_v4_stageB.blend")
print("STAGE_B_DONE", flush=True)
