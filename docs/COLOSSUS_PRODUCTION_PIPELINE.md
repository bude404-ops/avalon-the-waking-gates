> **NOTE — LEGACY WORKING NAMES.** Khaveth/Arashido et al. below are pre-Avalon-era working names kept for pipeline history. The pipeline itself is current and applies to the Twelve (canon names in docs/COLOSSI_CARDS.md). Filenames keep working names; lore uses the Name Bible canon.

> ⚠️ **UPDATED FOR THE GIANT ERA (Sept 4 2026):** the 3-stage pipeline (Stage 1 character lock on neutral background → Stage 2 world composition → Stage 3 3D turnaround) REMAINS ACTIVE, but all scale/proportion references update: giants are 100-ft PRIMORDIAL-MAX raw elemental giants (ART_DIRECTION.md v2.0), NOT 9m exaggerated-armor titans. Character Lock now locks the primordial anatomy; turnaround sheets feed the Meshy→Mixamo flow per COMBAT_SPEC §7.

# COLOSSUS PRODUCTION PIPELINE — Design Sheet System
Created September 4, 2026. Proposal pending BudE404 approval.

## THE PROBLEM

One hero image is being asked to hold 12+ doctrine rules at once (self-grown anatomy-fused armor, master-craft design, god-shaped world, Mythos Gate, grimdark tone, sigils ≤3, ember eyes, material unity, scale anchors, composition). The generator trades wins for regressions every roll — Arashido is 17 versions deep. Scene elements (camera, world, weather) and character design (anatomy, armor, weapon) are fighting each other for the model's attention in a single generation.

## THE FIX — 3-STAGE ART PIPELINE, PER DEITY

### STAGE 1 — CHARACTER LOCK (neutral background)
1. Write a **Design Card** for the deity first (one page, locked text, source of truth for all prompts): name, faction, class, weapon identity, silhouette notes, regional material, glow color, sigils (max 3 + positions), garment element, anatomy notes, doctrine checklist.
2. Roll the character on a **NEUTRAL STUDIO BACKGROUND** — plain stone dais, simple atmospheric backdrop, full figure standing, front view (plus side view pass if needed). NO world, NO Gate, NO weather, NO mortals. Iterate ONLY the character here. This is the design sheet.
3. BudE404 verdicts until KEEP → **character locked**.

### STAGE 2 — WORLD COMPOSITION (cinematic canon art)
4. With the LOCKED Stage-1 character as the reference image, roll the final cinematic: god-shaped exaggerated world, Mythos Gate conduit, tone/weather, scale anchors, distant full-figure dominance. The character can't drift — it's anchored by the approved reference.
5. BudE404 verdicts until KEEP → **canon art locked**. This is the art/approved canon image.

### STAGE 3 — 3D CONVERSION SHEET (production-ready)
6. From the locked Stage-1 design, generate the **3D turnaround sheet**: clean A-pose, front/side/back views, neutral lighting, no scene elements, weapon shown SEPARATELY beside the character (Meshy auto-rig fails when characters hold weapons — established workflow rule).
7. BudE404 approves the turnaround → it becomes the Meshy reference when he gives the explicit GO.
8. Meshy (image-to-3D, model only) → Mixamo (rig + animations, free library) → Godot/Unity import (BoneAttachment3D for weapon).

## WHY THE DOCTRINE IS ACTUALLY 3D-FRIENDLY

- **Anatomy-fused armor = one continuous mesh.** No separate plate meshes, no attachment/physics overhead, cleaner topology, easier rigging and skinning. The hardest art rule is the best possible 3D rule.
- **Turnaround sheets are exactly what image-to-3D wants** — clean orthogonal views of a consistent design beat a cinematic hero shot every time.
- **Weapons as separate props** (locked workflow fix) — generate, attach to hand bone in engine, swap easily.
- **Glow system = emissive maps + bloom.** Ember eyes, sigils, power-light all live in the emissive texture channel — cheap, and intensity can be driven by the FAITH resource in-engine (faith feeds the god = the god burns brighter).
- **Scale = 9m per combat spec**, exaggerated proportions carry over directly from the Warcraft-legendary stance.

## PILOT
F001 restart (Khaveth) runs this exact pipeline first — Design Card → neutral character lock → Mythos cinematic → 3D turnaround. Validate the system on one deity, then the roster follows.

## COST / CADENCE (per deity, once approved)
- Stage 1: 2-5 rolls (character only, fast iteration)
- Stage 2: 1-3 rolls (character anchored, scene only)
- Stage 3: 1-2 rolls (turnaround)
- 3D: ~72 credits/deity per the locked budget (30 model + 5 remesh + 5 rig + 12 anims + 20 weapon prop) — only with explicit GO.

## PIPELINE RULE — SINGLE-SUBJECT 3D INPUT (learned Sept 5, v1 GLB failure)
The image-to-3D input MUST be exactly ONE figure, ONE view — a single standing T-pose/A-pose render on a flat studio background. NEVER feed a multi-view turnaround sheet to image-to-3D: the generator meshes EVERY view on the sheet, producing fused multi-clone models (v1 Vaelthorn and Kiln-Warden both came out as 4 fused figures from 4-view sheets). The multi-view turnaround sheet remains a Stage-3 ART artifact only; the 3D input is its single-view descendant. Meshy input recipe: "Exactly ONE single figure, one single front view, no duplicates, no grid — full body, T-pose, arms straight out, weapon in hand, flat neutral background."


## STAGE 3 — 3D-INPUT SHEET RULES (locked Sept 5, BudE404)

1. **ONE SUBJECT ONLY.** The input image contains exactly one figure, one view. No multi-view grids, no turnarounds in a single frame — the generator clones every figure it sees and the 3D converter fuses all clones into the mesh. Splice or re-roll: single figure per image, always.
2. **ZERO WEAPONS in the body mesh.** Weapons are NEVER in the input sheet. A weapon merged into the body cannot be rigged separately — it moves as dead weight with whatever bone it touches, cannot be drawn, dropped, swapped, or two-handed. Instead: **weaponless base mesh → Mixamo rig → weapon as a SEPARATE mesh parented to the hand bone.** This also gives us swappable weapons (signature weapon + variants + shop skins) for free.
3. Neutral A-pose, arms slightly away from torso, open relaxed hands, flat grey studio background, full figure uncropped — the converter's dream input.

Canonical pipeline: canon art → weaponless single-figure A-pose input sheet → Meshy → weaponless base GLB → Mixamo rig → separate weapon prop (modeled from canon registry weapon) attached at hand bone → engine.