"""VOXEL SOLIDIFY: patch-soup AI mesh -> watertight solid via voxelize + fill + marching cubes."""
import trimesh, numpy as np, sys, time
from scipy import ndimage
from skimage import measure
SRC = "models/generated/AEDAN-CLEANED-V1.glb"
OUT = "/tmp/aedan-solid-v4.obj"
t0 = time.time()
scene = trimesh.load(SRC, force='mesh')
V = np.asarray(scene.vertices); F = np.asarray(scene.faces)
print(f"src verts={len(V)} faces={len(F)}", flush=True)
lo = V.min(axis=0); hi = V.max(axis=0)
H = float((hi - lo).max())
RES = 300                      # grid cells along longest axis
pitch = H / RES
print(f"H={H:.3f} pitch={pitch:.5f}", flush=True)
# voxelization via ray parity (trimesh voxelize uses surface voxelization)
vox = trimesh.voxel.creation.voxelize(scene, pitch=pitch)
mat = vox.matrix.copy()
print(f"surface cells={mat.sum()}", flush=True)
# close small gaps in the shell: binary closing (3x3x3), then fill holes
mat_c = ndimage.binary_closing(mat, structure=np.ones((3,3,3)))
mat_f = ndimage.binary_fill_holes(mat_c)
print(f"filled cells={mat_f.sum()} (fill ratio {mat_f.sum()/max(1,mat_c.sum()):.3f})", flush=True)
# pad + marching cubes -> watertight
mat_p = np.pad(mat_f, 1)
verts_m, faces_m, normals, _ = measure.marching_cubes(mat_p.astype(np.float32), level=0.5)
# scale back to world: voxel index -> world coords
origin = lo.min() - 1.5*pitch
verts_m = verts_m * pitch + (origin - pitch)
mesh = trimesh.Trimesh(vertices=verts_m, faces=faces_m, process=False)
mesh.remove_unreferenced_vertices()
# drop stray far-out components (any blob far from the body centroid)
comps = mesh.split(only_watertight=False)
if len(comps) > 1:
    main = max(comps, key=lambda m: len(m.faces))
    keep = [main]
    c0 = main.centroid
    for c in comps:
        if c is main: continue
        d = np.linalg.norm(c.centroid - c0)
        if d < 0.15*H: keep.append(c)
    mesh = trimesh.util.concatenate(keep)
    print(f"components kept={len(keep)}/{len(comps)}", flush=True)
# merge + fix
mesh.merge_vertices()
trimesh.repair.fix_normals(mesh)
print(f"SOLID verts={len(mesh.vertices)} faces={len(mesh.faces)} watertight={mesh.is_watertight}", flush=True)
mesh.export(OUT)
print(f"DONE in {time.time()-t0:.1f}s -> {OUT}", flush=True)
