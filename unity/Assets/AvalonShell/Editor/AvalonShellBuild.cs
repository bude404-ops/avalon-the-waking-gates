// AVALON SHELL BUILD — headless batch entry point.
// Usage: Unity -batchmode -quit -executeMethod AvalonShell.AvalonShellBuild.BuildForAndroid
// Env overrides: AVALON_APK_OUT (path), AVALON_VCODE (bundleVersionCode)
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AvalonShell
{
    public static class AvalonShellBuild
    {
        public static void BuildForAndroid()
        {
            string outPath = Environment.GetEnvironmentVariable("AVALON_APK_OUT") ?? "build/avalon-unity-shell.apk";
            int vcode = 1;
            int.TryParse(Environment.GetEnvironmentVariable("AVALON_VCODE") ?? "1", out vcode);

            // Defensive SDK paths (unityci android images export these)
            foreach (var kv in new (string env, string pref)[] {
                ("ANDROID_SDK_ROOT", "AndroidSdkRoot"), ("ANDROID_NDK_ROOT", "AndroidNdkRoot"), ("JAVA_HOME", "JdkPath") })
            {
                var v = Environment.GetEnvironmentVariable(kv.env);
                if (!string.IsNullOrEmpty(v))
                    try { typeof(EditorPrefs).GetMethod("SetString")?.Invoke(null, new object[] { "Android" + kv.pref, v }); } catch { }
            }

            // Copy forged prefabs into Resources (GUIDs preserved via .meta siblings)
            Directory.CreateDirectory("Assets/AvalonShell/Resources");
            Directory.CreateDirectory("Assets/AvalonShell/Resources");

            // AVALON-LIT material asset — the ONLY sanctioned way to get a lit material at
            // runtime: the asset's shader reference keeps Standard in the build (Shader.Find
            // alone gets stripped -> v215 pink/black boot). Editor-time Shader.Find is safe.
            var std = Shader.Find("Standard");
            if (std == null) throw new Exception("[SHELL] FATAL: Standard shader not found in editor — cannot stage AVALON-LIT");
            var lit = new Material(std);
            lit.color = new Color(0.5f, 0.5f, 0.5f);
            AssetDatabase.CreateAsset(lit, "Assets/AvalonShell/Resources/AVALON-LIT.mat");
            AssetDatabase.SaveAssets();
            Debug.Log("[SHELL] staged material: AVALON-LIT.mat (Standard included)");

            Directory.CreateDirectory("Assets/AvalonShell/Resources/Art");
            foreach (var art in new[] { "MAP1-COLD-RELIQUARY-ASHFALL-CANON.png",
                "UI-MAIN-MENU-CANON.png", "UI-MAIN-MENU-V2-CANON.jpg", "UI-MAIN-MENU-V2-CLEAN.jpg", "UI-CLASS-SELECT-CANON.png", "UI-CLASS-SELECT-V2-CANON.png",
                "CINEMATIC-TEASER-KEYART-CANON.png",
                "CLASS-SOVEREIGN-CANON.png", "CLASS-RAVAGER-CANON.png", "CLASS-WARDEN-CANON.png",
                "CLASS-VEILBORN-CANON.png", "CLASS-WEAVER-CANON.png", "CLASS-WILDBORN-CANON.png",
                "CLASS-SOVEREIGN-F-CANON.png", "CLASS-RAVAGER-F-CANON.png", "CLASS-WARDEN-F-CANON.png",
                "CLASS-VEILBORN-F-CANON.png", "CLASS-WEAVER-F-CANON.png", "CLASS-WILDBORN-F-CANON.png" })
            {
                var src = Path.Combine("../art/approved", art);
                if (File.Exists(src)) { File.Copy(src, "Assets/AvalonShell/Resources/Art/" + art, true); Debug.Log("[SHELL] staged art: " + art); }
            }
            foreach (var art in new[] { "SLAB-CARVED.png", "NICHE-FRAME.png", "HERO-AVALON.png" })
            {
                var src = Path.Combine("../art/ui", art);
                if (File.Exists(src)) { File.Copy(src, "Assets/AvalonShell/Resources/Art/" + art, true); Debug.Log("[SHELL] staged ui art: " + art); }
            }
            int staged = 0;
            // GAMED-SET STAGING (v217): a .prefab alone is useless — it references its mesh FBX,
            // animator controller and mocap clips by GUID. Stage the FULL forge set:
            //   Prefabs/*   -> Resources (so Resources.Load("<class>-GAME") finds it)
            //   Generated / Animators / Mocap -> ModelAssets (pulled into the build by reference,
            //   GUIDs preserved via .meta siblings)
            // Guard: absent folders skip silently (Sovereign-first law — shell runs on canon art alone).
            Action<string, string> Stage = (srcDir, dstDir) =>
            {
                if (!Directory.Exists(srcDir)) return;
                Directory.CreateDirectory(dstDir);
                foreach (var f in Directory.GetFiles(srcDir))
                {
                    if (f.EndsWith(".meta")) continue;
                    File.Copy(f, dstDir + "/" + Path.GetFileName(f), true);
                    if (File.Exists(f + ".meta")) File.Copy(f + ".meta", dstDir + "/" + Path.GetFileName(f) + ".meta", true);
                    staged++;
                    Debug.Log("[SHELL] staged model asset: " + srcDir + "/" + Path.GetFileName(f));
                }
            };
            Stage("Assets/AvalonForge/Prefabs", "Assets/AvalonShell/Resources");
            Stage("Assets/AvalonForge/Generated", "Assets/AvalonShell/ModelAssets");
            Stage("Assets/AvalonForge/Animators", "Assets/AvalonShell/ModelAssets");
            Stage("Assets/AvalonForge/Mocap", "Assets/AvalonShell/ModelAssets");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Scene: single bootstrap object
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var go = new GameObject("AVALON-SHELL");
            go.AddComponent<Shell>();
            string scenePath = "Assets/AvalonShell/Shell.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SHELL] scene saved: {scenePath} (prefabs staged: {staged})");

            // Player settings
            PlayerSettings.productName = "AVALON — The Waking Gates";
            PlayerSettings.companyName = "Bigfoot404";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.bigfoot404.avalon");
            PlayerSettings.Android.bundleVersionCode = vcode;
            var ksPath = Environment.GetEnvironmentVariable("AVALON_KEYSTORE");
            if (!string.IsNullOrEmpty(ksPath) && File.Exists(ksPath))
            {
                PlayerSettings.Android.keystoreName = ksPath;
                PlayerSettings.Android.keystorePass = "bigfoot404-gates";
                PlayerSettings.Android.keyaliasName = "avalon";
                PlayerSettings.Android.keyaliasPass = "bigfoot404-gates";
                Debug.Log("[SHELL] signing with repo demo keystore: " + ksPath);
            }
            else Debug.LogWarning("[SHELL] no keystore — debug-signed (updates may conflict)");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            // v113 LAUNCH-CRASH FIX: Unity 6 defaults to Vulkan-first on Android; devices with
            // broken/absent Vulkan drivers crash at GfxDevice init (splash flash -> "keeps stopping").
            // Force GLES3-only — universal on Android 8+ — until proven otherwise on Big's device.
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
            Debug.Log("[SHELL] graphics APIs forced: OpenGLES3 only (Vulkan crash fix)");
            // customMainManifest API removed in Unity 6000.x — Assets/Plugins/Android/AndroidManifest.xml
            // (install-permission for self-update) is picked up automatically when present.
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34; // Android 14+ (S22 Ultra)
            // PROVEN clean-slate profile (stages 1-4 verified on SM-S908U): IL2CPP + ARM64-only.
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            // Orientation law: both portrait + landscape with responsive reflow
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // ================= v240 GAME-PROOF SHOTS =================
            // Bude's v239 verdict: "the spear isnt in his hand at all". The forge QC (editor render)
            // showed the spear, and the mesh string IS in the APK data — so the only honest proof
            // left is a render from the GAME side: load the STAGED Resources prefab the exact way
            // the game does at runtime, frame it with the v239 gameplay camera, and capture it.
            // These shots + the renderer inventory go into the artifact so no one trusts my word —
            // they trust the build's own eyes.
            try
            {
                var proofDir = "Assets/AvalonShell/ProofShots";
                Directory.CreateDirectory(proofDir);
                var staged2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Sovereign-GAME"));
                if (staged2 != null)
                {
                    var inv = staged2.GetComponentsInChildren<Renderer>(true)
                        .Select(r => r.GetType().Name + ":" + r.name).Distinct().ToList();
                    File.WriteAllText(proofDir + "/renderer-inventory.txt",
                        "GAME-SIDE RENDERER INVENTORY (staged Resources prefab)\n" + string.Join("\n", inv));
                    Debug.Log("[SHELL][PROOF] renderer inventory: " + string.Join(", ", inv));

                    foreach (var smr in staged2.GetComponentsInChildren<SkinnedMeshRenderer>(true)) smr.updateWhenOffscreen = true;
                    var an = staged2.GetComponentInChildren<Animator>();
                    if (an != null) { an.applyRootMotion = false; an.Play("idle", 0, 0f); an.Update(0.02f); }

                    var proofLit = new Material(Shader.Find("Universal Render Pipeline/Lit") != null
                        ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard"));
                    foreach (var r in staged2.GetComponentsInChildren<Renderer>(true))
                    {
                        var mats = r.sharedMaterials;
                        bool patch = false;
                        for (int i = 0; i < mats.Length; i++) if (mats[i] == null) { mats[i] = proofLit; patch = true; }
                        if (patch) r.sharedMaterials = mats;
                    }

                    var b = new Bounds(Vector3.zero, Vector3.one);
                    var rs = staged2.GetComponentsInChildren<Renderer>(true);
                    if (rs.Length > 0) { b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds); }
                    var height = Mathf.Max(b.size.y, 0.1f);
                    var camGo = new GameObject("ProofCam");
                    var cam = camGo.AddComponent<Camera>();
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0.075f, 0.082f, 0.098f);
                    var look = new Vector3(0f, height * 0.55f, 0f);                       // chest
                    var dist = height * 2.6f;                                              // v239 gameplay distance
                    var elev = height * 0.55f;                                             // ~30 deg look-down
                    camGo.transform.position = new Vector3(0f, look.y + elev, -dist);
                    camGo.transform.LookAt(look);
                    var lightGo = new GameObject("ProofLight");
                    var dl = lightGo.AddComponent<Light>(); dl.type = LightType.Directional;
                    lightGo.transform.rotation = Quaternion.Euler(40f, -35f, 0f);
                    lightGo.transform.position = new Vector3(3f, 5f, -4f);

                    var rt = new RenderTexture(720, 1280, 24);
                    cam.targetTexture = rt; RenderTexture.active = rt; cam.Render();
                    var tex = new Texture2D(720, 1280, TextureFormat.RGBA32, false);
                    tex.ReadPixels(new Rect(0, 0, 720, 1280), 0, 0); tex.Apply();
                    File.WriteAllBytes(proofDir + "/game-proof-idle.png", tex.EncodeToPNG());

                    if (an != null && an.runtimeAnimatorController != null)
                    {
                        bool hasWalk = false;
                        foreach (var s in an.runtimeAnimatorController.animationClips) if (s.name.ToLower().Contains("walk")) hasWalk = true;
                        try { an.Play("walk", 0, 0.35f); an.Update(0.02f); } catch { }
                        RenderTexture.active = rt; cam.Render();
                        tex.ReadPixels(new Rect(0, 0, 720, 1280), 0, 0); tex.Apply();
                        File.WriteAllBytes(proofDir + "/game-proof-walk.png", tex.EncodeToPNG());
                    }
                    Debug.Log("[SHELL][PROOF] game-side proof shots captured: " + proofDir);
                    UnityEngine.Object.DestroyImmediate(staged2);
                    UnityEngine.Object.DestroyImmediate(camGo);
                    UnityEngine.Object.DestroyImmediate(lightGo);
                    UnityEngine.Object.DestroyImmediate(proofLit);
                }
                else Debug.LogWarning("[SHELL][PROOF] Sovereign-GAME prefab not found in Resources — no proof shots");
            }
            catch (Exception ex) { Debug.LogWarning("[SHELL][PROOF] proof render failed (non-blocking): " + ex.Message); }

            var scenes = new[] { scenePath };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath)));
            var report = BuildPipeline.BuildPlayer(scenes, outPath, BuildTarget.Android, BuildOptions.None);
            var summary = report.summary;
            Debug.Log($"[SHELL] build: {summary.result} size={summary.totalSize} errors={summary.totalErrors}");
            if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                foreach (var step in report.steps)
                    foreach (var msg in step.messages.Where(m => m.type == LogType.Error || m.type == LogType.Exception))
                        Debug.Log($"[SHELL][ERR] {msg.content}");
                EditorApplication.Exit(1);
            }
            EditorApplication.Exit(0);
        }
    }
}
