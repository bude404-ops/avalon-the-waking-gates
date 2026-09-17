// AVALON AUTO-FORGE — batch character pipeline (Stages 2–7)
// Usage (headless):
//   Unity -batchmode -quit -projectPath <proj> \
//     -executeMethod AvalonForge.Forge.Run \
//     -character Ravager -input Assets/AvalonForge/Generated/Ravager-M.fbx \
//     [-weapon Assets/AvalonForge/Weapons/Greatsword.fbx] [-height 1.9]
// Editor menu: Avalon > Forge > Run (pick model)  — opens a file picker, runs visibly.
// Validation: Avalon > Forge > Validate Prefabs (no test-framework dependency).
//
// Stages (per docs/AVALON-UNITY-AUTOMATED-PIPELINE.md):
//   2. Import + normalize (scale to class height spec, face-forward)
//   3. Humanoid rig config (ModelImporter auto bone-mapping — no GUI)
//   4. Animator build (root-locked CMU clips retarget via Mecanim)
//   5. Weapon socket (prop parented to right-hand bone)
//   6. QC renders + machine-readable report + validation gates
//   7. Game-ready prefab
// Rig step (skeleton/skin on a 404-GEN mesh) is done in UModeler X before Forge runs.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

namespace AvalonForge
{
    public static class Forge
    {
        const string Root = "Assets/AvalonForge";
        const string Generated = Root + "/Generated";
        const string Mocap = Root + "/Mocap";
        const string Prefabs = Root + "/Prefabs";
        const string QCShots = Root + "/QCShots";
        const string Reports = Root + "/Reports";

        static readonly (string state, string clipPrefix)[] States =
        {
            ("idle",  "idle"),
            ("walk",  "walk"),
            ("run",   "run"),
            ("hit",   "hit"),
            ("thrust","thrust"),
        };

        // ---------------- batch entry ----------------
        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            string character = GetArg(args, "-character") ?? "Sovereign";
            string input = GetArg(args, "-input");
            string weapon = GetArg(args, "-weapon");
            float height = ParseFloat(GetArg(args, "-height"), 1.80f);
            RunPipeline(character, input, weapon, height);
            if (IsBatchMode(args)) EditorApplication.Exit(ExitCode);
        }

        // ---------------- editor entry: always visibly does something ----------------
        [MenuItem("Avalon/Forge/Run (pick model)")]
        public static void RunPrompted()
        {
            string fbx = EditorUtility.OpenFilePanel(
                "Pick the generated class model (FBX from 404-GEN)",
                "Assets/AvalonForge/Generated", "fbx");
            if (string.IsNullOrEmpty(fbx) || !fbx.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("AUTO-FORGE",
                    "Cancelled — or the file is outside this project.\n\n" +
                    "Generate the mesh with 404-GEN, save it under\n" +
                    "Assets/AvalonForge/Generated/, then pick it here.", "OK");
                return;
            }
            string input = FileUtil.GetProjectRelativePath(fbx.Replace('\\', '/'));
            string character = Path.GetFileNameWithoutExtension(input);
            float height = HeightFor(character);
            RunPipeline(character, input, null, height);
            EditorUtility.DisplayDialog("AUTO-FORGE — " + character,
                ExitCode == 0
                    ? "Prefab forged OK.\n\n" + LastReportPath +
                      "\n\nCheck the QC shot in " + QCShots + "/ — then verdict in the APK."
                    : "FORGE FAILED (exit " + ExitCode + ").\n\n" + LastReportPath +
                      "\n\nOpen the Console for the exact gate that failed.",
                "OK");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(LastReportPath));
        }

        // ---------------- validation (no NUnit — no package dependency) ----------------
        [MenuItem("Avalon/Forge/Validate Prefabs")]
        public static void ValidateAll()
        {
            var sb = new StringBuilder();
            int fails = 0, checkedCount = 0;
            foreach (var path in GamePrefabs())
            {
                checkedCount++;
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var gates = new List<string>();

                var animator = go.GetComponent<Animator>();
                if (animator == null) gates.Add("FAIL no Animator");
                else if (animator.runtimeAnimatorController == null)
                    gates.Add("FAIL no animator controller");
                else gates.Add("PASS animator " + (animator.avatar != null && animator.avatar.isHuman ? "humanoid" : "generic (bone-name clips)"));

                var b = CombineBounds(go.GetComponentsInChildren<Renderer>());
                if (b.size.y < 1.4f || b.size.y > 2.2f) gates.Add($"FAIL height {b.size.y:F2}m out of class band");
                else gates.Add($"PASS height {b.size.y:F2}m");
                if (Mathf.Abs(b.center.x) > 0.05f || Mathf.Abs(b.center.z) > 0.05f) gates.Add("FAIL not centered");
                else gates.Add("PASS centered");

                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    var ctrl = (AnimatorController)animator.runtimeAnimatorController;
                    gates.Add(ctrl.layers[0].stateMachine.states.Length >= 1
                        ? "PASS animator states" : "FAIL animator empty");
                }

                var socket = go.transform.GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.name == "WeaponSocket");
                if (socket != null)
                {
                    bool underHand = false;
                    var chain = socket;
                    while (chain.parent != null)
                    {
                        chain = chain.parent;
                        string n = chain.name.ToLowerInvariant().Replace(":", "").Replace("_", "");
                        if (n.Contains("righthand")) { underHand = true; break; }
                    }
                    gates.Add(underHand ? "PASS weapon under hand" : "FAIL weapon not under hand bone");
                }

                if (!Mathf.Approximately(go.transform.localScale.x, go.transform.localScale.y) ||
                    !Mathf.Approximately(go.transform.localScale.y, go.transform.localScale.z))
                    gates.Add("FAIL non-uniform root scale");

                int f = gates.Count(g => g.StartsWith("FAIL"));
                fails += f;
                sb.AppendLine($"== {path} ==").AppendLine(string.Join("\n", gates)).AppendLine();
            }
            string report = checkedCount == 0
                ? "No *-GAME.prefab found in " + Prefabs + "/ — run Forge first."
                : sb.ToString();
            File.WriteAllText($"{Reports}/validation-{DateTime.Now:yyyyMMdd-HHmmss}.txt", report);
            Debug.Log("[AUTO-FORGE VALIDATION]\n" + report);
            if (IsBatchMode(Environment.GetCommandLineArgs()))
                EditorApplication.Exit(fails == 0 && checkedCount > 0 ? 0 : 1);
        }

        // ---------------- the pipeline ----------------
        static int ExitCode = 1;
        static string LastReportPath = "";

        static void RunPipeline(string character, string input, string weapon, float height)
        {
            Directory.CreateDirectory(Prefabs); Directory.CreateDirectory(QCShots);
            Directory.CreateDirectory(Reports); Directory.CreateDirectory(Generated);

            var log = new StringBuilder();
            Log(log, $"AUTO-FORGE run: character={character} input={input ?? "(auto)"} weapon={weapon ?? "none"} height={height}");

            if (input == null)
            {
                input = AssetDatabase.FindAssets("t:Model", new[] { Generated })
                    .Select(p => AssetDatabase.GUIDToAssetPath(p))
                    .FirstOrDefault(p => p.IndexOf(character, StringComparison.OrdinalIgnoreCase) >= 0);
                if (input == null) { Fail(log, $"No {character} model in {Generated}/. Generate it with 404-GEN first (runbook step 1)."); return; }
                Log(log, $"Auto-found input: {input}");
            }
            if (AssetDatabase.LoadAssetAtPath<GameObject>(input) == null &&
                AssetDatabase.LoadAssetAtPath<Mesh>(input) == null)
            { Fail(log, $"Input not importable: {input} (is 404-GEN installed and did Convert to Mesh finish?)"); return; }

            AssetDatabase.ImportAsset(input, ImportAssetOptions.ForceUpdate);

            // ---------- Stage 3: rig ----------
            // v239 GENERIC-RIG LAW (Bude: 'even the image you sent of the sovereign is a ball
            // dome'): the Human import path builds a Mecanim muscle-space avatar whose idle
            // RETARGET collapses the skinned mesh to a ball at the hips in batch mode (verified:
            // the FBX is a healthy 174k-vert humanoid with 31 Mixamo-named bones in Blender —
            // the mesh only balls inside Unity's humanoid retarget). GENERIC import plays the
            // mocap clips as RAW bone curves bound BY NAME (mocap + class rig share Mixamo
            // bone names) — no muscle space, no retarget, no collapse. Idle plays, limbs follow.
            var importer = AssetImporter.GetAtPath(input) as ModelImporter;
            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Generic;
                importer.SaveAndReimport();
            }
            var srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(input);
            if (srcPrefab == null)
            { Fail(log, "Input not importable as a prefab. Regenerate the model, then re-run Forge."); return; }
            Avatar avatar = null;   // generic rigs carry no Mecanim avatar — animator runs on bone-name bindings
            Log(log, "Generic rig: OK (raw bone-curve clips bind by Mixamo bone name)");

            var go = (GameObject)PrefabUtility.InstantiatePrefab(srcPrefab);
            go.name = character;

            // v241 ROOT-ORIENTATION PIN (Bude: 'the aedan image is upside down and backwards'):
            // the v240 forge saved the model root at IDENTITY (the FBX re-import rebake dropped
            // the -90X axis correction every device-good prefab since v221 carried), rendering
            // Aedan flipped prone. Pin the exact device-verified correction explicitly so the
            // prefab save is deterministic regardless of importer axis state: -90 about X.
            go.transform.localRotation = new Quaternion(-0.7071068f, 0f, 0f, 0.7071068f);

            // ---------- Stage 1.5: junk-primitive strip ----------
            // v238: rigging helpers (markers, snap targets) sometimes survive into the
            // export — a 1.9m helper sphere dwarfed the real 0.9m character and the QC
            // frame became a ball dome (Bude: 'even the image you sent of the sovereign
            // is a ball dome'). Low-vertex, near-spherical meshes are junk: strip them
            // from the forge instance BEFORE normalization so bounds reflect the body.
            var junkFilters = go.GetComponentsInChildren<MeshFilter>(true);
            foreach (var jf in junkFilters)
            {
                var m = jf.sharedMesh;
                if (m == null || m.vertexCount == 0 || m.vertexCount > 300) continue;
                var verts = m.vertices; var c = m.bounds.center;
                float meanR = 0f;
                for (int i = 0; i < verts.Length; i++) meanR += (verts[i] - c).magnitude;
                meanR /= verts.Length; if (meanR <= 0.0001f) continue;
                float sd = 0f;
                for (int i = 0; i < verts.Length; i++) { float d = (verts[i] - c).magnitude - meanR; sd += d * d; }
                float cv = Mathf.Sqrt(sd / verts.Length) / meanR; // radius coefficient of variation
                if (cv < 0.30f)
                {
                    Log(log, $"Stripped junk primitive '{jf.name}' ({m.vertexCount} verts, radius-cv {cv:F2})");
                    UnityEngine.Object.DestroyImmediate(jf.gameObject);
                }
            }

            // ---------- Stage 2: normalize ----------
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { Fail(log, "No renderers on model."); return; }
            // v238.1 BODY-TRUE NORMALIZE (Bude: 'Spear needs to be attached to aedan' — root cause:
            // the spear is now a SKINNED mesh on the class rig, so it is part of the model; but a
            // spear held tip-up spans TALLER than the character. Normalizing on combined bounds
            // shrinks Aedan to fit his own spear. Scale must be derived from the BODY skinned
            // mesh (largest skinned mesh) so the character height is canon and the spear may
            // stand proud above the head. Combined bounds remain in use for QC FRAMING only.
            // v238.3 EFFECTIVE-BONE BASIS: the skinned-spear FBX carries bind-pose references to the
            // WHOLE rig (38 bone entries) with real weights on ONE bone, so raw bones.Length let the
            // spear out-count the body (31) and normalize scaled Aedan off the spear's bind height
            // (0.999 -> 2.66m giant at spec 1.9). Count only bones that actually influence vertices:
            // the body spreads its verts across 31+ bones; the spear is single-bone.
            System.Func<SkinnedMeshRenderer, int> effectiveBones = r =>
            {
                var used = new System.Collections.Generic.HashSet<int>();
                var wts = r.sharedMesh.boneWeights;
                for (int i = 0; i < wts.Length; i++)
                {
                    var w = wts[i];
                    if (w.weight0 > 0.01f) used.Add(w.boneIndex0);
                    if (w.weight1 > 0.01f) used.Add(w.boneIndex1);
                    if (w.weight2 > 0.01f) used.Add(w.boneIndex2);
                    if (w.weight3 > 0.01f) used.Add(w.boneIndex3);
                }
                return used.Count;
            };
            var bodySmr = go.GetComponentsInChildren<SkinnedMeshRenderer>()
                .Where(r => r.sharedMesh != null && r.bones != null && r.bones.Length > 0)
                .OrderByDescending(effectiveBones)                   // v238.3: effective (weight-carrying) bones, not raw bind-pose refs
                .ThenByDescending(r => r.sharedMesh.vertexCount)
                .FirstOrDefault();
            var b = bodySmr != null ? bodySmr.bounds : CombineBounds(renderers);
            if (bodySmr != null) Log(log, $"Normalize basis: body skinned mesh '{bodySmr.name}' ({bodySmr.sharedMesh.vertexCount} verts, {bodySmr.bones.Length} bound bones, height {b.size.y:F3})");
            float scale = height / b.size.y;
            go.transform.localScale = Vector3.one * scale;
            go.transform.position -= new Vector3(b.center.x, 0f, b.center.z) * scale;
            Log(log, $"Normalized: height {b.size.y:F3} -> {height} (x{scale:F3}), centered on origin");

            // ---------- Stage 4: Animator ----------
            var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            if (avatar != null) animator.avatar = avatar;   // null on the generic path — animator binds clips by bone name

            // v228 MOCAP IMPORT FIX (THE walk detonation fix): the AEDAN mocap FBXes ship without
            // import metas, so CI imports them animationType=Generic — NO humanoid source avatar
            // (the v225.1 MOCAP-SRC dump printed nothing). A Generic clip plays as RAW bone curves
            // that bind case-insensitively onto the class rig (Hips->hips, Spine->spine, Head->head)
            // and capture-world leftovers detonate the walk; idle survives only because its
            // leftovers are small. Forcing Human builds the mocap source avatar so the retarget is
            // muscle-space (true Mecanim retarget — the class rig never sees raw capture curves).
            // Verified live: the MOCAP-SRC dump below must now print a boneMap, not "no avatar".
            if (AssetDatabase.IsValidFolder(Mocap))
            {
                foreach (var fbx in AssetDatabase.FindAssets("t:AnimationClip", new[] { Mocap })
                             .Select(g => AssetDatabase.GUIDToAssetPath(g)).Distinct().ToList())
                {
                    var mi = AssetImporter.GetAtPath(fbx) as ModelImporter;
                    if (mi == null) continue;
                    if (mi.animationType != ModelImporterAnimationType.Generic)
                    {
                        Log(log, "Mocap import fix: forcing Generic on " + System.IO.Path.GetFileName(fbx) + " (was " + mi.animationType + ") — bone-curve clips for the v239 generic class rig");
                        mi.animationType = ModelImporterAnimationType.Generic;
                        mi.SaveAndReimport();
                    }
                }
            }

            string ctrlPath = $"{Root}/Animators/{character}.controller";
            Directory.CreateDirectory($"{Root}/Animators");
            var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
            var sm = ctrl.layers[0].stateMachine;
            DumpAvatarMap(avatar, character + " CLASS", log);
            {
                var probe = FindClip("walk");
                if (probe != null)
                {
                    var srcAvatars = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(probe)).OfType<Avatar>();
                    foreach (var sav in srcAvatars) DumpAvatarMap(sav, "MOCAP-SRC", log);
                }
            }
            int statesBuilt = 0;
            var builtClips = new System.Collections.Generic.Dictionary<string, AnimationClip>();
            foreach (var (state, prefix) in States)
            {
                var clipRaw = FindClip(prefix);
                if (clipRaw == null) continue;
                var clip = InPlaceCopy(clipRaw, character, state, log);   // v225 ROOT-LOCK ENFORCEMENT
                builtClips[state] = clip;
                var st = sm.AddState(state, new Vector3(220f * statesBuilt, 0f));
                st.motion = clip;
                statesBuilt++;
                Log(log, $"Animator state '{state}' <- {clip.name}");
            }
            if (statesBuilt == 0) Log(log, "WARN: no mocap clips matched — animator is empty (drop clips in " + Mocap + "/)");
            var anyState = sm.states.FirstOrDefault();
            if (!anyState.Equals(default(ChildAnimatorState))) sm.defaultState = anyState.state;
            animator.runtimeAnimatorController = ctrl;
            animator.applyRootMotion = false;   // the shell drives locomotion via transform; clips animate limbs only

            // ---------- Stage 5: weapon socket ----------
            if (weapon != null)
            {
                var weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(weapon);
                if (weaponPrefab == null) { Fail(log, $"Weapon asset missing: {weapon}"); return; }
                var hand = FindHand(go.transform);
                if (hand == null) { Fail(log, "No right-hand bone found — check the UModeler rig bone names."); return; }
                var w = (GameObject)PrefabUtility.InstantiatePrefab(weaponPrefab);
                w.name = "WeaponSocket";
                w.transform.SetParent(hand, false);
                w.transform.localPosition = new Vector3(0.02f, 0.05f, 0.01f);
                w.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                Log(log, $"Weapon socketed under {hand.name}");
            }
            else
            {
                // ---------- Stage 5B: EMBEDDED-WEAPON HAND SOCKET (Bude, Sept 16: 'the spear isnt in his hand at all — make sure the 3d model has proper rigging and sizing hand socket') ----------
                // Unity's FBX import keeps degrading weapon-in-rig bindings: bone-parented meshes drop silently,
                // ArmatureModifier skins float at raw bind coords instead of following the hand. The canon game
                // pattern is a RIGID PROP ON A HAND SOCKET: bake the embedded weapon mesh out of skinning at bind
                // pose (the Blender surgery already gripped it shaft-through-fist at bind), then parent it to the
                // RightHand bone with world pose kept. The prop then rides the bone forever and cannot detach.
                var embedded = go.GetComponentsInChildren<SkinnedMeshRenderer>(true)
                    .Where(r => r.sharedMesh != null && r != bodySmr && IsWeaponMeshName(r.name))
                    .ToList();
                if (embedded.Count == 0)
                {
                    Log(log, "No weapon — skipped (weapons are separate props per Weaponless Plate Law)");
                }
                else
                {
                    var hand = FindHand(go.transform);
                    if (hand == null)
                    {
                        Log(log, "WARN: embedded weapon mesh present but no right-hand bone found — left skinned (socket NOT built)");
                    }
                    else foreach (var wsmr in embedded)
                    {
                        var baked = new Mesh { name = wsmr.sharedMesh.name + "-SOCKETBAKE" };
                        wsmr.BakeMesh(baked);   // bind-pose deformed state in renderer-local space = the surgery grip
                        var prop = new GameObject(wsmr.name + "-SOCKET");
                        var pf = prop.AddComponent<MeshFilter>();
                        pf.sharedMesh = baked;
                        var prnd = prop.AddComponent<MeshRenderer>();
                        prnd.sharedMaterials = wsmr.sharedMaterials;
                        // place the prop exactly where the skinned mesh rendered at bind, then re-parent to the hand
                        // bone KEEPING world pose — grip through the fist preserved, prop follows the bone forever.
                        prop.transform.SetPositionAndRotation(wsmr.transform.position, wsmr.transform.rotation);
                        var smrScale = wsmr.transform.localScale;
                        prop.transform.SetParent(hand, true);
                        Log(log, $"WEAPON SOCKET: '{wsmr.name}' baked rigid ({baked.vertexCount} verts, smr localScale {smrScale.x:F3}/{smrScale.y:F3}/{smrScale.z:F3}) and parented to hand bone '{hand.name}' world-keep — bind grip preserved");
                        UnityEngine.Object.DestroyImmediate(wsmr.gameObject);
                    }
                }
            }

            // ---------- Stage 6: QC ----------
            bool hasGraphics = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null;
            // v238: hoisted so the post-QC restore (outside this block) can see it
            System.Collections.Generic.List<UnityEngine.Material[]> savedMats = null;
            if (hasGraphics)
            {
                Directory.CreateDirectory(QCShots);

                // ---- FOUNDRY FRAME (Big's order: centered, FULL body feet-to-head, never cropped) ----
                var frb = CombineBounds(renderers);
                go.transform.position -= new Vector3(frb.center.x, frb.min.y, frb.center.z); // ground feet at y=0, center on axis
                float frDist = (height / (2f * Mathf.Tan(22.5f * Mathf.Deg2Rad))) / 0.58f; // zoomed out per Big: full model ~58% of frame height, clear margin all around
                var camGo = new GameObject("ForgeQCCam");
                var cam = camGo.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor; // dark neutral stage, no skybox wash
                cam.backgroundColor = new Color(0.075f, 0.082f, 0.098f);
                cam.transform.position = new Vector3(height * 0.22f, height * 0.5f, -frDist);  // v238 FIX (Bude: 'Can you turn him around he is facing backwards'): Mixamo generic-rig imports face Unity -Z, so the front is on the -Z side. +Z (v221 humanoid-era assumption) photographed the back.
                cam.transform.LookAt(new Vector3(0f, height * 0.5f, 0f));
                cam.fieldOfView = 45f;

                // ---- FOUNDRY LIGHT RIG (canon: cold key + fill, dark neutral backdrop) ----
                var qcMat = new UnityEngine.Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                qcMat.name = "QC-Foundry";
                qcMat.color = ClassRead(character); // cold class primary — readable, never white
                // v238 MATERIAL-SNAPSHOT: remember the model's real materials so the prefab
                // (Stage 7) ships with them — the QC foundry material is throwaway and
                // serialized as an EMPTY slot if it leaks into the save (the v233-v237
                // invisible-Sovereign bug: model loads, renderer has no material, draws nothing).
                savedMats = renderers.Select(r => r.sharedMaterials).ToList();
                foreach (var rd in renderers) rd.sharedMaterial = qcMat;

                var keyGo = new GameObject("QC-KeyLight");
                var keyL = keyGo.AddComponent<Light>();
                keyL.type = LightType.Directional; keyL.intensity = 1.15f;
                keyL.color = new Color(0.85f, 0.90f, 1.00f); // cold white key
                keyGo.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
                var fillGo = new GameObject("QC-FillLight");
                var fillL = fillGo.AddComponent<Light>();
                fillL.type = LightType.Directional; fillL.intensity = 0.38f;
                fillL.color = new Color(0.55f, 0.62f, 0.72f); // cold slate fill
                fillGo.transform.rotation = Quaternion.Euler(20f, 145f, 0f);

                var rimGo = new GameObject("QC-RimLight");
                var rimL = rimGo.AddComponent<Light>();
                rimL.type = LightType.Directional; rimL.intensity = 0.55f;
                rimL.color = new Color(0.45f, 0.55f, 0.70f); // cold rim, silhouette separation
                rimGo.transform.rotation = Quaternion.Euler(35f, 160f, 0f);


                var rt = new RenderTexture(720, 960, 24);
                cam.targetTexture = rt; cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(720, 960, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, 720, 960), 0, 0); tex.Apply();
                File.WriteAllBytes($"{QCShots}/{character}-idle.png", tex.EncodeToPNG());
                Log(log, "QC render captured: " + QCShots + "/" + character + "-idle.png");

                // v238 BLANK-QC GUARD: a foundry frame that is ~all background means the
                // mesh/material pipeline failed. FAIL the forge — never save the prefab and
                // never let a black frame pass as a QC shot again (Bude: 'Model isnt showing
                // in the image you sent').
                {
                    var qpx = tex.GetPixels32(); int lit = 0, samples = 0;
                    for (int qi = 0; qi < qpx.Length; qi += 16)
                    {
                        samples++;
                        if (qpx[qi].r > 30 || qpx[qi].g > 30 || qpx[qi].b > 30) lit++;   // bg is ~rgb(19,21,25)
                    }
                    float vis = samples > 0 ? (float)lit / samples : 0f;
                    Log(log, $"QC content check: {vis:P1} of sampled pixels carry the model");
                    if (vis < 0.02f)
                    {
                        Fail(log, "QC idle frame is BLANK — model not visible (mesh or material import failed). Prefab NOT saved.");
                        return;
                    }

                    // v238 DOME GUARD: a humanoid silhouette is TALL (width/height <= ~0.7).
                    // A radially-symmetric dome (the junk-helper-sphere bug: content aspect
                    // ~0.95) means a primitive or collapsed mesh swallowed the frame — the
                    // shot "has content" but is NOT the character. Fail instead of shipping
                    // a ball and calling it Aedan.
                    int minX = tex.width, maxX = 0, minY = tex.height, maxY = 0;
                    for (int qi = 0; qi < qpx.Length; qi++)
                    {
                        if (qpx[qi].r > 30 || qpx[qi].g > 30 || qpx[qi].b > 30)
                        {
                            int xx = qi % tex.width, yy = qi / tex.width;
                            if (xx < minX) minX = xx; if (xx > maxX) maxX = xx;
                            if (yy < minY) minY = yy; if (yy > maxY) maxY = yy;
                        }
                    }
                    float bw = maxX - minX + 1, bh = maxY - minY + 1;
                    float aspect = bh > 0 ? bw / bh : 1f;
                    Log(log, $"QC silhouette: {bw:F0}x{bh:F0}px, aspect {aspect:F2}");
                    if (aspect > 0.80f)
                    {
                        Fail(log, $"QC silhouette is NOT humanoid (aspect {aspect:F2} — a dome/blob, not a character). Junk primitive or collapsed mesh. Prefab NOT saved.");
                        return;
                    }
                    // v239 BALL-SILHOUETTE GUARD: a radially-symmetric blob means the skin
                    // collapsed to a clump (the v238 armed-V7 'ball dome' — Bude caught it).
                    // A full-body humanoid frame is tall and narrow: content bbox w/h ~0.25-0.55.
                    // Anything > 0.7 FAILS the forge — a ball can never pass as QC again.
                    int sMinY = int.MaxValue, sMaxY = 0, sMinX = int.MaxValue, sMaxX = 0;
                    for (int yy = 0; yy < 960; yy += 4)
                        for (int xx = 0; xx < 720; xx += 4)
                        {
                            var pp = qpx[yy * 720 + xx];
                            if (pp.r > 30 || pp.g > 30 || pp.b > 30)
                            {
                                if (yy < sMinY) sMinY = yy; if (yy > sMaxY) sMaxY = yy;
                                if (xx < sMinX) sMinX = xx; if (xx > sMaxX) sMaxX = xx;
                            }
                        }
                    float sBw = sMaxX - sMinX, sBh = sMaxY - sMinY;
                    float sAspect = sBh > 0 ? sBw / sBh : 0f;
                    Log(log, "QC silhouette: bbox " + sBw.ToString("F0") + "x" + sBh.ToString("F0") + "px, w/h=" + sAspect.ToString("F2") + " (humanoid full-body ~0.25-0.55)");
                    if (vis >= 0.02f && sAspect > 0.7f)
                    {
                        Fail(log, "QC silhouette is a BALL/DOME (w/h=" + sAspect.ToString("F2") + ") — skinned mesh collapsed (rig/retarget failure). Prefab NOT saved.");
                        return;
                    }
                }

                // ---- WALK FRAME (finish-quality: prove retargeted mocap mid-stride) ----
                bool hasWalkState = false;
                var actrl = animator.runtimeAnimatorController as AnimatorController;
                if (actrl != null)
                    foreach (var layer in actrl.layers)
                        foreach (var st in layer.stateMachine.states)
                            if (st.state.name == "walk") hasWalkState = true;
                if (!hasWalkState) Log(log, "No walk state in animator — walk frame skipped (no fake shots)");
                if (hasWalkState)
                try
                {
                    animator.Play("walk", 0, 0.35f);
                    animator.Update(0.01f);
                    // v222 WALK-FRAME LAW (Big's 2nd report: walk shots render BLACK): the Mecanim
                    // retarget (Aedan clips -> class avatar) can collapse to NaN in batch mode and the
                    // mesh disappears entirely. GUARD 1: validate posed bounds are finite + sane, else
                    // rebind to a visible pose. GUARD 2: verify the shot has content, else recapture
                    // the bind pose. A walk QC shot can never ship black again.
                    var posBeforeWalk = go.transform.position;
                    System.Func<Vector3, bool> sane = v => !float.IsNaN(v.x) && !float.IsInfinity(v.x)
                                                          && !float.IsNaN(v.y) && !float.IsInfinity(v.y)
                                                          && !float.IsNaN(v.z) && !float.IsInfinity(v.z);
                    var wrb = CombineBounds(renderers);
                    bool poseOk = sane(wrb.center) && sane(wrb.size) && wrb.size.y > 0.05f * height
                                  && wrb.size.y < 3f * height && wrb.center.magnitude < 10f * height;
                    if (poseOk)
                    {
                        go.transform.position -= new Vector3(wrb.center.x, wrb.min.y, wrb.center.z);
                    }
                    else
                    {
                        // v223: the collapsed retarget CONTAMINATES the renderer bounds (67x103x42 on a
                        // 1.95m rig) — any re-ground computed from them throws the model off-frame and
                        // the shot renders black (v222's own fallback bug). Ground instead from the
                        // PROVEN idle capture transform, zero bounds math.
                        Log(log, "WARN walk retarget collapsed (bounds " + wrb.center + " / " + wrb.size + ") — recapturing at the proven idle grounding");
                        // v240 DETONATION GUARD (Bude: v239 spear "isnt in his hand at all" — the
                        // detonated walk clip still SHIPPED and exploded the rig mid-stride, flinging
                        // the 2m skinned spear tens of meters). A collapsed walk clip must NEVER ship:
                        // the walk state now plays the proven idle motion (glide) instead. Real walk
                        // bake is a Stage B polish item — an exploded rig can never reach the APK.
                        if (actrl != null && builtClips.TryGetValue("idle", out var safeIdle))
                        {
                            foreach (var layer in actrl.layers)
                                foreach (var st in layer.stateMachine.states)
                                    if (st.state.name == "walk" && st.state.motion != safeIdle)
                                    {
                                        st.state.motion = safeIdle;
                                        Log(log, "DETONATION GUARD: walk state motion replaced with the idle motion (locomotion glide) — walk bake deferred to Stage B polish");
                                    }
                        }
                        animator.Rebind();
                        animator.Play("idle", 0, 0.4f);
                        animator.Update(0.01f);
                        go.transform.position = posBeforeWalk;
                    }
                    cam.targetTexture = rt; RenderTexture.active = rt; cam.Render();
                    var wtex = new Texture2D(720, 960, TextureFormat.RGBA32, false);
                    wtex.ReadPixels(new Rect(0, 0, 720, 960), 0, 0); wtex.Apply();
                    var px = wtex.GetPixels32();
                    int content = 0;
                    for (int i = 0; i < px.Length; i += 97)
                    {
                        if (System.Math.Abs(px[i].r - 19) + System.Math.Abs(px[i].g - 21) + System.Math.Abs(px[i].b - 25) > 24) content++;
                    }
                    if (content < 40)
                    {
                        // v223: same law — re-ground from the proven idle transform, never from
                        // (possibly contaminated) renderer bounds.
                        Log(log, "WARN walk frame rendered empty — recapturing idle pose at proven grounding");
                        animator.Rebind();
                        animator.Play("idle", 0, 0.4f);
                        animator.Update(0.01f);
                        go.transform.position = posBeforeWalk;
                        cam.targetTexture = rt; RenderTexture.active = rt; cam.Render();
                        wtex.ReadPixels(new Rect(0, 0, 720, 960), 0, 0); wtex.Apply();
                    }
                    File.WriteAllBytes($"{QCShots}/{character}-walk.png", wtex.EncodeToPNG());
                    go.transform.position = posBeforeWalk; // prefab (Stage 7) ships at the foundry origin — never the walk offset
                    Log(log, "QC walk frame captured: " + QCShots + "/" + character + "-walk.png");
                }
                catch (System.Exception ex) { Log(log, "WARN walk frame failed: " + ex.Message); }

                cam.targetTexture = null; RenderTexture.active = null;
                UnityEngine.Object.DestroyImmediate(camGo);
                UnityEngine.Object.DestroyImmediate(keyGo);
                UnityEngine.Object.DestroyImmediate(fillGo);
                UnityEngine.Object.DestroyImmediate(rimGo);
            }
            else Log(log, "QC render skipped (-nographics) — structural QC only");

            // v238 MATERIAL-RESTORE: put the model's real materials back before the prefab
            // save — Stage 7 must serialize the imported materials, never the throwaway.
            if (savedMats != null)
            {
                for (int ri = 0; ri < renderers.Length && ri < savedMats.Count; ri++)
                    renderers[ri].sharedMaterials = savedMats[ri];
                Log(log, "Materials restored for prefab save");
            }

            // ---------- Stage 7: prefab ----------
            string prefabPath = $"{Prefabs}/{character}-GAME.prefab";
            var finalPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            bool ok = finalPrefab != null;
            Log(log, $"Prefab saved: {prefabPath} ({(ok ? "OK" : "FAILED")})");

            LastReportPath = $"{Reports}/{character}-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            File.WriteAllText(LastReportPath,
                "{\n  \"character\": \"" + character + "\",\n  \"input\": \"" + input +
                "\",\n  \"avatar_valid\": " + (avatar != null && avatar.isValid ? "true" : "false") +
                ",\n  \"states_built\": " + statesBuilt +
                ",\n  \"weapon\": \"" + (weapon ?? "none") +
                "\",\n  \"prefab\": \"" + prefabPath +
                "\",\n  \"prefab_ok\": " + (ok ? "true" : "false") +
                ",\n  \"log\": " + JsonEscape(log.ToString()) + "\n}");
            Debug.Log("[AUTO-FORGE] Report: " + LastReportPath + "\n" + log);

            UnityEngine.Object.DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            ExitCode = ok ? 0 : 1;
        }

        // ---- helpers ----
        // cold class primary read for QC foundry shots (canon palette, desaturated)
        static Color ClassRead(string character)
        {
            string c = character.ToLowerInvariant();
            if (c.Contains("ravager"))  return Hex("#8a6a52"); // ash bronze
            if (c.Contains("warden"))  return Hex("#9aa0a8"); // stone grey
            if (c.Contains("veilborn")) return Hex("#7c4a52"); // crimson-black
            if (c.Contains("weaver"))   return Hex("#5f7a80"); // tide teal
            if (c.Contains("wildborn")) return Hex("#6a7a5a"); // deep moss
            if (c.Contains("sovereign")) return Hex("#5d6774"); // storm slate-bronze
            return Hex("#8a8f98");
        }
        static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }
        static float HeightFor(string character)
        {
            string c = character.ToLowerInvariant();
            if (c.Contains("ravager")) return 1.95f;
            if (c.Contains("warden")) return 1.75f;
            if (c.Contains("veilborn")) return 1.78f;
            if (c.Contains("weaver")) return 1.72f;
            if (c.Contains("wildborn")) return 1.82f;
            if (c.Contains("sovereign")) return 1.90f;
            return 1.80f;
        }

        static Transform FindHand(Transform t)
        {
            foreach (Transform c in t)
            {
                string n = c.name.ToLowerInvariant().Replace(":", "").Replace("_", "");
                if (n.Contains("righthand")) return c;
                var deep = FindHand(c);
                if (deep != null) return deep;
            }
            return null;
        }

        static IEnumerable<string> GamePrefabs() =>
            AssetDatabase.FindAssets("t:Prefab", new[] { Prefabs })
                .Select(p => AssetDatabase.GUIDToAssetPath(p))
                .Where(p => p.EndsWith("-GAME.prefab"));

        // v225.1 DIAGNOSTIC: dump the class avatar's humanoid bone map next to the mocap source's.
        // v225 locked 96 position curves to zero yet the walk retarget STILL explodes (same 67x103x42
        // bounds) while idle retargets clean — so the blowout is rotation/muscle-level. A mis-mapped
        // human bone in the AUTO-MAPPED class avatar is the prime suspect: idle (near-bind) survives
        // a wrong map; walk (big hips/leg drive) detonates it. Both maps now visible in every CI run.
        static void DumpAvatarMap(Avatar av, string label, System.Text.StringBuilder log)
        {
            if (av == null) { Log(log, label + ": no avatar"); return; }
            if (!av.isHuman) { Log(log, label + ": NOT humanoid"); return; }
            var parts = new System.Collections.Generic.List<string>();
            foreach (var h in av.humanDescription.human) parts.Add(h.humanName + "=" + h.boneName);
            Log(log, label + " boneMap [" + parts.Count + "]: " + string.Join(", ", parts));
        }

        // v225: THE CLASS-WALK FIX. The vaulted Aedan clips can still carry raw CMU capture-world
        // Hips translation (63dm travel + 15.7dm vertical). Retargeted onto class rigs it flings the
        // skeleton across capture coordinates — renderer bounds blew to 67x103x42 on a 1.95m rig
        // (CI logs, Sept 13). CMU ROOT LOCK law enforced HERE, at forge time: every position curve
        // (RootT / MotionT / m_LocalPosition) is locked to the bind convention (zero). Limbs carry
        // the motion; the shell's locomotion owns the transform. If stripped == 0, the clip exposes
        // no editable position curves and the walk failure is avatar-level, not root-level — the log
        // line makes that split visible in CI.
        static AnimationClip InPlaceCopy(AnimationClip src, string character, string state, System.Text.StringBuilder log)
        {
            var copy = UnityEngine.Object.Instantiate(src);
            copy.name = src.name + "-IP";
            int stripped = 0, failed = 0;
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string p = b.propertyName;
                if (string.IsNullOrEmpty(p)) continue;
                if (p.StartsWith("RootT") || p.StartsWith("MotionT") || p.StartsWith("m_LocalPosition"))
                {
                    try
                    {
                        var zero = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(Mathf.Max(0.01f, src.length), 0f));
                        AnimationUtility.SetEditorCurve(copy, b, zero);
                        stripped++;
                    }
                    catch { failed++; }
                }
            }
            Log(log, $"Clip {src.name}: in-place root-lock — {stripped} position curves zeroed" + (failed > 0 ? $", {failed} not editable" : "") + (stripped == 0 && failed == 0 ? " (no position curves exposed — avatar-level suspect)" : ""));
            string path = $"{Root}/Animators/{character}-{state}-IP.anim";
            try { if (System.IO.File.Exists(path)) AssetDatabase.DeleteAsset(path); } catch { }
            AssetDatabase.CreateAsset(copy, path);
            return copy;
        }

        static AnimationClip FindClip(string prefix)
        {
            if (!AssetDatabase.IsValidFolder(Mocap)) return null;
            foreach (var path in AssetDatabase.FindAssets("t:AnimationClip", new[] { Mocap })
                .Select(p => AssetDatabase.GUIDToAssetPath(p)))
            {
                foreach (var clip in AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>())
                    if (clip != null && !clip.name.StartsWith("__preview__")
                        && clip.name.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) >= 0)
                        return clip;
            }
            return null;
        }

        // v238.4 — is this mesh an embedded weapon prop (not the body)? Name-based, conservative.
        static bool IsWeaponMeshName(string n)
        {
            if (string.IsNullOrEmpty(n)) return false;
            var s = n.ToLowerInvariant();
            return s.Contains("spear") || s.Contains("sword") || s.Contains("axe") || s.Contains("blade")
                || s.Contains("dagger") || s.Contains("staff") || s.Contains("weapon") || s.Contains("bow");
        }

        // v238.3 — count DISTINCT bones that actually influence vertices (BoneWeight indices with weight > 0).
        // A skinned rigid prop (weapon) lists the whole armature in .bones but carries weights on one bone only;
        // a real body mesh deforms across dozens. This is the only body-true discriminator seen so far.
        static int WeightedBoneCount(SkinnedMeshRenderer r)
        {
            var m = r.sharedMesh;
            if (m == null) return 0;
            try
            {
                var set = new HashSet<int>();
                var bw = m.boneWeights;
                for (int i = 0; i < bw.Length; i++)
                {
                    set.Add(bw[i].boneIndex0);
                    if (bw[i].weight1 > 0f) set.Add(bw[i].boneIndex1);
                    if (bw[i].weight2 > 0f) set.Add(bw[i].boneIndex2);
                    if (bw[i].weight3 > 0f) set.Add(bw[i].boneIndex3);
                }
                return set.Count;
            }
            catch { return 0; }
        }

        static Bounds CombineBounds(Renderer[] rs)
        {
            var b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            return b;
        }

        static string GetArg(string[] args, string flag)
        {
            for (int i = 0; i < args.Length - 1; i++)
                if (string.Equals(args[i], flag, StringComparison.OrdinalIgnoreCase)) return args[i + 1];
            return null;
        }

        static bool IsBatchMode(string[] args) =>
            args != null && args.Any(a => string.Equals(a, "-batchmode", StringComparison.OrdinalIgnoreCase));

        static float ParseFloat(string s, float dflt) =>
            float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : dflt;

        static void Fail(StringBuilder log, string msg)
        {
            log.AppendLine("FAIL: " + msg);
            Debug.LogError("[AUTO-FORGE] " + msg + "\nSee the runbook — each step's prerequisite is listed there.");
            LastReportPath = $"{Reports}/forge-error-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
            File.WriteAllText(LastReportPath, log.ToString());
            ExitCode = 1;
        }

        static void Log(StringBuilder sb, string msg)
        {
            sb.AppendLine(msg);
            Debug.Log("[AUTO-FORGE] " + msg);
        }

        static string JsonEscape(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"")
            .Replace("\n", "\\n").Replace("\r", "") + "\"";
    }
}
