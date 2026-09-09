# PIPELINES — Single Source of Truth
_Last updated: Sept 9, 2026 (BIGagent404, BudE404 order: "clean up our pipelines and github so it reads properly of what we are using and doing")._
_Supersedes: STAGE3_3D_PIPELINE.md (legacy, kept for history)._

---

## 1. ART CANON PIPELINE (how every canon key is made)

```
LAW STACK → PROMPT → GENERATE → P-HASH CHECK → art/pending/ → BUD'S COURT (Telegram) → VERDICT → art/approved/ → COMMIT + PUSH → NOTES + CANON-LOCKS LOG
```

1. **Law stack first.** Every roll must cite its governing laws (Mythic Celtic grammar, female law stack, primordial law, enemy-art laws, realm palettes, etc. — see ART_DIRECTION.md + the canon docs).
2. **Generate** with the full prompt discipline: zero text in image (unless the piece IS a logo/wordmark), no deity names or the word "elven" in prompts, solo-subject reads, silhouette-first at phone scale.
3. **P-hash uniqueness check** against the vault — duplicates never go to court.
4. **Court:** one piece at a time, sent to BudE404's DM (Telegram, @BIGagent404_bot).
5. **Verdict protocol:** ONLY explicit words lock canon — "Keep", "Approved", "Lock it". Notes and preferences are not locks. No next piece rolls until the current one is verdicted.
6. **On lock:** promote to `art/approved/*-CANON.*`, commit + push to main, log in CANON-LOCKS.md + vault notes.
7. **Superseded rolls** move to `art/superseded/` (never deleted — git history is the archive).

## 2. 404-GEN — 2D → 3D PIPELINE (named by BudE404, Sept 9 2026)

```
LOCKED 2D CANON ART → 404-GEN (runner: tools/404gen/run_triposr.py) → GLB → UNITY
```

- **ENGINE OF RECORD (Sept 9 test, Big-approved live run): TripoSR** — the working route. Front plate in → GLB out in ~5s, 112k faces, free. First mesh: models/404gen/SOVEREIGN-M-404GEN-TEST-V1.glb (commit e2063b2).
- **RETIRED ROUTE (removed from the pipeline per Big): Hunyuan3D-2.1 public space** — blocked behind a paid GPU quota at anonymous tier; runner deleted (run_gen.py). Textured upgrade (Meshy-class or a paid GPU tier) is a DECISION for Big, not an installed route.
- **Input:** only LOCKED canon art. The approved piece is the anchor — never a pending or superseded variant.
- **GEN INPUT = dedicated FRONT-VIEW PLATE** (single full-body subject, A-pose, clean background, zero text); the 3-view turnaround sheet = QC reference for side/back verification.
- **Output:** shape mesh + QC pass against the canon silhouette before it enters the engine (current route is shape-only; texture pass pending Big's upgrade call).
- **Rig + animate:** Mixamo auto-rig + combat animation set, or in-engine (Unity).
- **Scale note:** gods render torso-up at cinematic scale in-engine; the 9m combat manifestation is set in Unity, not in the mesh.

## 3. ENGINE — UNITY (the one engine)

- Unity handles: world building, lighting, VFX, abilities, animation, terrain, cinematics, builds.
- **Asset Store policy:** generic environment filler only (rocks, ruins, props, sound, base animation sets). NEVER canon characters — the 12 classes, 6 gods, NPCs, and the bestiary come from our locked art through 404-GEN, because no store asset matches the law stack.
- **Lighting law in-engine:** two-layer light — cold desaturated ambient + one rich source (lantern flame / realm accent). Region colorways = Unity lighting + post-processing settings.
- Godot is RETIRED. Legacy scaffolding lives in `legacy-godot/` for history only.

## 4. GITHUB FLOW (house rule)

- One repo: `avalon-the-waking-gates` (this repo), branch `main`.
- **Every task or milestone completes with a commit + push, immediately.** Commit messages carry the verdict quotes and canon changes.
- The repo is the record: canon locks, supersessions, and pipeline decisions are all in the history.

## 5. WHAT FEEDS WHAT (the dependency map)

```
Canon art (approved) ──→ 404-GEN ──→ Unity (game)
      │                        │
      └──→ STAGE2 world pass ──┴──→ in-engine staging/lighting
      └──→ docs/ law stack ──→ all future rolls (laws govern every prompt)
```

- Character roster (12 classes + 6 gods + Vera + Ravenkin): CANON-COMPLETE — 404-GEN can start on heroes.
- Enemy pass: T1 complete; T2 Unmade Champion in court; queue behind it.
- STAGE2 24-realm world pass: queued after the enemy pass.
