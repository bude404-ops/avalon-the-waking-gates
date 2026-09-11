using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// ASHFALL MAP FORGE — Map #1: The Cold Reliquary (beats 1-4), built headless per
// docs/COLD_RELIQUARY_MAP_ARCHITECTURE.md + docs/AVALON-MAPS-PRODUCTION-PLAN.md.
// Laws enforced: Fire-Color Law (braziers/beacons amber-orange only), Reliquary-Region
// Aura Law (Ashfall oxide-red = the ONLY rich region hue, on the reliquary glow),
// Mythic World Law (colossal landmarks), rune law (no glyphs on props).
namespace AvalonForge
{
    public static class AshfallMapForge
    {
        static string Root => $"{Application.dataPath}/AvalonForge";
        static string MapShots => $"{Root}/MapShots";
        static string Reports => $"{Root}/Reports";
        static string _log = "";
        static void Log(string s) { _log += s + "\n"; Debug.Log("[AshfallMapForge] " + s); }

        public static void Run()
        {
            int code = 0;
            try
            {
                Directory.CreateDirectory(MapShots);
                Directory.CreateDirectory(Reports);
                Log("MAP FORGE START — ASHFALL / COLD RELIQUARY (beats 1-4)");

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                BuildWorld();
                Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "AvalonForge", "Scenes"));
                bool saved = EditorSceneManager.SaveScene(scene, "Assets/AvalonForge/Scenes/MAP-ASHFALL.unity");
                Log("Scene saved: " + saved + " -> Assets/AvalonForge/Scenes/MAP-ASHFALL.unity");

                RenderShots();
                Log("MAP FORGE COMPLETE — shots in " + MapShots);
            }
            catch (Exception ex)
            {
                code = 1;
                Log("FATAL: " + ex);
                Debug.LogError("[AshfallMapForge] FATAL: " + ex);
            }
            finally
            {
                File.WriteAllText($"{Reports}/ASHFALL-MAP-REPORT.txt", _log);
                Debug.Log("[AshfallMapForge] report written");
                if (code != 0) EditorApplication.Exit(code);
            }
        }

        // ---------- palette (cold desaturated + law-driven accents) ----------
        static readonly Color COL_AMBIENT = new Color(0.24f, 0.27f, 0.32f);   // cold slate ambient
        static readonly Color COL_KEY     = new Color(0.85f, 0.90f, 1.00f);   // cold key light
        static readonly Color COL_FOG     = new Color(0.13f, 0.145f, 0.17f);
        static readonly Color COL_STONE   = new Color(0.16f, 0.155f, 0.145f);  // weathered basalt
        static readonly Color COL_STONE2  = new Color(0.22f, 0.21f, 0.19f);    // lighter carved stone
        static readonly Color COL_BRONZE  = new Color(0.42f, 0.33f, 0.20f);    // bronze trim
        static readonly Color COL_WOOD    = new Color(0.075f, 0.065f, 0.055f); // charcoal wood
        static readonly Color COL_AMBER   = new Color(1.00f, 0.55f, 0.15f);    // FIRE-COLOR LAW (braziers)
        static readonly Color COL_OXIDE   = new Color(0.75f, 0.18f, 0.10f);    // ASHFALL REGION HUE (reliquary glow only)

        static Material Mat(Color c, string name, float smooth = 0.1f, float metal = 0f)
        {
            var m = new Material(Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit"));
            m.name = name;
            if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
            return m;
        }

        static Material MatEmissive(Color c, Color e, string name, float intensity = 2.2f)
        {
            var m = Mat(c, name);
            m.EnableKeyword("_EMISSION");
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", e * intensity);
            return m;
        }

        static GameObject Prim(PrimitiveType t, Vector3 pos, Vector3 scale, Material m, string name, Vector3? rotEuler = null)
        {
            var go = GameObject.CreatePrimitive(t);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = scale;
            if (rotEuler.HasValue) go.transform.rotation = Quaternion.Euler(rotEuler.Value);
            go.GetComponent<MeshRenderer>().sharedMaterial = m;
            return go;
        }

        // valley path centerline x(z) — Emberhollow plaza straight, then gentle winding
        static float PathX(float z) => (z <= 40f) ? 0f : Mathf.Sin((z - 40f) * 0.035f) * 14f;

        static void BuildWorld()
        {
            // ---- SKY / FOG / AMBIENT (cold desaturated, two-layer law) ----
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = COL_AMBIENT;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = COL_FOG;
            RenderSettings.fogStartDistance = 90f;
            RenderSettings.fogEndDistance = 420f;

            var sunGo = new GameObject("Sun-Cold");
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional; sun.intensity = 0.95f; sun.color = COL_KEY;
            sunGo.transform.rotation = Quaternion.Euler(42f, -38f, 0f);

            // ---- TERRAIN ----
            const int HM = 129;
            const float SIZE = 512f;
            float[,] heights = new float[HM, HM];
            for (int z = 0; z < HM; z++)
            for (int x = 0; x < HM; x++)
            {
                float wx = (x / (float)(HM - 1)) * SIZE - SIZE / 2f;
                float wz = (z / (float)(HM - 1)) * SIZE - SIZE / 2f;
                float px = PathX(Mathf.Clamp(wz, -256f, 256f));
                float d = Mathf.Abs(wx - px);
                float ridge = Mathf.Clamp01((d - 55f) / 130f);
                float fbm = Mathf.PerlinNoise(x * 0.045f, z * 0.045f) * 0.55f + Mathf.PerlinNoise(x * 0.12f, z * 0.12f) * 0.30f + Mathf.PerlinNoise(x * 0.31f, z * 0.31f) * 0.15f;
                float h = ridge * (14f + 26f * fbm) + fbm * 1.6f;
                if (wz > 240f) h += Mathf.Clamp01((wz - 240f) / 60f) * 9f;
                heights[z, x] = h;
            }
            var td = new TerrainData();
            td.heightmapResolution = HM;
            td.size = new Vector3(SIZE, 80f, SIZE);
            td.SetHeights(0, 0, heights);

            var basaltTex = NoiseTex(new Color(0.155f, 0.15f, 0.14f), new Color(0.20f, 0.19f, 0.175f));
            var roadTex = NoiseTex(new Color(0.10f, 0.085f, 0.07f), new Color(0.14f, 0.12f, 0.10f));
            Directory.CreateDirectory("Assets/AvalonForge/Generated/TerrainLayers");
            var l0 = SaveLayer(basaltTex, "Ashfall-Basalt");
            var l1 = SaveLayer(roadTex, "Ashfall-CharcoalRoad");
            td.terrainLayers = new[] { l0, l1 };
            const int AM = 129;
            float[,,] alpha = new float[AM, AM, 2];
            for (int z = 0; z < AM; z++)
            for (int x = 0; x < AM; x++)
            {
                float wx = (x / (float)(AM - 1)) * SIZE - SIZE / 2f;
                float wz = (z / (float)(AM - 1)) * SIZE - SIZE / 2f;
                float px = PathX(Mathf.Clamp(wz, -256f, 256f));
                float d = Mathf.Abs(wx - px);
                float roadPaint = (wz > -20f && wz < 310f) ? Mathf.Clamp01(1f - (d - 5f) / 5f) : 0f;
                alpha[z, x, 1] = roadPaint;
                alpha[z, x, 0] = 1f - roadPaint;
            }
            td.alphamapResolution = AM;
            td.SetAlphamaps(0, 0, alpha);

            var tGo = Terrain.CreateTerrainGameObject(td);
            tGo.name = "ASHFALL-TERRAIN";
            tGo.transform.position = new Vector3(-SIZE / 2f, 0f, -SIZE / 2f);

            // ---- LANDMARK MATERIALS ----
            var mStone = Mat(COL_STONE, "Stone-Basalt");
            var mStone2 = Mat(COL_STONE2, "Stone-Carved", 0.25f);
            var mBronze = Mat(COL_BRONZE, "Bronze-Trim", 0.35f, 0.55f);
            var mWood = Mat(COL_WOOD, "Wood-Charcoal");
            var mAmber = MatEmissive(new Color(1f, 0.62f, 0.2f), COL_AMBER, "Flame-Amber", 2.6f);
            var mOxide = MatEmissive(new Color(0.55f, 0.12f, 0.08f), COL_OXIDE, "Reliquary-OxideGlow", 2.2f);

            // ================= BEAT 1 — EMBERHOLLOW (town square, SAFE) =================
            var ember = new GameObject("B1-EMBERHOLLOW");
            for (int i = 0; i < 6; i++)
            {
                float a = i / 6f * Mathf.PI * 2f + 0.3f;
                Vector3 p = new Vector3(Mathf.Cos(a) * 13f, 2.1f, Mathf.Sin(a) * 13f);
                var s = Prim(PrimitiveType.Cube, p, new Vector3(1.6f, 4.2f, 1.2f), mStone2, "StandingStone-" + i, new Vector3(0, a * 57.3f + 8f * (i % 2), 0));
                s.transform.SetParent(ember.transform, true);
                Prim(PrimitiveType.Cube, p + new Vector3(0f, 4.35f, 0f), new Vector3(1.9f, 0.35f, 1.5f), mBronze, "Cap-" + i).transform.SetParent(ember.transform, true);
            }
            var braz = new GameObject("Landmark-Brazier"); braz.transform.SetParent(ember.transform);
            Prim(PrimitiveType.Cylinder, new Vector3(0, 0.9f, 0), new Vector3(3.4f, 0.9f, 3.4f), mStone, "Brazier-Base").transform.SetParent(braz.transform);
            Prim(PrimitiveType.Cylinder, new Vector3(0, 1.9f, 0), new Vector3(4.6f, 0.35f, 4.6f), mStone2, "Brazier-Bowl").transform.SetParent(braz.transform);
            Prim(PrimitiveType.Sphere, new Vector3(0, 2.6f, 0), new Vector3(2.6f, 3.2f, 2.6f), mAmber, "Brazier-Flame").transform.SetParent(braz.transform);
            MakeLight(new Vector3(0, 3.4f, 0), COL_AMBER, 3.2f, 42f).transform.SetParent(braz.transform);
            for (int i = 0; i < 3; i++)
            {
                float a = 1.1f + i * 1.9f;
                Vector3 hp = new Vector3(Mathf.Cos(a) * 26f, 1.4f, Mathf.Sin(a) * 26f + 6f);
                var hut = new GameObject("Hut-" + i); hut.transform.SetParent(ember.transform);
                Prim(PrimitiveType.Cube, hp, new Vector3(7f, 4.2f, 8.5f), mStone, "Walls").transform.SetParent(hut.transform);
                Prim(PrimitiveType.Cube, hp + new Vector3(0, 3.0f, 0), new Vector3(8.2f, 2.4f, 9.7f), mStone2, "Roof", new Vector3(0, 12f * (i - 1), 0)).transform.SetParent(hut.transform);
                Prim(PrimitiveType.Cube, hp + new Vector3(0, 1.0f, 4.4f), new Vector3(1.6f, 2.4f, 0.4f), mBronze, "Doorframe").transform.SetParent(hut.transform);
            }

            // ================= BEAT 2 — THE ROAD (charcoal forest, THREAT) =================
            var roadGo = new GameObject("B2-THE-ROAD");
            Prim(PrimitiveType.Cylinder, new Vector3(PathX(120f), 1.15f, 120f), new Vector3(0.8f, 11f, 0.8f), mWood, "FallenLogBridge", new Vector3(0, 0, 90f)).transform.SetParent(roadGo.transform);
            var rng = new System.Random(404);
            for (int i = 0; i < 70; i++)
            {
                float z = 45f + rng.Next(0, 155);
                float side = (rng.Next(2) == 0) ? -1f : 1f;
                float off = 12f + rng.Next(0, 60);
                float x = PathX(z) + side * off;
                var tree = new GameObject("DeadTree-" + i); tree.transform.SetParent(road.transform);
                float hT = 5.5f + rng.Next(0, 30) / 10f;
                Prim(PrimitiveType.Cylinder, new Vector3(x, hT / 2f, z), new Vector3(0.35f, hT / 2f, 0.35f), mWood, "Trunk", new Vector3(rng.Next(-4, 5), rng.Next(0, 180), rng.Next(-4, 5))).transform.SetParent(tree.transform);
                Prim(PrimitiveType.Cylinder, new Vector3(x + side * 0.7f, hT * 0.82f, z), new Vector3(0.18f, 1.7f, 0.18f), mWood, "BranchA", new Vector3(0, 0, 55f * side)).transform.SetParent(tree.transform);
                Prim(PrimitiveType.Cylinder, new Vector3(x - side * 0.5f, hT * 0.7f, z + 0.6f), new Vector3(0.15f, 1.3f, 0.15f), mWood, "BranchB", new Vector3(40f * side, 30f, 0)).transform.SetParent(tree.transform);
            }
            for (int i = 0; i < 24; i++)
            {
                float z = 30f + rng.Next(0, 280);
                float side = (rng.Next(2) == 0) ? -1f : 1f;
                Prim(PrimitiveType.Cube, new Vector3(PathX(z) + side * (8f + rng.Next(0, 70)), 0.4f + rng.Next(0, 10) / 10f, z),
                    new Vector3(1.2f + rng.Next(0, 20) / 10f, 0.9f + rng.Next(0, 12) / 10f, 1.4f + rng.Next(0, 20) / 10f), mStone, "Rock-" + i,
                    new Vector3(rng.Next(0, 20), rng.Next(0, 90), rng.Next(0, 12))).transform.SetParent(road.transform);
            }

            // ================= BEAT 3 — RUINED SHRINE (DISCOVERY) =================
            var shrine = new GameObject("B3-RUINED-SHRINE");
            float sx = PathX(220f);
            Prim(PrimitiveType.Cube, new Vector3(sx - 4f, 3.5f, 220f), new Vector3(1.8f, 7f, 1.8f), mStone2, "ArchPillar-L").transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Cube, new Vector3(sx + 4f, 2.4f, 220f), new Vector3(1.8f, 4.8f, 1.8f), mStone2, "ArchPillar-R-Broken").transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Cube, new Vector3(sx - 0.6f, 6.6f, 220f), new Vector3(9.5f, 1.1f, 2.0f), mStone2, "ArchLintel-Fallen", new Vector3(0, 0, -9f)).transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Cube, new Vector3(sx, 1.0f, 224f), new Vector3(2.8f, 1.0f, 1.6f), mStone2, "Altar").transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Cube, new Vector3(sx + 5.5f, 1.7f, 224f), new Vector3(0.9f, 3.4f, 0.4f), mStone, "LoreTablet", new Vector3(-8f, 25f, 0)).transform.SetParent(shrine.transform);
            var rel = new GameObject("Reliquary"); rel.transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Cylinder, new Vector3(sx, 0.7f, 216f), new Vector3(1.2f, 0.7f, 1.2f), mStone, "Reliquary-Pedestal").transform.SetParent(rel.transform);
            Prim(PrimitiveType.Sphere, new Vector3(sx, 1.9f, 216f), new Vector3(0.85f, 1.15f, 0.85f), mOxide, "Reliquary-Vessel").transform.SetParent(rel.transform);
            MakeLight(new Vector3(sx, 1.9f, 216f), COL_OXIDE, 2.6f, 24f).transform.SetParent(rel.transform);
            Prim(PrimitiveType.Cylinder, new Vector3(sx + 2.2f, 0.8f, 226f), new Vector3(0.9f, 0.8f, 0.9f), mStone, "ShrineBrazier-Base").transform.SetParent(shrine.transform);
            Prim(PrimitiveType.Sphere, new Vector3(sx + 2.2f, 1.75f, 226f), new Vector3(0.8f, 1.1f, 0.8f), mAmber, "ShrineBrazier-Flame").transform.SetParent(shrine.transform);
            MakeLight(new Vector3(sx + 2.2f, 2.0f, 226f), COL_AMBER, 2.0f, 18f).transform.SetParent(shrine.transform);

            // ================= BEAT 4 — WATCHTOWER APPROACH (THREAT) =================
            var wt = new GameObject("B4-WATCHTOWER");
            float wx2 = PathX(300f);
            Prim(PrimitiveType.Cylinder, new Vector3(wx2, 11f, 300f), new Vector3(4.2f, 11f, 4.2f), mStone, "Tower-Shaft").transform.SetParent(wt.transform);
            Prim(PrimitiveType.Cylinder, new Vector3(wx2, 22.4f, 300f), new Vector3(5.6f, 0.7f, 5.6f), mStone2, "Tower-Battlement").transform.SetParent(wt.transform);
            for (int i = 0; i < 8; i++)
            {
                float a = i / 8f * Mathf.PI * 2f;
                Prim(PrimitiveType.Cube, new Vector3(wx2 + Mathf.Cos(a) * 5.0f, 23.6f, 300f + Mathf.Sin(a) * 5.0f), new Vector3(0.9f, 1.6f, 0.9f), mStone2, "Cren-" + i, new Vector3(0, a * 57.3f, 0)).transform.SetParent(wt.transform);
            }
            Prim(PrimitiveType.Sphere, new Vector3(wx2, 24.2f, 300f), new Vector3(1.1f, 1.4f, 1.1f), mAmber, "Beacon-Flame").transform.SetParent(wt.transform);
            MakeLight(new Vector3(wx2, 24.5f, 300f), COL_AMBER, 2.4f, 60f).transform.SetParent(wt.transform);
            Prim(PrimitiveType.Cube, new Vector3(wx2 - 7f, 5f, 276f), new Vector3(2.2f, 10f, 2.2f), mStone2, "ApproachPost-L").transform.SetParent(wt.transform);
            Prim(PrimitiveType.Cube, new Vector3(wx2 + 7f, 5f, 276f), new Vector3(2.2f, 10f, 2.2f), mStone2, "ApproachPost-R").transform.SetParent(wt.transform);

            Log("World built: Emberhollow (b1) -> Road/CharcoalForest (b2) -> RuinedShrine+Reliquary (b3) -> Watchtower (b4)");
        }

        static Light MakeLight(Vector3 pos, Color c, float intensity, float range)
        {
            var go = new GameObject("Light-Point");
            go.transform.position = pos;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point; l.color = c; l.intensity = intensity; l.range = range;
            l.renderMode = LightRenderMode.ForcePixel;
            return l;
        }

        static Texture2D NoiseTex(Color a, Color b)
        {
            const int N = 64;
            var t = new Texture2D(N, N, TextureFormat.RGBA32, false);
            t.wrapMode = TextureWrapMode.Repeat;
            var px = new Color[N * N];
            var rng = new System.Random(77);
            for (int y = 0; y < N; y++)
            for (int x = 0; x < N; x++)
            {
                float n = Mathf.PerlinNoise(x * 0.18f, y * 0.18f) * 0.7f + (rng.Next(100) / 1000f);
                px[y * N + x] = Color.Lerp(a, b, Mathf.Clamp01(n));
            }
            t.SetPixels(px); t.Apply();
            return t;
        }

        static TerrainLayer SaveLayer(Texture2D tex, string name)
        {
            var l = new TerrainLayer { diffuseTexture = tex };
            AssetDatabase.CreateAsset(l, $"Assets/AvalonForge/Generated/TerrainLayers/{name}.terrainlayer");
            return l;
        }

        static void RenderShots()
        {
            Shot(new Vector3(58f, 92f, -68f), new Vector3(PathX(150f), 4f, 150f), 46f, "ASHFALL-AERIAL.png", 1920, 1080);
            Shot(new Vector3(PathX(26f) + 17f, 3.4f, 34f), new Vector3(0f, 2.6f, 0f), 58f, "ASHFALL-EMBERHOLLOW.png", 1920, 1080);
            float sx = PathX(220f);
            Shot(new Vector3(sx + 10f, 3.2f, 212f), new Vector3(sx, 2.2f, 218f), 55f, "ASHFALL-RELIQUARY.png", 1920, 1080);
        }

        static void Shot(Vector3 pos, Vector3 look, float fov, string file, int w, int h)
        {
            var camGo = new GameObject("Shot-" + file);
            var cam = camGo.AddComponent<Camera>();
            cam.transform.position = pos;
            cam.transform.LookAt(look);
            cam.fieldOfView = fov;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.078f, 0.086f, 0.10f);
            cam.farClipPlane = 700f;
            var rt = new RenderTexture(w, h, 24);
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0); tex.Apply();
            File.WriteAllBytes($"{MapShots}/{file}", tex.EncodeToPNG());
            Log("Shot captured: " + file);
            cam.targetTexture = null; RenderTexture.active = null;
            UnityEngine.Object.DestroyImmediate(camGo);
            rt.Release();
        }
    }
}
