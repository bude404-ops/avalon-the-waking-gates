"""AEDAN V7.1 FIX (Big's verdict: 'spear isn't in his hand + model looks like silver camouflage'):
1. Bake a real basecolor map — mythic-Celtic Sovereign palette by body region (bronze breastplate,
   storm-slate cloak, leather kilt/boots, pale skin, dark braided hair), muted per doctrine.
2. Drop the broken normal map (all-black), keep MR.
3. Re-grip the spear: vertical, blade UP (canon key art read), gripped low-third of the haft,
   child-of RightHand so it holds through idle + walk.
"""
import bpy, bmesh, math, os
from mathutils import Vector, Matrix
R = math.radians
OUT = '/tmp/v7fix'
os.makedirs(OUT, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath='/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb')

arm = next(o for o in bpy.context.scene.objects if o.type == 'ARMATURE')
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(meshes, key=lambda o: len(o.data.polygons))
spears = [o for o in meshes if 'pear' in o.name.lower()]
print('body:', body.name, 'faces:', len(body.data.polygons), '| old spear:', [s.name for s in spears], flush=True)

# normalize actions
for a in bpy.data.actions:
    for base in ('idle', 'walk'):
        if a.name.startswith(base) and a.name != base: a.name = base
print('actions:', [a.name for a in bpy.data.actions], flush=True)

sc = bpy.context.scene
sc.frame_set(1)
arm.update_tag()

# ---------- 1. region-palette basecolor bake ----------
zs = [v.co.z for v in body.data.vertices]
z0, z1 = min(zs), max(zs)
H = z1 - z0

mat_paint = bpy.data.materials.new('RegionPaint')
mat_paint.use_nodes = True
nt = mat_paint.node_tree
nt.nodes.clear()
geo = nt.nodes.new('ShaderNodeNewGeometry'); tex = nt.nodes.new('ShaderNodeTexCoord')
uv = nt.nodes.new('ShaderNodeUVMap'); uv.uv_map = body.data.uv_layers.active.name
ncoord = nt.nodes.new('ShaderNodeTexCoord')
noise = nt.nodes.new('ShaderNodeTexCoord')
nz = nt.nodes.new('ShaderNodeTexNoise'); nz.inputs['Scale'].default_value = 18.0; nz.inputs['Detail'].default_value = 8.0
img = nt.nodes.new('ShaderNodeTexImage')
bake_img = bpy.data.images.new('basecolor_v71', 4096, 4096, float_buffer=False)
bake_img.generated_color = (0.5, 0.5, 0.5, 1)
img.image = bake_img
emit = nt.nodes.new('ShaderNodeEmission')
out = nt.nodes.new('ShaderNodeOutputMaterial')

sep = nt.nodes.new('ShaderNodeSeparateXYZ')      # geometry position, object space
nsep = nt.nodes.new('ShaderNodeSeparateXYZ')     # normal
vns = nt.nodes.new('ShaderNodeSeparateXYZ')
nt.links.new(geo.outputs['Position'], sep.inputs[0])
nt.links.new(geo.outputs['Normal'], nsep.inputs[0])
nt.links.new(nz.outputs['Fac'], vns.inputs[0])

def nz01(node_name):
    """normalize geometry Z from z0..z1 into 0..1"""
    s = nt.nodes.new('ShaderNodeMath'); s.operation = 'SUBTRACT'; s.inputs[1].default_value = z0
    d = nt.nodes.new('ShaderNodeMath'); d.operation = 'DIVIDE'; d.inputs[1].default_value = H
    nt.links.new(sep.outputs[2], s.inputs[0]); nt.links.new(s.outputs[0], d.inputs[0])
    return d.outputs[0]

zn = nz01('z')
# radial distance from spine axis
ax = nt.nodes.new('ShaderNodeMath'); ax.operation = 'ABSOLUTE'
nt.links.new(sep.outputs[0], ax.inputs[0])
rad = nt.nodes.new('ShaderNodeMath'); rad.operation = 'MULTIPLY'
nt.links.new(ax.outputs[0], rad.inputs[0]); nt.links.new(ax.outputs[0], rad.inputs[1])
ry = nt.nodes.new('ShaderNodeMath'); ry.operation = 'MULTIPLY'
nt.links.new(sep.outputs[1], ry.inputs[0]); nt.links.new(sep.outputs[1], ry.inputs[1])
radd = nt.nodes.new('ShaderNodeMath'); radd.operation = 'ADD'
nt.links.new(rad.outputs[0], radd.inputs[0]); nt.links.new(ry.outputs[0], radd.inputs[1])

def band(lo, hi, src):
    g = nt.nodes.new('ShaderNodeMath'); g.operation = 'GREATER_THAN'; g.inputs[1].default_value = lo
    l = nt.nodes.new('ShaderNodeMath'); l.operation = 'LESS_THAN'; l.inputs[1].default_value = hi
    m = nt.nodes.new('ShaderNodeMath'); m.operation = 'MULTIPLY'
    nt.links.new(src, g.inputs[0]); nt.links.new(src, l.inputs[0])
    nt.links.new(g.outputs[0], m.inputs[0]); nt.links.new(l.outputs[0], m.inputs[1])
    return m.outputs[0]

def col(r, g, b):
    c = nt.nodes.new('ShaderNodeRGB'); c.outputs[0].default_value = (r, g, b, 1.0)
    return c.outputs[0]

def mixc(fac, a, b):
    m = nt.nodes.new('ShaderNodeMixRGB')
    nt.links.new(fac, m.inputs['Fac']); nt.links.new(a, m.inputs['Color1']); nt.links.new(b, m.inputs['Color2'])
    return m.outputs[0]

PALETTE = {
    'hair':   (0.085, 0.065, 0.055),
    'skin':   (0.62, 0.47, 0.38),
    'bronze': (0.165, 0.11, 0.058),
    'cloak':  (0.16, 0.175, 0.195),
    'kilt':   (0.10, 0.082, 0.068),
    'leg':    (0.085, 0.075, 0.068),
    'boot':   (0.048, 0.042, 0.038),
}

# base: legs
c = col(*PALETTE['leg'])
# boots (z < 0.16)
c = mixc(band(0.0, 0.16, zn), col(*PALETTE['boot']), c)
# kilt/hips 0.47..0.60
c = mixc(band(0.47, 0.60, zn), col(*PALETTE['kilt']), c)
# torso 0.60..0.86: bronze plate front, cloak on backfacing (normal -Y)
front_torso = band(0.60, 0.86, zn)
back_mask = nt.nodes.new('ShaderNodeMath'); back_mask.operation = 'LESS_THAN'
nt.links.new(nsep.outputs[1], back_mask.inputs[0]); back_mask.inputs[1].default_value = -0.25
torso_c = mixc(front_torso, col(*PALETTE['bronze']), col(*PALETTE['cloak']))
c = mixc(front_torso, torso_c, c)
# arms outside torso column in torso band = bare skin (sleeveless law) if far from axis
arm_side = nt.nodes.new('ShaderNodeMath'); arm_side.operation = 'GREATER_THAN'; arm_side.inputs[1].default_value = 0.14
nt.links.new(radd.outputs[0], arm_side.inputs[0])
c = mixc(mixc(arm_side.outputs[0], band(0.60, 0.86, zn), nt.nodes['Math'].outputs[0] if False else arm_side.outputs[0]), col(*PALETTE['skin']), c) if False else c
# (arms: mix where radial>0.14 AND z in torso band)
arm_and = nt.nodes.new('ShaderNodeMath'); arm_and.operation = 'MULTIPLY'
nt.links.new(arm_side.outputs[0], arm_and.inputs[0]); nt.links.new(band(0.60, 0.86, zn), arm_and.inputs[1])
c = mixc(arm_and.outputs[0], col(*PALETTE['skin']), c)
# neck+head 0.86..0.95 skin
c = mixc(band(0.86, 0.95, zn), col(*PALETTE['skin']), c)
# hair 0.95+
c = mixc(band(0.95, 1.01, zn), col(*PALETTE['hair']), c)

# weathering multiply (noise 0.88..1.0)
wbase = nt.nodes.new('ShaderNodeMath'); wbase.operation = 'MULTIPLY'; wbase.inputs[1].default_value = 0.12
nt.links.new(nz.outputs['Fac'], wbase.inputs[0])
wadd = nt.nodes.new('ShaderNodeMath'); wadd.operation = 'ADD'; wadd.inputs[1].default_value = 0.88
nt.links.new(wbase.outputs[0], wadd.inputs[0])
c = mixc(wadd.outputs[0], c, c)  # multiply-ish via fac->Color2 trick: fac drives mix of c with c... use Math multiply instead
# do a true multiply:
mulm = nt.nodes.new('ShaderNodeMixRGB'); mulm.blend_type = 'MULTIPLY'; mulm.inputs['Fac'].default_value = 1.0
nt.links.new(c, mulm.inputs['Color1'])
wclamp = nt.nodes.new('ShaderNodeMath'); wclamp.operation = 'MULTIPLY'; wclamp.inputs[1].default_value = 1.0
nt.links.new(wadd.outputs[0], mulm.inputs['Color2'])
nt.links.new(wadd.outputs[0], mulm.inputs['Fac'])
final = mulm.outputs[0]

nt.links.new(final, emit.inputs[0])
nt.links.new(emit.outputs[0], out.inputs['Surface'])

# assign paint material to every body slot, remember originals
orig_mats = list(body.data.materials)
body.data.materials.clear()
body.data.materials.append(mat_paint)

# bake EMIT into bake_img (object's own material -> UV image)
sc.render.engine = 'CYCLES'
sc.cycles.device = 'CPU'
sc.cycles.samples = 16
img_node_target = img
for n in mat_paint.node_tree.nodes:
    if n.bl_idname == 'ShaderNodeTexImage':
        n.select = True
        mat_paint.node_tree.nodes.active = n
        break
bpy.ops.object.select_all(action='DESELECT')
body.select_set(True)
bpy.context.view_layer.objects.active = body
res = bpy.ops.object.bake(type='EMIT', margin=12, use_clear=True, width=4096, height=4096)
bake_img.filepath_raw = os.path.join(OUT, 'basecolor.png')
bake_img.file_format = 'PNG'
bake_img.save()
print('BAKE_DONE', res, flush=True)

# restore original materials, then put the baked image in as BaseColor
body.data.materials.clear()
for m in orig_mats:
    body.data.materials.append(m)
main_mat = orig_mats[0]
mn = main_mat.node_tree.nodes
pb = next((n for n in mn if n.bl_idname == 'ShaderNodeBsdfPrincipled'), None)
if pb is None:
    pb = mn.new('ShaderNodeBsdfPrincipled')
    mout = next(n for n in mn if n.bl_idname == 'ShaderNodeOutputMaterial')
    mn.links.new(pb.outputs[0], mout.inputs['Surface'])
# remove broken normal map (all-black)
for n in list(mn):
    if n.bl_idname == 'ShaderNodeTexImage' and n.image and 'normal' in n.image.name.lower():
        for l in list(n.outputs[0].links): main_mat.node_tree.links.remove(l)
        mn.remove(n)
        continue
    if n.bl_idname == 'ShaderNodeNormalMap':
        for l in list(n.outputs[0].links): main_mat.node_tree.links.remove(l)
        mn.remove(n)
        continue
bcopy = bake_img.copy(); bcopy.name = 'basecolor_v71_copy'
newtex = mn.new('ShaderNodeTexImage'); newtex.image = bcopy
main_mat.node_tree.links.new(newtex.outputs[0], pb.inputs['Base Color'])
pb.inputs['Metallic'].default_value = 0.08
pb.inputs['Roughness'].default_value = 0.78
print('BODY_MAT_FIXED', flush=True)

# ---------- 2. spear: rebuild vertical, gripped low-third, child-of RightHand ----------
for s in spears:
    bpy.data.objects.remove(s, do_unlink=True)

hand = arm.pose.bones.get('RightHand')
print('hand bone:', hand.name if hand else 'NONE', flush=True)

spear = bpy.data.objects.new('SovereignSpear', bpy.data.meshes.new('SpearMesh'))
bpy.context.collection.objects.link(spear)
L = 1.35 * H
bm = bmesh.new()
haft_top = L * 0.78
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.012 * H, radius2=0.012 * H, depth=haft_top, matrix=Matrix.Translation(Vector((0, 0, haft_top / 2))))
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=4, radius1=0.034 * H, radius2=0.002, depth=L * 0.20, matrix=Matrix.Translation(Vector((0, 0, haft_top + L * 0.10))))
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.019 * H, radius2=0.019 * H, depth=0.022 * H, matrix=Matrix.Translation(Vector((0, 0, haft_top + 0.011 * H))))
for frac in (0.28, 0.33, 0.38, 0.43):
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.014 * H, radius2=0.014 * H, depth=0.009 * H, matrix=Matrix.Translation(Vector((0, 0, L * frac))))
bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=True, segments=16, radius1=0.015 * H, radius2=0.015 * H, depth=0.013 * H, matrix=Matrix.Translation(Vector((0, 0, haft_top - 0.022 * H))))
bm.to_mesh(spear.data); bm.free()

mat_br = bpy.data.materials.new('BronzeDark'); mat_br.use_nodes = True
mat_br.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value = (0.28, 0.19, 0.10, 1)
mat_br.node_tree.nodes['Principled BSDF'].inputs['Metallic'].default_value = 0.85
mat_br.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value = 0.45
mat_haft = bpy.data.materials.new('HaftWood'); mat_haft.use_nodes = True
mat_haft.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value = (0.12, 0.08, 0.05, 1)
mat_haft.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value = 0.85
spear.data.materials.append(mat_br); spear.data.materials.append(mat_haft)
for p in spear.data.polygons:
    zc = sum(spear.data.vertices[vi].co.z for vi in p.vertices) / len(p.vertices)
    p.material_index = 0 if zc > haft_top - 0.005 * H else 1
spear.data.polygons.foreach_set('use_smooth', [True] * len(spear.data.polygons))

sc.frame_set(1)
arm_eval = arm.evaluated_get(bpy.context.evaluated_depsgraph_get())
hand_bone = arm_eval.pose.bones.get('RightHand')
hand_w = (arm_eval.matrix_world @ hand_bone.matrix)
grip = hand_w.translation - Vector((0, 0, 0.32 * L))   # haft bottom 32% of L below the hand
spear.location = grip
spear.rotation_euler = (0, 0, 0)
bpy.context.view_layer.update()

co = spear.constraints.new('CHILD_OF')
co.target = arm
co.subtarget = 'RightHand'
inv = (arm.matrix_world @ arm.pose.bones['RightHand'].matrix).inverted() @ spear.matrix_world
co.inverse_matrix = inv
print('SPEAR_REGRIPPED grip_z=', round(grip.z, 3), flush=True)

# ---------- 3. QC renders (Cycles CPU) ----------
cam_data = bpy.data.cameras.new('QC'); cam = bpy.data.objects.new('QCcam', cam_data)
bpy.context.collection.objects.link(cam)
sc.camera = cam
# aim at the body's real world-space bounds (armature root may be rotated)
bpy.context.view_layer.update()
wverts = [body.matrix_world @ v.co for v in body.data.vertices]
cx = sum(v.x for v in wverts)/len(wverts); cy = sum(v.y for v in wverts)/len(wverts)
cminz = min(v.z for v in wverts); cmaxz = max(v.z for v in wverts)
cH = cmaxz - cminz
dist = cH * 2.6
cam.location = Vector((cx, cy - dist, cminz + cH*0.55))
focus = bpy.data.objects.new('QCfocus', None)
bpy.context.collection.objects.link(focus)
focus.location = Vector((cx, cy, cminz + cH*0.5))
tt = cam.constraints.new('TRACK_TO'); tt.target = focus; tt.track_axis = 'TRACK_NEGATIVE_Z'; tt.up_axis = 'UP_Y'
cam_data.lens = 55
light = bpy.data.objects.new('Sun', bpy.data.lights.new('Sun', 'SUN'))
bpy.context.collection.objects.link(light)
light.data.energy = 3.0
light.rotation_euler = (R(55), 0, R(25))
sc.render.resolution_x = 640; sc.render.resolution_y = 960
sc.render.film_transparent = False
sc.world = bpy.data.worlds.new('W'); sc.world.use_nodes = True
sc.world.node_tree.nodes['Background'].inputs[0].default_value = (0.04, 0.04, 0.045, 1)

for label, act in (('idle', 'idle'), ('walk', 'walk')):
    try:
        tr = arm.animation_data.action = bpy.data.actions[act]
        fr = int((tr.frame_range[0] + tr.frame_range[1]) / 2)
        sc.frame_set(fr)
    except Exception as e:
        print('anim err', e, flush=True)
    sc.render.filepath = os.path.join(OUT, f'qc_{label}.png')
    sc.cycles.samples = 48
    bpy.ops.render.render(write_still=True)
    print('RENDERED', label, flush=True)

# ---------- 4. export ----------
bpy.ops.export_scene.gltf(
    filepath='/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/generated/AEDAN-V7-MO-ARMED.glb',
    export_format='GLB', export_animations=True, export_anim_slide_to_zero=True,
    export_apply=False, export_yup=True, export_skins=True, export_morph=False,
    export_materials='EXPORT',
    export_image_format='AUTO', export_anim_single_armature=True)
print('EXPORT_DONE', flush=True)
