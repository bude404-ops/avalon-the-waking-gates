import bpy, math, sys
from mathutils import Vector, Matrix

GLB = sys.argv[-2]
OUT = sys.argv[-1]

bpy.ops.wm.read_factory_settings(use_empty=True)
sc = bpy.context.scene
sc.render.engine = 'CYCLES'
sc.cycles.samples = 64
sc.render.resolution_x = 900; sc.render.resolution_y = 1200

bpy.ops.import_scene.gltf(filepath=GLB)

mesh_objs = [o for o in bpy.context.scene.objects if o.type == 'MESH']

def bbox(objs):
    bmin = Vector((1e9,)*3); bmax = Vector((-1e9,)*3)
    for o in objs:
        for c in o.bound_box:
            w = o.matrix_world @ Vector(c)
            bmin.x = min(bmin.x, w.x); bmax.x = max(bmax.x, w.x)
            bmin.y = min(bmin.y, w.y); bmax.y = max(bmax.y, w.y)
            bmin.z = min(bmin.z, w.z); bmax.z = max(bmax.z, w.z)
    return bmin, bmax

bmin, bmax = bbox(mesh_objs)
ext = bmax - bmin
print("imported extents:", ext)
# bring the longest horizontal-ish axis upright to Z
longest = max(range(3), key=lambda i: ext[i])
rot = None
if longest == 0:      # height along X -> rotate about Y by -90 (X->Z)
    rot = Matrix.Rotation(-math.pi/2, 4, 'Y')
elif longest == 1:    # height along Y -> rotate about X by +90 (Y->Z)
    rot = Matrix.Rotation(math.pi/2, 4, 'X')
if rot:
    for o in mesh_objs:
        o.matrix_world = rot @ o.matrix_world

bmin, bmax = bbox(mesh_objs)
center = (bmin + bmax) / 2
ext = bmax - bmin
print("upright extents:", ext, "center:", center)

# ground the model: shift so min z at 0 and centered in XY
ground = Matrix.Translation(Vector((-center.x, -center.y, -bmin.z)))
for o in mesh_objs:
    o.matrix_world = ground @ o.matrix_world
bmin, bmax = bbox(mesh_objs)
center = (bmin + bmax) / 2
size = max(ext.x, ext.z)
r = size * 2.0

def new_obj(name, data):
    o = bpy.data.objects.new(name, data); sc.collection.objects.link(o); return o
key_data = bpy.data.lights.new('key', 'SUN'); key_data.energy = 3.0; key_data.angle = math.radians(35)
key = new_obj('key', key_data); key.rotation_euler = (math.radians(50), 0, math.radians(25))
fill_data = bpy.data.lights.new('fill', 'SUN'); fill_data.energy = 1.2; fill_data.color = (0.75, 0.78, 0.88)
fill = new_obj('fill', fill_data); fill.rotation_euler = (math.radians(120), 0, math.radians(200))
world = bpy.data.worlds.new('w'); world.use_nodes = True
world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.60, 0.60, 0.62, 1)
world.node_tree.nodes['Background'].inputs['Strength'].default_value = 1.0
sc.world = world
sc.view_settings.view_transform = 'Standard'

cam_data = bpy.data.cameras.new('cam'); cam_data.lens = 60
cam = new_obj('cam', cam_data)

def render_view(name, angle_deg):
    a = math.radians(angle_deg)
    h = size * 0.45
    cam.location = Vector((math.sin(a)*r, -math.cos(a)*r, h))
    target = Vector((0, 0, size*0.45))
    rot_quat = (target - cam.location).to_track_quat('-Z', 'Y')
    cam.rotation_euler = rot_quat.to_euler()
    sc.camera = cam
    sc.render.filepath = f"{OUT}/{name}"
    bpy.ops.render.render(write_still=True)

render_view('GEN-FRONT.png', 180)
render_view('GEN-THREEQ.png', 148)
render_view('GEN-SIDE.png', 90)
print("QC_RENDER_OK")
