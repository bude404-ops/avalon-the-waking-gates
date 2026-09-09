"""Auto-rig a cleaned humanoid GLB: slice-based bone placement + auto-weights + idle anim.
Usage: blender -b --python rig_humanoid.py -- <mesh.glb> <out_prefix>"""
import bpy, math
from mathutils import Vector
GLB, OUT = __import__('sys').argv[-2], __import__('sys').argv[-1]
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=GLB)
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH']
obj = max(meshes, key=lambda o: len(o.data.polygons))
bpy.context.view_layer.objects.active = obj
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.separate(type='LOOSE')
bpy.ops.object.mode_set(mode='OBJECT')

# gather world-space verts of the largest island (the body)
cands = [o for o in bpy.context.scene.objects if o.type == 'MESH']
body = max(cands, key=lambda o: len(o.data.polygons))
for o in cands:
    if o != body and len(o.data.polygons) < 400: bpy.data.objects.remove(o, do_unlink=True)
mw = body.matrix_world
verts = [mw @ v.co for v in body.data.vertices]
zs = [v.z for v in verts]; zmin, zmax = min(zs), max(zs); H = zmax - zmin

def slice_x(zc, band=0.02):
    lo, hi = zmin + (zc-band)*H, zmin + (zc+band)*H
    xs = [abs(v.x) for v in verts if lo <= v.z <= hi]
    return sum(xs)/len(xs) if xs else 0.0

def P(x, h): return Vector((x, 0.0, zmin + h*H))
# anatomy anchors (height fractions) + slice-based widths
sh_x = max(slice_x(0.80), 0.08*H)          # shoulder half-width
el_x = max(slice_x(0.61), 0.07*H)          # elbow x
wr_x = max(slice_x(0.46), 0.06*H)          # wrist x
hip_x = max(slice_x(0.44), 0.06*H)         # leg hip x
kn_x = max(slice_x(0.26), 0.06*H)          # knee x
an_x = max(slice_x(0.055), 0.05*H)         # ankle x
print(f"H={H:.3f} sh={sh_x:.3f} el={el_x:.3f} wr={wr_x:.3f} hip={hip_x:.3f} kn={kn_x:.3f} an={an_x:.3f}")

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
    return b
# spine chain
bone('hips',    P(0, 0.47), P(0, 0.60), None, False)
bone('spine',   P(0, 0.60), P(0, 0.68), 'hips')
bone('spine1',  P(0, 0.68), P(0, 0.76), 'spine')
bone('spine2',  P(0, 0.76), P(0, 0.83), 'spine1')
bone('neck',    P(0, 0.83), P(0, 0.88), 'spine2')
bone('head',    P(0, 0.88), P(0, 0.97), 'neck')
for s, sx in (('L', 1), ('R', -1)):
    bone(f'shoulder.{s}', P(sx*sh_x, 0.83), P(sx*sh_x, 0.80), 'spine2', False)
    bone(f'upper_arm.{s}', P(sx*sh_x, 0.80), P(sx*el_x, 0.61), f'shoulder.{s}')
    bone(f'forearm.{s}', P(sx*el_x, 0.61), P(sx*wr_x, 0.46), f'upper_arm.{s}')
    bone(f'hand.{s}', P(sx*wr_x, 0.46), P(sx*wr_x, 0.37), f'forearm.{s}')
    bone(f'upper_leg.{s}', P(sx*hip_x, 0.47), P(sx*kn_x, 0.26), 'hips', False)
    bone(f'lower_leg.{s}', P(sx*kn_x, 0.26), P(sx*an_x, 0.055), f'upper_leg.{s}')
    bone(f'foot.{s}', P(sx*an_x, 0.055), P(sx*an_x*0.8, 0.005), f'lower_leg.{s}')
bpy.ops.object.mode_set(mode='OBJECT')

# parent body + remaining gear islands to the rig with auto weights
for o in [o for o in bpy.context.scene.objects if o.type == 'MESH']:
    o.select_set(True)
bpy.context.view_layer.objects.active = rig
bpy.ops.object.parent_set(type='ARMATURE_AUTO')
print("RIGGED — auto weights applied to", len([o for o in bpy.context.scene.objects if o.type=='MESH']), "mesh objects")

# idle animation: 60-frame breathing loop
sc = bpy.context.scene; sc.frame_start, sc.frame_end = 1, 61
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='POSE')
pb = {b.name: b for b in rig.pose.bones}
def kf(bone_name, frame, rot=(0,0,0), loc=(0,0,0)):
    pb[bone_name].rotation_euler = Vector(rot)
    pb[bone_name].keyframe_insert('rotation_euler', frame=frame)
    if loc: pb[bone_name].location = Vector(loc); pb[bone_name].keyframe_insert('location', frame=frame)
amp = math.radians
for f, (bx, hx, hy) in ((1,(0,0,0)), (31,(amp(2.5),0,0)), (61,(0,0,0))):
    kf('spine2', f, rot=(bx,0,0))
    kf('head', f, rot=(bx*0.5, hy, 0))
    kf('upper_arm.L', f, rot=(0,0, -bx*0.8))
    kf('upper_arm.R', f, rot=(0,0, bx*0.8))
    kf('hips', f, loc=(0,0,-bx*0.002))
bpy.ops.object.mode_set(mode='OBJECT')
rig.animation_data.action.name = 'Idle'
print("IDLE_BAKED")

# export FBX (Unity) with the rigged meshes + GLB
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.context.scene.objects:
    if o.type in ('MESH', 'ARMATURE'): o.select_set(True)
bpy.ops.export_scene.fbx(filepath=OUT + '-RIGGED-V1.fbx', use_selection=True, add_leaf_bones=False, bake_anim=True)
bpy.ops.export_scene.gltf(filepath=OUT + '-RIGGED-V1.glb', use_selection=True, export_animations=True)
print("RIG_EXPORT_DONE")
