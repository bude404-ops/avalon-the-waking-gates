import bpy
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath="models/wip/gen404-aedan/output.glb")
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH']
print("MESH_OBJECTS:", len(meshes))
assert meshes, "no mesh found"
obj = max(meshes, key=lambda o: len(o.data.polygons))
mesh = obj.data
def tris(o): return sum(len(p.vertices)-2 for p in o.data.polygons)
print("TRIS_BEFORE:", tris(obj), "VERTS:", len(mesh.vertices))
bpy.context.view_layer.objects.active = obj
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.delete_loose(use_verts=True, use_edges=True, use_faces=True)
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.remove_doubles(threshold=0.0004)
bpy.ops.mesh.dissolve_degenerate()
bpy.ops.mesh.normals_make_consistent()
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.separate(type='LOOSE')
bpy.ops.object.mode_set(mode='OBJECT')
islands = [o for o in bpy.context.scene.objects if o.type == 'MESH']
islands.sort(key=tris, reverse=True)
main = islands[0]
small = [o for o in islands[1:] if tris(o) < 400]
print("TRIS_AFTER_CLEAN:", tris(main), "ISLANDS:", len(islands), "FLOATERS_DELETED:", len(small))
for o in small:
    bpy.data.objects.remove(o, do_unlink=True)
main.name = "Aedan"
bpy.context.view_layer.objects.active = main
game = main.copy(); game.data = main.data.copy(); game.name = "Aedan_GameReady"
bpy.context.collection.objects.link(game)
bpy.context.view_layer.objects.active = game
mod = game.modifiers.new("dec", 'DECIMATE'); mod.ratio = 0.2
bpy.ops.object.modifier_apply(modifier="dec")
print("GAME_TRIS:", tris(game))
bpy.ops.object.select_all(action='DESELECT')
main.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-CLEANED-V1.glb", use_selection=True, export_animations=False)
bpy.ops.object.select_all(action='DESELECT')
game.select_set(True)
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-GAMEREADY-V1.glb", use_selection=True, export_animations=False)
print("CLEANUP_DONE")
