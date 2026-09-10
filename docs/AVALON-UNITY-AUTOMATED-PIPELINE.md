# AVALON AUTO-FORGE — Unity as an Automated Production Environment

**Status:** Investigation complete (Sept 10, 2026) — capabilities verified against current
(2026) Unity documentation, official blogs, and Asset Store listings before recommending.
This is the design for the 12-model character pipeline that replaces per-model hand-fixing.

**Big's brief:** 2D REF → 404-GEN → 3D → Unity → cleanup → rig → animate → IK → test →
game-ready prefab, with Unity operated as an automated production environment instead of
a manually-driven Editor.

---

## 1. Executive Verdict

**Yes — buildable today, almost entirely inside the Unity ecosystem, near-zero cost.**
Three operation modes exist RIGHT NOW and each maps to a real, verified capability:

| Mode | Mechanism | Editor state | Who works | Big's involvement |
|------|-----------|--------------|-----------|-------------------|
| **A. Headless forge** | `Unity -batchmode -executeMethod` (official, documented) | CLOSED | My pipeline code runs unattended on his PC | None |
| **B. Agent-driven** | Unity AI Assistant — Agent mode (open beta, May 2026) | OPEN | Unity's built-in agent executes plans with approval | One approval, occasionally |
| **C. Remote operation** | Official **Unity MCP Server** (ships in the AI Assistant package) | OPEN | Me — driving his Editor remotely over MCP | One-time connection approval |

Modes A and C are the backbone: his machine becomes an unattended production node.
Mode A needs no Editor and no seat time at all.

---

## 2. The Pipeline — Stage by Stage

### Stage 0 — 2D reference (DONE, existing doctrine)
Locked canon art + V2 orthographic turnaround plates + weapon clean-plates (Clean-Plate
Law). No change.

### Stage 1 — 404-GEN: 2D → 3D mesh
**Verified** (guide.404.xyz, v0.7.1 May 2026): text or 2D-image → 3D generation natively
in-editor (1–2 min avg). Generates Gaussian Splats → "Convert to Mesh" pipeline bakes
to FBX mesh assets. Also: spatial cutouts, instant convex colliders, proxy shadows.
Unity 6000.3.2+ (his 6.3 seat qualifies). **Does NOT rig or animate** — outputs raw
geometry. GUI-driven (Window > 404-GEN Generator); CLI automation of the GUI is
UNVERIFIED — first-run test item, fallback is Unity AI agent clicks the queue.

### Stage 2 — Import, cleanup, normalize (FULLY AUTOMATABLE — Mode A/B/C)
Editor scripting (AssetPostprocessor / batch script):
- Scale normalization (1m height convention), orientation correction (Y-up, face-forward)
- `Mesh.Optimize()`, `RecalculateNormals/Tangents/Bounds`
- Topology/stat QC report: vert/tri counts, degenerate triangles, manifold checks
  (UnityEditor.MeshUtility), out-of-bounds detection — auto-generated JSON QC sheet
  per model, same verdict-queue protocol as art

### Stage 3 — Rigging (AUTOMATABLE with UModeler X — verified)
**UModeler X** (free, full commercial license, already the approved in-editor modeling
tool): includes **Rigging & Skinning — bone and weight work directly in Unity, with
auto-rigging**. 404-GEN mesh → UModeler X auto-rig → humanoid skeleton. Then the
**ModelImporter API**: set `AnimationType = Humanoid`, apply — Unity auto-maps bones to
the Humanoid avatar (verified behavior; it is exactly what the Rig tab does manually).
Both steps scriptable; first-of-class needs one human spot-check.

### Stage 4 — Animation (AUTOMATABLE — this kills the CMU/GLB bug class)
CMU mocap library (established doctrine) → FBX clips with root-lock → Unity **Mecanim
retargeting**: any Humanoid clip retargets onto any Humanoid avatar automatically. The
bind-matrix/offset class of bugs (every walk bug fought on Aedan) does not exist in this
path — Mecanim owns the retarget space math. AnimatorController is fully scriptable:
states, transitions, blend trees built by code, no Editor clicking.
Root-lock doctrine still applied at clip prep (my side, existing pipeline).

### Stage 5 — IK / Animation Rigging (AUTOMATABLE — official package)
**com.unity.animation.rigging** (verified): TwoBoneIKConstraint (feet/hands),
MultiAimConstraint (look-at, aiming), MultiParentConstraint (weapon to hand socket).
All components created and configured by script. Weapon attachment = one shared
`WeaponSocket` prefab + code — no per-model hand placement.

### Stage 6 — Testing (AUTOMATABLE — headless)
Unity Test Framework in batch mode: PlayMode tests assert pose bounds, root position
(pinned), animation states, IK targets. Screenshot batch renders (batchmode with
graphics, hidden window) push to the APK live channel — Big reviews in-app per the
APK-First Review Doctrine. No GIFs, no DM forensics.

### Stage 7 — Game-ready prefab (AUTOMATABLE)
`PrefabUtility.SaveAsPrefabAsset`, strip editor-only components, LOD sanity, then
publish via the existing live-update channel (content.json bump → APK self-sync).

---

## 3. The Seven Categories (Big's honest-capability matrix)

**1. Unity AI can actually do (verified — open beta, May 2026, Unity 6+):**
- Ask mode (read-only): scene questions, console-error diagnosis, API docs
- Plan mode: multi-step structured plans → approval → execution
- Agent mode: reads scene hierarchy, writes/edits C# files, creates/modifies GameObjects
  and components, runs operations, proposes diffs, one-click rollback; grounded in
  project context (scene graph, packages, platform)
- Generates placeholder sprites/textures/animations; performance recommendations
- CANNOT: production character meshes, production-quality animation, rigging, taste

**2. Unity packages can do:**
- Animation Rigging (IK, aim, parent — Stage 5)
- Test Framework (headless validation — Stage 6)
- AI Assistant package (Unity AI itself)
- Recorder (batch render passes)
- Inference Engine (optional — run AI models in-editor; not required for v1)

**3. Editor scripting/API can automate:**
- Everything asset-side: ModelImporter config, Humanoid avatar auto-mapping,
  AnimatorController construction, mesh ops, prefab build, AssetPostprocessor,
  full batchmode pipelines, screenshot QC renders
- This is 70% of the pipeline and it is boring, reliable, free

**4. Unity Asset Store packages:**
- **404-GEN** (free, v0.7.1) — 2D→3D generation, verified
- **UModeler X** (free, full commercial) — modeling, retopo, auto-rig, skinning, painting
- Both license-compliant (Asset Store EULA). Everything else needed is first-party.

**5. 404-GEN can do:**
- 2D-image → Gaussian splat → mesh conversion in-editor, splat cutouts, colliders
- NOT: rigging, animation, humanoid setup, batch API (unverified — test item #1)

**6. Requires another tool / outside Unity:**
- CMU clip prep (root-lock, FBX conversion) — existing sanctioned sandbox pipeline (mine)
- A network path for Mode C (MCP from my sandbox to his Editor: port-forward or tunnel) —
  one-time engineering task, only needed if he wants me driving the open Editor remotely

**7. Genuinely human:**
- Big's Keep/Re-roll verdicts (canon QC) — forever, by design
- First-of-class auto-rig spot check (is the skeleton clean?) — minutes per class
- 404-GEN queue supervision until GUI automation is verified — once
- Everything after the first model of each class: automated

---

## 4. Costs & Licensing
- 404-GEN: free. UModeler X: free. Animation Rigging / Test Framework: free.
- Unity AI: 1,000 free trial credits (14 days), then ~$10/mo — only needed if we use
  the in-Editor assistant; Modes A and C-batch don't require it.
- Unity MCP Server: included by default on Pro/Enterprise/Industry seats; Personal gets
  it during the AI trial window. Flag for budgeting — batch mode (Mode A) needs nothing.
- All Asset Store tools: commercial use on all license tiers (EULA).

## 5. Risks & First-Run Verification Items
1. 404-GEN GUI automation (can the queue be driven by script/agent?) — test on the seat;
   fallback: Unity AI agent or one manual click per model
2. 404-GEN mesh deformation quality after UModeler auto-rig (AI meshes are dense tris) —
   QC gate: canon key side-by-side + pose test before any model proceeds
3. MCP network path (Mode C) — engineering, not a blocker for Modes A/B
4. Big's seat stays on Unity 6000.3.x — 404-GEN requires exactly that; no upgrade needed

## 6. Implementation Plan (my side, on his word)
1. **AUTO-FORGE batch pipeline** (`Assets/AvalonForge/Editor/Forge.cs`): one static
   entry — `Unity -batchmode -executeMethod AvalonForge.Forge.Run -character <name>` —
   runs Stages 2–7 end-to-end, drops a QC sheet + prefab + APK-channel publish
2. **ForgeTest suite**: headless PlayMode tests (root pinned, pose bounds, IK targets)
3. **Weapon socket system**: MultiParent + shared socket prefab, per-class weapon props
   (weapon clean-plates already rolled, `docs/WEAPONS-404GEN-RUNBOOK.md`)
4. **MCP bridge** (after A is green): I connect to the open Editor, drive 404-GEN queue
   + generation passes remotely
5. First full run: Ravager (male) — proves the pipeline, then batch the remaining 10

---

*Verified sources: guide.404.xyz (404-GEN plugin docs v0.7.1), Asset Store listings
(404-GEN #311107, UModeler X), unity.com/blog AI tools beta (May 6 2026, Ask/Plan/Agent),
docs.unity3d.com com.unity.ai.assistant (MCP Server settings), com.unity.animation.rigging
docs (constraints), Unity batchmode/command-line docs, ModelImporter humanoid auto-map
behavior (Unity forums, official).*
