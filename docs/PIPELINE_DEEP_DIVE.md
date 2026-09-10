# PIPELINE DEEP DIVE — How the Pros (and AI-Native Teams) Actually Do This
*Research pass ordered by Big, Sept 9 2026. Purpose: first runs land near-perfect; kill the fix-fix-fix loop.*

## 1. THE HONEST POST-MORTEM — why our runs kept needing re-work

| Symptom Big saw | Root cause in our pipeline |
|---|---|
| "Blocky form" | We WELDED/voxelized the AI mesh. Welding seals holes but keeps soup topology; voxel smoothing then flattened silhouette + armor detail. **Welding is not retopology.** Pros rebuild a clean quad surface (retopo) and let a **normal map** carry the fine detail. |
| "Looks like Quicksilver" (wrong character) | One-view generation input. 404-GEN got a single front plate — the back/side/hair/beard/armor-silhouette were GUESSES. Meshy/Tripo docs confirm: with multi-view input "the back and sides come from your actual photos, not AI guesswork." |
| "Animations are really bad" | I hand-keyed rotation math. In production, **nobody hand-keys a walk cycle from scratch** — they retarget **mocap library clips** via Unity's humanoid (Mecanim Avatar) system, then polish. Hand-keying is a final-flourish tool, not the base. |
| "Weird dome" | Flat painting forced onto an equirect skybox. Fixed v0.9.2 — proper 360 renders or real Unity scenes only. |

## 2. HOW THE PROS DO CHARACTERS (the industry standard, stage by stage)

1. **Concept with TURNTAROUNDS** — front/side/back orthographic sheets on ONE character, same pose, same lighting. Consistency is designed in HERE, not fixed in 3D later.
2. **Blockout → Hi-poly sculpt** (ZBrush class tools) — this is the detail master.
3. **RETOROPOLOGY** — rebuild as a CLEAN quad mesh (~30-80K tris for heroes). Clean edge loops at deformation zones (shoulders, hips, knees, neck). This step is where "blocky" dies.
4. **UV unwrap** — tidy shells, predictable texel density.
5. **BAKE hi → low** — normal map + AO + textures projected from the sculpt onto the clean mesh. **The trick: the detail lives in the normal map, not in the triangles.** A 60K-tri mesh reads like the 600K sculpt.
6. **PBR texturing** (Substance class) — material layers, wear, story-detail.
7. **Rig** — standard humanoid skeleton; skin-weights get manual love ONLY at hotspots (shoulders, hips, face).
8. **Animate** — mocap library retargeted onto the rig; hand-keys only for style passes.
9. **Engine import** — FBX, humanoid rig → Mecanim Avatar → any humanoid clip retargets to any humanoid character.

## 3. HOW AI-FAST TEAMS USE THE SAME PIPELINE (the "quick" lane)

- AI gen (Meshy/Tripo/Rodin class) **replaces the sculpt stage** — image → mesh + PBR texture in minutes.
- They treat the AI mesh as a **hi-poly sculpt, never the shipped mesh**: retopo → bake → done. Tripo and Meshy even build remesh into the product because that stage is where the value is.
- **Fix zones, not fix-everything**: AI meshes fail at face/hands/inner cavities — artists spend minutes THERE only.
- Multi-view (1-4 images) is the standard quality lever: orthographic front/side/back of the same character.
- Benchmarked read (2026): Meshy = most consistently production-ready; Rodin = best geometric detail if you refine manually; Tripo = fastest; single-image input = the #1 cause of "wrong character" outputs.

## 4. THE NEW AVALON PIPELINE (v2) — doctrine proposal

**Stage A — Canon key art** (locked, unchanged). Our art-direction bible + QC bar.

**Stage B — TURNAROUND SHEETS (NEW)**: per character, one consistent orthographic set: front + side + back, same pose (weaponless A-pose), same lighting, generated FROM the locked canon art so it matches. This is the generation input. *This single change is the biggest first-run accuracy win available.*

**Stage C — 404-GEN generation** from the turnaround set (Big's Unity seat per the tool law; sandbox API route as already blessed for mobile-side input).

**Stage D — RETOPOLOGY (NEW, the missing step)**: rebuild clean quads at hero density (~60-100K tris) with good edge loops at shoulders/hips/knees. Route options (Big's verdict): (a) 404-GEN's built-in remesh if it has one, (b) UModeler X auto-retopo in-editor on Big's seat (our licensed tool, law-compliant), (c) sandbox Blender quad-remesh step (flag: outside the Unity-Only tool law — needs his explicit blessing).

**Stage E — BAKE (NEW)**: project the original dense mesh's detail onto the retopo'd mesh as normal map + AO + textures. Detail moves into maps; mesh stays light. No more trying to preserve 500K triangles.

**Stage F — Fix zones**: artist/agent minutes ONLY at face + hands (+ armor seams if needed). Never whole-body rebuilds.

**Stage G — Rig + MOCAP (UPGRADED)**: standard humanoid skeleton (Mecanim-compatible), then animations come from an Asset Store mocap library retargeted via Unity's humanoid Avatar system:
- Kevin Iglesias **Human Melee Animations FREE** (Asset Store, free) — sword/melee set
- PROTOFACTOR **ANIMSET: 1 HANDED MELEE WEAPON** (Asset Store) — one-handed set that fits spear work
- Additional melee/movement packs shortlisted per class kit
Hand-keyed procedural clips (my walk/thrust/hit) = PREVIEW placeholders only, replaced by mocap before any canon call.

**Stage H — QC gate vs canon**: turnaround renders (front/side/back) vs the Stage B sheet, silhouette check first — the canon silhouette is the acceptance bar. Reviewed in the APK viewer.

**Division of labor:** Big's Unity seat = 404-GEN gen + retopo + engine import. Sandbox = turnaround sheets, specs, prompts/settings, QC renders, repo, APK viewer. Mocap packs = installed on his seat, I write the retarget + integration steps.

## 5. WHY THIS KILLS THE BACK-AND-FORTH

- Multi-view input removes the guessing (wrong silhouette class of re-rolls gone).
- Retopo + bake removes the blocky/mushy class (surface is clean BY DESIGN, detail in maps).
- Mocap removes the robotic-animation class (real human motion data, retargeted).
- Fix-zones bound the effort: minutes on face/hands, not rebuilds.
- QC gate = silhouette vs canon turnaround before anything ships to his court.

## 6. VERDICTS NEEDED FROM BIG

1. **Turnaround sheets for the Twelve?** — I roll front/side/back sets from the locked canon art (per character, one at a time, verdict-gated).
2. **Retopo route:** 404-GEN built-in / UModeler X on your seat / sandbox Blender (needs law exception).
3. **Mocap packs greenlight** — Human Melee FREE + 1-Handed Melee as the animation source for the hero set?
