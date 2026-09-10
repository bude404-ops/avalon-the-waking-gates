"""AEDAN V7.2 — vertex-color paint pass (deterministic, no shader graph):
mythic-Celtic Sovereign palette per body region: engraved bronze breastplate (front),
storm-slate cloak (back), leather kilt + trousers + boots, pale skin (sleeveless arms),
dark braided hair. Muted/desaturated per doctrine. Then QC renders + export.
"""
import bpy, math, os
from mathutils import Vector
OUT = '/tmp/v7fix'
os.makedirs(OUT, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath='/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb')

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('body:', body.name, 'verts:', len(body.data.vertices), flush=True)

# ---- region classify per vertex (object space) ----
# Blender local space after glTF import: Z-up (height=+Z), face points -Y
zs = [v.co.z for v in body.data.vertices]
z0, z1 = min(zs), max(zs)
H = z1 - z0

PAL = {
    'boot':  (0.055, 0.047, 0.040),
    'leg':   (0.095, 0.083, 0.072),
    'kilt':  (0.115, 0.095, 0.078),
    'bronze':(0.20, 0.135, 0.070),
    'cloak': (0.165, 0.185, 0.205),
    'skin':  (0.62, 0.47, 0.38),
    'hair':  (0.085, 0.065, 0.055),
}

def region(co, n):
    zn = (co.z - z0) / H          # 0=feet, 1=head
    rad = math.sqrt(co.x*co.x + co.y*co.y)   # horizontal distance from spine axis
    if zn < 0.17: return 'boot'
    if zn < 0.47: return 'leg'
    if zn < 0.585: return 'kilt'
    if zn < 0.86:
        if rad > 0.14: return 'skin'          # arms hang outside the torso column
        return 'cloak' if n.y > 0.12 else 'bronze'   # +Y = backward = cloak
    if zn < 0.90: return 'skin'               # neck
    if zn < 0.94 and n.y < -0.25: return 'skin'  # face points -Y
    return 'hair'

# vertex color layer
vcol = body.data.color_attributes.new('Col', 'BYTE_COLOR', 'POINT')
import random
random.seed(404)
weath = [0.90 + 0.10 * random.random() for _ in range(len(body.data.vertices))]
for i, v in enumerate(body.data.vertices):
    n = v.normal
    r = region(v.co, n)
    c = PAL[r]
    m = weath[i]
    vcol.data[i].color = (c[0]*m, c[1]*m, c[2]*m, 1.0)

from collections import Counter
cnt = Counter(region(v.co, v.normal) for v in body.data.vertices)
print('regions:', dict(cnt), flush=True)

# ---- clean body material: white base color, vertex colors carry the palette ----
for m in list(body.data.materials):
    mat = m
    nt = mat.node_tree
    pb = next((n for n in nt.nodes if n.bl_idname == 'ShaderNodeBsdfPrincipled'), None)
    # strip ALL texture inputs (grey bake, broken normal)
    doomed = []
    for inp in pb.inputs:
        for l in list(inp.links):
            if l.from_node.bl_idname in ('ShaderNodeTexImage', 'ShaderNodeNormalMap'):
                doomed.append(l.from_node)
                nt.links.remove(l)
    for n in set(doomed):
        try: nt.nodes.remove(n)
        except Exception: pass
    at = nt.nodes.new('ShaderNodeAttribute')
    at.attribute_name = 'Col'
    nt.links.new(at.outputs['Color'], pb.inputs['Base Color'])
    pb.inputs['Metallic'].default_value = 0.06
    pb.inputs['Roughness'].default_value = 0.80
print('material cleaned', flush=True)

# ---- QC renders ----
sc = bpy.context.scene
cam_data = bpy.data.cameras.new('QC'); cam = bpy.data.objects.new('QCcam', cam_data)
bpy.context.collection.objects.link(cam)
mw = body.matrix_world
ws = [mw @ v.co for v in body.data.vertices]
cx = (min(v.x for v in ws) + max(v.x for v in ws)) / 2
cy = (min(v.y for v in ws) + max(v.y for v in ws)) / 2
cz = (min(v.z for v in ws) + max(v.z for v in ws)) / 2
WH = max(v.z for v in ws) - min(v.z for v in ws)
target = bpy.data.objects.new('QCtarget', None)
bpy.context.collection.objects.link(target)
target.location = Vector((cx, cy, cz))
cam.location = Vector((cx + 0.25 * WH, cy - 3.2 * WH, cz + 0.15 * WH))
tt = cam.constraints.new('TRACK_TO'); tt.target = target; tt.track_axis = 'TRACK_NEGATIVE_Z'; tt.up_axis = 'UP_Y'
cam_data.lens = 55
sc.camera = cam
print(f'QC CAM center=({cx:.2f},{cy:.2f},{cz:.2f}) H={WH:.2f}', flush=True)
sun = bpy.data.objects.new('Sun', bpy.data.lights.new('Sun', 'SUN'))
bpy.context.collection.objects.link(sun)
sun.data.energy = 6.0; sun.rotation_euler = (math.radians(50), 0, math.radians(28))
sc.render.resolution_x = 640; sc.render.resolution_y = 960
sc.render.engine = 'CYCLES'; sc.cycles.device = 'CPU'; sc.cycles.samples = 32
w = bpy.data.worlds.new('W'); sc.world = w; w.use_nodes = True
w.node_tree.nodes['Background'].inputs[0].default_value = (0.05, 0.05, 0.055, 1)
w.node_tree.nodes['Background'].inputs[1].default_value = 1.0

for label, fr in (('idle', 1), ('walk', 80)):
    sc.frame_set(fr)
    sc.render.filepath = os.path.join(OUT, f'v72_{label}.png')
    bpy.ops.render.render(write_still=True)
    print('RENDERED', label, flush=True)

# ---- export ----
bpy.ops.export_scene.gltf(
    filepath='/tmp/AEDAN-V7-2.glb',
    export_format='GLB', export_animations=True, export_animation_mode='ACTIONS',
    export_anim_single_armature=True, export_skins=True, export_apply=False)
print('EXPORT_DONE', flush=True)
