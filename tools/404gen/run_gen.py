#!/usr/bin/env python3
"""404-GEN Stage C runner: image -> 3D via tencent/Hunyuan3D-2.1 (free HF Space, A10G).
Usage: python3 run_gen.py <image_path> [steps]"""
import sys, json, time
from gradio_client import Client, handle_file

img_path = sys.argv[1]
steps = int(sys.argv[2]) if len(sys.argv) > 2 else 50
octree = int(sys.argv[3]) if len(sys.argv) > 3 else 256
args_octree = octree

c = Client("tencent/Hunyuan3D-2.1", verbose=False)
print(f"[404-GEN] submitting {img_path} (steps={steps})", flush=True)
t0 = time.time()
result = c.predict(
    image=handle_file(img_path),
    mv_image_front=None, mv_image_back=None, mv_image_left=None, mv_image_right=None,
    steps=steps, guidance_scale=5.0, seed=404,
    octree_resolution=args_octree, check_box_rembg=True, num_chunks=8000,
    randomize_seed=False, api_name="/generation_all",
)
print(f"[404-GEN] done in {round(time.time()-t0,1)}s", flush=True)
for x in result:
    print("[404-GEN] OUT:", str(x)[:500], flush=True)
