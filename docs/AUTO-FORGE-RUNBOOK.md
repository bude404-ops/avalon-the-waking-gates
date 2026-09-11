# AUTO-FORGE RUNBOOK — Operating the Pipeline (Big's seat)

**FORGE PRIORITY LAW (Sept 11 ~6:35 PM ET, Big: 'Hold on the other class models until the sovereign is completed to final finish game quality'):**
THE SOVEREIGN IS THE SINGLE ACTIVE FORGE TARGET. Bring the Sovereign (Aedan) to FINAL, FINISHED GAME QUALITY —
mesh QC, rig polish, root-locked idle/walk mocap, materials, weapon socket, QC render, GAMED prefab, in-shell integration.
ALL other class models (Ravager, Warden, Veilborn, Weaver, Wildborn — any batch already started) are HELD until Big
rules the Sovereign done. Do not generate, rig, or forge any other class until then.


**Doctrine:** docs/AVALON-UNITY-AUTOMATED-PIPELINE.md. Big generates + verdicts.
Forge does everything else. Weapons are separate props (Weaponless Plate Law).

---

## ONE-TIME SEAT SETUP (~15 min)

1. **Unity 6000.3.x** (404-GEN requires exactly this — no upgrade needed if already on it).
2. **Pull the repo** — the pipeline lives in `unity/Assets/AvalonForge/`:
   - `Editor/Forge.cs` — the batch pipeline (Stages 2–7)
   - `Tests/ForgeTests.cs` — headless validation gates
   - `Mocap/AEDAN-idle.fbx`, `Mocap/AEDAN-walk.fbx` — root-locked CMU clips (more classes: I generate on demand)
3. **Install from Asset Store** (Package Manager → My Assets):
   - **404—GEN 3D Generator** (free) — restart Editor after install
   - **UModeler X** (free) — for the auto-rig pass
4. Copy your project's `Assets/AvalonForge/` from the repo (or open the repo's
   `unity/` folder as the project — it carries the pipeline + mocap).

---

## PER-MODEL FLOW — ZERO HUMAN STEPS (Sept 11, Big: nobody has seat access; BIGagent404 owns the whole chain)

**LAW (Sept 11 2026, Big):** no step of the class pipeline requires a human or the Unity GUI.
The seat-runbook below is RETIRED — the CI seat + sandbox tooling own everything:

1. **GENERATE (headless 404-GEN API)** — `python3 tools/gen404/gen404_mesh.py <turnaround-plate> <out_dir> detailed`
   (API replica of the plugin's own calls, public MIT source, plugin-shipped key)
2. **RIG (headless Blender)** — `python3 tools/blender/rig_humanoid.py <mesh.glb> <out_prefix>` (bpy module)
   QC: bone names + CMU ROOT LOCK law (Hips locked to bind on all axes)
3. **FORGE (CI, headless Unity)** — commit the rigged FBX to `models/generated/`, dispatch forge-build.yml
4. **SIGN + VERIFY + PUBLISH** — SIGN GUARANTEE step (apksigner re-sign, DN check, launcher verify) → vault + live channel

### 1. GENERATE (legacy GUI route — RETIRED, kept for reference)
Window > 404-GEN 3D Generator → 2D Image Prompt slot → feed the class turnaround
plate → paste the class prompt → Generate → **Convert to Mesh** → save FBX to:
`Assets/AvalonForge/Generated/<CLASS>-M.fbx`

**Ravager prompt (first run — feed `art/pending/TURNAROUND-CLASS-RAVAGER-M-V1.png`):**
```
3D character model from reference: mythic Celtic barbarian warrior, massive broad
build, weathered brutal face, thick hair and braided beard, war-shaved sides with
braided top-knot, solid closed engraved bronze breastplate with high collar line,
sleeveless arms, oxide-red torn mantle over the shoulders, layered battle-kilt
panels, tall boots, torc and knotwork spiral patterns, ash-grey and oxide-red
whisper accents, cold desaturated palette, weathered ancient bronze, front-facing
neutral pose, full body, no weapons, hands completely empty, no script or runes,
low-poly friendly topology, PBR textures
```

### 2. AUTO-RIG (UModeler X, ~2 min)
UModeler X → Rigging → auto-rig the generated mesh → humanoid skeleton aligned
T-pose. Export/keep as FBX in the same path. **QC glance:** skeleton clean,
bones named (RightHand etc. preferred — Forge fuzzy-matches).

### 3. RUN THE FORGE (no human — headless)
Windows Task Scheduler / terminal:
```
Unity.exe -batchmode -quit -projectPath "<YOUR PROJECT>" ^
  -executeMethod AvalonForge.Forge.Run ^
  -character Ravager ^
  -input Assets/AvalonForge/Generated/Ravager-M.fbx ^
  -weapon Assets/AvalonForge/Weapons/Greatsword.fbx ^
  -height 1.95
```
(Or in-Editor: **Avalon > Forge > Run (prompt)**.)

Forge then does, unattended: humanoid auto-mapping → height normalization →
Animator build (idle/walk states from the root-locked CMU clips) → weapon socket
under the right-hand bone → QC render → `Prefabs/Ravager-GAME.prefab` →
`Reports/Ravager-<timestamp>.json`.

Exit code 0 = prefab shipped to `Prefabs/`. Exit 1 = the report says exactly
which gate failed.

### 4. HEADLESS VALIDATION GATES (no human, no packages needed)
In-Editor: **Avalon > Forge > Validate Prefabs** — every gate logs PASS/FAIL.
Headless:
```
Unity.exe -batchmode -quit -projectPath "<YOUR PROJECT>" ^
  -executeMethod AvalonForge.Forge.ValidateAll
```
Exit 0 = legal to ship (avatar valid humanoid, height in class band, centered,
states built, weapon under hand, uniform scale). Report lands in
Assets/AvalonForge/Reports/validation-*.txt.

---

## TROUBLESHOOTING — "I click it and nothing happens"

1. **Open the Console (Window > General > Console).** ANY red error kills the
   whole AvalonForge menu. Most common: NUnit errors from the optional test file —
   it ships DISABLED as .txt precisely so this can't happen; don't rename it
   unless Test Framework is installed.
2. **Menu click now always shows something:** Avalon > Forge > Run (pick model)
   opens a file picker FIRST — pick the FBX you generated. Cancel = the only
   silent exit, and that's by design.
3. **"No valid humanoid rig"** = the 404-GEN mesh has no skeleton yet → UModeler X
   auto-rig first (runbook step 2), then re-run.
4. **Double-clicking run-forge.ps1 does nothing** — Windows blocks .ps1 on
   double-click by default. Use `run-forge.bat` (same folder) or run the
   PowerShell line from a terminal.
5. **Still nothing?** Send a screenshot of the Console (or copy the red text)
   — that output pinpoints the exact line to fix.

---

## VERDICT LOOP (Big only)
QC renders land in `Assets/AvalonForge/QCShots/`. Review in-app per APK-First
doctrine — Keep/Re-roll goes to me, same as 2D canon.

## HEIGHT SPECS (class builds)
Sovereign 1.90 | Ravager 1.95 (broad) | Warden 1.75 (stocky-dense) | Veilborn 1.78 (lean)
Weaver 1.72 (slim) | Wildborn 1.82 (athletic-feral) — females per class spec on request.

## WHAT'S NEXT (my side, in flight)
- More mocap clips per class (run/hit/thrust) — CMU root-locked, same doctrine
- MCP bridge: once the Unity AI package is installed, Project Settings > AI >
  Unity MCP Server — approve my connection and I drive generation + forge runs
  directly; the GUI steps above become mine too
- APK channel exporter: forged prefab → GLB → live content.json (same self-update
  shell Aedan uses) so QC happens inside the APK like everything else
