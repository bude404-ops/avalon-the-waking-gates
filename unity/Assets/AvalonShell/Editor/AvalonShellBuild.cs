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
            // Guard: forged class prefabs don't exist until the forge lands (Sovereign-first law) —
            // the shell runs fine on 2D canon art alone; skip silently when the folder is absent.
            foreach (var f in (Directory.Exists("Assets/AvalonForge/Prefabs") ? Directory.GetFiles("Assets/AvalonForge/Prefabs", "*.prefab") : new string[0]))
            {
                var name = Path.GetFileName(f);
                File.Copy(f, "Assets/AvalonShell/Resources/" + name, true);
                var meta = f + ".meta";
                if (File.Exists(meta)) File.Copy(meta, "Assets/AvalonShell/Resources/" + name + ".meta", true);
                staged++;
                Debug.Log($"[SHELL] staged prefab: {name}");
            }
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
