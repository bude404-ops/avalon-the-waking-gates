"""AEDAN V7.6 — THE COMPLETE BUILD (one pass):
1. Paint vertex colors on the body (verified Z-up region classifier + canon palette)
2. Strip the body material's grey bake texture -> white base (COLOR_0 drives color in three.js)
3. Delete broken spear; import clean spear prop; strip its skeleton/skin (static mesh)
4. Orient blade-up vertical, grip at RightHand bind, parent to the bone
5. Purge QC* junk; lock Hips location fcurves on all actions; export.
"""
import bpy, math, random
from mathutils import Vector, Matrix
from collections import Counter

SRC = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb'
SPEAR = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/SPEAR-SOVEREIGN-PROP.glb'
OUT = '/tmp/AEDAN-V7-6.glb'

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('armature:', arm.name, '| body:', body.name, len(body.data.vertices), 'verts', flush=True)

# ---- 1. vertex color paint (Z-up local, face points -Y) ----
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

attrs = body.data.color_attributes
vcol = attrs.new('Col', 'FLOAT_COLOR', 'POINT')
try:
    attrs.active_color = vcol
except Exception as e:
    print('active_color warn:', e, flush=True)
random.seed(404)
weath = [0.90 + 0.10*random.random() for _ in range(len(body.data.vertices))]
for i, v in enumerate(body.data.vertices):
    c = PAL[region(v.co, v.normal)]
    m = weath[i]
    vcol.data[i].color = (c[0]*m, c[1]*m, c[2]*m, 1.0)
cnt = Counter(region(v.co, v.normal) for v in body.data.vertices)
print('painted regions:', dict(cnt), flush=True)

# ---- 2. body material: strip textures, white base ----
mat = body.data.materials[0]
nt = mat.node_tree
pb = next(n for n in nt.nodes if n.bl_idname == 'ShaderNodeBsdfPrincipled')
for inp in pb.inputs:
    for l in list(inp.links):
        if l.from_node.bl_idname in ('ShaderNodeTexImage', 'ShaderNodeNormalMap'):
            nt.nodes.remove(l.from_node)
pb.inputs['Base Color'].default_value = (1.0, 1.0, 1.0, 1.0)
pb.inputs['Metallic'].default_value = 0.06
pb.inputs['Roughness'].default_value = 0.80
print('body material stripped to white base', flush=True)

# ---- 3. purge junk + delete broken spear ----
for o in list(bpy.context.scene.objects):
    if o.name.startswith('QC') or (o.type == 'MESH' and o != body and 'Spear' in o.name):
        print('removing:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)

# ---- 4. clean spear prop, static ----
sc = bpy.context.scene
sc.frame_set(1)
bpy.context.view_layer.update()
hand = next(b for b in arm.pose.bones if 'Hand' in b.name and 'Left' not in b.name)
hand_world = arm.matrix_world @ hand.matrix
hand_pos = hand_world.translation
print('hand:', hand.name, tuple(round(v,3) for v in hand_pos), flush=True)

bpy.ops.import_scene.gltf(filepath=SPEAR)
spear = next(o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body)
# strip skeleton: remove armature modifier + vertex groups; delete the prop's armature objects
for m in list(spear.modifiers):
    if m.type == 'ARMATURE':
        spear.modifiers.remove(m)
spear.vertex_groups.clear()
for o in list(bpy.context.scene.objects):
    if o.type == 'ARMATURE' and o != arm:
        print('deleting prop armature:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)
print('spear static:', spear.name, 'verts:', len(spear.data.vertices), flush=True)

# blade detection via bronze material
slots = [m.name if m else '' for m in spear.data.materials]
blade_idx = next((i for i, n in enumerate(slots) if 'ronze' in n or 'lade' in n), 0)
blade_v, other_v = [], []
for p in spear.data.polygons:
    (blade_v if p.material_index == blade_idx else other_v).extend(p.vertices)
def avg(vids):
    return sum((spear.data.vertices[i].co for i in vids), Vector()) / len(vids)
blade_c, butt_c = avg(blade_v), avg(other_v)
v_local = (blade_c - butt_c).normalized()
print('spear slots:', slots, flush=True)

# orient blade -> world +Z, grip 35% up haft at hand
q = v_local.rotation_difference(Vector((0, 0, 1)))
R = q.to_matrix().to_4x4()
grip_local_point = butt_c + (blade_c - butt_c) * 0.35
M_wanted = (Matrix.Translation(hand_pos + Vector((0.05, 0.02, 0.0))) @ R
            @ Matrix.Translation(-grip_local_point))
spear.parent = arm
spear.parent_type = 'BONE'
spear.parent_bone = hand.name
spear.matrix_parent_inverse = Matrix()
spear.matrix_basis = hand_world.inverted() @ M_wanted
bpy.context.view_layer.update()
mw = spear.matrix_world
print('spear world pos:', tuple(round(v,3) for v in mw.translation), '| blade-up:', round((mw @ blade_c).z - (mw @ butt_c).z, 3), flush=True)

# ---- 5. lock Hips + export ----
locked = 0
for act in bpy.data.actions:
    for f in act.fcurves:
        if f.data_path.startswith('pose.bones') and 'Hips' in f.data_path and 'location' in f.data_path:
            for kp in f.keyframe_points:
                kp.co[1] = 0.0
            locked += 1
print('Hips fcurves locked:', locked, flush=True)

bpy.ops.export_scene.gltf(
    filepath=OUT, export_format='GLB',
    export_animations=True, export_animation_mode='ACTIONS',
    export_anim_single_armature=True, export_skins=True, export_apply=False)
print('EXPORT_DONE', flush=True)
