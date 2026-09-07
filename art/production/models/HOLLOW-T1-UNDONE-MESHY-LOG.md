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

## V7 RE-ROLL (Sept 7 2026 — face-void canon, post-v7 approval)
- Source: art/production/3d-inputs/3DINPUT-HOLLOW-T1-UNDONE-V7-APOSE.png (= v7 approved canon)
- Meshy task: 01a07cd4-f1e3-7677-8c48-d14d1337131b (api.meshy.ai/v1/image-to-3d — NOTE: API base moved from /openapi/api/v1 to /v1; response wraps task id in "result")
- Cost: 30 credits (should_outpaint false) → balance 1,565
- The Sept 7 morning model (task 01a07a82, chest-cavity) is RETIRED as source-canon — v7 face void supersedes. Game GLB pending download; void glow never baked — mist particle + near-zero-albedo shader live in Unity.

## CHAMPION ROLL (Sept 7 2026 — 2D approved by Big same day)
- T2 UNMADE CHAMPION v7 3D roll: task 01a07ce4-58ea (api.meshy.ai/v1, 30cr, outpaint off) → balance 1,535
- Source: art/approved/HOLLOW-T2-UNMADE-CHAMPION-V7STYLE-CANON.png (Big approved 'Keep the unmade champion')
- Pending download → compress (1024/webp/draco) → Mixamo rig → Unity wire for APK
