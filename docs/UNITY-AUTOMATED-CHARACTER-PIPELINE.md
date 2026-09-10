# AVALON — Unity-Native Automated Character Pipeline (v1, Sept 10 2026)

**Order:** Big — design an automated character-production environment inside the Unity
ecosystem, minimizing time spent sitting in the Editor.
**Verified against current (Sept 2026) Unity capabilities. Nothing below is assumed —
each claim is tagged with its category per Big's 7-way distinction:**

- [AI] = Unity AI / Unity's own AI
- [PKG] = official Unity package
- [API] = Unity Editor scripting / C# API
- [AS] = Asset Store product
- [GEN] = 404-GEN
- [EXT] = requires another tool
- [HUMAN] = genuinely requires a person

---

## THE ARCHITECTURE

```
BIG (verdicts only, in-APK)
  ↓
BIGagent404 (orchestration layer — authors every script, prompt, and QC gate;
  inspects results via rendered QC artifacts; iterates)
  ↓
[GEN] 404-GEN — ref sheet → base mesh (his seat, one GUI step — see §1)
  ↓
[API] Headless Unity batch run ON BIG'S OWN MACHINE (license-safe, no GUI, no seat open)
  — CharacterPipeline.cs runs the whole deterministic chain:
    import → normalize → validate/clean → rig → animate → IK → QC → prefab
  ↓
[PKG] Animation Rigging (weapon socket, hands, look-at, aiming)
  ↓
[PKG] Mecanim + CMU clip library (root-locked, converted off-seat by me)
  ↓
[API] QA render captures → published to the APK live shell (publish_live.sh)
  ↓
BIG's verdict in the APK (standing doctrine)
  ↓
GAME-READY PREFAB (versioned in repo)
```

## 1. GENERATION — 404-GEN (verified facts)

- FREE, v0.7.1 (May 2026), requires Unity 6000.3+, Built-in/URP/HDRP compatible. **Non-standard EULA** — must read it before automating.
- **No public scripting API documented.** If the package exposes no API, mesh generation is the ONE GUI step on Big's seat (~minutes per model, driven by my paste-prompts). If it exposes a public entry point, it joins the batch chain. VERIFY ON INSTALL.
- Output mesh is (as far as verified) UNRIGGED. This sets the rigging strategy below.

## 2. THE RIGGING HONESTY GAP

Unity has NO built-in auto-rigger. The Asset Store has NO real one-click auto-rig
product. Every auto-rig tool that exists (Meshy AI rig, AccuRIG, Auto-Rig Pro) is an
external application = BANNED under the Unity-Only Tool Law.

Two legitimate routes:

- **ROUTE A — RIGGED-BASE TRANSPLANT (recommended):** use a rigged humanoid base
  from the Asset Store (skeleton + skin weights + clips included). UModeler X
  (free, Asset Store, commercial OK) reshapes the base mesh to match the class ref
  sheet — keep the rig, reshape the flesh. [AS] + one [HUMAN] step (~15-30 min/model,
  guided by my per-class reshape spec). Rigging, skinning, and a clip library arrive
  pre-done.
- **ROUTE B — 404-GEN MESH NATIVE:** if 404-GEN's output turns out to be
  auto-rigged/skinned on install (some generators do this — VERIFY), Route A dies and
  the generated mesh goes straight into the batch chain.

## 3. WHAT THE BATCH CHAIN DOES (all deterministic, all real APIs)

`Unity -batchmode -quit -projectPath <proj> -executeMethod CharacterPipeline.ProcessClass -class Sovereign`
runs on Big's machine via Task Scheduler — his seat can be closed; the Editor runs
headless. Every step below is an existing, documented API:

| Step | Method | Category |
|---|---|---|
| Import GLB/FBX | glTFast (official) / built-in ModelImporter | [PKG]/[API] |
| Scale + orientation normalization | deterministic math vs per-class spec | [API] |
| Mesh inspection (tris, degenerates, non-manifold, UV coverage, bounds) | Mesh API report | [API] |
| Mesh cleanup (normals, tangents, bounds, unused verts) | Mesh API + RecalculateNormals | [API] |
| Material + texture assignment | AssetDatabase + per-class material script | [API] |
| Humanoid avatar creation | **AvatarBuilder.BuildHumanAvatar** (no GUI) | [API] |
| AnimatorController + blend tree build | UnityEditor.Animations API | [API] |
| CMU clip import + auto-retarget | ModelImporter humanoid config | [API] |
| Weapon socket + hand placement | TwoBoneIKConstraint, added by script | [PKG] |
| Look-at / aiming | MultiAimConstraint, added by script | [PKG] |
| Foot placement | AnimationRigging rig builders | [PKG] |
| IK enable/disable control | RigBuilder weights, scriptable | [PKG] |
| QA animation capture (PNG contact sheet) | Cinemachine-free camera render script | [API] |
| Performance (LODGroup, stats report) | LODGroup + Mesh API | [API] |
| Prefab creation | PrefabUtility.SaveAsPrefabAsset | [API] |
| Validation tests | Unity Test Framework (editmode) | [PKG] |
| Publish QC → APK live shell | publish_live.sh (mine) | [EXT→mine] |

## 4. WHERE AI ACTUALLY SITS (honest, verified)

- **Unity AI (Muse successor, Unity 6.2+, "Unity Points" credits):** chat/context
  assistant + generative helpers (sprite/texture/animate). It can help ME author C#
  and explain errors. It CANNOT autonomously operate the Editor — no Unity AI agent
  presses buttons or executes Editor ops. [AI] — optional, not load-bearing.
- **404-GEN:** the AI mesh generator — the AI in the core loop. [GEN]
- **BIGagent404 (me):** the decision-making agent — I author the pipeline code, the
  prompts, the QC gates; I inspect rendered QC artifacts and iterate; I publish to
  the APK shell. I operate through supported interfaces (git, batch runner, QC
  artifacts) — not through Unity's GUI.
- Where AI would be less reliable than a deterministic API, the pipeline uses the API.

## 5. LICENSING — PRECISE

1. **Headless Unity is NOT license-free.** Batch mode requires a valid, activated
   license with a signed-in session. Stated plainly per Big's instruction.
2. **Personal license on Big's own machine: fine.** His Hub-signed license covers
   scheduled batch runs on that machine with the Editor GUI closed. Zero extra cost,
   fully compliant. The seat just needs to stay licensed/signed-in; the machine on.
3. **Rented CI (game-ci/GitHub Actions) is effectively DEAD for Personal licenses
   since Unity killed manual .alf/.ulf activation.** Workarounds are fragile — we do
   NOT build the pipeline on rented CI.
4. **Unity Build Automation (cloud): free tier exists (2026 DevOps free tier).**
   Optional add-on for automated validation builds in Unity's cloud — no seat use,
   no local license consumed. Good for automated project validation runs; character
   QC still routes through the APK shell.
5. **404-GEN non-standard EULA:** read before any scripted/batch use of the package.
   If it forbids automation, mesh generation stays the one GUI step.
6. **Asset Store EULA (standard, incl. UModeler X):** use inside Editor projects,
   including batch mode, is permitted; commercial use on all tiers.

## 6. WHAT GENUINELY STILL NEEDS A HUMAN

1. **Big's verdicts** — canon taste calls, in the APK, per standing doctrine. Never automated.
2. **404-GEN generation GUI** — IF no API exists (§1 verify).
3. **Route A reshape pass** — IF we go rigged-base (§2): ~15-30 min/model in
   UModeler X. This is the single largest manual cost in the whole design; Route B
   verification is worth 30 minutes on the seat before anything else.

## 7. EXECUTION ORDER

1. Big installs 404-GEN in the project (Unity 6000.3+ required — confirm project version).
2. **Verify-first probes (30 min):** does 404-GEN expose an API? Does its output come
   rigged? Read its EULA. → decides Route A vs Route B.
3. I write CharacterPipeline.cs + per-class specs + CMU clip conversion (already have
   the root-lock conversion tooling).
4. One scheduled batch run on his machine proves the chain on Sovereign.
5. Remaining 11 classes run through the chain unattended.
6. Verdicts in the APK, one class at a time.
