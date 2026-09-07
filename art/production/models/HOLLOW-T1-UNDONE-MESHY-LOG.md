# HOLLOW T1 UNDONE — Meshy production log (Sept 7 2026)
- Source: art/approved/HOLLOW-T1-UNDONE-CANON.jpg
- 3D input: art/production/3d-inputs/3DINPUT-HOLLOW-T1-UNDONE-APOSE.png (AI conversion, canon-faithful, T-pose verified)
- Meshy task: 01a07a82-3593-77ba-96c8-8ed4f41dacdd (image-to-3d, PBR on, outpaint on)
- Cost: 60 credits (outpaint doubled the base 30 — DISABLE should_outpaint on future runs to hold 30)
- Raw output: 79MB GLB (kept out of git; expires from Meshy CDN ~Sept 11)
- Game version: HOLLOW-T1-UNDONE-GAME.glb (2.8MB, webp 1024 + draco via gltf-transform)
- Quality check (vision-verified): gaunt skeletal humanoid, hollow chest cavity, tattered rags, T-pose, canon palette — MATCH. Ember glow missing in mesh (cavity renders dark) — glow gets added IN-ENGINE (Unity emissive + ember particle), which is the correct dynamic-lighting approach anyway.
- Next per Meshy spending rule: rig via Mixamo (free) → animate via Mixamo library/Unity Animator (free).

## ALT TAKE (Sept 7, 2026, second Meshy task)
- Meshy task: 01a07a81-5999-74dc-b0fe-2bd2c62b75ad (image-to-3d, no outpaint — the 30cr baseline run; submitted during API schema debug)
- Game version: HOLLOW-T1-UNDONE-ALT-GAME.glb (291KB, prune + resize 1024 + webp + draco)
- Status: archived as alternate take (earlier input). CANON = the main GAME.glb from task 01a07a82.
- Meshy credit ledger: 60 (outpaint run) + 30 (this run) = 90 spent total → balance 1,595.
