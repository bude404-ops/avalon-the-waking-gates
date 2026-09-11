// AVALON CLEAN-SLATE APK — bare-bones stock Unity build (zero app code)
// Step 1 of the stepwise rebuild ladder (docs/CLEAN-SLATE-APK-PLAN.md):
//   v114 bare stock scene -> v115 + canon gallery viewer -> v116 + shell HUD -> v117 + self-updater
// This script is EDITOR-ONLY build tooling. Nothing here ships inside the APK runtime.
// Scene contents: static primitives only (camera, light, cubes) — ZERO MonoBehaviours.
// Target: Samsung Galaxy S23 / Android 14 (Big's device, Sept 11 2026).
//
// Usage (headless):
//   Unity -batchmode -quit -projectPath <proj> -executeMethod AvalonClean.CleanApkBuild.BuildForAndroid
// Env: AVALON_APK_OUT (path), AVALON_VCODE (bundleVersionCode, default 114), AVALON_KEYSTORE (repo demo keystore)

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AvalonClean
{
    public static class CleanApkBuild
    {
        public static void BuildForAndroid()
        {
            var outPath = Environment.GetEnvironmentVariable("AVALON_APK_OUT");
            if (string.IsNullOrEmpty(outPath)) outPath = "build/avalon-clean.apk";
            var vcode = 114;
            int.TryParse(Environment.GetEnvironmentVariable("AVALON_VCODE") ?? "", out vcode);

            Debug.Log($"[CLEAN] v{vcode} -> {outPath}");

            // ---- bare scene: a minimal brazier shrine of static primitives (zero scripts) ----
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            // DefaultGameObjects gives camera + directional light only.
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0f, 2.2f, -6.5f);
                cam.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
                cam.backgroundColor = new Color(0.03f, 0.03f, 0.04f);
                cam.clearFlags = CameraClearFlags.SolidColor;
            }

            var stone = new Material(Shader.Find("Standard")) { color = new Color(0.42f, 0.42f, 0.45f) };
            var ember = new Material(Shader.Find("Standard")) { color = new Color(0.9f, 0.45f, 0.1f) };
            ember.EnableKeyword("_EMISSION");
            ember.SetColor("_EmissionColor", new Color(0.95f, 0.5f, 0.12f) * 1.6f);

            // ground platform
            MakeCube("platform", new Vector3(0f, -0.25f, 0f), new Vector3(9f, 0.5f, 9f), stone);
            // ring of six megalith slabs
            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI / 3f;
                var p = new Vector3(Mathf.Sin(a) * 2.6f, 1.1f, Mathf.Cos(a) * 2.6f);
                var g = MakeCube("slab" + i, p, new Vector3(0.7f, 2.2f, 0.7f), stone);
                g.transform.rotation = Quaternion.Euler(0f, a * Mathf.Rad2Deg, 0f);
            }
            // the brazier: emissive amber cube on a short pillar
            MakeCube("pillar", new Vector3(0f, 0.4f, 0f), new Vector3(0.8f, 0.8f, 0.8f), stone);
            MakeCube("brazier", new Vector3(0f, 1.15f, 0f), new Vector3(0.55f, 0.55f, 0.55f), ember);

            AssetDatabase.CreateFolder("Assets", "AvalonClean");
            AssetDatabase.CreateFolder("Assets/AvalonClean", "Materials");
            AssetDatabase.CreateAsset(stone, "Assets/AvalonClean/Materials/Stone.mat");
            AssetDatabase.CreateAsset(ember, "Assets/AvalonClean/Materials/Ember.mat");
            const string scenePath = "Assets/AvalonClean/CleanSlate.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("[CLEAN] scene saved: " + scenePath);

            // ---- player settings: Galaxy S23 / Android 14, stock everything ----
            PlayerSettings.productName = "AVALON";
            PlayerSettings.companyName = "Bigfoot404";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.bigfoot404.avalon");
            PlayerSettings.Android.bundleVersionCode = vcode;
            PlayerSettings.bundleVersion = "0.20." + vcode;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;

            var ksPath = Environment.GetEnvironmentVariable("AVALON_KEYSTORE");
            if (!string.IsNullOrEmpty(ksPath) && File.Exists(ksPath))
            {
                PlayerSettings.Android.keystoreName = ksPath;
                PlayerSettings.Android.keystorePass = "bigfoot404-gates";
                PlayerSettings.Android.keyaliasName = "avalon";
                PlayerSettings.Android.keyaliasPass = "bigfoot404-gates";
            }
            else Debug.LogWarning("[CLEAN] no keystore env — CI SIGN GUARANTEE re-signs regardless");

            // v113 crash-fix pattern: GLES3 only (no Vulkan driver surprises)
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });

            // S23 = Snapdragon (ARM64). IL2CPP required for ARM64; ARMv7 dropped = half the APK size.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Android 14

            var scenes = new[] { scenePath };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
            var report = BuildPipeline.BuildPlayer(scenes, outPath, BuildTarget.Android, BuildOptions.None);
            var s = report.summary;
            Debug.Log($"[CLEAN] build: {s.result} size={s.totalSize} errors={s.totalErrors}");
            if (s.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                foreach (var step in report.steps)
                    foreach (var msg in step.messages.Where(m => m.type == LogType.Error || m.type == LogType.Exception))
                        Debug.Log("[CLEAN][ERR] " + msg.content);
                EditorApplication.Exit(1);
            }
            Debug.Log("[CLEAN] SUCCESS");
            EditorApplication.Exit(0);
        }

        static GameObject MakeCube(string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.position = pos;
            g.transform.localScale = scale;
            g.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return g;
        }
    }
}
