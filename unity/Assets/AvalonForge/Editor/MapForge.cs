using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// MAP FORGE — headless procedural realm-map builder + QC renders.
// Built for MAP #1: THE COLD RELIQUARY (ASHFALL) per docs/COLD_RELIQUARY_MAP_ARCHITECTURE.md
// Laws honored: Fire-Color Law (mortal fire amber-orange only), Reliquary-Region Aura Law
// (oxide-red ONLY on the reliquary glow), Two-Layer Cold Light, Double-Duty Geometry,
// knotwork law (patterns only — zero script/runes in props), Mythic World Law (god-scale gate).

namespace AvalonForge
{
    public static class MapForge
    {
        const string MAP_NAME = "ASHFALL";

        static string OutDir;
        static StringBuilder Log;

        // ---- palette (cold desaturated + amber fire + oxide-red reliquary only) ----
        static readonly Color ASH_GROUND = new Color(0.24f, 0.21f, 0.19f);
        static readonly Color ROCK_DARK   = new Color(0.16f, 0.14f, 0.13f);
        static readonly Color STONE_GREY  = new Color(0.38f, 0.37f, 0.35f);
        static readonly Color CHARCOAL_WOOD = new Color(0.13f, 0.11f, 0.10f);
        static readonly Color FIRE_AMBER  = new Color(1.0f, 0.55f, 0.16f);       // Fire-Color Law: mortal fire
        static readonly Color RELIQUARY_OXIDE = new Color(0.78f, 0.20f, 0.12f); // Ashfall aura — reliquary only
        static readonly Color COLD_AMBIENT = new Color(0.30f, 0.34f, 0.40f);
        static readonly Color COLD_SUN = new Color(0.72f, 0.76f, 0.84f);
        static readonly Color FOG_COLD = new Color(0.20f, 0.22f, 0.26f);

        public static void Run()
        {
            OutDir = Path.Combine(Application.dataPath, "AvalonForge", "MapShots");
            Directory.CreateDirectory(OutDir);
            string repDir = Path.Combine(Application.dataPath, "AvalonForge", "Reports");
            Directory.CreateDirectory(repDir);
            string scnDir = Path.Combine(Application.dataPath, "AvalonForge", "Scenes");
            Directory.CreateDirectory(scnDir);
            Log = new StringBuilder();
            L("MAP FORGE start — " + MAP_NAME + " (The Cold Reliquary)");

            try
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                BuildWorld();
                string scnPath = "Assets/AvalonForge/Scenes/MAP-" + MAP_NAME + ".unity";
                EditorSceneManager.SaveScene(scene, scnPath);
                L("scene saved: " + scnPath);

                RenderShot("MAP-" + MAP_NAME + "-topdown", 1024, 1024, new Vector3(0, 340, 0),
                    Quaternion.Euler(90, 0, 0), 0f, true);
                RenderShot("MAP-" + MAP_NAME + "-hero-road", 1280, 720, new Vector3(0, 4.5f, 132),
                    Quaternion.Euler(6, 0, 0), 62f, false);
                RenderShot("MAP-" + MAP_NAME + "-hero-emberhollow", 1280, 720, new Vector3(14, 3f, 96),
                    Quaternion.Euler(5, -28, 0), 55f, false);

                File.WriteAllText(Path.Combine(repDir, "MAP-" + MAP_NAME + ".json"),
                    "{\"map\":\"" + MAP_NAME + "\",\"status\":\"COMPLETE\",\"shots\":3}");
                L("MAP FORGE COMPLETE — 3 shots");
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                L("FATAL: " + e);
                File.WriteAllText(Path.Combine(repDir, "MAP-" + MAP_NAME + "-FAILED.json"),
                    "{\"map\":\"" + MAP_NAME + "\",\"error\":" + JsonSafe(e.ToString()) + "}");
                EditorApplication.Exit(1);
            }
        }

        // ================= WORLD =================
        static void BuildWorld()
        {
            // light law: two-layer cold light
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = COLD_AMBIENT;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = FOG_COLD;
            RenderSettings.fogDensity = 0.0035f;

            var sun = new GameObject("ColdSun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = COLD_SUN;
            sun.intensity = 0.75f;
            sun.transform.rotation = Quaternion.Euler(48, -35, 0);
            sun.shadows = LightShadows.None; // batch-safe

            BuildTerrain();
            BuildEmberhollow();     // beat 1: SAFE — town square + brazier landmark
            BuildRoad();            // beat 2: THREAT — charcoal forest + log bridge
            BuildRuinedShrine();    // beat 3: DISCOVERY — megalith ring + reliquary + brazier rite
            BuildWatchtowerRidge(); // beat 4: THREAT — drake strafe lane
            BuildGateHorizon();     // god-scale silhouette (Double-Duty Geometry)
            L("world built");
        }

        static void BuildTerrain()
        {
            var td = new TerrainData();
            td.heightmapResolution = 129;
            td.size = new Vector3(600, 60, 600);
            float[,] h = new float[129, 129];
            var seedPos = new Vector2(4.04f, 40.4f);
            for (int y = 0; y < 129; y++)
                for (int x = 0; x < 129; x++)
                {
                    float wx = x / 128f, wy = y / 128f;
                    float base_ = Mathf.PerlinNoise(wx * 3f + seedPos.x, wy * 3f + seedPos.y) * 0.10f;
                    float north = Mathf.SmoothStep(0.55f, 1.0f, wy) * 0.22f;          // ridge rising north
                    float ravine = 0.10f * Mathf.Exp(-Mathf.Pow((wy - 0.48f) * 14f, 2f)); // ravine dip (log bridge)
                    float bowl = -0.05f * Mathf.Exp(-Mathf.Pow((wx - 0.5f) * 3f, 2f) - Mathf.Pow((wy - 0.24f) * 3f, 2f));
                    h[y, x] = Mathf.Clamp01(base_ + north - ravine + bowl);
                }
            td.SetHeights(0, 0, h);
            var l1 = new TerrainLayer { diffuseTexture = FlatTex(ASH_GROUND) };
            var l2 = new TerrainLayer { diffuseTexture = FlatTex(ROCK_DARK) };
            td.terrainLayers = new[] { l1, l2 };
            var t = Terrain.CreateTerrainGameObject(td);
            t.name = "Terrain-" + MAP_NAME;
            L("terrain built 600x600");
        }

        static Texture2D FlatTex(Color c)
        {
            var t = new Texture2D(32, 32);
            for (int y = 0; y < 32; y++) for (int x = 0; x < 32; x++)
                t.SetPixel(x, y, c * UnityEngine.Random.Range(0.92f, 1.05f));
            t.Apply();
            return t;
        }

        // ---------- beats ----------
        static void BuildEmberhollow()
        {
            var root = new GameObject("EMBERHOLLOW");
            for (int i = 0; i < 6; i++) // town ring: 6 longhouses around the square
            {
                float a = (i / 6f) * Mathf.PI * 1.3f + Mathf.PI * 0.35f;
                Vector3 p = new Vector3(Mathf.Cos(a) * 26, 0, 80 + Mathf.Sin(a) * 26);
                var hs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hs.transform.SetParent(root.transform);
                hs.transform.position = p;
                hs.transform.rotation = Quaternion.Euler(0, -a * Mathf.Rad2Deg + 90, 0);
                hs.transform.localScale = new Vector3(8, 4, 5);
                Paint(hs, STONE_GREY * 0.85f);
                var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roof.transform.SetParent(root.transform);
                roof.transform.position = p + new Vector3(0, 2.7f, 0);
                roof.transform.rotation = hs.transform.rotation;
                roof.transform.localScale = new Vector3(9, 0.7f, 6);
                Paint(roof, CHARCOAL_WOOD);
            }
            Brazier(new Vector3(0, 0, 80), 3.4f, root.transform); // THE brazier landmark
            for (int i = 0; i < 3; i++)
                StandingStone(new Vector3(-14 + i * 14, 0, 56 - i * 3), root.transform, 1.6f + i * 0.2f);
            L("beat 1 EMBERHOLLOW: town square + brazier landmark");
        }

        static void BuildRoad()
        {
            var root = new GameObject("THE-ROAD");
            var road = GameObject.CreatePrimitive(PrimitiveType.Cube); // ash road strip
            road.transform.SetParent(root.transform);
            road.transform.position = new Vector3(0, 0.2f, 20);
            road.transform.localScale = new Vector3(7, 0.1f, 210);
            Paint(road, new Color(0.44f, 0.41f, 0.37f));
            var log = GameObject.CreatePrimitive(PrimitiveType.Cylinder); // fallen-log bridge
            log.transform.SetParent(root.transform);
            log.transform.position = new Vector3(0, 1.4f, -12);
            log.transform.rotation = Quaternion.Euler(90, 0, 0);
            log.transform.localScale = new Vector3(1.6f, 14, 1.6f);
            Paint(log, CHARCOAL_WOOD);
            int trees = 0;
            for (int i = 0; i < 60; i++) // charcoal forest flanking the road
            {
                float z = 40 - i * 6.5f;
                float side = (i % 2 == 0) ? 1 : -1;
                float off = 12 + Mathf.PerlinNoise(i * 0.3f, 7.7f) * 26;
                var tr = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tr.transform.SetParent(root.transform);
                tr.transform.position = new Vector3(side * off, 1.8f, z);
                tr.transform.localScale = new Vector3(0.5f, 3.6f, 0.5f);
                tr.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-7, 7), 0, UnityEngine.Random.Range(-7, 7));
                Paint(tr, CHARCOAL_WOOD);
                var cn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cn.transform.SetParent(root.transform);
                cn.transform.position = new Vector3(side * off, 4.6f, z);
                cn.transform.localScale = Vector3.one * UnityEngine.Random.Range(2.2f, 3.4f);
                Paint(cn, new Color(0.10f, 0.09f, 0.08f));
                trees++;
            }
            Brazier(new Vector3(-5, 0, 30), 1.8f, root.transform);
            Brazier(new Vector3(5, 0, -2), 1.8f, root.transform);
            L("beat 2 THE ROAD: charcoal forest + log bridge + " + trees + " trunks");
        }

        static void BuildRuinedShrine()
        {
            var root = new GameObject("RUINED-SHRINE");
            for (int i = 0; i < 7; i++) // megalith ring
            {
                float a = (i / 7f) * Mathf.PI * 2f;
                StandingStone(new Vector3(Mathf.Cos(a) * 13, 0, -60 + Mathf.Sin(a) * 13), root.transform, 4.2f + (i % 3) * 0.5f);
            }
            var p1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder); // broken arch
            p1.transform.SetParent(root.transform); p1.transform.position = new Vector3(-6, 3, -74);
            p1.transform.localScale = new Vector3(1.8f, 6, 1.8f); Paint(p1, ROCK_DARK * 1.4f);
            var p2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            p2.transform.SetParent(root.transform); p2.transform.position = new Vector3(6, 3, -74);
            p2.transform.localScale = new Vector3(1.8f, 6, 1.8f); Paint(p2, ROCK_DARK * 1.4f);
            var lintel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lintel.transform.SetParent(root.transform); lintel.transform.position = new Vector3(5, 0.7f, -71);
            lintel.transform.rotation = Quaternion.Euler(0, 30, 12); lintel.transform.localScale = new Vector3(11, 1.4f, 2);
            Paint(lintel, ROCK_DARK * 1.4f);
            Brazier(new Vector3(0, 0, -60), 2.6f, root.transform); // the rite flame
            var rel = GameObject.CreatePrimitive(PrimitiveType.Cube); // THE RELIQUARY — oxide-red, only red in realm
            rel.transform.SetParent(root.transform);
            rel.transform.position = new Vector3(0, 1.1f, -68);
            rel.transform.localScale = new Vector3(0.8f, 1.6f, 0.8f);
            var m = Paint(rel, STONE_GREY * 0.7f);
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", RELIQUARY_OXIDE * 1.6f);
            var rl = new GameObject("ReliquaryGlow").AddComponent<Light>();
            rl.transform.SetParent(root.transform);
            rl.transform.position = new Vector3(0, 2, -68);
            rl.type = LightType.Point; rl.color = RELIQUARY_OXIDE; rl.range = 16; rl.intensity = 1.4f;
            L("beat 3 RUINED SHRINE: megalith ring + broken arch + rite brazier + RELIQUARY (oxide-red)");
        }

        static void BuildWatchtowerRidge()
        {
            var root = new GameObject("WATCHTOWER-RIDGE");
            var tw = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tw.transform.SetParent(root.transform);
            tw.transform.position = new Vector3(28, 14, -190);
            tw.transform.localScale = new Vector3(6, 14, 6);
            Paint(tw, STONE_GREY * 0.9f);
            var crown = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crown.transform.SetParent(root.transform);
            crown.transform.position = new Vector3(28, 28.5f, -190);
            crown.transform.localScale = new Vector3(7.6f, 1.2f, 7.6f);
            Paint(crown, ROCK_DARK * 1.5f);
            Brazier(new Vector3(28, 29, -190), 1.2f, root.transform); // beacon on the crown
            L("beat 4 WATCHTOWER APPROACH: tower + beacon on north ridge");
        }

        static void BuildGateHorizon()
        {
            var root = new GameObject("GATE-HORIZON"); // god-scale broken archway — THE silhouette
            var gp1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gp1.transform.SetParent(root.transform); gp1.transform.position = new Vector3(-22, 26, -270);
            gp1.transform.localScale = new Vector3(9, 52, 9); Paint(gp1, ROCK_DARK * 1.15f);
            var gp2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gp2.transform.SetParent(root.transform); gp2.transform.position = new Vector3(22, 24, -272);
            gp2.transform.localScale = new Vector3(9, 48, 9); Paint(gp2, ROCK_DARK * 1.15f);
            var lintel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lintel.transform.SetParent(root.transform); lintel.transform.position = new Vector3(2, 52, -271);
            lintel.transform.rotation = Quaternion.Euler(4, 6, 3);
            lintel.transform.localScale = new Vector3(52, 6, 8); Paint(lintel, ROCK_DARK * 1.2f);
            L("GATE silhouette built (52u) on north horizon");
        }

        // ---------- kit ----------
        static void Brazier(Vector3 p, float scale, Transform parent)
        {
            var root = new GameObject("Brazier");
            root.transform.SetParent(parent); root.transform.position = p;
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.SetParent(root.transform); pillar.transform.localPosition = new Vector3(0, 0.9f * scale, 0);
            pillar.transform.localScale = new Vector3(0.9f * scale, 1.8f * scale, 0.9f * scale);
            Paint(pillar, STONE_GREY * 0.8f);
            var basin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basin.transform.SetParent(root.transform); basin.transform.localPosition = new Vector3(0, 2.0f * scale, 0);
            basin.transform.localScale = new Vector3(1.6f * scale, 0.5f * scale, 1.6f * scale);
            Paint(basin, ROCK_DARK * 1.6f);
            var flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.transform.SetParent(root.transform); flame.transform.localPosition = new Vector3(0, 2.5f * scale, 0);
            flame.transform.localScale = Vector3.one * (1.1f * scale);
            var fm = Paint(flame, FIRE_AMBER);
            fm.EnableKeyword("_EMISSION");
            fm.SetColor("_EmissionColor", FIRE_AMBER * 2.2f);
            var l = new GameObject("FireLight").AddComponent<Light>();
            l.transform.SetParent(root.transform);
            l.transform.localPosition = new Vector3(0, 2.6f * scale, 0);
            l.type = LightType.Point; l.color = FIRE_AMBER; l.range = 22 * scale; l.intensity = 1.5f;
        }

        static void StandingStone(Vector3 p, Transform parent, float h)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s.transform.SetParent(parent);
            s.transform.position = p + new Vector3(0, h / 2, 0);
            s.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(0, 90), UnityEngine.Random.Range(-4, 4));
            s.transform.localScale = new Vector3(0.9f, h, 0.6f);
            Paint(s, STONE_GREY * UnityEngine.Random.Range(0.75f, 0.95f));
        }

        static Material Paint(GameObject go, Color c)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return null;
            var mat = new UnityEngine.Material(Shader.Find("Standard"));
            mat.color = c;
            r.sharedMaterial = mat;
            return mat;
        }

        // ---------- renders ----------
        static void RenderShot(string name, int w, int h, Vector3 pos, Quaternion rot, float fov, bool ortho)
        {
            var camGo = new GameObject(name);
            var cam = camGo.AddComponent<Camera>();
            cam.transform.position = pos;
            cam.transform.rotation = rot;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = FOG_COLD;
            cam.farClipPlane = 900;
            if (ortho) { cam.orthographic = true; cam.orthographicSize = 300; cam.farClipPlane = 700; }
            else cam.fieldOfView = fov;

            var rt = new RenderTexture(w, h, 24);
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            File.WriteAllBytes(Path.Combine(OutDir, name + ".png"), tex.EncodeToPNG());
            RenderTexture.active = null;
            cam.targetTexture = null;
            UnityEngine.Object.DestroyImmediate(camGo);
            L("render captured: " + name + ".png");
        }

        static void L(string s)
        {
            Log.AppendLine(s);
            Debug.Log("[MapForge] " + s);
        }

        static string JsonSafe(string s) { return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", " ") + "\""; }
    }
}
