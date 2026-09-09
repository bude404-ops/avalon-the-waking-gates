import bpy, bmesh, math
from mathutils import Vector

OUT = "/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/wip"
TEX = "/app/conversations/69f56989777455158d4472f4/mythos-canonical/models/textures/CELTIC-KNOTWORK-BRONZE-TEX.png"

bpy.ops.wm.read_factory_settings(use_empty=True)
sc = bpy.context.scene
sc.render.engine = 'CYCLES'
sc.cycles.samples = 48
sc.render.resolution_x = 900; sc.render.resolution_y = 1200

def mat(name, color, rough=0.55, metal=0.0, tex=None, tex_scale=0.35, tex_bump=0.35):
    m = bpy.data.materials.new(name); m.use_nodes = True
    bs = m.node_tree.nodes['Principled BSDF']
    bs.inputs['Base Color'].default_value = (color[0], color[1], color[2], 1)
    bs.inputs['Roughness'].default_value = rough
    bs.inputs['Metallic'].default_value = metal
    if tex:
        img = bpy.data.images.load(tex)
        tn = m.node_tree.nodes.new('ShaderNodeTexImage'); tn.image = img
        mapping = m.node_tree.nodes.new('ShaderNodeMapping')
        tc = m.node_tree.nodes.new('ShaderNodeTexCoord')
        m.node_tree.links.new(tc.outputs['UV'], mapping.inputs['Vector'])
        mapping.inputs['Scale'].default_value = (tex_scale, tex_scale, 1)
        m.node_tree.links.new(mapping.outputs['Vector'], tn.inputs['Vector'])
        bump = m.node_tree.nodes.new('ShaderNodeBump')
        bump.inputs['Strength'].default_value = tex_bump
        m.node_tree.links.new(tn.outputs['Color'], bump.inputs['Height'])
        m.node_tree.links.new(tn.outputs['Color'], bs.inputs['Base Color'])
        m.node_tree.links.new(bump.outputs['Normal'], bs.inputs['Normal'])
    return m

M_SKIN   = mat('skin',   (0.72, 0.52, 0.40), rough=0.6)
M_BRONZE = mat('bronze', (0.30, 0.20, 0.11), rough=0.42, metal=0.85, tex=TEX, tex_scale=0.9, tex_bump=0.5)
M_LEATH  = mat('leather', (0.13, 0.10, 0.08), rough=0.7)
M_HAIR   = mat('hair',   (0.16, 0.11, 0.07), rough=0.65)
M_CLOTH  = mat('cloth',  (0.20, 0.22, 0.26), rough=0.85)
M_DARKC  = mat('darkcloth', (0.10, 0.10, 0.12), rough=0.9)
M_EYE    = mat('eye',    (0.02, 0.02, 0.03), rough=0.2)

def new_obj(name, data):
    o = bpy.data.objects.new(name, data)
    sc.collection.objects.link(o); return o

def smooth_subsurf(o, levels=2):
    mod = o.modifiers.new('sub', 'SUBSURF'); mod.levels = levels; mod.render_levels = levels
    for p in o.data.polygons: p.use_smooth = True
    return o

def ell(name, rx, ry, rz, loc, mat_=None):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=28, ring_count=18, radius=1, location=loc)
    o = bpy.context.active_object; o.name = name
    o.scale = (rx, ry, rz)
    smooth_subsurf(o)
    if mat_: o.data.materials.append(mat_)
    return o

def tube(name, r0, r1, p0, p1, seg=20, mat_=None):
    mesh = bpy.data.meshes.new(name)
    d = p1 - p0
    bm = bmesh.new()
    ring0, ring1 = [], []
    for i in range(seg):
        a = 2*math.pi*i/seg
        ring0.append(bm.verts.new(Vector((math.cos(a)*r0, math.sin(a)*r0, 0))))
        ring1.append(bm.verts.new(Vector((math.cos(a)*r1, math.sin(a)*r1, 0)) + d))
    for i in range(seg):
        bm.faces.new([ring0[i], ring0[(i+1)%seg], ring1[(i+1)%seg], ring1[i]])
    bm.to_mesh(mesh); bm.free()
    o = new_obj(name, mesh)
    o.location = p0
    smooth_subsurf(o)
    if mat_: o.data.materials.append(mat_)
    return o

def limb(name, p0, p1, r0, r1, mat_, seg=18, rings=10):
    mesh = bpy.data.meshes.new(name)
    d = p1 - p0; L = d.length
    dn = d.normalized() if L > 1e-6 else Vector((0, 0, 1))
    ref = Vector((0, 0, 1)) if abs(dn.z) < 0.9 else Vector((1, 0, 0))
    u = dn.cross(ref).normalized()
    v = dn.cross(u)
    bm = bmesh.new()
    rgs = []
    for j in range(rings + 1):
        t = j / rings
        r = r0 + (r1 - r0) * t
        c = dn * (L * t)
        rgs.append([bm.verts.new(c + u*(math.cos(2*math.pi*i/seg)*r) + v*(math.sin(2*math.pi*i/seg)*r))
                   for i in range(seg)])
    for j in range(rings):
        for i in range(seg):
            bm.faces.new([rgs[j][i], rgs[j][(i+1)%seg], rgs[j+1][(i+1)%seg], rgs[j+1][i]])
    for (ring, ctr, push) in ((rgs[0], Vector((0,0,0)), -r0*0.4), (rgs[-1], dn*L, r1*0.4)):
        pole = bm.verts.new(ctr + dn*push)
        for i in range(seg):
            j = (i+1) % seg
            if push < 0:
                bm.faces.new([pole, ring[i], ring[j]])
            else:
                bm.faces.new([pole, ring[j], ring[i]])
    bm.to_mesh(mesh); bm.free()
    o = new_obj(name, mesh)
    o.location = p0
    smooth_subsurf(o)
    if mat_: mesh.materials.append(mat_)
    return o

def cloak_panel(name, mat_):
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    rows = 8
    verts = [[None]*2 for _ in range(rows+1)]
    for j in range(rows+1):
        t = j / rows
        z = 1.63 - t*0.72
        y = 0.02 + t*t*0.26
        w = 0.20 + t*0.36
        for k, sx in enumerate((-1, 1)):
            verts[j][k] = bm.verts.new(Vector((sx*w, y, z)))
    for j in range(rows):
        bm.faces.new([verts[j][0], verts[j][1], verts[j+1][1], verts[j+1][0]])
    bm.to_mesh(mesh); bm.free()
    o = new_obj(name, mesh)
    sol = o.modifiers.new('sol', 'SOLIDIFY'); sol.thickness = 0.025; sol.offset = -1
    for p in mesh.polygons: p.use_smooth = True
    mesh.materials.append(mat_)
    return o

# ===== AEDAN STORMCROWN — Sovereign of Skyrend (statuesque 1.92m) =====
ell('chest', 0.21, 0.13, 0.20, Vector((0, 0, 1.38)), M_SKIN)
ell('abs', 0.145, 0.10, 0.16, Vector((0, 0, 1.12)), M_SKIN)
ell('pelvis', 0.155, 0.105, 0.115, Vector((0, 0, 0.95)), M_SKIN)
tube('neck', 0.052, 0.06, Vector((0, 0, 1.55)), Vector((0, 0, 1.63)), 16, M_SKIN)

ell('head', 0.088, 0.098, 0.115, Vector((0, 0, 1.72)), M_SKIN)
ell('jaw', 0.058, 0.062, 0.07, Vector((0, -0.008, 1.665)), M_SKIN)
for sx in (-1, 1):
    ell('eye', 0.013, 0.013, 0.008, Vector((sx*0.032, -0.085, 1.737)), M_EYE)
ell('brow', 0.055, 0.028, 0.018, Vector((0, -0.082, 1.752)), M_SKIN)
ell('nose', 0.014, 0.02, 0.038, Vector((0, -0.10, 1.708)), M_SKIN)

ell('hair_cap', 0.098, 0.108, 0.115, Vector((0, 0.008, 1.757)), M_HAIR)
limb('braid', Vector((0, 0.075, 1.76)), Vector((0, 0.105, 1.30)), 0.030, 0.008, M_HAIR)
ell('beard', 0.052, 0.048, 0.085, Vector((0, -0.048, 1.663)), M_HAIR)
ell('moustache', 0.03, 0.012, 0.014, Vector((0, -0.094, 1.714)), M_HAIR)
for zz in (1.70, 1.58, 1.46):
    bpy.ops.mesh.primitive_torus_add(major_radius=0.024, minor_radius=0.006,
        location=(0, 0.055 + (1.76-zz)*0.36, zz), rotation=(math.radians(20), 0, 0))
    ring = bpy.context.active_object; ring.name = f'ring_{zz}'
    ring.data.materials.append(M_BRONZE)
    smooth_subsurf(ring)

def arm(sx):
    sh = Vector((sx*0.215, 0, 1.48))
    el = Vector((sx*0.315, -0.02, 1.21))
    wr = Vector((sx*0.40, -0.045, 0.95))
    hd = Vector((sx*0.415, -0.05, 0.87))
    ell(f'shoulder_{sx}', 0.075, 0.062, 0.075, sh, M_SKIN)
    limb(f'upper_{sx}', sh, el, 0.060, 0.046, M_SKIN)
    limb(f'fore_{sx}', el, wr, 0.046, 0.032, M_SKIN)
    ell(f'hand_{sx}', 0.034, 0.022, 0.056, hd, M_SKIN)
arm(-1); arm(1)

def leg(sx):
    hip = Vector((sx*0.085, 0, 0.92))
    kn = Vector((sx*0.095, 0.01, 0.50))
    ank = Vector((sx*0.098, -0.01, 0.09))
    limb(f'thigh_{sx}', hip, kn, 0.082, 0.056, M_SKIN)
    limb(f'calf_{sx}', kn, ank, 0.056, 0.034, M_SKIN)
    ell(f'foot_{sx}', 0.042, 0.078, 0.034, Vector((sx*0.10, -0.05, 0.045)), M_SKIN)
leg(-1); leg(1)

# ===== ARMOR (Sovereign spec) =====
ell('breastplate', 0.226, 0.148, 0.207, Vector((0, -0.004, 1.38)), M_BRONZE)
ell('backplate', 0.202, 0.138, 0.192, Vector((0, 0.066, 1.38)), M_BRONZE)
tube('collar', 0.064, 0.066, Vector((0, 0, 1.575)), Vector((0, 0, 1.632)), 20, M_BRONZE)
for sx in (-1, 1):
    tube(f'bracer_{sx}', 0.052, 0.042, Vector((sx*0.352, -0.032, 1.06)), Vector((sx*0.386, -0.039, 0.965)), 16, M_BRONZE)
tube('waistband', 0.168, 0.158, Vector((0, 0, 1.035)), Vector((0, 0, 1.005)), 20, M_BRONZE)
for i in range(8):
    a = 2*math.pi*i/8
    x, y = math.sin(a), math.cos(a)
    limb(f'kilt_{i}', Vector((x*0.150, y*0.150, 1.005)), Vector((x*0.185, y*0.185, 0.67)), 0.046, 0.022, M_DARKC if i % 2 else M_LEATH)
tube('trouser_l', 0.060, 0.052, Vector((-0.093, 0.005, 0.52)), Vector((-0.096, -0.005, 0.24)), 14, M_DARKC)
tube('trouser_r', 0.060, 0.052, Vector((0.093, 0.005, 0.52)), Vector((0.096, -0.005, 0.24)), 14, M_DARKC)
for sx in (-1, 1):
    tube(f'boot_{sx}', 0.060, 0.042, Vector((sx*0.098, -0.008, 0.44)), Vector((sx*0.098, -0.008, 0.10)), 16, M_LEATH)
    ell(f'bootfoot_{sx}', 0.050, 0.088, 0.038, Vector((sx*0.10, -0.058, 0.048)), M_LEATH)
cloak_panel('cloak', M_CLOTH)
bpy.ops.mesh.primitive_torus_add(major_radius=0.078, minor_radius=0.013, location=(0, 0, 1.605), rotation=(math.pi/2, 0, 0))
bpy.context.active_object.name = 'torc'
bpy.context.active_object.data.materials.append(M_BRONZE)
smooth_subsurf(bpy.context.active_object)

# ===== QC RENDER =====
cam_data = bpy.data.cameras.new('cam'); cam = new_obj('cam', cam_data)
cam_data.lens = 80
key_data = bpy.data.lights.new('key', 'SUN'); key_data.energy = 3.0; key_data.angle = math.radians(35)
key = new_obj('key', key_data)
key.rotation_euler = (math.radians(50), 0, math.radians(25))
fill_data = bpy.data.lights.new('fill', 'SUN'); fill_data.energy = 0.9; fill_data.color = (0.7, 0.75, 0.85)
fill = new_obj('fill', fill_data)
fill.rotation_euler = (math.radians(120), 0, math.radians(200))

world = bpy.data.worlds.new('w'); world.use_nodes = True
world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.60, 0.60, 0.62, 1)
world.node_tree.nodes['Background'].inputs['Strength'].default_value = 1.0
sc.world = world
sc.view_settings.view_transform = 'Standard'

def render_view(name, angle_deg):
    a = math.radians(angle_deg)
    cam.location = (math.sin(a)*2.7, -math.cos(a)*2.7, 1.02)
    target = Vector((0, 0, 1.0))
    rot_quat = (target - cam.location).to_track_quat('-Z', 'Y')
    cam.rotation_euler = rot_quat.to_euler()
    sc.camera = cam
    sc.render.filepath = f"{OUT}/{name}"
    bpy.ops.render.render(write_still=True)

render_view('AEDAN-M-V1-FRONT.png', 0)
render_view('AEDAN-M-V1-THREEQ.png', 32)
render_view('AEDAN-M-V1-SIDE.png', 90)

bpy.ops.export_scene.gltf(filepath=f"{OUT}/AEDAN-STORMCROWN-V1.glb", export_format='GLB')
bpy.ops.wm.save_as_mainfile(filepath=f"{OUT}/AEDAN-V1.blend")
for o in bpy.data.objects:
    if o.type in ("MESH", "CURVE"):
        d = o.dimensions
        if max(d) > 1.2:
            print(f"LARGE: {o.name} dims=({d.x:.2f},{d.y:.2f},{d.z:.2f})")
print("EXPORT_OK")
