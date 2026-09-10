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
                else if (animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
                    gates.Add("FAIL avatar invalid/not humanoid");
                else gates.Add("PASS avatar humanoid");

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

            // ---------- Stage 3: Humanoid rig ----------
            var importer = AssetImporter.GetAtPath(input) as ModelImporter;
            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.SaveAndReimport();
            }
            var avatar = AssetDatabase.LoadAllAssetsAtPath(input).OfType<Avatar>().FirstOrDefault();
            var srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(input);
            if (avatar == null || !avatar.isValid || srcPrefab == null)
            { Fail(log, "No valid humanoid rig. Run UModeler X auto-rig on the 404-GEN mesh first (runbook step 2), then re-run Forge."); return; }
            Log(log, "Humanoid avatar: OK (auto bone-mapped)");

            var go = (GameObject)PrefabUtility.InstantiatePrefab(srcPrefab);
            go.name = character;

            // ---------- Stage 2: normalize ----------
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { Fail(log, "No renderers on model."); return; }
            var b = CombineBounds(renderers);
            float scale = height / b.size.y;
            go.transform.localScale = Vector3.one * scale;
            go.transform.position -= new Vector3(b.center.x, 0f, b.center.z) * scale;
            Log(log, $"Normalized: height {b.size.y:F3} -> {height} (x{scale:F3}), centered on origin");

            // ---------- Stage 4: Animator ----------
            var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            animator.avatar = avatar;

            string ctrlPath = $"{Root}/Animators/{character}.controller";
            Directory.CreateDirectory($"{Root}/Animators");
            var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
            var sm = ctrl.layers[0].stateMachine;
            int statesBuilt = 0;
            foreach (var (state, prefix) in States)
            {
                var clip = FindClip(prefix);
                if (clip == null) continue;
                var st = sm.AddState(state, new Vector3(220f * statesBuilt, 0f));
                st.motion = clip;
                statesBuilt++;
                Log(log, $"Animator state '{state}' <- {clip.name}");
            }
            if (statesBuilt == 0) Log(log, "WARN: no mocap clips matched — animator is empty (drop clips in " + Mocap + "/)");
            var anyState = sm.states.FirstOrDefault();
            if (!anyState.Equals(default(ChildAnimatorState))) sm.defaultState = anyState.state;
            animator.runtimeAnimatorController = ctrl;

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
            else Log(log, "No weapon — skipped (weapons are separate props per Weaponless Plate Law)");

            // ---------- Stage 6: QC ----------
            bool hasGraphics = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null;
            if (hasGraphics)
            {
                Directory.CreateDirectory(QCShots);
                go.transform.position = Vector3.zero;
                var camGo = new GameObject("ForgeQCCam");
                var cam = camGo.AddComponent<Camera>();
                cam.transform.position = new Vector3(0.6f * height, height * 0.62f, -height * 2.6f);
                cam.transform.LookAt(new Vector3(0f, height * 0.55f, 0f));
                cam.fieldOfView = 45f;
                var rt = new RenderTexture(720, 960, 24);
                cam.targetTexture = rt; cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(720, 960, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, 720, 960), 0, 0); tex.Apply();
                File.WriteAllBytes($"{QCShots}/{character}-idle.png", tex.EncodeToPNG());
                Log(log, "QC render captured: " + QCShots + "/" + character + "-idle.png");
                cam.targetTexture = null; RenderTexture.active = null;
                UnityEngine.Object.DestroyImmediate(camGo);
            }
            else Log(log, "QC render skipped (-nographics) — structural QC only");

            // ---------- Stage 7: prefab ----------
            string prefabPath = $"{Prefabs}/{character}-GAME.prefab";
            var finalPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            bool ok = finalPrefab != null;
            Log(log, $"Prefab saved: {prefabPath} ({(ok ? "OK" : "FAILED")})");

            LastReportPath = $"{Reports}/{character}-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            File.WriteAllText(LastReportPath,
                "{\n  \"character\": \"" + character + "\",\n  \"input\": \"" + input +
                "\",\n  \"avatar_valid\": " + (avatar.isValid ? "true" : "false") +
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

        static AnimationClip FindClip(string prefix)
        {
            if (!AssetDatabase.IsValidFolder(Mocap)) return null;
            foreach (var path in AssetDatabase.FindAssets("t:AnimationClip", new[] { Mocap })
                .Select(p => AssetDatabase.GUIDToAssetPath(p)))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip != null && clip.name.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) >= 0)
                    return clip;
            }
            return null;
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
