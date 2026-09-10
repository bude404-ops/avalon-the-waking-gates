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
            int staged = 0;
            foreach (var f in Directory.GetFiles("Assets/AvalonForge/Prefabs", "*-GAME.prefab"))
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
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

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
