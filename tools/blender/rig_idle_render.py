"""Live rig + amplified idle + frame render in one session (no gltf round-trip)."""
import bpy, math, time, os
from mathutils import Vector
GLB = "models/generated/AEDAN-GAMEREADY-V1.glb"
FRAMES_DIR = "/tmp/idleframes2"
R = math.radians
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=GLB)
cands = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(cands, key=lambda o: len(o.data.polygons))
for o in cands:
    if o != body and len(o.data.polygons) < 400: bpy.data.objects.remove(o, do_unlink=True)
verts = [body.matrix_world @ v.co for v in body.data.vertices]
zs = [v.z for v in verts]; zmin, zmax = min(zs), max(zs); H = zmax - zmin
def slice_x(zc, band=0.02):
    lo, hi = zmin + (zc-band)*H, zmin + (zc+band)*H
    xs = [abs(v.x) for v in verts if lo <= v.z <= hi]
    return sum(xs)/len(xs) if xs else 0.0
def P(x, h): return Vector((x, 0.0, zmin + h*H))
sh_x = max(slice_x(0.80), 0.08*H); el_x = max(slice_x(0.61), 0.07*H)
wr_x = max(slice_x(0.46), 0.06*H); hip_x = max(slice_x(0.44), 0.06*H)
kn_x = max(slice_x(0.26), 0.06*H); an_x = max(slice_x(0.055), 0.05*H)
arm = bpy.data.armatures.new('AedanRig')
rig = bpy.data.objects.new('AedanRig', arm)
bpy.context.collection.objects.link(rig)
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='EDIT')
eb = arm.edit_bones
def bone(name, head, tail, parent, connect=True):
    b = eb.new(name); b.head, b.tail = head, tail
    b.parent = eb.get(parent) if parent else None
    b.use_connect = connect and parent is not None
bone('hips', P(0,0.47), P(0,0.60), None, False)
bone('spine', P(0,0.60), P(0,0.68), 'hips')
bone('spine1', P(0,0.68), P(0,0.76), 'spine')
bone('spine2', P(0,0.76), P(0,0.83), 'spine1')
bone('neck', P(0,0.83), P(0,0.88), 'spine2')
bone('head', P(0,0.88), P(0,0.97), 'neck')
for s, sx in (('L',1),('R',-1)):
    bone(f'shoulder.{s}', P(sx*sh_x,0.83), P(sx*sh_x,0.80), 'spine2', False)
    bone(f'upper_arm.{s}', P(sx*sh_x,0.80), P(sx*el_x,0.61), f'shoulder.{s}')
    bone(f'forearm.{s}', P(sx*el_x,0.61), P(sx*wr_x,0.46), f'upper_arm.{s}')
    bone(f'hand.{s}', P(sx*wr_x,0.46), P(sx*wr_x,0.37), f'forearm.{s}')
    bone(f'upper_leg.{s}', P(sx*hip_x,0.47), P(sx*kn_x,0.26), 'hips', False)
    bone(f'lower_leg.{s}', P(sx*kn_x,0.26), P(sx*an_x,0.055), f'upper_leg.{s}')
    bone(f'foot.{s}', P(sx*an_x,0.055), P(sx*an_x*0.8,0.005), f'lower_leg.{s}')
bpy.ops.object.mode_set(mode='OBJECT')
for o in [o for o in bpy.context.scene.objects if o.type == 'MESH']: o.select_set(True)
bpy.context.view_layer.objects.active = rig
bpy.ops.object.parent_set(type='ARMATURE_AUTO')
sc = bpy.context.scene
sc.frame_start, sc.frame_end = 1, 61
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}
def kf(bn, f, rot=None, loc=None):
    b = pb[bn]
    b.rotation_mode = 'XYZ'  # CRITICAL: pose bones default to quaternion — euler set is silently ignored otherwise
    if rot is not None:
        b.rotation_euler = Vector(rot); b.keyframe_insert('rotation_euler', frame=f)
    if loc is not None:
        b.location = Vector(loc); b.keyframe_insert('location', frame=f)
for f, amp in ((1,0), (31,1), (61,0)):
    kf('spine2', f, rot=(R(4)*amp, 0, 0))
    kf('spine1', f, rot=(R(2)*amp, 0, 0))
    kf('head', f, rot=(R(-1.5)*amp, R(2)*amp, 0))
    kf('upper_arm.L', f, rot=(0, 0, R(-5)*amp))
    kf('upper_arm.R', f, rot=(0, 0, R(5)*amp))
    kf('hips', f, loc=(0, 0, -0.012*H*amp))
bpy.ops.object.mode_set(mode='OBJECT')
sc.frame_set(31)
print("PROBE_F31 spine2 euler:", tuple(round(v,3) for v in rig.pose.bones['spine2'].rotation_euler))
print("RIGGED_IDLE_V2_BAKED")
bpy.context.view_layer.update()
dims = body.dimensions
size = max(dims.x, dims.z); r = size*2.2; h = dims.z*0.45
cam_data = bpy.data.cameras.new('qc'); cam_data.lens = 60
cam = bpy.data.objects.new('qc', cam_data); sc.collection.objects.link(cam)
cam.location = Vector((0, r, h))
cam.rotation_euler = ((Vector((0,0,h)) - cam.location).to_track_quat('-Z','Y')).to_euler()
sc.camera = cam
key = bpy.data.objects.new('key', bpy.data.lights.new('key','SUN'))
key.data.energy = 3.0; key.data.angle = R(35); key.rotation_euler = (R(50), 0, R(155))
sc.collection.objects.link(key)
fill = bpy.data.objects.new('fill', bpy.data.lights.new('fill','SUN'))
fill.data.energy = 1.2; fill.data.color = (0.75, 0.78, 0.88); fill.rotation_euler = (R(120), 0, R(200))
sc.collection.objects.link(fill)
world = bpy.data.worlds.new('w'); world.use_nodes = True
world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.60, 0.60, 0.62, 1)
world.node_tree.nodes['Background'].inputs['Strength'].default_value = 1.0
sc.world = world
sc.view_settings.view_transform = 'Standard'
sc.render.engine = 'CYCLES'
sc.cycles.samples = 40
sc.render.resolution_x = 560; sc.render.resolution_y = 760
os.makedirs(FRAMES_DIR, exist_ok=True)
t0 = time.time()
for f in range(1, 62, 2):
    sc.frame_set(f)
    bpy.context.view_layer.update()
    dg = bpy.context.evaluated_depsgraph_get()
    sc.render.filepath = f"{FRAMES_DIR}/idle_{f:03d}.png"
    bpy.ops.render.render(write_still=True)
print(f"RENDER_DONE {time.time()-t0:.0f}s")
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.context.scene.objects:
    if o.type in ('MESH','ARMATURE'): o.select_set(True)
bpy.ops.export_scene.fbx(filepath="models/generated/AEDAN-RIGGED-V2.fbx", use_selection=True, add_leaf_bones=False, bake_anim=True, embed_textures=True, path_mode='COPY')
bpy.ops.export_scene.gltf(filepath="models/generated/AEDAN-RIGGED-V2.glb", use_selection=True, export_animations=True)
print("EXPORT_V2_DONE")
