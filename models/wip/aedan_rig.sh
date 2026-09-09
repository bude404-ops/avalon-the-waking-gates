#!/bin/bash
set -x
exec 2>&1
mkdir -p /workspace/out
cd /workspace
echo "RIG_STAGE: apt"
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq 2>/dev/null
apt-get install -y -qq git python3-pip python3-dev libgl1 libglib2.0-0 curl unzip 2>/dev/null
echo "RIG_STAGE: repo"
git clone --quiet https://github.com/Tencent/Hunyuan3D-2.git 2>/dev/null
cd Hunyuan3D-2
echo "RIG_STAGE: pip-reqs"
pip3 install --quiet --root-user-action=ignore -r requirements.txt 2>&1 | tail -2
echo "RIG_STAGE: input"
curl -sL "$INPUT_URL" -o /workspace/input.png
python3 -c "from PIL import Image; im=Image.open('/workspace/input.png'); print('INPUT_OK', im.size)"
cat > /workspace/gen.py << 'GENPY'
import os
os.environ.setdefault('HF_HOME', '/workspace/hf')
from hy3dgen.shapegen import Hunyuan3DDiTFlowMatchingPipeline
from hy3dgen.texgen import Hunyuan3DPaintPipeline
import torch
img = '/workspace/input.png'
print('GEN_STAGE: shape-load', flush=True)
shape = Hunyuan3DDiTFlowMatchingPipeline.from_pretrained('tencent/Hunyuan3D-2', subfolder='hunyuan3d-dit-v2-0', variant='fp16', use_simplification=True)
print('GEN_STAGE: shape-run', flush=True)
mesh = shape(image=img, num_inference_steps=30, guidance_scale=5.5, octree_resolution=256, num_chunks=20000)[0]
mesh.export('/workspace/out/AEDAN-STORMCROWN-404GEN-SHAPE.glb')
print('GEN_STAGE: shape-done', flush=True)
del shape
torch.cuda.empty_cache()
try:
    print('GEN_STAGE: tex-load', flush=True)
    paint = Hunyuan3DPaintPipeline.from_pretrained('tencent/Hunyuan3D-2', subfolder='hunyuan3d-paint-v2-0', variant='fp16')
    print('GEN_STAGE: tex-run', flush=True)
    mesh = paint(mesh, image=img)
    mesh.export('/workspace/out/AEDAN-STORMCROWN-404GEN-TEXTURED.glb')
    print('GEN_STAGE: tex-done', flush=True)
except Exception as e:
    print('GEN_STAGE: tex-FAILED', repr(e), flush=True)
print('GEN_DONE', flush=True)
GENPY
echo "RIG_STAGE: gen"
python3 /workspace/gen.py
echo "RIG_STAGE: serving"
cd /workspace/out && ls -la
python3 -m http.server 8888
