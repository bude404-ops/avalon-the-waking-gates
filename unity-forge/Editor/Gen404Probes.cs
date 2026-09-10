// AVALON FORGE — 404-GEN verify-first probes (runs once, before the pipeline).
// Entry: Unity.exe -batchmode -quit -projectPath <proj> -executeMethod AvalonForge.Editor.Gen404Probes.Run -logFile probes.log
// Answers the three Route-A-vs-B questions from docs/UNITY-AUTOMATED-CHARACTER-PIPELINE.md:
//   1. Does 404-GEN expose a scriptable API (public static entry points we can call in batch)?
//   2. Do its generated assets come rigged (SkinnedMeshRenderer with bones)?
//   3. What does its EULA permit (we surface the license text for review — a human reads it)?
// Output: forge-artifacts/probe-report.json

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AvalonForge.Editor
{
    public static class Gen404Probes
    {
        public static void Run()
        {
            int exit = 0;
            try
            {
                var projRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                var outDir = Path.Combine(projRoot, "forge-artifacts");
                Directory.CreateDirectory(outDir);

                // ---- Probe 1: 404-GEN assembly + API surface -----------------
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetName().Name.IndexOf("404", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                (a.GetName().Name + " " + a.FullName).IndexOf("gen", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                var api = new
                {
                    foundAssemblies = assemblies.Select(a => a.GetName().Name).ToArray(),
                    publicStaticEntryPoints = assemblies.SelectMany(a => a.GetTypes())
                        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Where(m => m.DeclaringType != null && (m.DeclaringType.Name.IndexOf("404") >= 0 ||
                                 m.DeclaringType.Namespace?.IndexOf("404", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 m.DeclaringType.Name.IndexOf("Gen", StringComparison.OrdinalIgnoreCase) >= 0))
                            .Select(m => $"{m.DeclaringType.FullName}.{m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})"))
                        .Distinct().Take(50).ToArray(),
                    editorWindows = assemblies.SelectMany(a => a.GetTypes())
                        .Where(t => typeof(EditorWindow).IsAssignableFrom(t)).Select(t => t.FullName).ToArray(),
                };

                // ---- Probe 2: are generated assets rigged? --------------------
                // Scan project for meshes likely produced by 404-GEN and check for bones.
                var generated = AssetDatabase.FindAssets("t:ModelImporter")
                    .Select(g => AssetDatabase.GUIDToAssetPath(g))
                    .Where(p => p.Contains("404", StringComparison.OrdinalIgnoreCase) ||
                                p.Contains("Generated", StringComparison.OrdinalIgnoreCase) ||
                                p.Contains("Splat", StringComparison.OrdinalIgnoreCase))
                    .Take(100).ToArray();
                var rigInfo = generated.Select(p =>
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                    var smr = go ? go.GetComponentInChildren<SkinnedMeshRenderer>() : null;
                    return new
                    {
                        asset = p,
                        hasSkinnedMesh = smr != null,
                        boneCount = smr?.bones?.Length ?? 0,
                        boneNames = smr?.bones?.Where(b => b).Select(b => b.name).Take(10).ToArray() ?? new string[0],
                    };
                }).ToArray();

                // ---- Probe 3: EULA / license files surfaced -------------------
                var eulaFiles = Directory.GetFiles(Path.Combine(Application.dataPath, ".."), "LICENSE*", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(Application.dataPath, "EULA*", SearchOption.AllDirectories))
                    .Concat(Directory.GetFiles(Application.dataPath, "LICENSE*", SearchOption.AllDirectories))
                    .Where(f => f.IndexOf("404", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                f.IndexOf("gen", StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(Path.GetFullPath).Distinct().ToArray();

                // ---- Probe 4: packages + versions (environment snapshot) -------
                var packages = Directory.GetFiles(Path.Combine(projRoot, "Packages"), "*.json")
                    .SelectMany(f => File.ReadAllLines(f).Where(l => l.Trim().StartsWith("\"com."))
                        .Select(l => l.Trim().TrimEnd(','))).Distinct().ToArray();

                var report = new
                {
                    unityVersion = Application.unityVersion,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    probe1_api = api,
                    probe2_riggedOutput = new { scanned = rigInfo.Length, anyRigged = rigInfo.Any(r => r.hasSkinnedMesh), assets = rigInfo },
                    probe3_eulaFiles = eulaFiles,
                    probe4_packages = packages,
                    verdictHint = new
                    {
                        route = rigInfo.Any(r => r.hasSkinnedMesh) ? "B — 404-GEN output appears rigged; skip the transplant route"
                                                                    : "A — no rigged output found yet; plan the UModeler X transplant pass",
                        batchAutomatable = api.publicStaticEntryPoints.Length > 0
                            ? "likely — public static entry points exist (review their signatures)"
                            : "not found — generation stays a GUI step until an API appears",
                    },
                };
                File.WriteAllText(Path.Combine(outDir, "probe-report.json"),
                    JsonConvert.SerializeObject(report, Formatting.Indented));
                Debug.Log("FORGE PROBES DONE -> forge-artifacts/probe-report.json");
            }
            catch (Exception e) { Debug.LogException(e); exit = 2; }
            finally { EditorApplication.Exit(exit); }
        }
    }
}
