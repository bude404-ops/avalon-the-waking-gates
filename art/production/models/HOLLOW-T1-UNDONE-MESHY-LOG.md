# HOLLOW T1 UNDONE — Meshy production log (Sept 7 2026)
- Source: art/approved/HOLLOW-T1-UNDONE-CANON.jpg
- 3D input: art/production/3d-inputs/3DINPUT-HOLLOW-T1-UNDONE-APOSE.png (AI conversion, canon-faithful, T-pose verified)
- Meshy task: 01a07a82-3593-77ba-96c8-8ed4f41dacdd (image-to-3d, PBR on, outpaint on)
- Cost: 60 credits (outpaint doubled the base 30 — DISABLE should_outpaint on future runs to hold 30)
- Raw output: 79MB GLB (kept out of git; expires from Meshy CDN ~Sept 11)
- Game version: HOLLOW-T1-UNDONE-GAME.glb (2.8MB, webp 1024 + draco via gltf-transform)
- Quality check (vision-verified): gaunt skeletal humanoid, hollow chest cavity, tattered rags, T-pose, canon palette — MATCH. Ember glow missing in mesh (cavity renders dark) — glow gets added IN-ENGINE (Unity emissive + ember particle), which is the correct dynamic-lighting approach anyway.
- Next per Meshy spending rule: rig via Mixamo (free) → animate via Mixamo library/Unity Animator (free).
