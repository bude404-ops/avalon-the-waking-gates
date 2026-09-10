// AVALON SHELL — in-game 3D review surface (character select + character/skill tab).
// Built entirely at runtime: stage, cold-light rig, uGUI panels, class cards.
// Bootstrapped headless by AvalonShellBuild into the APK build scene.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AvalonShell
{
    public class Shell : MonoBehaviour
    {
        const int CANON_BG = 0x0d0e10;
        static readonly string[] CLASS_ORDER = { "Sovereign", "Ravager", "Warden", "Veilborn", "Weaver", "Wildborn" };

        Animator animator; GameObject model; Transform stagePivot;
        Camera cam; float camDist = 2.8f; float camYaw = 25f;
        GameObject panelSkills; Text statLine;
        string activeClass = "Sovereign";
        readonly Dictionary<string, GameObject> loaded = new Dictionary<string, GameObject>();

        void Start()
        {
            // ---------- stage ----------
            stagePivot = new GameObject("StagePivot").transform;
            cam = gameObject.AddComponent<Camera>();
            cam.backgroundColor = Hex(CANON_BG); cam.clearFlags = CameraClearFlags.SolidColor;
            cam.fieldOfView = 40f; cam.nearClipPlane = 0.01f; cam.farClipPlane = 60f;
            var key = MakeLight(new Color(0.87f, 0.90f, 0.95f), 2.1f, new Vector3(2, 3, 3));
            var fill = MakeLight(new Color(0.68f, 0.73f, 0.80f), 0.8f, new Vector3(-2, 1.5f, -2.5f));
            var rim = MakeLight(new Color(0.45f, 0.53f, 0.66f), 0.9f, new Vector3(-1.5f, 2.2f, -3));
            key.name = "KeyLight"; fill.name = "FillLight"; rim.name = "RimLight";
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.42f, 0.42f, 0.45f);

            // ---------- ui ----------
            var canvas = MakeCanvas();
            MakeTopBar(canvas);
            MakeClassSelect(canvas);
            MakeAnimControls(canvas);
            panelSkills = MakeSkillPanel(canvas);

            LoadClass(activeClass);
        }

        // ================= stage =================
        Light MakeLight(Color c, float i, Vector3 pos)
        { var g = new GameObject(); var l = g.AddComponent<Light>(); l.type = LightType.Directional; l.color = c; l.intensity = i; g.transform.position = pos; return l; }

        void LoadClass(string cls)
        {
            activeClass = cls;
            foreach (var kv in loaded) kv.Value.SetActive(kv.Key == cls);
            if (loaded.ContainsKey(cls)) { BindModel(loaded[cls]); return; }
            var prefab = Resources.Load<GameObject>(cls + "-GAME");
            if (prefab == null)
            {
                statLine.text = cls.ToUpper() + " — STILL IN THE FORGE";
                if (model != null) model.SetActive(false);
                return;
            }
            var inst = Instantiate(prefab, stagePivot);
            inst.transform.localPosition = Vector3.zero;
            loaded[cls] = inst;
            BindModel(inst);
        }

        void BindModel(GameObject go)
        {
            model = go;
            animator = go.GetComponentInChildren<Animator>();
            var rends = go.GetComponentsInChildren<Renderer>();
            Bounds b = rends.Length > 0 ? rends[0].bounds : new Bounds(Vector3.zero, Vector3.one);
            foreach (var r in rends) b.Encapsulate(r.bounds);
            float h = Mathf.Max(b.size.y, 0.1f);
            camDist = h * 1.55f;
            if (animator != null) animator.CrossFade("idle", 0f);
            if (statLine != null) statLine.text = cls_status(activeClass) + " \u2022 " + Mathf.RoundToInt(h * 100) + " CM \u2022 FORGE MODEL";
        }

        string cls_status(string cls) { return Resources.Load<GameObject>(cls + "-GAME") != null ? cls.ToUpper() + " OF THE GATES" : cls.ToUpper() + " — IN THE FORGE"; }

        // ================= per-frame: orbit =================
        Vector2 lastTouch0, lastTouch1; bool dragging;
        void Update()
        {
            if (Input.touchCount == 1)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) { dragging = true; lastTouch0 = t.position; }
                else if (t.phase == TouchPhase.Moved && dragging)
                { camYaw += (t.position.x - lastTouch0.x) * 0.4f; lastTouch0 = t.position; }
                else if (t.phase == TouchPhase.Ended) dragging = false;
            }
            else if (Input.GetMouseButtonDown(0)) { dragging = true; lastTouch0 = Input.mousePosition; }
            else if (Input.GetMouseButton(0) && dragging) { camYaw += ((Vector2)Input.mousePosition - lastTouch0).x * 0.35f; lastTouch0 = Input.mousePosition; }
            else if (Input.GetMouseButtonUp(0)) dragging = false;

            if (Input.touchCount == 2)
            {
                var t0 = Input.GetTouch(0); var t1 = Input.GetTouch(1);
                float d0 = Vector2.Distance(lastTouch0, lastTouch1 == default ? t1.position : lastTouch1);
                float d1 = Vector2.Distance(t0.position, t1.position);
                camDist = Mathf.Clamp(camDist * (d0 / Mathf.Max(d1, 1f)), 0.5f, 12f);
                lastTouch0 = t0.position; lastTouch1 = t1.position;
            }
            camDist = Mathf.Clamp(camDist * (1f - Input.GetAxis("Mouse ScrollWheel")), 0.5f, 12f);

            float pitch = 0.15f; // slight down-tilt
            var target = new Vector3(0, camDist * 0.30f, 0);
            float cy = Mathf.Cos(camYaw * Mathf.Deg2Rad), sy = Mathf.Sin(camYaw * Mathf.Deg2Rad);
            cam.transform.position = target + new Vector3(sy, pitch, cy) * camDist;
            cam.transform.LookAt(target);
        }

        // ================= ui factory =================
        Canvas MakeCanvas()
        {
            var go = new GameObject("Canvas");
            var c = go.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<GraphicRaycaster>();
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            return c;
        }

        Font Font() { return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); }

        GameObject Panel(Transform parent, string name, Color c)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>(); img.color = c;
            return go;
        }

        Text Label(Transform parent, string txt, int size, Color c, TextAnchor anchor)
        {
            var go = new GameObject("lbl");
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>(); t.font = Font(); t.text = txt; t.fontSize = size; t.color = c;
            t.alignment = anchor;
            return t;
        }

        Button Btn(Transform parent, string txt, int size)
        {
            var go = new GameObject("btn-" + txt);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>(); img.color = new Color(0.07f, 0.08f, 0.10f, 0.86f);
            var b = go.AddComponent<Button>();
            var t = Label(go.transform, txt, size, Hex(0xa3895a), TextAnchor.MiddleCenter);
            (t.transform as RectTransform).sizeDelta = Vector2.zero;
            (t.transform as RectTransform).anchorMin = Vector2.zero; (t.transform as RectTransform).anchorMax = Vector2.one;
            return b;
        }

        void MakeTopBar(Canvas canvas)
        {
            var bar = Panel(canvas.transform, "TopBar", new Color(0.039f, 0.043f, 0.051f, 0.94f));
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 92);
            var title = Label(bar.transform, "AVALON — THE WAKING GATES", 20, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            var trt = title.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0.55f); trt.anchorMax = new Vector2(1, 1); trt.offsetMin = new Vector2(24, 0); trt.offsetMax = new Vector2(-24, -8);
            var sub = Label(bar.transform, "THE GATES REVIEW BUILD — UNITY STAGE", 12, Hex(0x8a8578), TextAnchor.MiddleLeft);
            var srt = sub.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0, 0); srt.anchorMax = new Vector2(1, 0.45f); srt.offsetMin = new Vector2(24, 8); srt.offsetMax = new Vector2(-24, 0);
        }

        void MakeClassSelect(Canvas canvas)
        {
            var bar = Panel(canvas.transform, "ClassBar", new Color(0.039f, 0.043f, 0.051f, 0.0f));
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 0); rt.pivot = new Vector2(0.5f, 0); rt.sizeDelta = new Vector2(0, 120);
            // horizontal strip of 6 class cards
            float w = 1f / CLASS_ORDER.Length;
            for (int i = 0; i < CLASS_ORDER.Length; i++)
            {
                string cls = CLASS_ORDER[i];
                bool unlocked = Resources.Load<GameObject>(cls + "-GAME") != null;
                var card = Panel(bar.transform, "card-" + cls, new Color(0.07f, 0.08f, 0.10f, 0.86f));
                var crt = card.GetComponent<RectTransform>();
                crt.anchorMin = new Vector2(w * i + 0.01f, 0.25f); crt.anchorMax = new Vector2(w * (i + 1) - 0.01f, 0.92f);
                var name = Label(card.transform, cls.ToUpper(), 15, unlocked ? Hex(0xe6ddca) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                (name.transform as RectTransform).anchorMin = new Vector2(0, 0.4f); (name.transform as RectTransform).anchorMax = Vector2.one;
                var status = Label(card.transform, unlocked ? "FORGED" : "IN THE FORGE", 10, Hex(0x8a8578), TextAnchor.MiddleCenter);
                (status.transform as RectTransform).anchorMin = Vector2.zero; (status.transform as RectTransform).anchorMax = new Vector2(1, 0.4f);
                var b = card.AddComponent<Button>();
                string captured = cls;
                b.onClick.AddListener(() => { LoadClass(captured); CloseSkills(); });
            }
        }

        void MakeAnimControls(Canvas canvas)
        {
            var holder = new GameObject("AnimControls");
            holder.transform.SetParent(canvas.transform, false);
            var hrt = holder.AddComponent<RectTransform>();
            hrt.anchorMin = new Vector2(0.5f, 0); hrt.anchorMax = new Vector2(0.5f, 0); hrt.pivot = new Vector2(0.5f, 0);
            hrt.anchoredPosition = new Vector2(0, 130);

            var bIdle = Btn(holder.transform, "IDLE", 12);
            (bIdle.transform as RectTransform).sizeDelta = new Vector2(110, 40);
            (bIdle.transform as RectTransform).anchoredPosition = new Vector2(-62, 0);
            bIdle.onClick.AddListener(() => { if (animator) animator.CrossFade("idle", 0.25f); });

            var bWalk = Btn(holder.transform, "WALK", 12);
            (bWalk.transform as RectTransform).sizeDelta = new Vector2(110, 40);
            (bWalk.transform as RectTransform).anchoredPosition = new Vector2(62, 0);
            bWalk.onClick.AddListener(() => { if (animator) animator.CrossFade("walk", 0.25f); });

            var bTab = Btn(holder.transform, "CHARACTER", 11);
            (bTab.transform as RectTransform).sizeDelta = new Vector2(160, 34);
            (bTab.transform as RectTransform).anchoredPosition = new Vector2(0, 52);
            bTab.onClick.AddListener(() => panelSkills.SetActive(!panelSkills.activeSelf));

            statLine = Label(canvas.transform, "STARTING", 11, Hex(0x8a8578), TextAnchor.MiddleRight);
            var srt = statLine.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.5f, 1); srt.anchorMax = new Vector2(1, 1); srt.offsetMin = new Vector2(0, -36); srt.offsetMax = new Vector2(-20, -14);
            srt.transform.SetParent(holder.transform.parent, true);
        }

        GameObject MakeSkillPanel(Canvas canvas)
        {
            var panel = Panel(canvas.transform, "SkillPanel", new Color(0.043f, 0.047f, 0.055f, 0.96f));
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(320, 0);
            var title = Label(panel.transform, "CHARACTER", 16, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            var trt = title.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 1); trt.anchorMax = Vector2.one; trt.offsetMin = new Vector2(18, -56); trt.offsetMax = new Vector2(-18, -20);
            string[] slots = { "SKILL PATH I — SEALED", "SKILL PATH II — SEALED", "SKILL PATH III — SEALED", "MASTER CLASS — SEALED", "DIVINE BOND — SEALED", "RELIC — SEALED" };
            for (int i = 0; i < slots.Length; i++)
            {
                var row = Panel(panel.transform, "slot" + i, new Color(0.07f, 0.08f, 0.10f, 0.9f));
                var rrt = row.GetComponent<RectTransform>();
                rrt.anchorMin = new Vector2(0.06f, 1 - (0.09f * (i + 1) + 0.06f)); rrt.anchorMax = new Vector2(0.94f, 1 - (0.09f * i + 0.07f));
                Label(row.transform, slots[i], 12, Hex(0x8a8578), TextAnchor.MiddleLeft).GetComponent<RectTransform>().offsetMin = new Vector2(14, 0);
            }
            var note = Label(panel.transform, "PROGRESSION SHIP WITH THE GAME BUILD — STAGE SET FOR LEVEL-UP UI", 10, Hex(0x6f6a5e), TextAnchor.LowerLeft);
            var nrt = note.GetComponent<RectTransform>();
            nrt.anchorMin = Vector2.zero; nrt.anchorMax = new Vector2(1, 0.12f); nrt.offsetMin = new Vector2(18, 10); nrt.offsetMax = new Vector2(-18, 0);
            panel.SetActive(false);
            return panel;
        }

        void CloseSkills() { if (panelSkills != null) panelSkills.SetActive(false); }

        static Color Hex(int v)
        {
            return new Color(((v >> 16) & 0xFF) / 255f, ((v >> 8) & 0xFF) / 255f, (v & 0xFF) / 255f);
        }
    }
}
