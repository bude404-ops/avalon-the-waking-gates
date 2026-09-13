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
                "UI-MAIN-MENU-CANON.png", "UI-CLASS-SELECT-CANON.png",
                "CINEMATIC-TEASER-KEYART-CANON.png",
                "CLASS-SOVEREIGN-CANON.png", "CLASS-RAVAGER-CANON.png", "CLASS-WARDEN-CANON.png",
                "CLASS-VEILBORN-CANON.png", "CLASS-WEAVER-CANON.png", "CLASS-WILDBORN-CANON.png" })
            {
                var src = Path.Combine("../art/approved", art);
                if (File.Exists(src)) { File.Copy(src, "Assets/AvalonShell/Resources/Art/" + art, true); Debug.Log("[SHELL] staged art: " + art); }
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
