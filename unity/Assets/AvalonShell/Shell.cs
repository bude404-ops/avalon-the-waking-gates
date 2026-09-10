// AVALON SHELL v2 — game-structure review build.
// TITLE -> CHARACTER SELECT -> IN-GAME (HUD + skill tab over the stage).
// All UI built at runtime; bootstrapped headless by AvalonShellBuild.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AvalonShell
{
    public class Shell : MonoBehaviour
    {
        const int CANON_BG = 0x0d0e10;
        enum State { Title, Select, Game }

        class ClassDef { public string name, realm, role, accent; public string blurb;
            public ClassDef(string n, string r, string ro, string a, string b) { name = n; realm = r; role = ro; accent = a; blurb = b; } }
        static readonly ClassDef[] CLASSES = {
            new ClassDef("Sovereign", "SKYREND", "THE LANCE OF SKYREND", "#7288a8", "Commanding calm. Storm-slate banners and the gate-rune spear. Storm crown of the first realm."),
            new ClassDef("Ravager", "ASHFALL", "THE FURY OF ASHFALL", "#b3502e", "Fierce. Oxide-red mantle and a greatsword sworn to the caldera. Where the Ravager walks, ash follows."),
            new ClassDef("Warden", "STONEHEART", "THE WALL OF STONEHEART", "#9a9789", "Unyielding. Bone-grey cloak and a warhammer older than the first door. The line that does not break."),
            new ClassDef("Veilborn", "DUSKMOURN", "THE SHADOW OF DUSKMOURN", "#8c2f3a", "Cold, watchful. Crimson-wrapped blades that move before they are seen. The dusk keeps its own."),
            new ClassDef("Weaver", "MARENTH", "THE TIDE OF MARENTH", "#3f8f8a", "Distant, serene. Tide-teal threads pulled from the world's edge. What the tide writes, the Weaver reads."),
            new ClassDef("Wildborn", "EVERBLOOM", "THE BEAST OF EVERBLOOM", "#4d6b3c", "Feral, alert. Moss-bound claws of the deep green. The wild does not kneel — it answers."),
        };

        State state = State.Title;
        ClassDef chosen = CLASSES[0];
        GameObject panelTitle, panelSelect, panelGame, panelSkills;
        Camera cam; float camDist = 2.8f; float camYaw = 25f;
        Animator animator; GameObject model; Transform stagePivot;
        Text hudLine;
        readonly Dictionary<string, GameObject> loaded = new Dictionary<string, GameObject>();
        readonly List<Image> cardTints = new List<Image>();

        void Start()
        {
            stagePivot = new GameObject("StagePivot").transform;
            cam = gameObject.AddComponent<Camera>();
            cam.backgroundColor = Hex(CANON_BG); cam.clearFlags = CameraClearFlags.SolidColor;
            cam.fieldOfView = 40f; cam.nearClipPlane = 0.01f; cam.farClipPlane = 60f;
            MakeLight(new Color(0.87f, 0.90f, 0.95f), 2.1f, new Vector3(2, 3, 3)).name = "KeyLight";
            MakeLight(new Color(0.68f, 0.73f, 0.80f), 0.8f, new Vector2(-2, -2.5f)).name = "FillLight";
            MakeLight(new Color(0.45f, 0.53f, 0.66f), 0.9f, new Vector3(-1.5f, 2.2f, -3)).name = "RimLight";
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.42f, 0.42f, 0.45f);

            var canvas = MakeCanvas();
            panelTitle = BuildTitle(canvas.transform);
            panelSelect = BuildSelect(canvas.transform);
            panelGame = BuildGame(canvas.transform);
            panelSkills = BuildSkills(canvas.transform);
            SetState(State.Title);
        }

        void SetState(State s)
        {
            state = s;
            panelTitle.SetActive(s == State.Title);
            panelSelect.SetActive(s == State.Select);
            panelGame.SetActive(s == State.Game);
            if (s != State.Game && panelSkills.activeSelf) panelSkills.SetActive(false);
            if (s == State.Title && model != null) foreach (var kv in loaded) kv.Value.SetActive(false);
            if (s == State.Select) SelectCard(chosen);
        }

        // ================= TITLE =================
        GameObject BuildTitle(Transform parent)
        {
            var p = Panel(parent, "Title", new Color(0.051f, 0.055f, 0.063f, 0.97f));
            Stretch(p.transform);
            Label(p.transform, "AVALON", 64, Hex(0xe6ddca), TextAnchor.MiddleCenter).rect().Stretch(p.transform);
            var sub = Label(p.transform, "THE WAKING GATES", 18, Hex(0xa3895a), TextAnchor.MiddleCenter);
            sub.rect().anchorMin = new Vector2(0, 0.40f); sub.rect().anchorMax = new Vector2(1, 0.52f);
            var tag = Label(p.transform, "REVIEW BUILD — UNITY STAGE", 11, Hex(0x6f6a5e), TextAnchor.MiddleCenter);
            tag.rect().anchorMin = new Vector2(0, 0.06f); tag.rect().anchorMax = new Vector2(1, 0.12f);
            var enter = Btn(p.transform, "ENTER THE GATES", 16);
            var ert = enter.transform as RectTransform;
            ert.anchorMin = new Vector2(0.5f, 0.22f); ert.anchorMax = new Vector2(0.5f, 0.22f);
            ert.sizeDelta = new Vector2(280, 56);
            enter.onClick.AddListener(() => SetState(State.Select));
            return p;
        }

        // ================= SELECT =================
        GameObject BuildSelect(Transform parent)
        {
            var p = Panel(parent, "Select", new Color(0, 0, 0, 0));
            Stretch(p.transform);
            var dim = Panel(p.transform, "Dim", new Color(0, 0, 0, 0.35f));
            Stretch(dim.transform);
            var head = Label(p.transform, "CHOOSE YOUR CLASS", 26, Hex(0xe6ddca), TextAnchor.MiddleCenter);
            head.rect().anchorMin = new Vector2(0, 0.90f); head.rect().anchorMax = new Vector2(1, 0.99f);

            float w = 1f / CLASSES.Length;
            for (int i = 0; i < CLASSES.Length; i++)
            {
                var cd = CLASSES[i];
                bool unlocked = Resources.Load<GameObject>(cd.name + "-GAME") != null;
                var card = Panel(p.transform, "card-" + cd.name, new Color(0.07f, 0.08f, 0.10f, 0.92f));
                var crt = card.rect();
                crt.anchorMin = new Vector2(w * i + 0.008f, 0.26f); crt.anchorMax = new Vector2(w * (i + 1) - 0.008f, 0.80f);
                var nm = Label(card.transform, cd.name.ToUpper(), 16, unlocked ? Hex(0xe6ddca) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                nm.rect().anchorMin = new Vector2(0, 0.78f); nm.rect().anchorMax = Vector2.one;
                var ro = Label(card.transform, cd.role, 10, Hex(0xa3895a), TextAnchor.MiddleCenter);
                ro.rect().anchorMin = new Vector2(0, 0.66f); ro.rect().anchorMax = new Vector2(1, 0.78f);
                var bl = Label(card.transform, cd.blurb, 10, Hex(0x8a8578), TextAnchor.UpperCenter);
                bl.rect().anchorMin = new Vector2(0.08f, 0.10f); bl.rect().anchorMax = new Vector2(0.92f, 0.66f);
                var st = Label(card.transform, unlocked ? "FORGED" : "IN THE FORGE", 10, Hex(0x8a8578), TextAnchor.MiddleCenter);
                st.rect().anchorMin = Vector2.zero; st.rect().anchorMax = new Vector2(1, 0.10f);
                var b = card.AddComponent<Button>(); cardTints.Add(card.GetComponent<Image>());
                var captured = cd;
                b.onClick.AddListener(() => SelectCard(captured));
            }

            var enter = Btn(p.transform, "ENTER THE GATES", 14);
            var ert = enter.transform as RectTransform;
            ert.anchorMin = new Vector2(0.5f, 0.08f); ert.anchorMax = new Vector2(0.5f, 0.08f);
            ert.sizeDelta = new Vector2(240, 48);
            enter.onClick.AddListener(() => SetState(State.Game));
            var back = Btn(p.transform, "BACK", 12);
            var brt = back.transform as RectTransform;
            brt.anchorMin = new Vector2(0.01f, 0.95f); brt.anchorMax = new Vector2(0.16f, 0.99f); brt.offsetMax = Vector2.zero; brt.offsetMin = Vector2.zero;
            back.onClick.AddListener(() => SetState(State.Title));
            return p;
        }

        void SelectCard(ClassDef cd)
        {
            chosen = cd;
            for (int i = 0; i < CLASSES.Length; i++)
                cardTints[i].color = (CLASSES[i] == cd) ? new Color(0.16f, 0.14f, 0.09f, 0.96f) : new Color(0.07f, 0.08f, 0.10f, 0.92f);
            LoadClass(cd.name);
        }

        // ================= GAME =================
        GameObject BuildGame(Transform parent)
        {
            var p = Panel(parent, "Game", new Color(0, 0, 0, 0));
            Stretch(p.transform);

            // HUD top-left: class crest panel
            var crest = Panel(p.transform, "Crest", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            var crt = crest.rect();
            crt.anchorMin = new Vector2(0.01f, 0.94f); crt.anchorMax = new Vector2(0.38f, 0.99f);
            var cls = Label(crest.transform, "SOVEREIGN", 15, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            cls.rect().anchorMin = new Vector2(0, 0.5f); cls.rect().anchorMax = Vector2.one; cls.rect().offsetMin = new Vector2(14, 0); cls.rect().offsetMax = new Vector2(-10, -4);
            var lvl = Label(crest.transform, "LEVEL 1 — SKYREND", 10, Hex(0x8a8578), TextAnchor.MiddleLeft);
            lvl.rect().anchorMin = Vector2.zero; lvl.rect().anchorMax = new Vector2(1, 0.5f); lvl.rect().offsetMin = new Vector2(14, 4); lvl.rect().offsetMax = new Vector2(-10, 0);

            // HUD: health + stamina bars under the crest
            var hpBack = Panel(p.transform, "HPBack", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            hpBack.rect().anchorMin = new Vector2(0.01f, 0.885f); hpBack.rect().anchorMax = new Vector2(0.38f, 0.935f);
            var hpFill = Panel(hpBack.transform, "HPFill", new Color(0.42f, 0.47f, 0.55f, 0.95f));
            hpFill.rect().anchorMin = new Vector2(0.02f, 0.25f); hpFill.rect().anchorMax = new Vector2(0.98f, 0.75f);
            var hpLbl = Label(hpBack.transform, "HEALTH", 9, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            hpLbl.rect().anchorMin = new Vector2(0.02f, 0.25f); hpLbl.rect().anchorMax = new Vector2(0.98f, 0.75f); hpLbl.rect().offsetMin = new Vector2(8, 0);
            var spBack = Panel(p.transform, "SPBack", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            spBack.rect().anchorMin = new Vector2(0.01f, 0.835f); spBack.rect().anchorMax = new Vector2(0.38f, 0.885f);
            var spFill = Panel(spBack.transform, "SPFill", new Color(0.64f, 0.54f, 0.35f, 0.95f));
            spFill.rect().anchorMin = new Vector2(0.02f, 0.25f); spFill.rect().anchorMax = new Vector2(0.98f, 0.75f);
            var spLbl = Label(spBack.transform, "STAMINA", 9, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            spLbl.rect().anchorMin = new Vector2(0.02f, 0.25f); spLbl.rect().anchorMax = new Vector2(0.98f, 0.75f); spLbl.rect().offsetMin = new Vector2(8, 0);

            // HUD: quest tracker under the lantern
            var quest = Panel(p.transform, "Quest", new Color(0.039f, 0.043f, 0.051f, 0.82f));
            quest.rect().anchorMin = new Vector2(0.62f, 0.885f); quest.rect().anchorMax = new Vector2(0.99f, 0.935f);
            var qLbl = Label(quest.transform, "QUEST — THE GATES AWAKEN (I)", 10, Hex(0xa3895a), TextAnchor.MiddleRight);
            qLbl.rect().Stretch(quest.transform); qLbl.rect().offsetMin = new Vector2(10, 0); qLbl.rect().offsetMax = new Vector2(-12, 0);

            // HUD top-right: lantern (unlit)
            var lantern = Panel(p.transform, "Lantern", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            var lrt = lantern.rect();
            lrt.anchorMin = new Vector2(0.62f, 0.94f); lrt.anchorMax = new Vector2(0.99f, 0.99f);
            Label(lantern.transform, "THE LANTERN — UNLIT", 11, Hex(0x8a8578), TextAnchor.MiddleRight).rect().Stretch(lantern.transform);

            // ability bar bottom-center: 4 sealed slots
            for (int i = 0; i < 4; i++)
            {
                var slot = Panel(p.transform, "ab" + i, new Color(0.07f, 0.08f, 0.10f, 0.92f));
                var srt = slot.rect();
                srt.anchorMin = new Vector2(0.5f + (i - 1.5f) * 0.09f, 0.02f);
                srt.anchorMax = new Vector2(0.5f + (i - 0.5f) * 0.09f, 0.10f);
                Label(slot.transform, "SEALED", 9, Hex(0x6f6a5e), TextAnchor.MiddleCenter).rect().Stretch(slot.transform);
            }

            // debug controls + character tab
            var bIdle = Btn(p.transform, "IDLE", 12);
            (bIdle.transform as RectTransform).sizeDelta = new Vector2(100, 38);
            (bIdle.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.13f); (bIdle.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.13f);
            (bIdle.transform as RectTransform).anchoredPosition = new Vector2(-56, 0);
            bIdle.onClick.AddListener(() => { if (animator) animator.CrossFade("idle", 0.25f); });

            var bWalk = Btn(p.transform, "WALK", 12);
            (bWalk.transform as RectTransform).sizeDelta = new Vector2(100, 38);
            (bWalk.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.13f); (bWalk.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.13f);
            (bWalk.transform as RectTransform).anchoredPosition = new Vector2(56, 0);
            bWalk.onClick.AddListener(() => { if (animator) animator.CrossFade("walk", 0.25f); });

            var bTab = Btn(p.transform, "CHARACTER", 11);
            (bTab.transform as RectTransform).anchorMin = new Vector2(0.995f, 0.02f); (bTab.transform as RectTransform).anchorMax = new Vector2(0.995f, 0.02f);
            (bTab.transform as RectTransform).anchoredPosition = new Vector2(-90, 30);
            (bTab.transform as RectTransform).sizeDelta = new Vector2(160, 36);
            bTab.onClick.AddListener(() => panelSkills.SetActive(!panelSkills.activeSelf));

            var bMenu = Btn(p.transform, "MENU", 10);
            (bMenu.transform as RectTransform).anchorMin = new Vector2(0.005f, 0.02f); (bMenu.transform as RectTransform).anchorMax = new Vector2(0.005f, 0.02f);
            (bMenu.transform as RectTransform).anchoredPosition = new Vector2(52, 30);
            (bMenu.transform as RectTransform).sizeDelta = new Vector2(96, 32);
            bMenu.onClick.AddListener(() => SetState(State.Select));

            hudLine = Label(p.transform, "", 10, Hex(0x8a8578), TextAnchor.MiddleRight);
            hudLine.rect().anchorMin = new Vector2(0.4f, 0.905f); hudLine.rect().anchorMax = new Vector2(0.99f, 0.94f);
            return p;
        }

        GameObject BuildSkills(Transform parent)
        {
            var panel = Panel(parent, "SkillPanel", new Color(0.043f, 0.047f, 0.055f, 0.97f));
            var rt = panel.rect();
            rt.anchorMin = new Vector2(1, 0); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(340, 0);
            var title = Label(panel.transform, "CHARACTER — SKILL PATHS", 15, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            title.rect().anchorMin = new Vector2(0, 1); title.rect().anchorMax = Vector2.one; title.rect().offsetMin = new Vector2(18, -52); title.rect().offsetMax = new Vector2(-18, -20);
            string[] slots = { "SKILL PATH I — SEALED", "SKILL PATH II — SEALED", "SKILL PATH III — SEALED", "MASTER CLASS — SEALED", "DIVINE BOND — SEALED", "RELIC — SEALED" };
            for (int i = 0; i < slots.Length; i++)
            {
                var row = Panel(panel.transform, "slot" + i, new Color(0.07f, 0.08f, 0.10f, 0.9f));
                var rrt = row.rect();
                rrt.anchorMin = new Vector2(0.06f, 1 - (0.085f * (i + 1) + 0.05f)); rrt.anchorMax = new Vector2(0.94f, 1 - (0.085f * i + 0.06f));
                Label(row.transform, slots[i], 12, Hex(0x8a8578), TextAnchor.MiddleLeft).rect().offsetMin = new Vector2(14, 0);
            }
            var note = Label(panel.transform, "PROGRESSION SHIPS WITH THE GAME — STAGE SET FOR LEVEL-UP UI", 10, Hex(0x6f6a5e), TextAnchor.LowerLeft);
            note.rect().anchorMin = Vector2.zero; note.rect().anchorMax = new Vector2(1, 0.10f); note.rect().offsetMin = new Vector2(18, 10); note.rect().offsetMax = new Vector2(-18, 0);
            var close = Btn(panel.transform, "CLOSE", 10);
            var crt = close.rect();
            crt.anchorMin = new Vector2(1, 1); crt.anchorMax = new Vector2(1, 1); crt.pivot = new Vector2(1, 1);
            crt.anchoredPosition = new Vector2(-10, -10); crt.sizeDelta = new Vector2(70, 30);
            close.onClick.AddListener(() => panel.SetActive(false));
            panel.SetActive(false);
            return panel;
        }

        // ================= model =================
        void LoadClass(string cls)
        {
            foreach (var kv in loaded) kv.Value.SetActive(kv.Key == cls);
            if (loaded.ContainsKey(cls)) { BindModel(loaded[cls]); return; }
            var prefab = Resources.Load<GameObject>(cls + "-GAME");
            if (prefab == null) { model = null; animator = null; return; }
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
            camDist = Mathf.Max(b.size.y, 0.1f) * 1.55f;
            if (animator != null) animator.CrossFade("idle", 0f);
            if (hudLine != null) hudLine.text = chosen.name.ToUpper() + " — " + chosen.realm + " \u2022 " + Mathf.RoundToInt(b.size.y * 100) + " CM \u2022 FORGE MODEL";
        }

        // ================= frame: orbit =================
        Vector2 lastTouch0, lastTouch1; bool dragging;
        void Update()
        {
            bool overUI = EventSystem.current != null &&
                ((Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject()) ||
                 (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)));
            if (Input.touchCount == 1 && !overUI)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) { dragging = true; lastTouch0 = t.position; }
                else if (t.phase == TouchPhase.Moved && dragging) { camYaw += (t.position.x - lastTouch0.x) * 0.4f; lastTouch0 = t.position; }
                else if (t.phase == TouchPhase.Ended) dragging = false;
            }
            else if (Input.GetMouseButtonDown(0) && !overUI) { dragging = true; lastTouch0 = Input.mousePosition; }
            else if (Input.GetMouseButton(0) && dragging) { camYaw += ((Vector2)Input.mousePosition - lastTouch0).x * 0.35f; lastTouch0 = Input.mousePosition; }
            else if (Input.GetMouseButtonUp(0)) dragging = false;

            if (Input.touchCount == 2)
            {
                var t0 = Input.GetTouch(0); var t1 = Input.GetTouch(1);
                float d1 = Vector2.Distance(t0.position, t1.position);
                float d0 = Vector2.Distance(lastTouch0, lastTouch1);
                if (lastTouch0 != default && d0 > 1f) camDist = Mathf.Clamp(camDist * (d0 / d1), 0.5f, 12f);
                lastTouch0 = t0.position; lastTouch1 = t1.position;
            }
            else if (Input.touchCount != 2) { lastTouch0 = default; lastTouch1 = default; }
            camDist = Mathf.Clamp(camDist * (1f - Input.GetAxis("Mouse ScrollWheel")), 0.5f, 12f);

            var target = new Vector3(0, camDist * 0.30f, 0);
            float cy = Mathf.Cos(camYaw * Mathf.Deg2Rad), sy = Mathf.Sin(camYaw * Mathf.Deg2Rad);
            cam.transform.position = target + new Vector3(sy, 0.15f, cy) * camDist;
            cam.transform.LookAt(target);
        }

        // ================= ui factory =================
        Light MakeLight(Color c, float i, Vector3 pos)
        { var g = new GameObject(); var l = g.AddComponent<Light>(); l.type = LightType.Directional; l.color = c; l.intensity = i; g.transform.position = pos; return l; }
        Light MakeLight(Color c, float i, Vector2 pos)
        { return MakeLight(c, i, new Vector3(pos.x, 1.5f, pos.y)); }

        Canvas MakeCanvas()
        {
            var go = new GameObject("Canvas");
            var c = go.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<GraphicRaycaster>();
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            return c;
        }
        Font Font() { return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); }
        GameObject Panel(Transform parent, string name, Color c)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<Image>().color = c;
            return go;
        }
        Text Label(Transform parent, string txt, int size, Color c, TextAnchor anchor)
        {
            var go = new GameObject("lbl");
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>(); t.font = Font(); t.text = txt; t.fontSize = size; t.color = c; t.alignment = anchor;
            return t;
        }
        Button Btn(Transform parent, string txt, int size)
        {
            var go = new GameObject("btn-" + txt);
            go.transform.SetParent(parent, false);
            go.AddComponent<Image>().color = new Color(0.07f, 0.08f, 0.10f, 0.86f);
            var b = go.AddComponent<Button>();
            var t = Label(go.transform, txt, size, Hex(0xa3895a), TextAnchor.MiddleCenter);
            t.rect().Stretch(go.transform);
            return b;
        }
        static Color Hex(int v) { return new Color(((v >> 16) & 0xFF) / 255f, ((v >> 8) & 0xFF) / 255f, (v & 0xFF) / 255f); }
    }

    public static class UiExt
    {
        public static RectTransform rect(this Text t) { return t.transform as RectTransform; }
        public static void Stretch(this RectTransform r, Transform parent)
        { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero; r.transform.SetParent(parent, true); }
        public static RectTransform Stretch(this Text t)
        { var r = t.rect(); r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero; return r; }
    }
}
