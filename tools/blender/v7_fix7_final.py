"""AEDAN V7.7 — FINAL (lessons applied):
1. v7.1 import; purge QC junk + broken spear
2. spear prop: APPLY its armature modifier at bind (bakes the skinned
   placement in the hand), strip vertex groups, delete the duplicate
   armature, then parent the static spear to the RightHand bone
   -> exports as a child of the hand joint, rides hand animation
3. paint canon palette vertex colors (Z-up classifier)
4. WIRE the color attribute into the body material's Base Color
   (the exporter skips vertex colors the material doesn't use!)
5. lock Hips location fcurves -> walk in place
6. export
"""
import bpy, math, random
from mathutils import Vector, Matrix
from collections import Counter

SRC = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb'
SPEAR = '/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/SPEAR-SOVEREIGN-PROP.glb'
OUT = '/tmp/AEDAN-V7-7.glb'

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=SRC)

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
body = max([o for o in bpy.context.scene.objects if o.type == 'MESH'], key=lambda o: len(o.data.polygons))
print('armature:', arm.name, '| body:', body.name, flush=True)

for o in list(bpy.context.scene.objects):
    if o.name.startswith('QC') or (o.type == 'MESH' and o != body and 'Spear' in o.name):
        print('removing:', o.name, flush=True)
        bpy.data.objects.remove(o, do_unlink=True)

# ---- spear: bake skinned placement at bind, then static parent to hand ----
bpy.ops.import_scene.gltf(filepath=SPEAR)
spear = next(o for o in bpy.context.scene.objects if o.type == 'MESH' and o != body)
dup_arms = [o for o in bpy.context.scene.objects if o.type == 'ARMATURE' and o != arm]
# retarget modifier to the prop armature first (as imported), then apply at bind
sc = bpy.context.scene
sc.frame_set(1)
bpy.context.view_layer.update()
# apply the armature modifier: freezes skinned verts at the bind placement
for m in list(spear.modifiers):
    if m.type == 'ARMATURE':
        spear.vertex_groups.clear()
        bpy.context.view_layer.objects.active = spear
        try:
            bpy.ops.object.modifier_apply(modifier=m.name)
            print('spear armature modifier applied (bind baked)', flush=True)
        except Exception as e:
            print('apply failed:', e, flush=True)
    else:
        spear.modifiers.remove(m)
# now delete the duplicate armature
for a in dup_arms:
    print('removing duplicate armature:', a.name, flush=True)
    bpy.data.objects.remove(a, do_unlink=True)
bpy.context.view_layer.update()
# measure the baked spear: blade end = bronze material verts
spear_mesh = spear.data
mat_names = [m.name if m else '' for m in spear_mesh.materials]
blade_idx = next((i for i, n in enumerate(mat_names) if 'ronze' in n or 'lade' in n), 0)
blade_v, butt_v = [], []
for p in spear_mesh.polygons:
    (blade_v if p.material_index == blade_idx else butt_v).extend(p.vertices)
avg = lambda vids: sum((spear_mesh.vertices[i].co for i in vids), Vector()) / max(1, len(vids))
blade_c, butt_c = avg(blade_v), avg(butt_v)
print('spear baked: butt', tuple(round(v,3) for v in butt_c), 'blade', tuple(round(v,3) for v in blade_c), flush=True)
# orient blade up (+Z world): build wanted world matrix
v_local = (blade_c - butt_c)
L = v_local.length
if L < 1e-6:
    print('SPEAR MEASURE FAILED — cannot orient', flush=True)
else:
    q = (v_local / L).rotation_difference(Vector((0, 0, 1)))
    # current world matrix of the (now static) spear object
    mw = spear.matrix_world.copy()
    # rebuild: keep translation of the current grip center, rotate blade to +Z
    # grip point = point 35% from butt toward blade, in world now:
    grip_world = mw @ (butt_c + (blade_c - butt_c) * 0.35)
    hand = next(b for b in arm.pose.bones if 'Hand' in b.name and 'Left' not in b.name)
    arm.update_tag(); bpy.context.view_layer.update()
    hand_world = arm.matrix_world @ hand.matrix
    # wanted: blade points +Z, grip sits at hand + tiny outward offset
    R = q.to_matrix().to_4x4()
    M_wanted = Matrix.Translation(hand_world.translation + Vector((0.05, 0.02, 0))) @ R @ Matrix.Translation(-(butt_c + (blade_c - butt_c) * 0.35))
    spear.parent = arm
    spear.parent_type = 'BONE'
    spear.parent_bone = hand.name
    spear.matrix_parent_inverse = Matrix()
    spear.matrix_basis = hand_world.inverted() @ M_wanted
    bpy.context.view_layer.update()
    print('spear parented to', hand.name, '| world pos:', tuple(round(v,3) for v in spear.matrix_world.translation), flush=True)

# ---- vertex color paint (Z-up local classifier) ----
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

attrs = body.data.color_attributes
vcol = attrs.new('Col', 'FLOAT_COLOR', 'POINT')
attrs.active_color = vcol
random.seed(404)
weath = [0.90 + 0.10*random.random() for _ in range(len(body.data.vertices))]
cnt = Counter()
for i, v in enumerate(body.data.vertices):
    c = PAL[region(v.co, v.normal)]
    m = weath[i]
    vcol.data[i].color = (c[0]*m, c[1]*m, c[2]*m, 1.0)
    cnt[region(v.co, v.normal)] += 1
print('painted regions:', dict(cnt), flush=True)

# ---- WIRE the attribute into the material (or the exporter drops it) ----
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
print('material wired: Col -> Base Color', flush=True)

# ---- lock Hips ----
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
