"""AEDAN V7.3 — vertex-color paint v2: OVERWRITE the mesh's existing COLOR_0
(it was shipping as a grey mask and drowning my palette), wire it into the
Principled Base Color so QC renders show the truth, then export.
"""
import bpy, math, os, random
from mathutils import Vector
from collections import Counter, defaultdict
OUT = '/tmp/v7fix'
os.makedirs(OUT, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath='/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb')

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('body:', body.name, 'verts:', len(body.data.vertices), flush=True)
print('existing color attrs:', [(a.name, a.data_type, a.domain) for a in body.data.color_attributes], flush=True)

zs = [v.co.z for v in body.data.vertices]
z0, z1 = min(zs), max(zs)
H = z1 - z0

PAL = {
    'boot':  (0.055, 0.047, 0.040),
    'leg':   (0.10, 0.085, 0.072),
    'kilt':  (0.125, 0.10, 0.08),
    'bronze':(0.23, 0.145, 0.075),
    'cloak': (0.17, 0.19, 0.21),
    'skin':  (0.62, 0.47, 0.38),
    'hair':  (0.09, 0.068, 0.055),
}

def region(co, n):
    zn = (co.z - z0) / H
    rad = math.sqrt(co.x*co.x + 0.35*co.y*co.y)
    if zn < 0.17: return 'boot'
    if zn < 0.47: return 'leg'
    if zn < 0.585: return 'kilt'
    if zn < 0.86:
        if rad > 0.14: return 'skin'
        return 'cloak' if n.y < -0.12 else 'bronze'
    if zn < 0.935: return 'skin'
    return 'hair'

# use the EXISTING attribute if present, else create; then make it the active render color
attrs = body.data.color_attributes
if len(attrs):
    vcol = attrs[0] if attrs[0].data_type in ('FLOAT_COLOR', 'BYTE_COLOR') else attrs.new('Col', 'FLOAT_COLOR', 'POINT')
else:
    vcol = attrs.new('Col', 'FLOAT_COLOR', 'POINT')
try:
    attrs.active_color = vcol
except Exception as e:
    print('active_color set failed:', e, flush=True)
print('paint target:', vcol.name, vcol.data_type, flush=True)

random.seed(404)
weath = [0.90 + 0.10*random.random() for _ in range(len(body.data.vertices))]
for i, v in enumerate(body.data.vertices):
    c = PAL[region(v.co, v.normal)]
    m = weath[i]
    vcol.data[i].color = (c[0]*m, c[1]*m, c[2]*m, 1.0)
cnt = Counter(region(v.co, v.normal) for v in body.data.vertices)
print('regions:', dict(cnt), flush=True)

# ---- material: Attribute -> Base Color, strip texture inputs ----
mat = body.data.materials[0]
nt = mat.node_tree
pb = next(n for n in nt.nodes if n.bl_idname == 'ShaderNodeBsdfPrincipled')
for inp in pb.inputs:
    for l in list(inp.links):
        if l.from_node.bl_idname in ('ShaderNodeTexImage', 'ShaderNodeNormalMap', 'ShaderNodeAttribute'):
            nt.nodes.remove(l.from_node)
att = nt.nodes.new('ShaderNodeAttribute')
att.attribute_name = vcol.name
nt.links.new(att.outputs['Color'], pb.inputs['Base Color'])
pb.inputs['Metallic'].default_value = 0.06
pb.inputs['Roughness'].default_value = 0.80
print('material wired: attr -> base color', flush=True)

# spear material sanity
spear = next((o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body), None)
if spear:
    for sm in spear.data.materials:
        if sm and sm.node_tree:
            sp = next((n for n in sm.node_tree.nodes if n.bl_idname=='ShaderNodeBsdfPrincipled'), None)
            if sp:
                bc = tuple(round(x,2) for x in sp.inputs['Base Color'].default_value[:3])
                print('spear mat:', sm.name, bc, flush=True)

# ---- QC ----
sc = bpy.context.scene
cam_data = bpy.data.cameras.new('QC'); cam = bpy.data.objects.new('QCcam', cam_data)
bpy.context.collection.objects.link(cam)
sc.camera = cam
cam.location = Vector((1.0, -4.4, 1.35)); cam.rotation_euler = (math.radians(76), 0, 0)
cam_data.lens = 50
sun = bpy.data.objects.new('Sun', bpy.data.lights.new('Sun', 'SUN'))
bpy.context.collection.objects.link(sun)
sun.data.energy = 3.2; sun.rotation_euler = (math.radians(50), 0, math.radians(28))
sc.render.resolution_x = 640; sc.render.resolution_y = 960
sc.render.engine = 'CYCLES'; sc.cycles.device = 'CPU'; sc.cycles.samples = 32
w = bpy.data.worlds.new('W'); sc.world = w; w.use_nodes = True
w.node_tree.nodes['Background'].inputs[0].default_value = (0.035, 0.035, 0.04, 1)
for label, fr in (('idle', 1), ('walk', 100)):
    sc.frame_set(fr)
    sc.render.filepath = os.path.join(OUT, f'v73_{label}.png')
    bpy.ops.render.render(write_still=True)
    print('RENDERED', label, flush=True)

bpy.ops.export_scene.gltf(
    filepath='/tmp/AEDAN-V7-3.glb', export_format='GLB',
    export_animations=True, export_animation_mode='ACTIONS',
    export_anim_single_armature=True, export_skins=True, export_apply=False)
print('EXPORT_DONE', flush=True)
