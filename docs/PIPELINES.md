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

## 2. UNITY-FIRST 3D PIPELINE (Big's ruling, Sept 9 2026: 'Why are you not using Unity and its assets for the 3d model? Remove the old pipelines for that so you stop doing that')

**The 404-GEN image-to-3D route (Meshy/TripoSR/Hunyuan3D) is RETIRED.** The generation pipeline is removed from the repo; no mesh generation from our art.

**LOCKED 2D CANON ART (art direction bible) → UNITY ASSET STORE (source models/rigs/anims) → UNITY (retint, restyle, light, animate)**

- **Source of models:** Unity Asset Store FIRST — rigged humanoid bases, modular armor + weapon packs, creature packs, environment kits. Canon characters now come from store assets matched to canon silhouettes; the locked 2D art is the art-direction reference for choosing + customizing them (silhouette, kit, palette).
- **Customization in Unity:** materials + retints follow the standing laws — muted palette, per-Order/class accent colorways, weathering in materials; lighting follows the Lantern-Region Aura Law (cold ambient, lantern the only rich source, hue = regional).
- **Animation:** Asset Store animation packs + Unity's animation tooling.
- **QC:** every sourced asset is judged side-by-side against its locked canon key art before it goes in-game.

## 3. ENGINE — UNITY (the one engine)

- Unity handles: world building, lighting, VFX, abilities, animation, terrain, cinematics, builds.
- **Asset Store policy (UNITY-FIRST, supersedes the filler-only policy):** store assets serve everything — environments, props, sound, animation, AND canon characters (matched to canon silhouettes + customized per the law stack). The locked 2D art stays the art-direction bible and the QC bar; no asset ships without matching its canon key.
- **Lighting law in-engine:** two-layer light — cold desaturated ambient + one rich source (lantern flame / realm accent). Region colorways = Unity lighting + post-processing settings.
- Godot is RETIRED. Legacy scaffolding lives in `legacy-godot/` for history only.

## 4. GITHUB FLOW (house rule)

- One repo: `avalon-the-waking-gates` (this repo), branch `main`.
- **Every task or milestone completes with a commit + push, immediately.** Commit messages carry the verdict quotes and canon changes.
- The repo is the record: canon locks, supersessions, and pipeline decisions are all in the history.

## 5. WHAT FEEDS WHAT (the dependency map)

```
Canon art (approved, art direction) ──→ Unity Asset Store (source) ──→ Unity (restyle + animate) ──→ game
      │                        │
      └──→ STAGE2 world pass ──┴──→ in-engine staging/lighting
      └──→ docs/ law stack ──→ all future rolls (laws govern every prompt)
```

- Character roster (12 classes + 6 gods + Vera + Ravenkin): CANON-COMPLETE — Unity asset sourcing can start on heroes.
- Enemy pass: T1 complete; T2 Unmade Champion in court; queue behind it.
- STAGE2 24-realm world pass: queued after the enemy pass.
