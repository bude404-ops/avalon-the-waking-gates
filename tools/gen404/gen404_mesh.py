#!/usr/bin/env python3
"""404-GEN Mesh v2 client — replica of the Unity plugin's API calls (public MIT source).
Usage: gen404_mesh.py <image.png> <out_dir> [quality: basic|standard|detailed]
"""
import sys, os, json, time, urllib.request, urllib.parse
from PIL import Image

API = "https://api-eu.404.xyz"
KEY = "6eca4068-3be6-4d30-b828-f63cda3bc35b"  # generic key shipped in the plugin (README: "generic limited API key is included")

QUALITY = {
    "basic":    ("1024", 1024),
    "standard": ("1024_cascade", 2048),
    "detailed": ("1536_cascade", 4096),
}

def multipart(fields, boundary):
    body = b""
    for name, value, ctype, fname in fields:
        body += f"--{boundary}\r\n".encode()
        if fname:
            body += f'Content-Disposition: form-data; name="{name}"; filename="{fname}"\r\n'.encode()
        else:
            body += f'Content-Disposition: form-data; name="{name}"\r\n'.encode()
        if ctype:
            body += f"Content-Type: {ctype}\r\n".encode()
        body += b"\r\n" + (value if isinstance(value, bytes) else value.encode()) + b"\r\n"
    body += f"--{boundary}--\r\n".encode()
    return body

def main():
    img_path, out_dir = sys.argv[1], sys.argv[2]
    quality = sys.argv[3] if len(sys.argv) > 3 else "detailed"
    pipeline, tex_size = QUALITY[quality]
    os.makedirs(out_dir, exist_ok=True)

    # encode image: <=2048px JPEG q90 (plugin: EncodeMeshV2Jpeg)
    im = Image.open(img_path).convert("RGB")
    if max(im.size) > 2048:
        im.thumbnail((2048, 2048), Image.LANCZOS)
    jpg_path = os.path.join(out_dir, "input.jpg")
    im.save(jpg_path, "JPEG", quality=90)
    with open(jpg_path, "rb") as f: img_bytes = f.read()
    if len(img_bytes) > 6*1024*1024: raise SystemExit("image exceeds 6MB")

    model_params = json.dumps({"pipeline_type": pipeline, "texture_size": tex_size, "face_count": 500000})
    boundary = "----UnityMeshV2Boundary" + os.urandom(8).hex()
    body = multipart([
        ("model", "404-mesh-v2", None, None),
        ("model_params", model_params, "application/json", None),
        ("seed", "42", None, None),
        ("image", img_bytes, "image/jpeg", "image.jpg"),
    ], boundary)
    req = urllib.request.Request(API + "/add_task", data=body, method="POST", headers={
        "Content-Type": f"multipart/form-data; boundary={boundary}",
        "x-api-key": KEY,
    })
    with urllib.request.urlopen(req, timeout=120) as r:
        resp = json.load(r)
    task_id = resp.get("id")
    if not task_id: raise SystemExit(f"no task id: {resp}")
    print("TASK_ID:", task_id); open(os.path.join(out_dir, "task_id.txt"), "w").write(task_id)

    # poll
    t0 = time.time()
    while time.time() - t0 < 580:
        time.sleep(15)
        u = API + "/get_status?" + urllib.parse.urlencode({"id": task_id})
        with urllib.request.urlopen(urllib.request.Request(u, headers={"x-api-key": KEY}), timeout=60) as r:
            st = json.load(r)
        print(f"[{int(time.time()-t0)}s] status={st.get('status')} msg={st.get('message','')} detail={st.get('detail','')[:120]}", flush=True)
        s = (st.get("status") or "").lower()
        if s in ("success", "completed", "done", "finished"):
            break
        if s in ("failed", "failure", "error"):
            raise SystemExit("generation failed: " + str(st))
    else:
        print("POLL_TIMEOUT — task still running, re-poll later with the task id")
        return
    u = API + "/get_result?" + urllib.parse.urlencode({"id": task_id})
    with urllib.request.urlopen(urllib.request.Request(u, headers={"x-api-key": KEY}), timeout=300) as r:
        data = r.read()
    if data[:4] != b"glTF": raise SystemExit(f"expected GLB, got {data[:8].hex()}")
    out = os.path.join(out_dir, "output.glb")
    open(out, "wb").write(data)
    print("GLB_SAVED:", out, len(data), "bytes")

if __name__ == "__main__":
    main()
