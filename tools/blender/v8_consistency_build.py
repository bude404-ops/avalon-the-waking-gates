"""AEDAN V8 — THE CONSISTENCY BUILD
Root cause of 'crushes into itself': the rig carries a ~2.87x object-level scale
(CMU import artifact). The exporter writes the bone chain without that scale,
but writes mesh + inverse-binds WITH it -> animation drives joints in a
miniature space while the skin expects the full-size one -> collapse.

Fix: bake ALL transforms (armature + meshes) into a single consistent space
BEFORE export. Plus the two lessons already learned:
- paint as BYTE_COLOR and WIRE Attribute->Base Color in the material
  (the exporter fakes a white COLOR_0 when the material ignores the attribute)
- spear: static mesh parented to the RightHand bone, prop skeleton stripped
"""
import bpy, math, random
from mathutils import Vector, Matrix
from collections import Counter

SRC = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb'
SPEAR = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/SPEAR-SOVEREIGN-PROP.glb'
OUT = '/tmp/AEDAN-V8.glb'

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('armature scale before bake:', tuple(round(s,3) for s in arm.scale), flush=True)

# ---- 1. BAKE ALL TRANSFORMS: one consistent world space ----
for obj in [arm]:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True, properties=True)
    obj.select_set(False)
print('armature baked. scale now:', tuple(round(s,4) for s in arm.scale), flush=True)
print('body matrix_world scale BEFORE bake:', tuple(round(v,3) for v in body.matrix_world.to_scale()), '| body data z-span:', round(z1-z0, 4) if 'z1' in dir() else 'n/a', flush=True)
print('body object matrix (pre-bake):\n', body.matrix_world, flush=True)
# find the hand bone's armature-local rest position
eb = arm.data.bones.get('RightHand')
print('RightHand edit-bone armature-space head:', tuple(round(v,3) for v in eb.head_local) if eb else 'NOT FOUND', flush=True)
import mathutils
print('armature data (edit bones) z extent:', flush=True)
zb = [b.head_local.z for b in arm.data.bones]
print('  bones z:', round(min(zb),3), '..', round(max(zb),3), flush=True)
arm.update_tag()
bpy.context.view_layer.update()

# body mesh follows the armature as child — its object transform carries the
# same legacy scale; bake it too so mesh data is in world/consistent space
mw = body.matrix_world.copy()
bpy.context.view_layer.objects.active = body
body.select_set(True)
bpy.ops.object.transform_apply(location=True, rotation=True, scale=True, properties=True)
body.select_set(False)
print('body baked.', flush=True)

# purge junk + broken spear
for o in list(bpy.context.scene.objects):
    if o.name.startswith('QC') or (o.type == 'MESH' and o != body and 'Spear' in o.name):
        print('removing:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)

# ---- 2. static spear parented to RightHand ----
sc = bpy.context.scene
sc.frame_set(1)
bpy.context.view_layer.update()
hand = next(b for b in arm.pose.bones if 'Hand' in b.name and 'Left' not in b.name)
arm.update_tag()
bpy.context.view_layer.update()
hand_world = arm.matrix_world @ hand.matrix
print('hand bone world (baked space):', tuple(round(v,3) for v in hand_world.translation), flush=True)

bpy.ops.import_scene.gltf(filepath=SPEAR)
spear = next(o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body)
for m in list(spear.modifiers):
    spear.modifiers.remove(m)
spear.vertex_groups.clear()
for o in list(bpy.context.scene.objects):
    if o.type == 'ARMATURE' and o != arm:
        print('deleting prop armature:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)
spear_mesh = spear.data
mat_names = [m.name if m else '' for m in spear_mesh.materials]
blade_idx = next((i for i, n in enumerate(mat_names) if 'ronze' in n or 'lade' in n), 0)
blade_v, butt_v = [], []
for p in spear_mesh.polygons:
    (blade_v if p.material_index == blade_idx else butt_v).extend(p.vertices)
avg = lambda vids: sum((spear_mesh.vertices[i].co for i in vids), Vector()) / max(1, len(vids))
blade_c, butt_c = avg(blade_v), avg(butt_v)
v_dir = (blade_c - butt_c)
L = v_dir.length
if L < 1e-6:
    raise SystemExit('SPEAR MEASURE FAILED')
q = (v_dir / L).rotation_difference(Vector((0, 0, 1)))
grip_local = butt_c + v_dir * 0.35
M_wanted = (Matrix.Translation(hand_world.translation + Vector((0.05, 0.02, 0.0)))
            @ q.to_matrix().to_4x4() @ Matrix.Translation(-grip_local))
spear.parent = arm
spear.parent_type = 'BONE'
spear.parent_bone = hand.name
spear.matrix_parent_inverse = Matrix()
spear.matrix_basis = hand_world.inverted() @ M_wanted
bpy.context.view_layer.update()
print('spear world:', tuple(round(v,3) for v in spear.matrix_world.translation), flush=True)

# ---- 3. paint (BYTE_COLOR + verified classifier) + WIRE into material ----
zs = [v.co.z for v in body.data.vertices]
z0, z1 = min(zs), max(zs)
print('BODY MESH DATA z-span:', round(z1-z0,3), flush=True)
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
    rad = math.sqrt(co.x*co.x + co.y*co.y)
    if zn < 0.17: return 'boot'
    if zn < 0.47: return 'leg'
    if zn < 0.585: return 'kilt'
    if zn < 0.86:
        if rad > 0.14: return 'skin'
        return 'cloak' if n.y > 0.12 else 'bronze'
    if zn < 0.90: return 'skin'
    if zn < 0.94 and n.y < -0.25: return 'skin'
    return 'hair'

vcol = body.data.color_attributes.new('Col', 'BYTE_COLOR', 'POINT')
random.seed(404)
weath = [0.90 + 0.10 * random.random() for _ in range(len(body.data.vertices))]
cnt = Counter()
for i, v in enumerate(body.data.vertices):
    r = region(v.co, v.normal)
    m = weath[i]
    c = PAL[r]
    vcol.data[i].color = (c[0]*m, c[1]*m, c[2]*m, 1.0)
    cnt[r] += 1
print('painted:', dict(cnt), flush=True)

mat = body.data.materials[0]
nt = mat.node_tree
pb = next(n for n in nt.nodes if n.bl_idname == 'ShaderNodeBsdfPrincipled')
for inp in pb.inputs:
    for l in list(inp.links):
        if l.from_node.bl_idname in ('ShaderNodeTexImage', 'ShaderNodeNormalMap', 'ShaderNodeAttribute'):
            doomed = l.from_node
            nt.links.remove(l)
            try: nt.nodes.remove(doomed)
            except Exception: pass
att = nt.nodes.new('ShaderNodeAttribute')
att.attribute_name = 'Col'
nt.links.new(att.outputs['Color'], pb.inputs['Base Color'])
pb.inputs['Metallic'].default_value = 0.06
pb.inputs['Roughness'].default_value = 0.80
print('material: Col wired to Base Color', flush=True)

# ---- 4. lock Hips ----
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
