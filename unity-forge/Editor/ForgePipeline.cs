// AVALON FORGE — deterministic character pipeline for Unity batch mode.
// Entry: Unity.exe -batchmode -quit -projectPath <proj> -executeMethod AvalonForge.Editor.ForgePipeline.Run -logFile forge.log
// Everything here is deterministic Unity API — no AI, no GUI. BIGagent404 authors
// and iterates via forge-artifacts; Big reviews in the APK per doctrine.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if AVA_ANIMRIG
using Unity.Animation.Rigging;
#endif

namespace AvalonForge.Editor
{
    public static class ForgePipeline
    {
        public const string ConfigPath = "forge-config.json";      // project root
        public const string ArtifactDir = "forge-artifacts";     // project root

        // ------------------------------------------------------------------
        public static void Run()
        {
            int exit = 1;
            try
            {
                string projRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string cfgPath = Path.Combine(projRoot, ConfigPath);
                if (!File.Exists(cfgPath)) { Fail("forge-config.json not found at project root."); return; }

                var cfg = JsonConvert.DeserializeObject<ForgeConfig>(File.ReadAllText(cfgPath));
                string cls = Environment.GetCommandLineArg("-class");
                if (string.IsNullOrEmpty(cls)) cls = cfg.@class;
                var spec = cfg.classes.FirstOrDefault(c => string.Equals(c.name, cls, StringComparison.OrdinalIgnoreCase));
                if (spec == null) { Fail($"class '{cls}' not in forge-config.json"); return; }

                var log = new ForgeLog(projRoot, spec.name);
                log.Step("AVALON FORGE run", $"class={spec.name} unity={Application.unityVersion}");

                // ---- 1. IMPORT & SOURCE CHECK --------------------------------
                string srcAsset = ResolveAsset(projRoot, spec.source);
                if (srcAsset == null) { Fail($"source mesh not found: {spec.source}. Run 404-GEN first (see probes).", log); return; }
                log.Step("import", $"asset={srcAsset}");
                AssetDatabase.ImportAsset(srcAsset, ImportAssetOptions.ForceUpdate);
                GameObject srcGo = AssetDatabase.LoadAssetAtPath<GameObject>(srcAsset);
                if (srcGo == null) { Fail("source asset did not load as GameObject.", log); return; }

                // ---- 2. MESH INSPECTION & CLEANUP ----------------------------
                var report = MeshReport.Build(srcGo);
                log.Step("mesh", report.ToString());
                if (report.triCount > spec.maxTris) { Fail($"tri count {report.triCount} over budget {spec.maxTris}", log); return; }
                if (report.degenerateTris > 0 && spec.failOnDegenerate) { Fail($"{report.degenerateTris} degenerate tris", log); return; }

                // ---- 3. SCALE + ORIENTATION NORMALIZATION --------------------
                var go = GameObject.Instantiate(srcGo);
                go.name = spec.name + "_FORGED";
                Normalize(go, spec, log);

                // ---- 4. RIG CHECK / AVATAR SETUP ------------------------------
                var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
                var anim = go.GetComponentInChildren<Animator>();
                if (smr == null || smr.bones == null || smr.bones.Length == 0)
                {
                    if (spec.riggedBaseAsset != null)
                    {
                        log.Step("rig", "source unrigged — transplanting rigged base skeleton");
                        // Route A: bind onto a rigged base skeleton (weights via UModeler X
                        // auto-rig on the seat, or swap the SkinnedMeshRenderer's bones here).
                        log.Warn("Route A transplant requires UModeler X pass — logged for seat work.");
                    }
                    else { Fail("source has no skinned rig and no riggedBaseAsset configured.", log); return; }
                }
                else if (anim != null && anim.avatar == null)
                {
                    var avatar = AvatarBuilder.BuildHumanAvatar(go, BuildHumanDescription(go, smr));
                    anim.avatar = avatar;
                    log.Step("avatar", avatar.isValid ? $"humanoid avatar built, human description mapped ({avatar.name})"
                                                     : "avatar INVALID — bone mapping failed");
                    if (!avatar.isValid) { Fail("AvatarBuilder mapping failed — check bone names in probe report.", log); return; }
                }
                else log.Step("avatar", "avatar already present on source");

                // ---- 5. ANIMATOR + CLIP STATES -------------------------------
                var controller = BuildAnimator(spec, log);
                anim.runtimeAnimatorController = controller;

                // ---- 6. ANIMATION RIGGING (IK / weapon socket) -----------------
#if AVA_ANIMRIG
                if (spec.weaponSocket != null) AttachWeaponRig(go, spec, log);
#else
                log.Warn("Animation Rigging package not installed (define AVA_ANIMRIG once com.unity.animation.rigging is added).");
#endif

                // ---- 7. PREFAB BUILD -------------------------------------------
                string prefabDir = "Assets/AvalonForge/Prefabs";
                Directory.CreateDirectory(prefabDir);
                string prefabPath = $"{prefabDir}/{spec.name}_FORGED.prefab";
                var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                log.Step("prefab", prefab != null ? prefabPath : "SAVE FAILED");
                if (prefab == null) { Fail("prefab save failed", log); return; }

                // ---- 8. QC REPORT ---------------------------------------------
                var qc = new
                {
                    @class = spec.name,
                    unityVersion = Application.unityVersion,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    mesh = report,
                    prefab = prefabPath,
                    heightM = MeasureHeight(go).ToString("F3"),
                    clips = spec.clips.Select(c => $"{c.name}:{ResolveAsset(projRoot, c.file)}").ToArray(),
                };
                log.Step("qc", JsonConvert.SerializeObject(qc, Formatting.Indented));
                exit = 0;
            }
            catch (Exception e)
            {
            try { File.WriteAllText(Path.Combine(Path.GetFullPath(Path.Combine(Application.dataPath, "..")), ArtifactDir, "error.txt"), e.ToString()); }
                catch { }
                Debug.LogException(e);
                exit = 2;
            }
            finally
            {
                EditorApplication.Exit(exit);
            }
        }

        // ====================================================================
        class ForgeConfig { public string @class; public List<ClassSpec> classes = new(); }
        public class ClassSpec
        {
            public string name; public string source; public int maxTris = 60000;
            public bool failOnDegenerate = false;
            public float targetHeightM = 0.9f; public float faceYawDeg = 0f;
            public string riggedBaseAsset;
            public List<ClipSpec> clips = new();
            public WeaponSocketSpec weaponSocket;
        }
        class ClipSpec { public string name; public string file; public bool loop = true; }
        class WeaponSocketSpec { public string handBone = "RightHand"; public float[] offset = { 0, 0, 0 }; public string propAsset; }

        // ---- helpers --------------------------------------------------------
        static string ResolveAsset(string projRoot, string path)
        {
            string inAssets = path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) ? path : "Assets/" + path.TrimStart('/');
            if (File.Exists(Path.Combine(projRoot, inAssets))) return inAssets;
            var found = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(path));
            return found.Length > 0 ? AssetDatabase.GUIDToAssetPath(found[0]) : null;
        }

        static void Normalize(GameObject go, ClassSpec spec, ForgeLog log)
        {
            float h = MeasureHeight(go);
            float k = h > 0.001f ? spec.targetHeightM / h : 1f;
            go.transform.localScale = Vector3.one * k;
            go.transform.rotation = Quaternion.Euler(0, spec.faceYawDeg, 0);
            go.transform.position = Vector3.zero;
            log.Step("normalize", $"height {h:F3}m -> {spec.targetHeightM}m (x{k:F3}), yaw={spec.faceYawDeg}");
            foreach (var r in go.GetComponentsInChildren<Renderer>())
            {
                var mf = r as MeshFilter;
                if (mf != null && mf.sharedMesh != null)
                { mf.sharedMesh.RecalculateBounds(); mf.sharedMesh.RecalculateNormals(); mf.sharedMesh.RecalculateTangents(); }
                var sm = r as SkinnedMeshRenderer; if (sm != null) { sm.updateWhenOffscreen = false; }
            }
        }

        static float MeasureHeight(GameObject go)
        {
            float y0 = float.MaxValue, y1 = float.MinValue; bool any = false;
            foreach (var r in go.GetComponentsInChildren<Renderer>())
            { var b = r.bounds; y0 = Mathf.Min(y0, b.min.y); y1 = Mathf.Max(y1, b.max.y); any = true; }
            return any ? y1 - y0 : 0;
        }

        static HumanDescription BuildHumanDescription(GameObject go, SkinnedMeshRenderer smr)
        {
            var map = new Dictionary<string, HumanBone>
            {
                {"Hips", HumanBone.Hips},{"Spine", HumanBone.Spine},{"Spine1", HumanBone.Chest},
                {"Neck", HumanBone.Neck},{"Head", HumanBone.Head},{"Chest", HumanBone.Chest},
                {"LeftShoulder", HumanBone.LeftShoulder},{"LeftArm", HumanBone.LeftUpperArm},
                {"LeftForeArm", HumanBone.LeftLowerArm},{"LeftHand", HumanBone.LeftHand},
                {"RightShoulder", HumanBone.RightShoulder},{"RightArm", HumanBone.RightUpperArm},
                {"RightForeArm", HumanBone.RightLowerArm},{"RightHand", HumanBone.RightHand},
                {"LeftUpLeg", HumanBone.LeftUpperLeg},{"LeftLeg", HumanBone.LeftLowerLeg},
                {"LeftFoot", HumanBone.LeftFoot},{"LeftToeBase", HumanBone.LeftToes},
                {"RightUpLeg", HumanBone.RightUpperLeg},{"RightLeg", HumanBone.RightLowerLeg},
                {"RightFoot", HumanBone.RightFoot},{"RightToeBase", HumanBone.RightToes},
            };
            var humanBones = new List<HumanBone>();
            foreach (var b in smr.bones)
                if (b != null && map.TryGetValue(b.name, out var hb)) humanBones.Add(hb);
            var desc = new HumanDescription
            {
                human = humanBones.ToArray(),
                skeleton = smr.bones.Where(b => b != null).Select(b => b.name).ToArray(),
                armStretch = 0.05f, feetSpacing = 0.1f, upperArmTwist = 0.5f,
                lowerArmTwist = 0.5f, upperLegTwist = 0.5f, lowerLegTwist = 0.5f,
            };
            return desc;
        }

        static AnimatorController BuildAnimator(ClassSpec spec, ForgeLog log)
        {
            string dir = "Assets/AvalonForge/Controllers";
            Directory.CreateDirectory(dir);
            var ctrl = new AnimatorController { name = spec.name + "_CTRL" };
            AnimatorController.SetAnimatorControllerOffset(ctrl, dir + "/" + ctrl.name + ".controller");
            ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
            var root = ctrl.AddLayer("Base").stateMachine;
            var states = new Dictionary<string, AnimatorState>();
            foreach (var c in spec.clips) states[c.name] = root.AddState(c.name);
            root.defaultState = states.Values.FirstOrDefault();
            log.Step("animator", $"{states.Count} states: {string.Join(",", states.Keys)}");
            return ctrl;
        }

#if AVA_ANIMRIG
        static void AttachWeaponRig(GameObject go, ClassSpec spec, ForgeLog log)
        {
            var rigGo = new GameObject("AvalonRig"); rigGo.transform.SetParent(go.transform, false);
            var rig = rigGo.AddComponent<Rig>();
            var builder = go.AddComponent<RigBuilder>(); builder.rigLayers.Add(new RigLayer(rig));
            var socket = new GameObject("WeaponSocket");
            var hand = FindDeep(go.transform, spec.weaponSocket.handBone);
            if (hand == null) { log.Warn($"hand bone '{spec.weaponSocket.handBone}' not found — socket skipped"); return; }
            socket.transform.SetParent(hand, false);
            socket.transform.localPosition = new Vector3(spec.weaponSocket.offset[0], spec.weaponSocket.offset[1], spec.weaponSocket.offset[2]);
            log.Step("weapon-rig", $"socket parented to {hand.name}, offset {string.Join(",", spec.weaponSocket.offset)}");
        }
        static Transform FindDeep(Transform t, string name)
        { if (t.name == name) return t; foreach (Transform c in t) { var f = FindDeep(c, name); if (f) return f; } return null; }
#endif

        // ---- mesh QC ---------------------------------------------------------
        class MeshReport
        {
            public int meshCount, vertCount, triCount, degenerateTris, noUV;
            public bool hasSkin;
            public override string ToString() =>
                $"meshes={meshCount} verts={vertCount} tris={triCount} degenerate={degenerateTris} noUV={noUV} skinned={hasSkin}";
            public static MeshReport Build(GameObject go)
            {
                var r = new MeshReport();
                foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
                {
                    var m = mf.sharedMesh; if (m == null) continue;
                    r.meshCount++; r.vertCount += m.vertexCount; r.triCount += (int)(m.GetIndexCount(0) / 3);
                    if (!m.HasAttribute(VertexAttribute.TexCoord0)) r.noUV++;
                    for (int s = 0; s < m.subMeshCount; s++)
                    {
                        var idx = m.GetIndices(s);
                        for (int i = 0; i < idx.Length; i += 3)
                            if (idx[i] == idx[i + 1] || idx[i] == idx[i + 2] || idx[i + 1] == idx[i + 2]) r.degenerateTris++;
                    }
                }
                r.hasSkin = go.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                return r;
            }
        }

        // ---- logging ----------------------------------------------------------
        class ForgeLog
        {
            readonly List<string> _lines = new();
            readonly string _dir, _file;
            public ForgeLog(string projRoot, string cls)
            {
                _dir = Path.Combine(projRoot, ArtifactDir);
                Directory.CreateDirectory(_dir);
                _file = Path.Combine(_dir, $"forge-{cls}-{DateTime.Now:yyyyMMdd-HHmmss}.log");
            }
            public void Step(string tag, string msg) { var l = $"[{DateTime.Now:HH:mm:ss}] {tag,-12} {msg}"; _lines.Add(l); Debug.Log("FORGE " + l); }
            public void Warn(string msg) => Step("WARN", msg);
            public void Flush() => File.WriteAllLines(_file, _lines);
        }

        static void Fail(string why, ForgeLog log = null)
        {
            Debug.LogError("FORGE FAIL: " + why);
            log?.Step("FAIL", why);
            log?.Flush();
        }
    }
}
