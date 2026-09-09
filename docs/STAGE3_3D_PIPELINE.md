# STAGE 3 — 3D TURNAROUND PIPELINE (Character Lock → Meshy → Mixamo → Godot/Unity)

**Active Sept 5 2026 (BudE404 order: kits refresh → codex deep-lore pass → 3D turnaround sheets).**
The 24 canonized designs + 3 Hollow champions convert to game-ready 3D via the production pipeline. Character pipeline transfers natively to BOTH engines (FBX/GLB from Mixamo imports to Godot 4 and Unity without rework).

## STEP 1 — TURNAROUND SHEETS (this stage's deliverable)
One sheet per deity, generated from its canon art as the anchor reference. Sheet spec:
- **A-POSE** front / side / back — full figure, all three views, consistent anatomy and materials across views
- **NEUTRAL LIGHT-GRAY BACKGROUND** — zero environment, zero realm, zero lighting drama (realm belongs to Stage 2; the sheet is a 3D reference, not a poster)
- **FLAT EVEN LIGHTING** — material colors read true for texturing
- **WEAPON SHOWN SEPARATE** — the element-forged weapon (greatsword/staff/champion blade) as its own prop beside the figure, never in-hand in all views (Meshy needs the body clean for rigging)
- **ZERO TEXT, zero labels, zero annotations** (standing prompt rule)
- **Identity locked to canon**: giants = primordial raw element, element-formed face, zero armor; sprites = serious-fey, Avalon human-craft armor grammar, signature headgear
- Giants rendered at humanoid standard proportions ON THE SHEET (the 9m combat scale is set in-engine, not in the sheet)

## STEP 2 — MESHY (image-to-3D)
One Meshy image-to-3D job per view set; A-pose + clean background maximizes mesh quality. Output: textured GLB. QC pass per model: silhouette match to canon, element/material identity, weapon as separate mesh.

## STEP 3 — MIXAMO (rig + animation)
Auto-rig the GLB, import the frozen combat-spec animation set: idle / walk 4 m/s / charge 8 m/s / attack (L1 swing, L2 cleave, HEAVY slam) / death. Weight = timescale 0.55–0.7x on the Mixamo clips (titan weight law). Export FBX (Unity) + GLB (Godot).

## STEP 4 — ENGINE INTEGRATION
- Godot 4: trauma camera + footstep weight + ground-slam shockwave scripts (game-feel spec, already staged)
- Unity: C# staging mirror (already staged) — no builds/spend without BudE404's explicit go (dual-engine doctrine)
- Combat scale: giants manifest at 9 m in-engine (100 ft primordial stays lore/art-side)

## ORDER (faction-by-faction, one verdict at a time — the compressed-iteration method)
**PILOT: Embermere / Vaelthorn turnaround sheet → verdict → if clean, the full Embermere four, then Galemarch → Bloomweald → Gloambarrow → Marenvale → Stonefell, then the 3 Hollow champions (enemy rigs last).**

---
**> SUPERSEDED Sept 9 2026 by docs/PIPELINES.md** — the pipeline is now 404-GEN → Unity (Godot retired). This doc kept for history.
