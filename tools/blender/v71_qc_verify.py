"""QC the exported V7.1 GLB: full idle + walk mid + right-hand closeup (spear grip check)."""
import bpy, os
from mathutils import Vector
R = __import__('math').radians
OUT = '/tmp/v7fix'
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath='/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb')
for a in bpy.data.actions:
    for base in ('idle', 'walk'):
        if a.name.startswith(base) and a.name != base: a.name = base

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(meshes, key=lambda o: len(o.data.polygons))
spear = next((o for o in meshes if 'pear' in o.name.lower()), None)
print('body:', body.name, '| spear:', spear.name if spear else 'NONE', flush=True)

sc = bpy.context.scene
sc.render.engine = 'CYCLES'
sc.cycles.device = 'CPU'
sc.cycles.samples = 64
sc.view_settings.view_transform = 'Standard'
sc.view_settings.look = 'None'
sc.world = bpy.data.worlds.new('W'); sc.world.use_nodes = True
sc.world.node_tree.nodes['Background'].inputs[0].default_value = (0.05, 0.05, 0.055, 1)
sc.render.resolution_x = 640; sc.render.resolution_y = 960

sun = bpy.data.objects.new('Sun', bpy.data.lights.new('Sun', 'SUN'))
bpy.context.collection.objects.link(sun)
sun.data.energy = 4.0
sun.rotation_euler = (R(55), 0, R(25))
fill = bpy.data.objects.new('Fill', bpy.data.lights.new('Fill', 'SUN'))
bpy.context.collection.objects.link(fill)
fill.data.energy = 1.2
fill.data.color = (0.7, 0.75, 0.85)
fill.rotation_euler = (R(120), 0, R(205))

cam_data = bpy.data.cameras.new('QC'); cam = bpy.data.objects.new('QCcam', cam_data)
bpy.context.collection.objects.link(cam)
sc.camera = cam
focus = bpy.data.objects.new('QCfocus', None)
bpy.context.collection.objects.link(focus)
tt = cam.constraints.new('TRACK_TO'); tt.target = focus; tt.track_axis = 'TRACK_NEGATIVE_Z'; tt.up_axis = 'UP_Y'
cam_data.lens = 55

bpy.context.view_layer.update()
wverts = [body.matrix_world @ v.co for v in body.data.vertices]
cx = sum(v.x for v in wverts)/len(wverts); cy = sum(v.y for v in wverts)/len(wverts)
cz0, cz1 = min(v.z for v in wverts), max(v.z for v in wverts)
cH = cz1 - cz0

def render_view(label, act, cam_loc, f_loc, lens=55, res=(640, 960)):
    if act in bpy.data.actions:
        arm.animation_data.action = bpy.data.actions[act]
        fr = int(sum(bpy.data.actions[act].frame_range) / 2)
        sc.frame_set(fr)
    cam.location = Vector(cam_loc)
    focus.location = Vector(f_loc)
    cam_data.lens = lens
    sc.render.resolution_x, sc.render.resolution_y = res
    sc.render.filepath = os.path.join(OUT, f'v71_{label}.png')
    bpy.ops.render.render(write_still=True)
    print('RENDERED', label, flush=True)

render_view('idle', 'idle', (cx, cy - cH*2.4, cz0 + cH*0.55), (cx, cy, cz0 + cH*0.5))
render_view('walk', 'walk', (cx, cy - cH*2.4, cz0 + cH*0.55), (cx, cy, cz0 + cH*0.5))
# right-hand closeup: hand bone world position
sc.frame_set(1)
arm_eval = arm.evaluated_get(bpy.context.evaluated_depsgraph_get())
hand = arm_eval.pose.bones.get('RightHand')
hw = (arm_eval.matrix_world @ hand.matrix).translation
print('hand world:', tuple(round(c,3) for c in hw), flush=True)
render_view('hand', 'idle', (hw.x - cH*0.15, hw.y - cH*0.6, hw.z + cH*0.08),
            (hw.x + cH*0.02, hw.y + cH*0.25, hw.z + cH*0.02), lens=85, res=(640, 640))
print('QC_DONE', flush=True)
