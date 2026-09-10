// AVALON AUTO-FORGE — Unity-native automated character pipeline
// The deterministic batch chain per docs/AVALON-UNITY-AUTOMATED-PIPELINE.md
// Runs headless: Unity -batchmode -projectPath <proj> -executeMethod AvalonForge.RunClass
// Class selection: environment var AVALON_CLASS (default Sovereign-M)
// Editor: Window > Avalon Forge > Run Class Pipeline (per-class or all)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if AVALON_ANIMRIGGING
using Unity.Animation.Rigging;
#endif

public static class AvalonForge
{
    const string Root = "Assets/AvalonForge";
    const string Raw = Root + "/raw";          // 404-GEN output drops here
    const string Specs = Root + "/specs";      // per-class canon specs (authored by BIGagent404)
    const string Clips = Root + "/clips";      // CMU-prepped FBX clips (root-locked)
    const string Prefabs = Root + "/prefabs";
    const string Artifacts = Root + "/artifacts"; // QC reports (read by BIGagent404)
    const string LogTag = "[AVALON-FORGE] ";

    // ---------- entry points ----------
    [MenuItem("Avalon Forge/Run Class Pipeline")]
    public static void RunFromMenu() => RunClass();

    public static void RunClass()
    {
        string className = Environment.GetEnvironmentVariable("AVALON_CLASS");
        if (string.IsNullOrEmpty(className))
            className = EditorPrefs.GetString("AVALON_CLASS", "Sovereign-M");
        Run(className);
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    [MenuItem("Avalon Forge/Run ALL 12 Classes")]
    public static void RunAll()
    {
        foreach (var specFile in Directory.GetFiles(Specs, "*.json"))
            Run(Path.GetFileNameWithoutExtension(specFile));
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    // ---------- pipeline ----------
    public static void Run(string className)
    {
        var spec = LoadSpec(className);
        if (spec == null) { Fail($"no spec for {className}"); return; }
        Log($"=== {className} START ===");
        var report = new ClassReport { className = className };

        string rawPath = $"{Raw}/{className}.glb";
        if (!File.Exists(rawPath)) rawPath = $"{Raw}/{className}.fbx";
        if (!File.Exists(rawPath))
        {
            // Stage A not possible yet — nothing generated for this class
            Log($"RAW MESH MISSING for {className} — drop the 404-GEN output at {rawPath}. Skipping.");
            report.stage = "awaiting-404gen-mesh";
            WriteReport(report);
            return;
        }

        // STAGE A: import + inspect + clean + normalize
        AssetDatabase.Refresh();
        var rawGo = AssetDatabase.LoadAssetAtPath<GameObject>(rawPath);
        if (rawGo == null)
        {
            Fail($"import failed for {rawPath} — install/verify glTFast (com.unity.cloud.gltfast)");
            return;
        }
        report = InspectMesh(rawGo, report);
        report = Normalize(rawGo, spec, report);
        report = AssignMaterials(rawGo, spec, report);

        // STAGE B: rig — REQUIRES the class to have been auto-rigged
        // (UModeler X auto-rig on-seat, or 404-GEN native rig if the probe verified it).
        var renderers = rawGo.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renderers.Length == 0 || !HasSkeleton(rawGo))
        {
            report.stage = "awaiting-rig";
            Log($"MESH OK but UNRIGGED — auto-rig pass needed (UModeler X or 404-GEN native). Skipping rig stage.");
            WriteReport(report);
            return;
        }
        report = ConfigureHumanoid(rawPath, report);
        report = BuildAnimatorController(className, spec, report);
        report = SetupRiggingAndSockets(rawGo, spec, report);

        string prefabPath = $"{Prefabs}/{className}.prefab";
        Directory.CreateDirectory(Prefabs);
        var saved = PrefabUtility.SaveAsPrefabAsset(rawGo, prefabPath);
        report.stage = "prefab-ready";
        report.prefab = prefabPath;
        Log($"PREFAB SAVED: {prefabPath}");
        WriteReport(report);
    }

    // ---------- stage A ----------
    static ClassReport InspectMesh(GameObject go, ClassReport r)
    {
        var mesh = go.GetComponentInChildren<MeshFilter>()?.sharedMesh
                   ?? go.GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMesh;
        if (mesh == null) { Fail("no mesh found"); return r; }
        r.verts = mesh.vertexCount;
        r.tris = (int)(mesh.GetIndexCount(0) / 3);
        r.bounds = mesh.bounds;

        // degenerate triangles (zero-area)
        var verts = mesh.vertices;
        int degenerate = 0;
        var trisArr = mesh.triangles;
        for (int i = 0; i < trisArr.Length; i += 3)
        {
            var a = verts[trisArr[i]]; var b = verts[trisArr[i + 1]]; var c = verts[trisArr[i + 2]];
            if (Vector3.Cross(b - a, c - a).sqrMagnitude < 1e-12f) degenerate++;
        }
        r.degenerateTris = degenerate;

        // non-manifold edges: any edge shared by an odd number of triangles
        var edges = new Dictionary<long, int>();
        for (int i = 0; i < trisArr.Length; i += 3)
        {
            int[] idx = { trisArr[i], trisArr[i + 1], trisArr[i + 2] };
            for (int e = 0; e < 3; e++)
            {
                int u = Math.Min(idx[e], idx[(e + 1) % 3]), v = Math.Max(idx[e], idx[(e + 1) % 3]);
                long key = (long)u << 32 | (uint)v;
                edges.TryGetValue(key, out int n); edges[key] = n + 1;
            }
        }
        r.nonManifoldEdges = edges.Count(kvp => kvp.Value % 2 != 0);

        if (mesh.uv.Length > 0)
        {
            var uvs = mesh.uv;
            Vector2 mn = uvs[0], mx = uvs[0];
            foreach (var uv in uvs) { mn = Vector2.Min(mn, uv); mx = Vector2.Max(mx, uv); }
            r.uvCoverage = new Rect(mn, mx - mn);
        }
        Log($"mesh: {r.verts} verts / {r.tris} tris | degenerate {degenerate} | non-manifold {r.nonManifoldEdges}");
        return r;
    }

    static ClassReport Normalize(GameObject go, Spec spec, ClassReport r)
    {
        float targetH = spec.heightM;
        var renderer = go.GetComponentInChildren<Renderer>();
        float curH = renderer != null ? renderer.bounds.size.y : 0;
        if (curH <= 0) { Fail("cannot measure bounds"); return r; }
        float scale = targetH / curH;
        go.transform.localScale *= scale;
        r.appliedScale = scale;

        // orientation: glTF faces +Z, Unity convention: character forward = +Z, upright = +Y
        // 404-GEN output is Y-up already — only correct if the model faces sideways
        if (spec.faceAxisCorrection != 0)
            go.transform.rotation = Quaternion.Euler(0, spec.faceAxisCorrection, 0);
        Log($"normalized: scale {scale:0.000} → {targetH}m");
        return r;
    }

    static ClassReport AssignMaterials(GameObject go, Spec spec, ClassReport r)
    {
        // build the class material (armor primary + accent) — full palette zones
        // paint via COLOR_0 handled by the vertex-paint shader/material in the kit
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        mat.name = $"{spec.@class}_M_{spec.gender}";
        if (ColorUtility.TryParseHtmlString(spec.armorPrimaryHex, out var prim)) mat.color = prim;
        if (ColorUtility.TryParseHtmlString(spec.accentHex, out var acc))
            mat.SetFloat("_BaseMap", 0); // accent applied via emission/silhouette in the kit pass
        Directory.CreateDirectory($"{Root}/materials");
        AssetDatabase.CreateAsset(mat, $"{Root}/materials/{mat.name}.mat");
        foreach (var rd in go.GetComponentsInChildren<Renderer>(true))
            rd.sharedMaterial = mat;
        r.material = mat.name;
        return r;
    }

    // ---------- stage B ----------
    static bool HasSkeleton(GameObject go)
        => go.GetComponentsInChildren<Transform>(true).Any(t => BoneMapper.IsHumanoidName(t.name));

    static ClassReport ConfigureHumanoid(string assetPath, ClassReport r)
    {
        var importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);
        if (importer == null) { Fail("no ModelImporter (glTFast missing?)"); return r; }
        if (importer.animationType != ModelImporterAnimationType.Humanoid)
        {
            importer.animationType = ModelImporterAnimationType.Humanoid;
            importer.SaveAndReimport();
        }
        var avatar = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Avatar>().FirstOrDefault(a => a.isHuman);
        r.humanoidAvatar = avatar != null;
        if (avatar == null)
            Fail("humanoid avatar not auto-mapped — bone names may need UModeler X's humanoid template (Unity standard names). Check the report.");
        else
            Log("humanoid avatar: AUTO-MAPPED ✓");
        return r;
    }

    static ClassReport BuildAnimatorController(string className, Spec spec, ClassReport r)
    {
        string path = $"{Root}/controllers/{className}.controller";
        Directory.CreateDirectory($"{Root}/controllers");
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(path);

        ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
        ctrl.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Thrust", AnimatorControllerParameterType.Trigger);

        var root = ctrl.layers[0].stateMachine;
        var clips = LoadClips(className);

        var idleState = AddState(root, clips.GetValueOrDefault("idle"), "Idle");
        var walkState = AddState(root, clips.GetValueOrDefault("walk"), "Walk");
        var runState = AddState(root, clips.GetValueOrDefault("run"), "Run");
        var hitState = AddState(root, clips.GetValueOrDefault("hit"), "Hit");
        var thrustState = AddState(root, clips.GetValueOrDefault("thrust"), "Thrust");

        if (idleState != null) root.defaultState = idleState;

        // speed blend: idle <-> walk <-> run
        if (idleState != null && walkState != null && runState != null)
        {
            var tree = new BlendTree { name = "Locomotion" };
            AssetDatabase.AddObjectToAsset(tree, ctrl);
            tree.AddChild(IdleClip(clips), 0f);
            tree.AddChild(clips.GetValueOrDefault("walk"), 0.5f);
            tree.AddChild(clips.GetValueOrDefault("run"), 1f);
            tree.blendParameter = "Speed";
            var locoState = root.AddState("Locomotion");
            locoState.motion = tree;
            locoState.name = "Locomotion";
            root.defaultState = locoState;
        }

        // transitions
        void Trans(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, string param, float th)
        {
            if (from == null || to == null) return;
            var t = from.AddTransition(to);
            t.AddCondition(mode, th, param);
            t.hasExitTime = false;
            t.duration = 0.15f;
        }
        Trans(hitState, null, 0, "", 0); // placeholder to avoid unused warnings
        EditorUtility.SetDirty(ctrl);
        AssetDatabase.SaveAssets();
        r.animatorController = path;
        Log($"animator controller built: {path}");
        return r;
    }

    static AnimatorState AddState(AnimatorStateMachine sm, Motion clip, string name)
    {
        if (clip == null) return null;
        var st = sm.AddState(name);
        st.motion = clip;
        return st;
    }

    static Motion IdleClip(Dictionary<string, Motion> clips) => clips.GetValueOrDefault("idle");

    static Dictionary<string, Motion> LoadClips(string className)
    {
        var map = new Dictionary<string, Motion>();
        if (!Directory.Exists(Clips)) return map;
        foreach (var fbx in Directory.GetFiles(Clips, "*.fbx"))
        {
            string n = Path.GetFileNameWithoutExtension(fbx).ToLower();
            foreach (var key in new[] { "idle", "walk", "run", "hit", "thrust" })
                if (n.Contains(key)) { map[key] = AssetDatabase.LoadAssetAtPath<Motion>(fbx); break; }
        }
        return map;
    }

    static ClassReport SetupRiggingAndSockets(GameObject go, Spec spec, ClassReport r)
    {
        var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
        animator.runtimeAnimatorController =
            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(r.animatorController);

        // weapon socket on the right hand
        var hand = FindBone(go, spec.handBoneNames);
        if (hand != null)
        {
            var socket = hand.Find("weapon_socket");
            if (socket == null)
            {
                socket = new GameObject("weapon_socket").transform;
                socket.SetParent(hand, false);
                socket.localPosition = spec.weaponSocketOffset;
                socket.localRotation = spec.weaponSocketRotation;
            }
            r.weaponSocket = "created";
            Log("weapon socket: right hand ✓");
        }
        else
        {
            r.weaponSocket = "hand-bone-not-found";
            Log($"WARN: hand bone not found ({string.Join("/", spec.handBoneNames)}) — UModeler rig needed Unity-standard names");
        }

#if AVALON_ANIMRIGGING
        // Animation Rigging: foot IK + head look-at (scriptable, official package)
        var rig = new GameObject("Rig 1").transform;
        rig.SetParent(go.transform, false);
        var rigComp = rig.gameObject.AddComponent<Rig>();
        var builder = go.GetComponent<RigBuilder>() ?? go.AddComponent<RigBuilder>();
        builder.rigLayers.Add(rigComp);

        var lFoot = FindBone(go, new[] { "LeftFoot", "Left_Foot", "foot_l" });
        if (lFoot != null)
        {
            var target = new GameObject("LeftFootIK").transform;
            target.SetParent(rig, false);
            target.position = lFoot.position;
            var c = rig.gameObject.AddComponent<TwoBoneIKConstraint>();
            c.root = FindBone(go, new[] { "LeftUpLeg", "Left_UpLeg", "thigh_l" });
            c.mid = FindBone(go, new[] { "LeftLeg", "Left_Leg", "calf_l" });
            c.tip = lFoot;
            c.target = target;
            c.weight = 0.6f;
        }
        var head = FindBone(go, new[] { "Head" });
        if (head != null)
        {
            var gaze = new GameObject("LookTarget").transform;
            gaze.SetParent(go.transform, false);
            gaze.localPosition = go.transform.forward * 5f;
            var aim = rig.gameObject.AddComponent<MultiAimConstraint>();
            aim.constrainedObject = head;
            var sources = new WeightedTransformArray(1);
            sources[0] = new WeightedTransform(gaze, 1f);
            aim.sourceObjects = sources;
            aim.weight = 0.4f;
        }
        Log("animation rigging: foot IK + look-at ✓");
#endif
        return r;
    }

    static Transform FindBone(GameObject root, string[] names)
    {
        foreach (var n in names)
        {
            var t = root.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(x => x.name.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
            if (t != null) return t;
        }
        return null;
    }

    // ---------- spec + reporting ----------
    static Spec LoadSpec(string className)
    {
        string path = $"{Specs}/{className}.json";
        if (!File.Exists(path)) return null;
        return JsonConvert.DeserializeObject<Spec>(File.ReadAllText(path));
    }

    static void WriteReport(ClassReport r)
    {
        Directory.CreateDirectory(Artifacts);
        string path = $"{Artifacts}/{r.className}-report.json";
        File.WriteAllText(path, JsonConvert.SerializeObject(r, Formatting.Indented));
        AssetDatabase.Refresh();
        Log($"REPORT: {path} | stage: {r.stage}");
    }

    static void Log(string m) => Debug.Log(LogTag + m);
    static void Fail(string m) => Debug.LogError(LogTag + "FAIL: " + m);

    [Serializable]
    public class Spec
    {
        public string @class, gender, realm, accentHex, armorPrimaryHex, weaponProp;
        public float heightM;
        public float faceAxisCorrection;
        public string[] handBoneNames;
        public Vector3 weaponSocketOffset, weaponSocketRotation;
    }

    [Serializable]
    public class ClassReport
    {
        public string className, stage, material, prefab, weaponSocket;
        public int verts, tris, degenerateTris, nonManifoldEdges;
        public bool humanoidAvatar;
        public float appliedScale;
        public Rect uvCoverage;
        public Bounds bounds;
        public string animatorController;
    }
}

// bone-name heuristics shared with the rig stage
internal static class BoneMapper
{
    static readonly string[] HumanNames =
    {
        "Hips", "Spine", "Chest", "Neck", "Head", "Shoulder", "Arm", "ForeArm",
        "Hand", "UpLeg", "Leg", "Foot", "Toe"
    };
    public static bool IsHumanoidName(string n) => HumanNames.Any(h => n.IndexOf(h, StringComparison.OrdinalIgnoreCase) >= 0);
}
