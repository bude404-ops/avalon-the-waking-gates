# AVALON FORGE — Ops layer (probes + runner + converter)

**Two folders, one flow:** the pipeline ENGINE lives at `tools/unity/AvalonForge.cs`
(12 class canon specs in `tools/unity/AvalonForge/specs/`, entry `AvalonForge.RunClass`,
class via `AVALON_CLASS` env var). THIS folder is the ops layer: verify probes,
Task Scheduler runner, and the CMU root-lock converter. `run-forge.ps1 -Mode pipeline`
drives the engine.

Implements `docs/UNITY-AUTOMATED-CHARACTER-PIPELINE.md`. Big's seat never needs a
person sitting in the Editor: the pipeline runs headless via batch mode, artifacts
come back through the repo, BIGagent404 inspects and iterates, verdicts happen in
the APK.

## Setup (one-time, ~10 min)

1. **Unity 6000.3.11f1 or newer** (404-GEN requires 6000.3+). Open the project once.
2. **Install 404-GEN** from the Asset Store (My Assets → download → import). Follow
   its graphics API settings (DX12 on Windows). Restart the Editor.
3. **Install packages** (Package Manager):
   - `com.unity.animation.rigging` (official, free — IK/weapon sockets)
   - a glTF importer (e.g. glTFast, if 404-GEN output lands as GLB)
   - `com.unity.nuget.newtonsoft-json` (for the config/JSON — or paste Newtonsoft in)
4. **Copy this folder into the project** as `Assets/AvalonForge/` (Editor scripts
   compile into the Editor assembly) plus `forge-config.json` at the project root.
5. Run probes once (below). The report picks Route A vs Route B.

## Run

```powershell
# First run — 404-GEN verification probes (API? rigged output? EULA? packages?)
.\run-forge.ps1 -Mode probes

# Generate the class mesh in 404-GEN (Window > 404-GEN 3D Generator) using the
# paste-prompt from docs/WEAPONS-404GEN-RUNBOOK.md / the class ref sheet,
# then Convert to Mesh -> Assets/404GEN/<Class>_M.fbx

# Then run the pipeline (Editor can be closed)
.\run-forge.ps1 -Mode pipeline -Class Sovereign

# Unattended nightly runs while the seat is idle:
.\run-forge.ps1 -Mode pipeline -Schedule "22:00"
```

Batch mode is **not** license-free: the machine must hold an activated, signed-in
Unity license. A signed-in Personal seat on Big's own machine covers headless
batch runs. Do NOT attempt rented CI — Personal manual activation is dead.

## What the pipeline does (ForgePipeline.Run)

1. Loads `forge-config.json`, resolves the class spec and source mesh.
2. Mesh inspection: vertex/tri counts, degenerate triangles, missing UVs, tri
   budget enforcement (`maxTris`).
3. Scale + orientation normalization: measured height → `targetHeightM`, yaw to
   `faceYawDeg`, recalculated bounds/normals/tangents.
4. Rig check; if unrigged → Route A transplant is flagged for the UModeler X pass.
   If rigged → `AvatarBuilder.BuildHumanAvatar` builds the humanoid avatar with the
   CMU/standard bone map (Hips, Spine, LeftArm, LeftUpLeg, ...).
5. AnimatorController + states per spec clips (idle/walk, Speed parameter).
6. Animation Rigging (when installed): weapon socket parented to the hand bone with
   the spec offset, RigBuilder layer for two-bone IK / multi-aim.
7. Prefab build: `Assets/AvalonForge/Prefabs/<Class>_FORGED.prefab`.
8. QC report JSON → `forge-artifacts/` (committed back so BIGagent404 can inspect).

## Animation input

CMU clips go through `convert_cmu.py` (Blender headless on the forge box or by
BIGagent404's sandbox — it applies the **CMU MOCAP ROOT LOCK** doctrine: Hips
translation zeroed on all axes; motion reads through the 31 rotation channels).
Output FBX drops into `Assets/AvalonForge/Clips/`.

## Probe report → route decision

`forge-artifacts/probe-report.json`:
- `probe1_api.foundAssemblies / publicStaticEntryPoints / editorWindows` — whether
  404-GEN can be batch-driven (Route for full no-GUI) or stays a GUI click.
- `probe2_riggedOutput.anyRigged` — TRUE → Route B (skip UModeler transplant entirely).
- `probe3_eulaFiles` — the EULA path; a human reads the automation terms.
- `verdictHint` — the route + automation verdict, pre-summarized.

## Honest limits (unchanged from the architecture doc)

- No auto-rig in Unity; Route A needs the UModeler X pass on the seat (~15-30
  min/class) unless probe 2 proves 404-GEN outputs rigged meshes.
- Verdicts stay human: review in the APK, one class at a time.
- Unity AI's assistant is not load-bearing here; it can help author edits but
  cannot drive the Editor autonomously.
