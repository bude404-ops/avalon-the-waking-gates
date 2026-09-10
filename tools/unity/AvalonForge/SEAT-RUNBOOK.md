# AVALON AUTO-FORGE — Seat Setup & Run (one-time, ~30 min)

## Install (Package Manager / My Assets)
1. **404-GEN 3D Generator** (free) — Window > 404-GEN 3D Generator. Requires Unity 6000.3+.
2. **UModeler X** (free) — in-editor modeling + **auto-rigging + skinning**.
3. **glTFast** (com.unity.cloud.gltfast, official) — GLB import.
4. **Animation Rigging** (com.unity.animation.rigging, official) — foot IK / look-at / weapon constraint.

## Copy into the project
- `AvalonForge.cs` → `Assets/Editor/`
- `specs/` → `Assets/AvalonForge/specs/`
- clip FBX files (from BIGagent404) → `Assets/AvalonForge/clips/`

## One-time MCP approval (lets BIGagent404 drive the Editor remotely)
Edit > Project Settings > AI > Unity MCP Server → allow BIGagent404 client.
(Requires Unity AI beta package + Pro seat, or during the Personal trial.)

## Generate a class mesh (404-GEN)
Window > 404-GEN 3D Generator → 2D Image Prompt = class ref plate
(`art/pending/REF404-GEN-CLASS-*-V2.png`) → Generate → Convert to Mesh →
save as `Assets/AvalonForge/raw/<Class>.glb`.
**PROBES (first run only):** (a) does 404-GEN expose a public API? (b) does its
output include a skeleton/skin? (c) read its EULA on automation. → report to BIGagent404.

## Rig pass (once per class, UModeler X — ~minutes)
If probe (b) = NO skeleton: open the mesh in UModeler X → Rigging tab →
Auto-Rig (humanoid) → verify the hand/head bone names → save.
If probe (b) = YES: skip this entirely.

## Run the pipeline (headless, no seat open)
Windows Task Scheduler (or before closing the Editor):
`Unity.exe -batchmode -projectPath <proj> -executeMethod AvalonForge.RunClass`
with env var `AVALON_CLASS=Sovereign-M` (or menu: Avalon Forge > Run ALL 12 Classes).

Output: `Assets/AvalonForge/prefabs/<Class>.prefab` +
`Assets/AvalonForge/artifacts/<Class>-report.json` (BIGagent404 reads the report,
QC-gates it, and publishes to the APK live shell for the verdict).
