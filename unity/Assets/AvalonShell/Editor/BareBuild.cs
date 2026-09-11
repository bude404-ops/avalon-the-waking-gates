// AVALON CLEAN-SLATE BUILD — stage 1: BARE
// Empty scene (camera + light only), ZERO game code, ZERO custom manifest.
// Device-locked profile: Samsung Galaxy S22 Ultra (SM-S908U) — Big's test bench.
// Env overrides: AVALON_APK_OUT (path), AVALON_VCODE (bundleVersionCode), AVALON_KEYSTORE (repo AVALON-DEMO.keystore)
// Layer plan (one layer added per build, Big tests each):
//   stage 1 = bare   (this file)      stage 2 = + 3D scene
//   stage 3 = + shell HUD             stage 4 = + self-updater
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AvalonShell
{
    public static class BareBuild
    {
        public static void Run()
        {
            int vcode = 201;
            int.TryParse(Environment.GetEnvironmentVariable("AVALON_VCODE") ?? "201", out vcode);
            var outPath = Environment.GetEnvironmentVariable("AVALON_APK_OUT") ?? "build/avalon-bare.apk";

            // ---- Player settings: SM-S908U device profile ----
            PlayerSettings.productName = "AVALON";
            PlayerSettings.companyName = "Bigfoot404";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.bigfoot404.avalon");
            PlayerSettings.Android.bundleVersionCode = vcode;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34; // Android 14+ (S22 Ultra)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP); // arm64 requires IL2CPP
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64; // S22 Ultra is arm64-only
            // Orientation law: both portrait + landscape with responsive reflow
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            // v113 crash fix: GLES3-only — Vulkan-first crashed at GfxDevice init on the test bench
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });

            var ksPath = Environment.GetEnvironmentVariable("AVALON_KEYSTORE");
            if (!string.IsNullOrEmpty(ksPath) && File.Exists(ksPath))
            {
                PlayerSettings.Android.keystoreName = ksPath;
                PlayerSettings.Android.keystorePass = "bigfoot404-gates";
                PlayerSettings.Android.keyaliasName = "avalon";
                PlayerSettings.Android.keyaliasPass = "bigfoot404-gates";
            }

            // ---- Bare scene: default camera + light, nothing else ----
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            const string scenePath = "Assets/Scenes/Bare.unity";
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath)));
            var report = BuildPipeline.BuildPlayer(new[] { scenePath }, outPath, BuildTarget.Android, BuildOptions.None);
            var s = report.summary;
            Debug.Log($"[BARE] result={s.result} size={s.totalSize} errors={s.totalErrors}");
            if (s.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                EditorApplication.Exit(1);
            EditorApplication.Exit(0);
        }
    }
}
