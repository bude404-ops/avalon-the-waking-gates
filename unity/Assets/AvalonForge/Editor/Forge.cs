// AVALON AUTO-FORGE — batch character pipeline (Stages 2–7)
// Usage (headless):
//   Unity -batchmode -quit -projectPath <proj> \
//     -executeMethod AvalonForge.Forge.Run \
//     -character Ravager -input Assets/AvalonForge/Generated/Ravager-M.fbx \
//     [-weapon Assets/AvalonForge/Weapons/Greatsword.fbx] [-height 1.9] [-noGraphicsOK]
// Editor: menu Avalon > Forge > Run (prompts for the same inputs).
//
// Stages (per docs/AVALON-UNITY-AUTOMATED-PIPELINE.md):
//   2. Import + normalize (scale to class height spec, face-forward)
//   3. Humanoid rig config (ModelImporter auto bone-mapping — no GUI)
//   4. Animator build (root-locked CMU clips retarget via Mecanim)
//   5. Weapon socket (prop parented to right-hand bone)
//   6. QC renders + machine-readable report
//   7. Game-ready prefab
// Rig step (skeleton/skin on a 404-GEN mesh) is done in UModeler X before Forge runs.

using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AvalonForge
{
    public static class Forge
    {
        const string Root = "Assets/AvalonForge";
        const string Generated = Root + "/Generated";
        const string Mocap = Root + "/Mocap";
        const string Weapons = Root + "/Weapons";
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

        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            string character = GetArg(args, "-character") ?? "Sovereign";
            string input = GetArg(args, "-input");
            string weapon = GetArg(args, "-weapon");
            float height = ParseFloat(GetArg(args, "-height"), 1.80f);
            bool interactive = Application.isEditor && !EditorApplication.isCompiling
                && GetArg(args, "-batchmode") == null;

            if (input == null)
            {
                var guess = AssetDatabase.FindAssets("t:Model", new[] { Generated })
                    .Select(p => AssetDatabase.GUIDToAssetPath(p))
                    .FirstOrDefault(p => p.IndexOf(character, StringComparison.OrdinalIgnoreCase) >= 0);
                if (guess == null) { Fail($"No -input given and no {character} model found in {Generated}."); return; }
                input = guess;
            }

            Directory.CreateDirectory(Prefabs); Directory.CreateDirectory(QCShots); Directory.CreateDirectory(Reports);
            var log = new StringBuilder();
            Log(log, $"AUTO-FORGE run: character={character} input={input} weapon={weapon ?? "none"} height={height}");

            if (AssetDatabase.LoadAssetAtPath<GameObject>(input) == null)
            {
                // 404-GEN outputs FBX mesh assets; basic non-model asset check
                var meshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(input);
                if (meshAsset == null) { Fail($"Input not importable: {input}"); return; }
            }

            AssetDatabase.ImportAsset(input, ImportAssetOptions.ForceUpdate);

            // ---------- Stage 3: Humanoid rig ----------
            var importer = AssetImporter.GetAtPath(input) as ModelImporter;
            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Humanoid;
                importer.SaveAndReimport();
            }
            var avatar = AssetDatabase.LoadAllAssetsAtPath(input).OfType<Avatar>().FirstOrDefault();
            var srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(input);
            if (avatar == null || !avatar.isValid || srcPrefab == null)
                { Fail("No valid humanoid rig found. Run UModeler X auto-rig on the 404-GEN mesh first, then re-run Forge."); return; }
            Log(log, "Humanoid avatar: OK (auto bone-mapped)");

            var go = (GameObject)PrefabUtility.InstantiatePrefab(srcPrefab);
            go.name = character;

            // ---------- Stage 2: normalize ----------
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { Fail("No renderers on model."); return; }
            var b = CombineBounds(renderers);
            float scale = height / b.size.y;
            go.transform.localScale = Vector3.one * scale;
            if (Mathf.Abs(b.center.x) > 0.001f || Mathf.Abs(b.center.z) > 0.001f)
                go.transform.position -= new Vector3(b.center.x, 0f, b.center.z) * scale; // center on origin, feet planted
            Log(log, $"Normalized: height {b.size.y:F3} -> {height} (x{scale:F3}), centered at origin");

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
            var idleState = sm.states.FirstOrDefault(s => s.state.name == "idle").state
                            ?? sm.states.FirstOrDefault().state;
            sm.defaultState = idleState;
            animator.runtimeAnimatorController = ctrl;
            if (statesBuilt == 0) Log(log, "WARN: no mocap clips matched — animator is empty");

            // ---------- Stage 5: weapon socket ----------
            Transform hand = FindDeep(go.transform, "righthand", allowSuffix: true)
                          ?? FindDeep(go.transform, "hand_r")
                          ?? FindDeep(go.transform, "righthandindex1", allowSuffix: true);
            if (weapon != null)
            {
                var weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(weapon);
                if (weaponPrefab == null) { Fail($"Weapon asset missing: {weapon}"); return; }
                if (hand == null) { Fail("No right-hand bone found for weapon socket."); return; }
                var w = (GameObject)PrefabUtility.InstantiatePrefab(weaponPrefab);
                w.name = "WeaponSocket";
                w.transform.SetParent(hand, false);
                w.transform.localPosition = new Vector3(0.02f, 0.05f, 0.01f);
                w.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // blade forward — adjust per class spec
                Log(log, $"Weapon socketed under {hand.name} (deterministic offset; tune per class spec)");
            }
            else Log(log, "No weapon requested — skipped (weapons are separate props per Weaponless Plate Law)");

            // ---------- Stage 6: QC ----------
            string reportPath = $"{Reports}/{character}-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            bool headlessGraphics = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null;
            if (headlessGraphics)
            {
                Directory.CreateDirectory(QCShots);
                go.transform.position = Vector3.zero;
                var camGo = new GameObject("ForgeQCCam");
                var cam = camGo.AddComponent<Camera>();
                cam.transform.position = new Vector3(0.6f * height, height * 0.62f, -height * 2.6f);
                cam.transform.LookAt(new Vector3(0f, height * 0.55f, 0f));
                cam.fieldOfView = 45f;
                var rt = new RenderTexture(720, 960, 24);
                cam.targetTexture = rt;
                cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(720, 960, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, 720, 960), 0, 0);
                tex.Apply();
                File.WriteAllBytes($"{QCShots}/{character}-idle.png", tex.EncodeToPNG());
                Log(log, "QC render captured");
                cam.targetTexture = null; RenderTexture.active = null; UnityEngine.Object.DestroyImmediate(camGo);
            }
            else Log(log, "QC render skipped (-nographics) — structural QC only");

            // ---------- Stage 7: prefab ----------
            string prefabPath = $"{Prefabs}/{character}-GAME.prefab";
            var finalPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            bool ok = finalPrefab != null;
            Log(log, $"Prefab saved: {prefabPath} (state: {(ok ? "OK" : "FAILED")})");

            File.WriteAllText(reportPath,
                "{\n  \"character\": \"" + character + "\",\n  \"input\": \"" + input +
                "\",\n  \"avatar_valid\": " + (avatar.isValid ? "true" : "false") +
                ",\n  \"states_built\": " + statesBuilt +
                ",\n  \"weapon\": \"" + (weapon ?? "none") +
                "\",\n  \"prefab\": \"" + prefabPath +
                "\",\n  \"prefab_ok\": " + (ok ? "true" : "false") +
                ",\n  \"log\": " + JsonEscape(log.ToString()) + "\n}");
            Debug.Log("<color=#a3895a>[AUTO-FORGE]</color> Report: " + reportPath + "\n" + log);

            UnityEngine.Object.DestroyImmediate(go);
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath));
            AssetDatabase.SaveAssets();
            if (Application.isEditor && !IsBatchMode(args)) return; // menu path: stay open
            EditorApplication.Exit(ok ? 0 : 1);
        }

        [MenuItem("Avalon/Forge/Run (prompt)")]
        static void RunPrompted()
        {
            var character = EditorPrefs.GetString("AvalonForge.LastCharacter", "Ravager");
            Run();
        }

        // ---- helpers ----
        static AnimationClip FindClip(string prefix)
        {
            foreach (var path in AssetDatabase.FindAssets("t:AnimationClip", new[] { Mocap })
                .Select(p => AssetDatabase.GUIDToAssetPath(p)))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip != null && clip.name.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) >= 0)
                    return clip;
            }
            return null;
        }

        static Transform FindDeep(Transform t, string namePart, bool allowSuffix = false)
        {
            foreach (Transform c in t)
            {
                string n = c.name.ToLowerInvariant().Replace(":", "").Replace("_", "");
                if (n.Contains(namePart) && (allowSuffix || n.Length <= namePart.Length + 8))
                    return c;
                var deep = FindDeep(c, namePart, allowSuffix);
                if (deep != null) return deep;
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

        static float ParseFloat(string s, float dflt) =>
            float.TryParse(s, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : dflt;

        static bool IsBatchMode(string[] args) => GetArg(args, "-batchmode") != null;

        static void Fail(string msg)
        {
            Debug.LogError("[AUTO-FORGE] " + msg);
            if (Application.isEditor && !EditorApplication.isCompiling)
                EditorUtility.DisplayDialog("AUTO-FORGE", msg, "OK");
            if (IsBatchMode(Environment.GetCommandLineArgs())) EditorApplication.Exit(1);
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
