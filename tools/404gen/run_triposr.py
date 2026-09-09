#!/usr/bin/env python3
"""404-GEN Stage C runner (TripoSR route): image -> 3D GLB via stabilityai/TripoSR space (fast, free)."""
import sys, time
from gradio_client import Client, handle_file

img_path = sys.argv[1]
c = Client("stabilityai/TripoSR", verbose=False)
print("[404-GEN/TripoSR] preprocessing...", flush=True)
t0 = time.time()
c.predict(handle_file(img_path), api_name="/check_input_image")
processed = c.predict(handle_file(img_path), True, 0.85, api_name="/preprocess")
print("[404-GEN/TripoSR] generating mesh...", flush=True)
obj, glb = c.predict(handle_file(processed), 320, api_name="/generate")
print(f"[404-GEN/TripoSR] done in {round(time.time()-t0,1)}s", flush=True)
print("[404-GEN/TripoSR] GLB:", glb, flush=True)
print("[404-GEN/TripoSR] OBJ:", obj, flush=True)
