// AVALON SHELL v2 — game-structure review build.
// TITLE -> CHARACTER SELECT -> IN-GAME (HUD + skill tab over the stage).
// All UI built at runtime; bootstrapped headless by AvalonShellBuild.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AvalonShell
{
    public class Shell : MonoBehaviour
    {
        const int CANON_BG = 0x0d0e10;
        // SELF-UPDATE — Big's order: on open, the app checks for a newer build and installs it.
        const string UPDATE_MANIFEST_URL = "https://mcontwitter-glitch.github.io/avalon-3d-viewer/live/game.json";
        [Serializable] class ApkMeta { public int versionCode; public string url; }
        [Serializable] class GameManifest { public ApkMeta apk; }
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
        GameObject panelTitle, panelSelect, panelGame, panelSkills, panelMap, panelSettings, panelAchievements;
        Camera cam; float camDist = 2.8f; float camYaw = 25f;
        Animator animator; GameObject model; Transform stagePivot;
        Text hudLine;
        GameObject questCard;
        Coroutine realmFade;

        // ================= gameplay: THE COLD HEARTH OATH (Campaign I tutorial) =================
        GameObject storyCard;
        Canvas canvas;
        Vector3 walkTarget; bool hasWalkTarget; const float walkSpeed = 1.4f;
        // MOBILE-CONTROL-BIBLE Stage A: floating joystick (left half) + camera split (right half)
        int joyFinger = -1, camFinger = -1;
        Vector2 joyOrigin; bool joyActive; Vector2 joyVec;
        Image joyBaseImg, joyNubImg;
        float lastCamDragT = -99f;
        bool maybeTap; Vector2 tapStart;
        int questStage = 0;                 // 0 reach the hearth, 1 carry (3 encounters), 2 to the gate, 3 complete
        bool[] met = new bool[3];
        bool faithUnlocked; System.Collections.Generic.List<string> choices = new System.Collections.Generic.List<string>();
        Text questLine, beliefLbl; RectTransform beliefFill;
        Vector3 hearthPos = new Vector3(0, 0, 7);
        Vector3[] encPos = { new Vector3(-6, 0, 2), new Vector3(6, 0, -3), new Vector3(-2, 0, -8) };
        Vector3 gatePos = new Vector3(0, 0, -13);
        Transform canvasT;
        readonly Dictionary<string, GameObject> loaded = new Dictionary<string, GameObject>();
        readonly List<Image> cardTints = new List<Image>();
        readonly List<RectTransform> cardRects = new List<RectTransform>();
        bool wasPortrait;

        // AVALON-LIT: real material asset staged by the build (its shader is
        // reference-counted into the APK by the asset itself). Runtime Shader.Find
        // returns null for stripped shaders -> missing material -> the v215
        // pink/black boot screen. Never do that again.
        static Material litMat;
        Material Lit()
        {
            if (litMat == null) litMat = Resources.Load<Material>("AVALON-LIT");
            return litMat;
        }
        Material Lit(Color c) { var m = Lit(); if (m == null) return null; m = Instantiate(m); m.color = c; return m; }

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

            // --- world layer: armor-clad — a world-building failure must never
            //     kill the title UI again (v215 pink/black boot lesson) ---
            try
            {
                // The Cold Reliquary ground: cold slate floor for the route (collider for tap-to-walk)
                var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground"; ground.transform.position = Vector3.zero; ground.transform.localScale = new Vector3(4, 1, 4);
                var gmat = Lit(new Color(0.055f, 0.06f, 0.075f));
                if (gmat != null) { ground.GetComponent<MeshRenderer>().material = gmat; ground.GetComponent<MeshRenderer>().enabled = gmat != null; }
                // Waymarks: dark stone pillars, amber crown light (Fire-Color Law — mortal fire is natural amber)
                MakeWaymark(hearthPos, "OathHearthMark");
                for (int i = 0; i < encPos.Length; i++) MakeWaymark(encPos[i], "EncounterMark" + i);
                MakeWaymark(gatePos, "CinderGateMark");
            }
            catch (Exception e) { Debug.LogError("[SHELL] world layer failed (UI continues): " + e.Message); }

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortraitUpsideDown = false;
            wasPortrait = Screen.height > Screen.width;

            canvas = MakeCanvas();
            canvasT = canvas.transform;
            panelTitle = BuildTitle(canvas.transform);
            panelSelect = BuildSelect(canvas.transform);
            panelGame = BuildGame(canvas.transform);
            panelMap = BuildMap(canvas.transform);
            panelSettings = BuildSettings(canvas.transform);
            panelAchievements = BuildAchievements(canvas.transform);
            panelSkills = BuildSkills(canvas.transform);
            LayoutCards();
            SetState(State.Title);
            // v219 INPUT PROBE: counts every raw touch Unity receives on the title screen.
            // If this number moves when you tap, input is alive (tap zones then respond loudly);
            // if it never moves, the input pipeline itself is dead on that device. Decisive either way.
            probeLbl = Label(canvasT, "INPUT 0", 10, new Color(0.55f, 0.52f, 0.47f, 0.8f), TextAnchor.UpperLeft);
            var plrt = probeLbl.rect();
            plrt.anchorMin = new Vector2(0.005f, 0.985f); plrt.anchorMax = new Vector2(0.20f, 1.0f); plrt.offsetMin = Vector2.zero; plrt.offsetMax = Vector2.zero;
            stateLbl = Label(canvasT, "TITLE", 10, new Color(0.55f, 0.52f, 0.47f, 0.8f), TextAnchor.UpperLeft);
            var slrt = stateLbl.rect();
            slrt.anchorMin = new Vector2(0.005f, 0.965f); slrt.anchorMax = new Vector2(0.20f, 0.982f); slrt.offsetMin = Vector2.zero; slrt.offsetMax = Vector2.zero;
            StartCoroutine(CheckForUpdate());
        }

        // ================= SELF-UPDATE =================
        IEnumerator CheckForUpdate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var toast = Label(canvasT, "", 12, Hex(0xa3895a), TextAnchor.MiddleCenter);
            var trt = toast.transform as RectTransform;
            trt.anchorMin = new Vector2(0, 0.955f); trt.anchorMax = new Vector2(1, 1.0f); trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            toast.gameObject.SetActive(false);

            // step 1 — manifest fetch (yields only inside using = try/finally, legal in iterators)
            UnityWebRequest req = null;
            try { req = UnityWebRequest.Get(UPDATE_MANIFEST_URL); req.timeout = 10; }
            catch (Exception e) { Debug.LogWarning("[SHELL] self-update: " + e.Message); yield break; }

            GameManifest manifest = null;
            using (req)
            {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    try { manifest = JsonUtility.FromJson<GameManifest>(req.downloadHandler.text); }
                    catch (Exception e) { Debug.LogWarning("[SHELL] manifest parse: " + e.Message); }
                }
            }
            if (manifest == null || manifest.apk == null || string.IsNullOrEmpty(manifest.apk.url)) yield break;

            // step 2 — compare with installed build
            int installed = -1;
            try { installed = InstalledVersionCode(); }
            catch (Exception e) { Debug.LogWarning("[SHELL] vc: " + e.Message); }
            if (installed <= 0 || manifest.apk.versionCode <= installed) yield break;   // already newest

            // step 3 — download the new APK
            toast.text = "NEW BUILD FOUND — DOWNLOADING…";
            toast.gameObject.SetActive(true);
            byte[] data = null;
            var dl = new UnityWebRequest(manifest.apk.url, "GET", new DownloadHandlerBuffer(), null);
            using (dl)
            {
                dl.timeout = 300;
                var op = dl.SendWebRequest();
                while (!op.isDone) yield return null;
                if (dl.result == UnityWebRequest.Result.Success)
                {
                    try { data = dl.downloadHandler.data; }
                    catch (Exception e) { Debug.LogWarning("[SHELL] download read: " + e.Message); }
                }
            }
            if (data == null || data.Length == 0)
            {
                toast.text = "UPDATE DOWNLOAD FAILED";
                yield return new WaitForSeconds(2.5f);
                toast.gameObject.SetActive(false);
                yield break;
            }

            // step 4 — hand to the system installer
            try
            {
                string apkPath = Path.Combine(Application.persistentDataPath, "avalon-update.apk");
                File.WriteAllBytes(apkPath, data);
                toast.text = "UPDATE READY — CONFIRM INSTALL";
                InstallApk(apkPath);
            }
            catch (Exception e) { Debug.LogWarning("[SHELL] install: " + e.Message); }
#endif
            yield break; // keeps this a valid iterator in editor/standalone configs
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        int InstalledVersionCode()
        {
            try
            {
                using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var act = up.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var pm = act.Call<AndroidJavaObject>("getPackageManager"))
                using (var info = pm.Call<AndroidJavaObject>("getPackageInfo", act.Call<string>("getPackageName"), 0))
                    return info.Get<int>("versionCode");
            }
            catch (Exception e) { Debug.LogWarning("[SHELL] vc: " + e.Message); return -1; }
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        void InstallApk(string path)
        {
            try
            {
                AndroidJavaObject session = null;
                using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var act = up.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var pm = act.Call<AndroidJavaObject>("getPackageManager"))
                using (var installer = pm.Call<AndroidJavaObject>("getPackageInstaller"))
                {
                    var pars = new AndroidJavaObject("android.content.pm.PackageInstaller$SessionParams", 1 /* MODE_FULL_INSTALL */);
                    int sid = installer.Call<int>("createSession", pars);
                    session = installer.Call<AndroidJavaObject>("openSession", sid);
                    using (var outs = session.Call<AndroidJavaObject>("openWrite", "avalon-update.apk", 0L, -1L))
                    {
                        byte[] bytes = File.ReadAllBytes(path);
                        outs.Call("write", bytes);
                        outs.Call("flush");
                        outs.Call("close");
                    }
                    var intent = new AndroidJavaObject("android.content.Intent", "com.bigfoot404.avalon.UPDATE_DONE");
                    var pending = new AndroidJavaClass("android.app.PendingIntent").CallStatic<AndroidJavaObject>(
                        "getBroadcast", act, 0, intent, 0x0C000000 /* UPDATE_CURRENT | IMMUTABLE */);
                    session.Call("commit", pending.Call<AndroidJavaObject>("getIntentSender"));
                    Debug.Log("[SHELL] update session committed — system install dialog should appear");
                }
            }
            catch (Exception e) { Debug.LogWarning("[SHELL] install: " + e.Message); }
        }
#endif

        IEnumerator ZonePulse(RectTransform rt)
        {
            // v220: the slab visibly depresses — no more guessing whether a tap registered
            if (rt == null) yield break;
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.07f) { rt.localScale = Vector3.one * (1f - 0.05f * t); yield return null; }
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.16f) { rt.localScale = Vector3.one * (0.95f + 0.05f * t); yield return null; }
            rt.localScale = Vector3.one;
        }
        IEnumerator PanelFlash(GameObject panel)
        {
            // v219: every screen change READS as a change — brief black wipe-in so a new
            // screen never looks like the same still image (plates are full-frame by law)
            var wipeGo = new GameObject("Wipe");
            wipeGo.transform.SetParent(panel.transform, false);
            var wi = wipeGo.AddComponent<Image>(); wi.color = new Color(0, 0, 0, 0.55f); wi.raycastTarget = false;
            var wrt = wi.rect(); wrt.Stretch(); wrt.offsetMin = Vector2.zero; wrt.offsetMax = Vector2.zero;
            wi.transform.SetAsLastSibling();
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.30f)
            {
                wi.color = new Color(0, 0, 0, 0.55f * (1f - t));
                yield return null;
            }
            Destroy(wipeGo);
        }
        void SetState(State s)
        {
            state = s;
            if (stateLbl != null) stateLbl.text = s.ToString().ToUpper();
            panelTitle.SetActive(s == State.Title);
            panelSelect.SetActive(s == State.Select);
            panelGame.SetActive(s == State.Game);
            if (s == State.Title || s == State.Select || s == State.Game)
            {
                var np = s == State.Title ? panelTitle : s == State.Select ? panelSelect : panelGame;
                if (np != null && np.activeSelf) StartCoroutine(PanelFlash(np));
            }
            if (s != State.Game && panelSkills.activeSelf) panelSkills.SetActive(false);
            if (s == State.Title && model != null) foreach (var kv in loaded) kv.Value.SetActive(false);
            if (s == State.Select) SelectCard(chosen);
            if (s == State.Game && hudLine != null)
                hudLine.text = chosen.name.ToUpper() + " — " + chosen.realm + " \u2022 TAP THE GROUND TO WALK";
            if (s == State.Game && questCard != null)
            {
                if (realmFade != null) StopCoroutine(realmFade);
                realmFade = StartCoroutine(RealmCardFade());
            }
        }

        // ---- v219 DIAGNOSTIC: raw input probe + state readout (title only, muted, decisive on-device evidence) ----
        int probeCount = 0;
        Text probeLbl = null;
        Text stateLbl = null;
        // ================= canon plates as screens (Big, Sept 13: the update "isn't what it should be like") =================
        // The plates he approved ARE the UI — full-screen plate art with invisible tap zones over the carved options.
        // aspect-true fit: the canon plate letterboxes into any orientation — NEVER stretch
        // (Big, Sept 13: the screen must BE the plate, not a distorted image of it)
        // ================= quest: THE COLD HEARTH OATH (Campaign I, Kingdom Quest 1) =================
        // A dying forge-judge cannot pass sentence without a witness of standing.
        // You carry the hearth-coal across the gate-town: three encounters, each a CHOICE.
        // The town changes off who you crossed. Ends with the Mark ceremony + Faith unlock.
        void QuestStations()
        {
            if (storyCard != null || model == null) return;
            var p = model.transform.position;
            if (questStage == 0 && Near(p, hearthPos))
                OpenStory("THE OATH-HEARTH",
                    "The forge-judge\u2019s coal waits on the oath-hearth \u2014 it burns without fuel.\n\nA dying man\u2019s sentence rides on your carrying it to the Cinder Gate. The gate-town will test you on the way.",
                    new string[] { "TAKE THE COAL" },
                    new System.Action[] { delegate { questStage = 1; choices.Add("coal-taken"); SaveGame(); UpdateQuestLine(); } });
            else if (questStage == 1)
            {
                if (!met[0] && Near(p, encPos[0]))
                    OpenStory("THE GILDED HAND",
                        "A guildman steps from the ash-glass, palms open.\n\n\u201cSet the coal down, friend. There\u2019s more silver in this than a judge\u2019s favor.\u201d",
                        new string[] { "TAKE HIS SILVER", "REFUSE" },
                        new System.Action[] {
                            delegate { met[0] = true; choices.Add("silver-taken"); SaveGame(); UpdateQuestLine(); },
                            delegate { met[0] = true; choices.Add("guild-refused"); SaveGame(); UpdateQuestLine(); } });
                else if (!met[1] && Near(p, encPos[1]))
                    OpenStory("THE ENFORCER",
                        "The guild\u2019s enforcer bars the road, iron cudgel loose in his hand.\n\n\u201cThe coal goes no further.\u201d",
                        new string[] { "STAND YOUR GROUND", "TAKE THE LONG WAY" },
                        new System.Action[] {
                            delegate { met[1] = true; choices.Add("stood-ground"); SaveGame(); UpdateQuestLine(); },
                            delegate { met[1] = true; choices.Add("long-way"); SaveGame(); UpdateQuestLine(); } });
                else if (!met[2] && Near(p, encPos[2]))
                    OpenStory("THE WIDOW\u2019S PLEA",
                        "A widow kneels in the cinders, hearth long cold.\n\n\u201cOne ember. For my children. The judge will never miss it.\u201d",
                        new string[] { "GIVE THE EMBER", "KEEP THE COAL WHOLE" },
                        new System.Action[] {
                            delegate { met[2] = true; choices.Add("ember-given"); SaveGame(); UpdateQuestLine(); },
                            delegate { met[2] = true; choices.Add("coal-whole"); SaveGame(); UpdateQuestLine(); } });
                else if (met[0] && met[1] && met[2]) { questStage = 2; UpdateQuestLine(); }
            }
            else if (questStage == 2 && Near(p, gatePos))
                OpenStory("THE CINDER GATE \u2014 THE MARK",
                    "The gate\u2019s brazier takes the coal, and the flame steadies.\n\nThe judge\u2019s sentence is witnessed. The gate remembers you now: ONE OF THE MARKED.\n\nYour choices live in the town behind you \u2014 silver, ground, and ember all leave marks.\n\nCODEX UNLOCKED: THE COLD HEARTH OATH",
                    new string[] { "RECEIVE THE MARK" },
                    new System.Action[] { delegate { CompleteQuest(); } });
        }

        void CompleteQuest()
        {
            questStage = 3; faithUnlocked = true; SaveGame(); UpdateQuestLine();
            if (beliefFill != null) beliefFill.anchorMax = new Vector2(0.60f, 0.75f);
            if (beliefLbl != null) beliefLbl.text = "BELIEF \u2014 ALIT";
        }

        void UpdateQuestLine()
        {
            if (questLine == null) return;
            if (questStage == 0) questLine.text = "THE COLD HEARTH OATH \u2014 REACH THE OATH-HEARTH";
            else if (questStage == 1) questLine.text = "CARRY THE COAL \u2014 THE GATE-TOWN TESTS YOU (" + ((met[0] ? 1 : 0) + (met[1] ? 1 : 0) + (met[2] ? 1 : 0)) + "/3)";
            else if (questStage == 2) questLine.text = "CARRY THE COAL TO THE CINDER GATE";
            else questLine.text = "THE OATH IS WITNESSED \u2014 ONE OF THE MARKED";
        }

        // ================= story cards =================
        // 2D FOR WHAT IS MYTH (UI Design System): encounters + choices live as dark slate story cards.
        void OpenStory(string title, string body, string[] labels, System.Action[] acts)
        {
            if (storyCard != null) return;
            hasWalkTarget = false; if (animator) animator.CrossFade("idle", 0.2f);
            var p = Panel(canvas.transform, "StoryCard", new Color(0.035f, 0.039f, 0.047f, 0.96f));
            p.transform.Stretch();
            var ruleCol = Hex(0xa3895a); ruleCol.a = 0.9f;
            var topRule = Panel(p.transform, "Rule", ruleCol);
            topRule.rect().anchorMin = new Vector2(0.2f, 0.86f); topRule.rect().anchorMax = new Vector2(0.8f, 0.864f);
            var h = Label(p.transform, title, 22, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            h.rect().anchorMin = new Vector2(0.05f, 0.77f); h.rect().anchorMax = new Vector2(0.95f, 0.86f);
            var b = Label(p.transform, body, 14, Hex(0xc8c2b2), TextAnchor.UpperCenter);
            b.rect().anchorMin = new Vector2(0.08f, 0.30f); b.rect().anchorMax = new Vector2(0.92f, 0.75f);
            for (int i = 0; i < labels.Length; i++)
            {
                var btn = Btn(p.transform, labels[i], 13);
                var brt = btn.transform as RectTransform;
                brt.anchorMin = new Vector2(0.5f, 0.06f); brt.anchorMax = new Vector2(0.5f, 0.06f);
                brt.sizeDelta = new Vector2(300, 46);
                brt.anchoredPosition = new Vector2(0, 24 + i * 58);
                var act = acts[i];
                btn.onClick.AddListener(() => { UnityEngine.Object.Destroy(storyCard); storyCard = null; if (act != null) act(); });
            }
            storyCard = p;
        }

        void SaveGame()
        {
            string metS = (met[0] ? "1" : "0") + (met[1] ? "1" : "0") + (met[2] ? "1" : "0");
            string chS = string.Join(";", choices.ToArray());
            PlayerPrefs.SetString("avalon.save", chosen.name + "|" + questStage + "|" + metS + "|" + chS);
            PlayerPrefs.Save();
        }

        bool LoadSave()
        {
            if (!PlayerPrefs.HasKey("avalon.save")) return false;
            try
            {
                var parts = PlayerPrefs.GetString("avalon.save").Split('|');
                var cd = CLASSES.FirstOrDefault(c => c.name == parts[0]);
                if (cd == null) return false;
                chosen = cd; questStage = int.Parse(parts[1]);
                if (parts[2].Length >= 3) { met[0] = parts[2][0] == '1'; met[1] = parts[2][1] == '1'; met[2] = parts[2][2] == '1'; }
                choices = new System.Collections.Generic.List<string>((parts.Length > 3 && parts[3].Length > 0) ? parts[3].Split(';') : new string[0]);
                faithUnlocked = questStage == 3;
                return true;
            } catch { return false; }
        }

        // Law 3 region title card: 2.5s hold, 1s fade, then gone — the world takes over.
        IEnumerator RealmCardFade()
        {
            var cg = questCard.GetComponent<CanvasGroup>();
            if (cg == null) cg = questCard.AddComponent<CanvasGroup>();
            questCard.SetActive(true);
            cg.alpha = 1f;
            yield return new WaitForSeconds(2.5f);
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                cg.alpha = 1f - t;
                yield return null;
            }
            cg.alpha = 0f;
            questCard.SetActive(false);
        }

        // ---- v226 PLATE-STYLE RESTYLE (Big, Sept 13: "take the style and layouts of the reference
        // images and apply that to ours"): the canon plates' carved-stone grammar as REAL UI.
        // Stone slab (dark #141c1f face) inside a bronze hairline edge, engraved bronze Cinzel
        // label, plate-faithful anchor fractions — and the v224 dual-path wiring untouched. ----
        Button CarvedBtn(Transform parent, string txt, float ax0, float ax1, float ay0, float ay1, bool live, System.Action act)
        {
            // bronze hairline edge (outer slab)
            var slab = new GameObject("slab-" + txt);
            slab.transform.SetParent(parent, false);
            var edge = slab.AddComponent<Image>();
            edge.sprite = Rounded(); edge.color = new Color(0.42f, 0.34f, 0.21f, 0.85f);   // dark bronze edge
            var ert = edge.rect();
            ert.anchorMin = new Vector2(ax0, ay0); ert.anchorMax = new Vector2(ax1, ay1);
            ert.offsetMin = Vector2.zero; ert.offsetMax = Vector2.zero;
            // stone face (inner)
            var face = new GameObject("face");
            face.transform.SetParent(slab.transform, false);
            var fim = face.AddComponent<Image>();
            fim.sprite = Rounded(); fim.color = new Color(0.078f, 0.105f, 0.121f, 0.96f);  // #141c1f stone
            fim.rect().Stretch();
            var frt = fim.rect();
            frt.anchorMin = new Vector2(0.035f, 0.11f); frt.anchorMax = new Vector2(0.965f, 0.89f);
            // engraved bronze label
            var lbl = Label(slab.transform, txt, 15, Hex(0xc2a367), TextAnchor.MiddleCenter);
            lbl.rect().Stretch();
            var b = slab.AddComponent<Button>();
            b.targetGraphic = fim;
            var cb = b.colors; cb.normalColor = new Color(1, 1, 1, 1); cb.highlightedColor = new Color(1.14f, 1.10f, 0.92f, 1);
            cb.pressedColor = new Color(0.72f, 0.62f, 0.40f, 1); cb.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.5f);
            b.colors = cb;
            b.interactable = live;
            if (live)
            {
                b.onClick.AddListener(() => act());
                TapTo(ert, act);
            }
            return b;
        }

        // ================= TITLE =================
GameObject BuildTitle(Transform parent)
        {
            // v224 GROUND-UP UI (Big, Sept 13: "redo the menu and ui... build it from ground up to
            // be like the real game"): a REAL game menu drawn entirely in code. Keyart is a dimmed
            // backdrop only — never the interface. Every option is a real button, wired twice
            // (EventSystem + TapRouter raw-touch path) so the menu responds no matter what.
            var p = Panel(parent, "Title", new Color(0.051f, 0.055f, 0.063f, 0.97f));
            p.transform.Stretch();

            var bgSprite = Art("CINEMATIC-TEASER-KEYART-CANON");
            if (bgSprite != null)
            {
                var bg = new GameObject("KeyArt");
                bg.transform.SetParent(p.transform, false);
                var bi = bg.AddComponent<Image>();
                bi.sprite = bgSprite; bi.preserveAspect = true; bi.color = new Color(0.60f, 0.58f, 0.56f, 1f);
                bi.raycastTarget = false;
                bi.rect().Stretch();
                var shade = Panel(p.transform, "Shade", new Color(0.03f, 0.033f, 0.04f, 0.60f));
                shade.transform.Stretch();
                shade.GetComponent<Image>().raycastTarget = false;
                var bottom = Panel(p.transform, "BottomFade", new Color(0.03f, 0.033f, 0.04f, 0.82f));
                var brt = bottom.rect();
                brt.anchorMin = new Vector2(0, 0); brt.anchorMax = new Vector2(1, 0.58f); brt.offsetMin = Vector2.zero; brt.offsetMax = Vector2.zero;
                bottom.GetComponent<Image>().raycastTarget = false;
            }

            var title = Label(p.transform, "AVALON", 64, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            title.rect().anchorMin = new Vector2(0, 0.72f); title.rect().anchorMax = new Vector2(1, 0.92f);
            var ruleCol = Hex(0xa3895a); ruleCol.a = 0.85f;
            var rule = Panel(p.transform, "Rule", ruleCol);
            rule.GetComponent<Image>().raycastTarget = false;
            var rrt = rule.rect();
            rrt.anchorMin = new Vector2(0.30f, 0.70f); rrt.anchorMax = new Vector2(0.70f, 0.704f); rrt.offsetMin = Vector2.zero; rrt.offsetMax = Vector2.zero;
            var sub = Label(p.transform, "THE WAKING GATES", 16, Hex(0xa3895a), TextAnchor.MiddleCenter);
            sub.rect().anchorMin = new Vector2(0, 0.64f); sub.rect().anchorMax = new Vector2(1, 0.70f);

            bool hasSave = PlayerPrefs.HasKey("avalon.save");
            MenuBtn(p.transform, "NEW JOURNEY", true, delegate { SetState(State.Select); }, 0);
            MenuBtn(p.transform, "CONTINUE", hasSave, delegate {
                if (LoadSave())
                {
                    LoadClass(chosen.name); UpdateQuestLine();
                    if (faithUnlocked)
                    {
                        if (beliefFill != null) beliefFill.anchorMax = new Vector2(0.60f, 0.75f);
                        if (beliefLbl != null) beliefLbl.text = "BELIEF \u2014 ALIT";
                    }
                    SetState(State.Game);
                }
            }, 1);
            MenuBtn(p.transform, "GATES", true, delegate { OpenMap(); }, 2);
            MenuBtn(p.transform, "ACHIEVEMENTS", true, delegate { panelAchievements.SetActive(true); }, 3);
            MenuBtn(p.transform, "SETTINGS", true, delegate { panelSettings.SetActive(true); }, 4);

#if UNITY_ANDROID && !UNITY_EDITOR
            int stampVc = InstalledVersionCode();
#else
            int stampVc = 0;
#endif
            var stampTxt = Label(p.transform, "SHELL v" + stampVc + " \u2022 " + System.DateTime.Now.ToString("MMM d"), 11, new Color(0.72f, 0.66f, 0.55f, 0.85f), TextAnchor.UpperRight);
            stampTxt.rect().anchorMin = new Vector2(0.78f, 0.012f); stampTxt.rect().anchorMax = new Vector2(0.995f, 0.030f);
            var ver = Label(p.transform, "V 0.2.11  \u00a9 2026 WAKING GATES, INC. ALL RIGHTS RESERVED.", 9, Hex(0x8a8578), TextAnchor.MiddleCenter);
            ver.rect().anchorMin = new Vector2(0, 0.005f); ver.rect().anchorMax = new Vector2(1, 0.04f);
            return p;
        }

        // a REAL menu button: wired twice — Button.onClick AND the TapRouter raw-touch path.
        // Double-fire is idempotent for every menu action (state sets + panel toggles), so the
        // redundancy is safe — and the menu works even if the EventSystem never fires on-device.
        Button MenuBtn(Transform parent, string txt, bool live, System.Action act, int slot)
        {
            var b = Btn(parent, txt, 15);
            b.interactable = live;
            var brt = b.transform as RectTransform;
            brt.anchorMin = new Vector2(0.5f, 0.5f); brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.sizeDelta = new Vector2(280, 46);
            brt.anchoredPosition = new Vector2(0, 44 - slot * 56);
            if (live)
            {
                b.onClick.AddListener(() => act());
                TapTo(brt, act);
            }
            return b;
        }

        // ---- v224 TAPROUTER: raw-touch hit-testing, fully independent of the EventSystem ----
        // Real games don't bet the menu on one input pipeline. Raw touches are hit-tested directly
        // against registered rects every frame — a second, parallel path to every nav action.
        class Tappable { public RectTransform rt; public System.Action act; public string name; }
        readonly List<Tappable> tappables = new List<Tappable>();
        int lastRouterFrame = -1; string lastRouterName = "";
        void TapTo(RectTransform rt, System.Action act)
        {
            tappables.Add(new Tappable { rt = rt, act = act, name = rt.name });
        }
        void RunTaps()
        {
            int n = Input.touchCount;
            for (int t = 0; t < n; t++)
            {
                if (Input.GetTouch(t).phase != TouchPhase.Began) continue;
                FireAt(Input.GetTouch(t).position);
            }
            if (n == 0 && Input.GetMouseButtonDown(0)) FireAt((Vector2)Input.mousePosition);
        }
        void FireAt(Vector2 screenPos)
        {
            for (int i = tappables.Count - 1; i >= 0; i--)
            {
                var tp = tappables[i];
                if (tp.rt == null || !tp.rt.gameObject.activeInHierarchy) continue;
                if (RectTransformUtility.RectangleContainsScreenPoint(tp.rt, screenPos, null))
                {
                    int f = Time.frameCount;
                    if (f == lastRouterFrame && tp.name == lastRouterName) return;
                    lastRouterFrame = f; lastRouterName = tp.name;
                    if (stateLbl != null) stateLbl.text = "TAP " + tp.name;
                    tp.act();
                    return;
                }
            }
        }

        // ---- SETTINGS (real options, wired) ----
        GameObject BuildSettings(Transform parent)
        {
            var p = Panel(parent, "SettingsPanel", new Color(0.043f, 0.047f, 0.055f, 0.97f));
            p.transform.Stretch();
            var head = Label(p.transform, "SETTINGS", 22, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            head.rect().anchorMin = new Vector2(0, 0.86f); head.rect().anchorMax = new Vector2(1, 0.98f);
            var tb = Btn(p.transform, "INPUT PROBE: ON", 13);
            var trt = tb.transform as RectTransform;
            trt.anchorMin = new Vector2(0.5f, 0.5f); trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.sizeDelta = new Vector2(300, 46); trt.anchoredPosition = new Vector2(0, 40);
            float lastToggle = -1f;
            System.Action toggle = delegate {
                if (Time.time - lastToggle < 0.25f) return;   // double-path guard
                lastToggle = Time.time;
                if (probeLbl == null) return;
                probeLbl.gameObject.SetActive(!probeLbl.gameObject.activeSelf);
                var l = tb.GetComponentInChildren<Text>();
                if (l != null) l.text = "INPUT PROBE: " + (probeLbl.gameObject.activeSelf ? "ON" : "OFF");
            };
            tb.onClick.AddListener(() => toggle());
            TapTo(trt, toggle);
            var note = Label(p.transform, "MORE OPTIONS SHIP WITH THE FULL GAME BUILD", 10, Hex(0x6f6a5e), TextAnchor.MiddleCenter);
            note.rect().anchorMin = new Vector2(0, 0.52f); note.rect().anchorMax = new Vector2(1, 0.58f);
            var close = Btn(p.transform, "CLOSE", 12);
            var crt = close.transform as RectTransform;
            crt.anchorMin = new Vector2(0.5f, 0.5f); crt.anchorMax = new Vector2(0.5f, 0.5f);
            crt.sizeDelta = new Vector2(140, 42); crt.anchoredPosition = new Vector2(0, -30);
            close.onClick.AddListener(() => p.SetActive(false));
            TapTo(crt, () => p.SetActive(false));
            p.SetActive(false);
            return p;
        }

        // ---- ACHIEVEMENTS (real progress from the save) ----
        GameObject BuildAchievements(Transform parent)
        {
            var p = Panel(parent, "AchievementsPanel", new Color(0.043f, 0.047f, 0.055f, 0.97f));
            p.transform.Stretch();
            var head = Label(p.transform, "ACHIEVEMENTS", 22, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            head.rect().anchorMin = new Vector2(0, 0.86f); head.rect().anchorMax = new Vector2(1, 0.98f);
            string[] lines;
            if (PlayerPrefs.HasKey("avalon.save"))
            {
                try
                {
                    var parts = PlayerPrefs.GetString("avalon.save").Split('|');
                    int stage = int.Parse(parts[1]);
                    string[] stageNames = { "THE COLD HEARTH OATH \u2014 UNBEGUN", "THE COLD HEARTH OATH \u2014 COAL IN HAND", "THE COLD HEARTH OATH \u2014 THE GATE AWAITS", "THE COLD HEARTH OATH \u2014 WITNESSED" };
                    lines = new string[] {
                        "MARKED CLASS \u2014 " + parts[0].ToUpper(),
                        stageNames[System.Math.Min(stage, 3)],
                        (stage >= 3 ? "BELIEF \u2014 ALIT" : "BELIEF \u2014 UNLIT"),
                        "REALMS CROSSED \u2014 SKYREND",
                    };
                }
                catch { lines = new string[] { "THE MARKS ARE UNREADABLE \u2014 BEGIN A JOURNEY" }; }
            }
            else lines = new string[] { "NO DEEDS YET \u2014 BEGIN A JOURNEY" };
            for (int i = 0; i < lines.Length; i++)
            {
                var l = Label(p.transform, lines[i], 13, i == 1 && lines[i].Contains("WITNESSED") ? Hex(0xa3895a) : Hex(0xc8c2b2), TextAnchor.MiddleCenter);
                l.rect().anchorMin = new Vector2(0.05f, 0.68f - i * 0.09f); l.rect().anchorMax = new Vector2(0.95f, 0.74f - i * 0.09f);
            }
            var close = Btn(p.transform, "CLOSE", 12);
            var crt = close.transform as RectTransform;
            crt.anchorMin = new Vector2(0.5f, 0.5f); crt.anchorMax = new Vector2(0.5f, 0.5f);
            crt.sizeDelta = new Vector2(140, 42); crt.anchoredPosition = new Vector2(0, -30);
            close.onClick.AddListener(() => p.SetActive(false));
            TapTo(crt, () => p.SetActive(false));
            p.SetActive(false);
            return p;
        }

        // ================= SELECT =================
GameObject BuildSelect(Transform parent)
        {
            // v224 GROUND-UP: real class-select screen — 2x3 (portrait 3x2) card grid, every card a
            // real button (EventSystem + TapRouter), ember highlight on the chosen, real BEGIN/BACK.
            var p = Panel(parent, "Select", new Color(0.043f, 0.047f, 0.055f, 0.97f));
            p.transform.Stretch();
            var head = Label(p.transform, "CHOOSE YOUR CLASS", 24, Hex(0xe6ddca), TextAnchor.MiddleCenter);
            head.rect().anchorMin = new Vector2(0, 0.90f); head.rect().anchorMax = new Vector2(1, 0.99f);
            var head2 = Label(p.transform, "AVALON \u2014 THE WAKING GATES", 11, Hex(0xa3895a), TextAnchor.MiddleCenter);
            head2.rect().anchorMin = new Vector2(0, 0.99f); head2.rect().anchorMax = new Vector2(1, 1.04f);

            for (int i = 0; i < CLASSES.Length; i++)
            {
                var cd = CLASSES[i];
                bool unlocked = ModelPrefab(cd.name) != null;
                var card = Panel(p.transform, "card-" + cd.name, new Color(0.07f, 0.08f, 0.10f, 0.92f));
                var crt = card.rect();
                cardRects.Add(crt);
                var art = Art("CLASS-" + cd.name.ToUpper() + "-CANON");
                if (art != null)
                {
                    var ai = new GameObject("art");
                    ai.transform.SetParent(card.transform, false);
                    var img = ai.AddComponent<Image>();
                    img.sprite = art; img.preserveAspect = true; img.color = unlocked ? new Color(0.92f, 0.90f, 0.86f, 1f) : new Color(0.45f, 0.45f, 0.45f, 0.55f);
                    img.rect().Stretch();
                    var band = Panel(card.transform, "Band", new Color(0.03f, 0.033f, 0.04f, 0.80f));
                    var bandRt = band.rect();
                    bandRt.anchorMin = new Vector2(0, 0); bandRt.anchorMax = new Vector2(1, 0.44f); bandRt.offsetMin = Vector2.zero; bandRt.offsetMax = Vector2.zero;
                }
                var nm = Label(card.transform, cd.name.ToUpper(), 16, unlocked ? Hex(0xe6ddca) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                nm.rect().anchorMin = new Vector2(0, 0.32f); nm.rect().anchorMax = new Vector2(1, 0.44f);
                var ro = Label(card.transform, cd.role, 9, Hex(0xa3895a), TextAnchor.MiddleCenter);
                ro.rect().anchorMin = new Vector2(0, 0.24f); ro.rect().anchorMax = new Vector2(1, 0.32f);
                var bl = Label(card.transform, cd.blurb, 9, Hex(0x9a9588), TextAnchor.UpperCenter);
                bl.rect().anchorMin = new Vector2(0.07f, 0.10f); bl.rect().anchorMax = new Vector2(0.93f, 0.24f);
                var st = Label(card.transform, unlocked ? "FORGED" : "IN THE FORGE", 10, unlocked ? Hex(0xa3895a) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                st.rect().anchorMin = Vector2.zero; st.rect().anchorMax = new Vector2(1, 0.10f);
                var b = card.AddComponent<Button>(); cardTints.Add(card.GetComponent<Image>());
                var captured = cd;
                b.onClick.AddListener(() => SelectCard(captured));
                TapTo(crt, () => SelectCard(captured));   // router: works even if the EventSystem is dead
            }

            var enter = Btn(p.transform, "BEGIN THE WAKENING", 14);
            var ert = enter.transform as RectTransform;
            ert.anchorMin = new Vector2(0.5f, 0.055f); ert.anchorMax = new Vector2(0.5f, 0.055f);
            ert.sizeDelta = new Vector2(230, 48);
            ert.anchoredPosition = new Vector2(-120, 0);
            enter.onClick.AddListener(() => SetState(State.Game));
            TapTo(ert, () => SetState(State.Game));
            var back = Btn(p.transform, "BACK", 14);
            var brt = back.transform as RectTransform;
            brt.anchorMin = new Vector2(0.5f, 0.055f); brt.anchorMax = new Vector2(0.5f, 0.055f);
            brt.sizeDelta = new Vector2(120, 48);
            brt.anchoredPosition = new Vector2(105, 0);
            back.onClick.AddListener(() => SetState(State.Title));
            TapTo(brt, () => SetState(State.Title));
            return p;
        }

        void SelectCard(ClassDef cd)
        {
            chosen = cd;
            for (int i = 0; i < CLASSES.Length; i++)
                cardTints[i].color = (CLASSES[i] == cd) ? new Color(0.16f, 0.14f, 0.09f, 0.96f) : new Color(0.07f, 0.08f, 0.10f, 0.92f);
            LoadClass(cd.name);
        }

        // Responsive layout: portrait = 2 rows of 3 cards; landscape = 1 row of 6. Relayout on rotate.
        void LayoutCards()
        {
            bool portrait = Screen.height > Screen.width;
            for (int i = 0; i < cardRects.Count && i < CLASSES.Length; i++)
            {
                var crt = cardRects[i];
                int row, col;
                if (portrait) { row = i / 3; col = i % 3; }
                else { row = 0; col = i; }
                float cw = 1f / (portrait ? 3 : CLASSES.Length);
                float yMax = portrait ? (row == 0 ? 0.86f : 0.36f) : 0.72f;
                float yMin = portrait ? (row == 0 ? 0.42f : 0.12f) : 0.28f;
                crt.anchorMin = new Vector2(cw * col + 0.008f, yMin);
                crt.anchorMax = new Vector2(cw * (col + 1) - 0.008f, yMax);
            }
            if (cam != null) camDist = portrait ? 3.4f : 2.8f;   // pull camera back in portrait to frame the model
        }

        // ================= GAME =================
        GameObject BuildGame(Transform parent)
        {
            var p = Panel(parent, "Game", new Color(0, 0, 0, 0));
            p.transform.Stretch();

            // HUD top-left: class crest panel
            var crest = Panel(p.transform, "Crest", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            var crt = crest.rect();
            crt.anchorMin = new Vector2(0.01f, 0.94f); crt.anchorMax = new Vector2(0.38f, 0.99f);
            var cls = Label(crest.transform, "SOVEREIGN", 15, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            cls.rect().anchorMin = new Vector2(0, 0.5f); cls.rect().anchorMax = Vector2.one; cls.rect().offsetMin = new Vector2(14, 0); cls.rect().offsetMax = new Vector2(-10, -4);
            var lvl = Label(crest.transform, "LEVEL 1 — SKYREND", 10, Hex(0x8a8578), TextAnchor.MiddleLeft);
            lvl.rect().anchorMin = Vector2.zero; lvl.rect().anchorMax = new Vector2(1, 0.5f); lvl.rect().offsetMin = new Vector2(14, 4); lvl.rect().offsetMax = new Vector2(-10, 0);

            // HUD: health + belief bars under the crest
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
            var spLbl = Label(spBack.transform, faithUnlocked ? "BELIEF" : "BELIEF \u2014 SEALED", 9, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            spLbl.rect().anchorMin = new Vector2(0.02f, 0.25f); spLbl.rect().anchorMax = new Vector2(0.98f, 0.75f); spLbl.rect().offsetMin = new Vector2(8, 0);
            beliefLbl = spLbl; beliefFill = spFill.rect();
            if (faithUnlocked) beliefFill.anchorMax = new Vector2(0.60f, 0.75f);
            else beliefFill.anchorMax = new Vector2(0.06f, 0.75f);

            // HUD quest line TOP-CENTER per canon plate (REACH THE FIRST GATE) + Law 2 one-line queue
            var qLbl = Label(p.transform, "THE COLD HEARTH OATH \u2014 REACH THE OATH-HEARTH", 12, Hex(0xe6ddca), TextAnchor.MiddleCenter);
            qLbl.rect().anchorMin = new Vector2(0.24f, 0.955f); qLbl.rect().anchorMax = new Vector2(0.76f, 0.99f);
            questLine = qLbl; UpdateQuestLine();
            // Region title card (Law 3): SKYREND + epigraph + accent underline, fades 2.5s after entering
            var realmCard = Panel(p.transform, "RealmCard", new Color(0, 0, 0, 0));
            var rcrt = realmCard.rect();
            rcrt.anchorMin = new Vector2(0.20f, 0.78f); rcrt.anchorMax = new Vector2(0.80f, 0.95f);
            var realmName = Label(realmCard.transform, "SKYREND", 34, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            realmName.rect().anchorMin = new Vector2(0, 0.42f); realmName.rect().anchorMax = new Vector2(1, 1f);
            var epigraph = Label(realmCard.transform, "The gate remembers who lit the flame.", 12, Hex(0x8a8578), TextAnchor.MiddleCenter);
            epigraph.rect().anchorMin = new Vector2(0, 0.24f); epigraph.rect().anchorMax = new Vector2(1, 0.42f);
            var slate = new Color(0.45f, 0.53f, 0.66f, 0.85f);   // storm-slate accent (Skyrend)
            var underline = Panel(realmCard.transform, "Underline", slate);
            var urt = underline.rect();
            urt.anchorMin = new Vector2(0.40f, 0.19f); urt.anchorMax = new Vector2(0.60f, 0.22f); urt.offsetMin = Vector2.zero; urt.offsetMax = Vector2.zero;
            realmCard.AddComponent<CanvasGroup>();
            var realmCardObj = realmCard;
            questCard = realmCardObj;

            // HUD top-right: reliquary (unlit)
            var lantern = Panel(p.transform, "Lantern", new Color(0.039f, 0.043f, 0.051f, 0.88f));
            var lrt = lantern.rect();
            lrt.anchorMin = new Vector2(0.62f, 0.94f); lrt.anchorMax = new Vector2(0.99f, 0.99f);
            Label(lantern.transform, "THE RELIQUARY — UNLIT", 11, Hex(0x8a8578), TextAnchor.MiddleRight).rect().Stretch(lantern.transform);

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
            var bMap = Btn(p.transform, "MAP", 11);
            (bMap.transform as RectTransform).anchorMin = new Vector2(0.995f, 0.02f); (bMap.transform as RectTransform).anchorMax = new Vector2(0.995f, 0.02f);
            (bMap.transform as RectTransform).anchoredPosition = new Vector2(-270, 30);
            (bMap.transform as RectTransform).sizeDelta = new Vector2(90, 36);
            bMap.onClick.AddListener(() => { if (panelMap.activeSelf) panelMap.SetActive(false); else OpenMap(); });

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

        // ---- v221 PIN PASS: myth-veiled map pins ----
        static readonly (string site, string veiled, string trueName, Vector2 at, int revealStage)[] MAP_PINS = new (string, string, string, Vector2, int)[] {
            ("OATH-HEARTH",   "A COLD LIGHT IN THE MIST",     "OATH-HEARTH",    new Vector2(0.18f, 0.22f), 1),
            ("GATE-TOWN",     "SHAPES GATHER IN THE FOG",     "THE GATE-TOWN",  new Vector2(0.50f, 0.46f), 2),
            ("FIRST-GATE",    "AN UNLIT SHAPE, FAR OFF",      "THE FIRST GATE", new Vector2(0.82f, 0.72f), 3),
        };
        System.Collections.Generic.List<GameObject> mapPins;

        void OpenMap()
        {
            if (mapPins != null)
                for (int i = 0; i < mapPins.Count; i++)
                    if (mapPins[i] != null)
                    {
                        bool disc = questStage >= MAP_PINS[i].revealStage;
                        mapPins[i].transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = disc ? MAP_PINS[i].trueName : MAP_PINS[i].veiled;
                        var img = mapPins[i].GetComponent<UnityEngine.UI.Image>();
                        var c = img.color; c.r = disc ? 0.86f : 0.42f; c.g = disc ? 0.62f : 0.39f; c.b = disc ? 0.28f : 0.30f; img.color = c;
                    }
            panelMap.SetActive(true);
        }

        GameObject BuildMapPin(Transform mapRoot, int i)
        {
            var pin = Panel(mapRoot, "Pin-" + MAP_PINS[i].site, new Color(0.42f, 0.39f, 0.30f, 0.95f));
            var prt = pin.transform as RectTransform;
            var at = MAP_PINS[i].at;
            float px = 0.03f + at.x * 0.94f, py = 0.03f + at.y * 0.87f;
            prt.anchorMin = new Vector2(px, py); prt.anchorMax = new Vector2(px, py);
            prt.sizeDelta = new Vector2(22, 22); prt.localEulerAngles = new Vector3(0, 0, 45);
            var nm = Label(pin.transform, MAP_PINS[i].trueName, 10, Hex(0xd8c9a3), TextAnchor.UpperCenter);
            nm.name = "Name";
            nm.rect().localEulerAngles = new Vector3(0, 0, -45);
            nm.rect().anchorMin = new Vector2(0, -0.4f); nm.rect().anchorMax = new Vector2(0, -0.4f);
            nm.rect().sizeDelta = new Vector2(220, 30); nm.rect().anchoredPosition = new Vector2(0, -4);
            return pin;
        }

        // ================= MAP =================
        // Stage 5 of GAME-UI-LAYOUT: landmark map. Cold Reliquary slice live (approved canon plate);
        // landmark pins land with the pin pass.
        GameObject BuildMap(Transform parent)
        {
            var p = Panel(parent, "MapPanel", new Color(0.043f, 0.047f, 0.055f, 0.97f));
            p.transform.Stretch();
            var head = Label(p.transform, "COLD RELIQUARY — ASHFALL", 15, Hex(0xe6ddca), TextAnchor.MiddleLeft);
            head.rect().anchorMin = new Vector2(0, 1); head.rect().anchorMax = new Vector2(1, 1); head.rect().offsetMin = new Vector2(18, -48); head.rect().offsetMax = new Vector2(-18, -16);
            var mapArt = Art("MAP1-COLD-RELIQUARY-ASHFALL-CANON");
            if (mapArt != null)
            {
                var holder = new GameObject("MapArt");
                holder.transform.SetParent(p.transform, false);
                var img = holder.AddComponent<Image>();
                img.sprite = mapArt; img.preserveAspect = true; img.color = new Color(0.95f, 0.94f, 0.92f, 1f);
                img.rect().anchorMin = new Vector2(0.03f, 0.03f); img.rect().anchorMax = new Vector2(0.97f, 0.90f);
                img.rect().offsetMin = Vector2.zero; img.rect().offsetMax = Vector2.zero;
            }
            else
            {
                var none = Label(p.transform, "THE GATES HAVE NOT DRAWN THIS PATH YET", 13, Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                none.rect().anchorMin = new Vector2(0.1f, 0.4f); none.rect().anchorMax = new Vector2(0.9f, 0.6f);
            }
            mapPins = new System.Collections.Generic.List<GameObject>();
            for (int i = 0; i < MAP_PINS.Length; i++) mapPins.Add(BuildMapPin(p.transform, i));
            // v221 PIN PASS — myth-veiled landmark pins (build-order item 3):
            // discovered sites show their true names; undiscovered ones stay VEILED
            // (knowledge = unlock — the WORLD-AND-DUNGEON-LAW discovery doctrine).
            var sub = Label(p.transform, "QUEST PIN — REACH THE FIRST GATE", 10, Hex(0xa3895a), TextAnchor.MiddleLeft);
            sub.rect().anchorMin = new Vector2(0.03f, 0.0f); sub.rect().anchorMax = new Vector2(0.97f, 0.05f); sub.rect().offsetMin = new Vector2(8, 6); sub.rect().offsetMax = new Vector2(-8, 0);
            var close = Btn(p.transform, "CLOSE", 11);
            var crt = close.rect();
            crt.anchorMin = new Vector2(1, 1); crt.anchorMax = new Vector2(1, 1); crt.pivot = new Vector2(1, 1);
            crt.anchoredPosition = new Vector2(-10, -10); crt.sizeDelta = new Vector2(80, 32);
            close.onClick.AddListener(() => p.SetActive(false));
            p.SetActive(false);
            return p;
        }

        // ================= model =================
        void LoadClass(string cls)
        {
            foreach (var kv in loaded) kv.Value.SetActive(kv.Key == cls);
            if (loaded.ContainsKey(cls)) { BindModel(loaded[cls]); return; }
            var prefab = ModelPrefab(cls);
            if (prefab == null)
            {
                model = null; animator = null;
                if (hudLine != null) hudLine.text = "NO MODEL STAGED — FORGE OUTPUT MISSING (" + cls.ToUpper() + ")";
                return;
            }
            if (hudLine != null) hudLine.text = "LOADING " + prefab.name.ToUpper() + " • FORGE MODEL";
            var inst = Instantiate(prefab, stagePivot);
            inst.transform.localPosition = Vector3.zero;
            loaded[cls] = inst;
            BindModel(inst);
        }

        GameObject ModelPrefab(string cls)
        {
            var exact = Resources.Load<GameObject>(cls + "-GAME");
            if (exact != null) return exact;
            foreach (var go in Resources.LoadAll<GameObject>(""))
                if (go.name.IndexOf("-GAME", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    go.name.IndexOf(cls, StringComparison.OrdinalIgnoreCase) >= 0)
                    return go;
            return null;
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

        // ================= gameplay: waymarks =================
        void MakeWaymark(Vector3 pos, string name)
        {
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = name; pillar.transform.position = pos + new Vector3(0, 0.45f, 0);
            pillar.transform.localScale = new Vector3(0.22f, 0.45f, 0.22f);
            var m = Lit(new Color(0.08f, 0.085f, 0.10f));
            if (m != null)
            {
                if (m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f)); }
                pillar.GetComponent<MeshRenderer>().material = m;
            }
            var coal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            coal.name = name + "Flame"; coal.transform.SetParent(pillar.transform, false);
            coal.transform.localPosition = new Vector3(0, 0.52f, 0); coal.transform.localScale = new Vector3(0.13f, 0.10f, 0.13f);
            var fm = Lit(new Color(1f, 0.62f, 0.25f));
            if (fm != null)
            {
                if (fm.HasProperty("_EmissionColor")) { fm.EnableKeyword("_EMISSION"); fm.SetColor("_EmissionColor", new Color(1.9f, 0.85f, 0.2f)); }
                coal.GetComponent<MeshRenderer>().material = fm;
            }
            var l = new GameObject(name + "Light"); l.transform.position = pos + new Vector3(0, 1.1f, 0);
            var light = l.AddComponent<Light>(); light.type = LightType.Point; light.color = new Color(1f, 0.62f, 0.25f);
            light.range = 4f; light.intensity = 0.9f;
        }

        bool Near(Vector3 a, Vector3 b) { return (a - b).sqrMagnitude < 3.2f; }

        void TryWalkTo(Vector2 screen)
        {
            var ray = cam.ScreenPointToRay(new Vector3(screen.x, screen.y, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 80f) && hit.collider != null && hit.collider.name == "Ground")
            {
                var p = hit.point;
                if (new Vector2(p.x, p.z).sqrMagnitude < 320f) { walkTarget = p; hasWalkTarget = true; }
            }
        }

        // ================= frame: orbit + walk =================
        Vector2 lastTouch0, lastTouch1; bool dragging;
        void Update()
        {
            // v219 raw input probe — counts touches BEFORE any UI raycast is involved
            bool rawTap = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            if (!rawTap && Input.GetMouseButtonDown(0)) rawTap = true;
            if (rawTap) { probeCount++; if (probeLbl != null) probeLbl.text = "INPUT " + probeCount; }
            RunTaps();   // v224: raw-touch router — nav works even if the EventSystem is dead
            // orientation flip: refit plates + relayout ONCE on change (was missing before)
            bool nowPortrait = Screen.height > Screen.width;
            if (nowPortrait != wasPortrait)
            {
                wasPortrait = nowPortrait;
                if (canvas != null) LayoutCards();
            }
            // ================= MOBILE-CONTROL-BIBLE Stage A: left = move, right = look =================
            bool overUI = EventSystem.current != null;
            if (state == State.Game && model != null && storyCard == null)
            {
                EnsureJoystick();
                float leftW = Screen.width * 0.5f;
                // ---- touch: joystick (left half) / camera (right half) / quick-tap = walk order ----
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    bool uiHit = overUI && t.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(t.fingerId);
                    if (uiHit) continue;
                    if (t.phase == TouchPhase.Began)
                    {
                        if (t.position.x < leftW && joyFinger == -1)
                        {
                            joyFinger = t.fingerId; joyOrigin = t.position; joyActive = false; tapStart = t.position; maybeTap = true;
                        }
                        else if (camFinger == -1) { camFinger = t.fingerId; lastTouch0 = t.position; dragging = true; }
                    }
                    else if (t.fingerId == joyFinger)
                    {
                        var d = t.position - joyOrigin;
                        if (!joyActive && d.sqrMagnitude > 480f) { joyActive = true; maybeTap = false; hasWalkTarget = false; }
                        if (joyActive)
                        {
                            joyVec = Vector2.ClampMagnitude(d / 95f, 1f);
                            joyBaseImg.gameObject.SetActive(true);
                            joyBaseImg.rect().anchoredPosition = joyOrigin / canvas.scaleFactor;
                            joyNubImg.rect().anchoredPosition = joyVec * 60f;
                        }
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                        {
                            if (!joyActive && maybeTap) { TryWalkTo(t.position); maybeTap = false; }
                            joyFinger = -1; joyActive = false; joyVec = Vector2.zero;
                            if (joyBaseImg != null) joyBaseImg.gameObject.SetActive(false);
                        }
                    }
                    else if (t.fingerId == camFinger)
                    {
                        if (t.phase == TouchPhase.Moved) { camYaw += (t.position.x - lastTouch0.x) * 0.4f; lastTouch0 = t.position; lastCamDragT = Time.time; }
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) { camFinger = -1; dragging = false; }
                    }
                }
                // ---- mouse fallback (editor/QA): quick click = walk, drag = orbit ----
                if (Input.touchCount == 0)
                {
                    if (Input.GetMouseButtonDown(0) && !(overUI && EventSystem.current.IsPointerOverGameObject())) { maybeTap = true; tapStart = Input.mousePosition; dragging = true; lastTouch0 = Input.mousePosition; }
                    else if (Input.GetMouseButton(0) && maybeTap && (((Vector2)Input.mousePosition) - tapStart).sqrMagnitude > 500f) maybeTap = false;
                    else if (Input.GetMouseButton(0) && dragging) { camYaw += ((Vector2)Input.mousePosition - lastTouch0).x * 0.35f; lastTouch0 = Input.mousePosition; lastCamDragT = Time.time; }
                    else if (Input.GetMouseButtonUp(0)) { if (maybeTap) { TryWalkTo(Input.mousePosition); maybeTap = false; } dragging = false; }
                }
            }
            else
            {
                maybeTap = false; joyFinger = -1; camFinger = -1; joyActive = false; joyVec = Vector2.zero;
                if (joyBaseImg != null) joyBaseImg.gameObject.SetActive(false);
            }

            // pinch zoom (any two fingers) + wheel
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

            // --- walking the route (root-locked walk clip carries the stride; transform carries the travel) ---
            if (state == State.Game && model != null)
            {
                // --- Stage A: floating joystick drives the Sovereign (camera-relative, eased facing) ---
                if (joyActive && joyVec.sqrMagnitude > 0.02f)
                {
                    hasWalkTarget = false;
                    float cyj = Mathf.Cos(camYaw * Mathf.Deg2Rad), syj = Mathf.Sin(camYaw * Mathf.Deg2Rad);
                    var fwd = new Vector3(-syj, 0, -cyj);                                // camera -> player forward
                    var right = Vector3.Cross(fwd, Vector3.up);
                    var dir = Vector3.ClampMagnitude(fwd * joyVec.y + right * joyVec.x, 1f);
                    float spd = walkSpeed * 1.35f * Mathf.Clamp01(dir.magnitude);        // push full = run-read
                    if (dir.sqrMagnitude > 0.02f)
                    {
                        model.transform.position += dir * (spd * Time.deltaTime);
                        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
                    }
                    if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("walk")) animator.CrossFade("walk", 0.2f);
                    // auto-follow: after 3s without manual orbit, the camera eases behind the heading
                    if (Time.time - lastCamDragT > 3f)
                    {
                        float wantYaw = Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg;
                        float dyaw = Mathf.DeltaAngle(camYaw, wantYaw);
                        camYaw += dyaw * Mathf.Clamp01(1.6f * Time.deltaTime);
                    }
                }
                else if (hasWalkTarget)
                {
                    var p = model.transform.position; var d = walkTarget - p; d.y = 0;
                    if (d.magnitude < 0.12f) { hasWalkTarget = false; if (animator) animator.CrossFade("idle", 0.25f); }
                    else
                    {
                        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("walk")) animator.CrossFade("walk", 0.2f);
                        model.transform.position = p + d.normalized * (walkSpeed * Time.deltaTime);
                        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(d), 8f * Time.deltaTime);
                    }
                }
                QuestStations();
            }
            var target = (state == State.Game && model != null)
                ? model.transform.position + new Vector3(0, camDist * 0.30f, 0)
                : new Vector3(0, camDist * 0.30f, 0);
            float cy = Mathf.Cos(camYaw * Mathf.Deg2Rad), sy = Mathf.Sin(camYaw * Mathf.Deg2Rad);
            cam.transform.position = target + new Vector3(sy, 0.15f, cy) * camDist;
            cam.transform.LookAt(target);
        }

        // ================= floating joystick (Stage A) =================
        Sprite MakeDiscSprite(bool ring)
        {
            int N = 128; var tex = new Texture2D(N, N, TextureFormat.RGBA32, false);
            float c = (N - 1) * 0.5f;
            for (int y = 0; y < N; y++) for (int x = 0; x < N; x++)
            {
                float r = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c;
                Color col = new Color(0, 0, 0, 0);
                if (ring)
                {
                    if (r < 0.44f) col = new Color(0.04f, 0.045f, 0.055f, 0.42f);          // dark slate fill
                    else if (r < 0.50f) col = new Color(0.639f, 0.537f, 0.353f, 0.92f);    // bronze lip
                }
                else
                {
                    if (r < 0.42f) col = new Color(0.90f, 0.86f, 0.79f, 0.88f);            // cream nub
                    else if (r < 0.50f) col = new Color(0.639f, 0.537f, 0.353f, 0.95f);    // bronze edge
                }
                tex.SetPixel(x, y, col);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), N / 0.5f * 0.5f);
        }

        void EnsureJoystick()
        {
            if (joyBaseImg != null) return;
            var g = new GameObject("JoyBase");
            g.transform.SetParent(canvas.transform, false);
            joyBaseImg = g.AddComponent<Image>();
            joyBaseImg.sprite = MakeDiscSprite(true); joyBaseImg.raycastTarget = false;
            var brt = joyBaseImg.rect();
            brt.anchorMin = brt.anchorMax = Vector2.zero; // pixel-pos driven
            brt.sizeDelta = new Vector2(190, 190); brt.anchoredPosition = new Vector2(-500, -500);
            var n = new GameObject("JoyNub");
            n.transform.SetParent(g.transform, false);
            joyNubImg = n.AddComponent<Image>();
            joyNubImg.sprite = MakeDiscSprite(false); joyNubImg.raycastTarget = false;
            var nrt = joyNubImg.rect();
            nrt.anchorMin = nrt.anchorMax = new Vector2(0.5f, 0.5f);
            nrt.sizeDelta = new Vector2(78, 78);
            g.SetActive(false);
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
            var cs = go.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            return c;
        }
        Font uiFont;
        Font Font()
        {
            if (uiFont == null)
            {
                uiFont = Resources.Load<Font>("Fonts/Cinzel-Bold");
                if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return uiFont;
        }

        // Canon art cache — Resources/Art staged by the build; null-guarded everywhere.
        readonly Dictionary<string, Sprite> artCache = new Dictionary<string, Sprite>();
        Sprite Art(string name)
        {
            if (artCache.ContainsKey(name)) return artCache[name];
            var tex = Resources.Load<Texture2D>("Art/" + name);
            Sprite s = null;
            if (tex != null) s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.height);
            artCache[name] = s;
            return s;
        }

        // Rounded-rect sprite (runtime-generated) for game-feel buttons.
        Sprite rounded;
        Sprite Rounded()
        {
            if (rounded != null) return rounded;
            int n = 48, r = 10;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var px = new Color32[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = x < r ? r - x : (x >= n - r ? x - (n - 1 - r) : 0);
                    float dy = y < r ? r - y : (y >= n - r ? y - (n - 1 - r) : 0);
                    bool inside = dx <= 0 || dy <= 0 || (dx * dx + dy * dy) <= r * r;
                    px[y * n + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                }
            tex.SetPixels32(px); tex.Apply();
            rounded = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n, 0, SpriteMeshType.FullRect);
            return rounded;
        }
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
            var img = go.AddComponent<Image>();
            img.sprite = Rounded();
            img.color = new Color(0.10f, 0.095f, 0.075f, 0.92f);
            var imgRt = img.rect(); imgRt.offsetMin = new Vector2(4, 4); imgRt.offsetMax = new Vector2(-4, -4);
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
        public static RectTransform rect(this GameObject go) { return go.transform as RectTransform; }
        public static RectTransform rect(this Component c) { return c.transform as RectTransform; }
        public static void Stretch(this Transform t)
        { var r = t as RectTransform; if (r != null) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero; } }
        public static void Stretch(this RectTransform r, Transform parent)
        { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero; r.transform.SetParent(parent, true); }
        public static RectTransform Stretch(this Text t)
        { var r = t.rect(); r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero; return r; }
    }
}GameObject BuildTitle(Transform parent)
        {
            // v226: the UI-MAIN-MENU-CANON plate's style + LAYOUT on the v224 real-UI chassis.
            // AVALON hero at the plate's 7-11% band, carved option slats at the plate's carved-list
            // positions (CONTINUE 58 / NEW JOURNEY 65 / GATES 72 / ACHIEVEMENTS 78 / SETTINGS 85,
            // top-fraction), stone shelf behind the list, bronze engraved labels. Fully real,
            // fully dual-wired — the plate is the style reference, never the screen.
            var p = Panel(parent, "Title", new Color(0.055f, 0.078f, 0.090f, 1f));
            p.transform.Stretch();

            var bgSprite = Art("CINEMATIC-TEASER-KEYART-CANON");
            if (bgSprite != null)
            {
                var bg = new GameObject("KeyArt");
                bg.transform.SetParent(p.transform, false);
                var bi = bg.AddComponent<Image>();
                bi.sprite = bgSprite; bi.preserveAspect = true; bi.color = new Color(0.45f, 0.44f, 0.43f, 1f);
                bi.raycastTarget = false;
                bi.rect().Stretch();
                var shade = Panel(p.transform, "Shade", new Color(0.04f, 0.055f, 0.065f, 0.66f));
                shade.transform.Stretch();
                shade.GetComponent<Image>().raycastTarget = false;
            }

            // AVALON hero — plate band 6.6-11.3% (anchor y = 1 - 0.115 .. 1 - 0.066)
            var title = Label(p.transform, "AVALON", 72, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            title.rect().anchorMin = new Vector2(0, 0.885f); title.rect().anchorMax = new Vector2(1, 0.985f);
            // rule + sub — plate band 17.6-22.7%
            var ruleCol = Hex(0xa3895a); ruleCol.a = 0.85f;
            var rule = Panel(p.transform, "Rule", ruleCol);
            rule.GetComponent<Image>().raycastTarget = false;
            var rrt = rule.rect();
            rrt.anchorMin = new Vector2(0.30f, 0.815f); rrt.anchorMax = new Vector2(0.70f, 0.819f); rrt.offsetMin = Vector2.zero; rrt.offsetMax = Vector2.zero;
            var sub = Label(p.transform, "THE WAKING GATES", 18, Hex(0xa3895a), TextAnchor.MiddleCenter);
            sub.rect().anchorMin = new Vector2(0, 0.77f); sub.rect().anchorMax = new Vector2(1, 0.815f);

            // stone shelf behind the carved list (plate's lower-third band)
            var shelfEdge = Panel(p.transform, "ShelfEdge", new Color(0.42f, 0.34f, 0.21f, 0.55f));
            var sert = shelfEdge.rect();
            sert.anchorMin = new Vector2(0.235f, 0.085f); sert.anchorMax = new Vector2(0.765f, 0.475f); sert.offsetMin = Vector2.zero; sert.offsetMax = Vector2.zero;
            shelfEdge.GetComponent<Image>().raycastTarget = false;
            var shelf = Panel(shelfEdge.transform, "ShelfFace", new Color(0.055f, 0.075f, 0.086f, 0.88f));
            shelf.transform.Stretch();
            var shrt = shelf.rect();
            shrt.anchorMin = new Vector2(0.012f, 0.03f); shrt.anchorMax = new Vector2(0.988f, 0.97f); shrt.offsetMin = Vector2.zero; shrt.offsetMax = Vector2.zero;
            shelf.GetComponent<Image>().raycastTarget = false;

            bool hasSave = PlayerPrefs.HasKey("avalon.save");
            // carved slats at the plate's own option positions (top-fraction -> anchor)
            CarvedBtn(p.transform, "CONTINUE", 0.30f, 0.70f, 0.415f, 0.465f, hasSave, delegate {
                if (LoadSave())
                {
                    LoadClass(chosen.name); UpdateQuestLine();
                    if (faithUnlocked)
                    {
                        if (beliefFill != null) beliefFill.anchorMax = new Vector2(0.60f, 0.75f);
                        if (beliefLbl != null) beliefLbl.text = "BELIEF \u2014 ALIT";
                    }
                    SetState(State.Game);
                }
            });
            CarvedBtn(p.transform, "NEW JOURNEY", 0.30f, 0.70f, 0.345f, 0.395f, true, delegate { SetState(State.Select); });
            CarvedBtn(p.transform, "GATES", 0.30f, 0.70f, 0.275f, 0.325f, true, delegate { OpenMap(); });
            CarvedBtn(p.transform, "ACHIEVEMENTS", 0.30f, 0.70f, 0.205f, 0.255f, true, delegate { panelAchievements.SetActive(true); });
            CarvedBtn(p.transform, "SETTINGS", 0.30f, 0.70f, 0.135f, 0.185f, true, delegate { panelSettings.SetActive(true); });

#if UNITY_ANDROID && !UNITY_EDITOR
            int stampVc = InstalledVersionCode();
#else
            int stampVc = 0;
#endif
            var stampTxt = Label(p.transform, "SHELL v" + stampVc + " \u2022 " + System.DateTime.Now.ToString("MMM d"), 11, new Color(0.72f, 0.66f, 0.55f, 0.85f), TextAnchor.UpperRight);
            stampTxt.rect().anchorMin = new Vector2(0.78f, 0.012f); stampTxt.rect().anchorMax = new Vector2(0.995f, 0.030f);
            var ver = Label(p.transform, "V 0.2.11  \u00a9 2026 WAKING GATES, INC. ALL RIGHTS RESERVED.", 9, Hex(0x8a8578), TextAnchor.MiddleCenter);
            ver.rect().anchorMin = new Vector2(0, 0.005f); ver.rect().anchorMax = new Vector2(1, 0.04f);
            return p;
        }GameObject BuildSelect(Transform parent)
        {
            // v226: the UI-CLASS-SELECT-CANON plate's style + LAYOUT on the real-UI chassis.
            // AVALON overline at the plate's 2% band, CHOOSE YOUR CLASS at 7-9%, class cards as
            // carved stone niches (bronze-framed, art inside, bronze name band), BEGIN + BACK as
            // carved slabs at the plate's 88-93% band. Responsive grid + dual wiring untouched.
            var p = Panel(parent, "Select", new Color(0.055f, 0.078f, 0.090f, 1f));
            p.transform.Stretch();
            var overline = Label(p.transform, "AVALON", 16, Hex(0xa3895a), TextAnchor.MiddleCenter);
            overline.rect().anchorMin = new Vector2(0, 0.965f); overline.rect().anchorMax = new Vector2(1, 1.02f);
            var head = Label(p.transform, "CHOOSE YOUR CLASS", 30, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            head.rect().anchorMin = new Vector2(0, 0.875f); head.rect().anchorMax = new Vector2(1, 0.962f);

            for (int i = 0; i < CLASSES.Length; i++)
            {
                var cd = CLASSES[i];
                bool unlocked = ModelPrefab(cd.name) != null;
                // carved niche: bronze frame + stone face
                var niche = Panel(p.transform, "card-" + cd.name, new Color(0.42f, 0.34f, 0.21f, 0.80f));
                var crt = niche.rect();
                cardRects.Add(crt);
                var face = Panel(niche.transform, "Face", new Color(0.078f, 0.105f, 0.121f, 0.94f));
                var frt = face.rect();
                frt.anchorMin = new Vector2(0.018f, 0.018f); frt.anchorMax = new Vector2(0.982f, 0.982f); frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;
                var art = Art("CLASS-" + cd.name.ToUpper() + "-CANON");
                if (art != null)
                {
                    var ai = new GameObject("art");
                    ai.transform.SetParent(face.transform, false);
                    var img = ai.AddComponent<Image>();
                    img.sprite = art; img.preserveAspect = true; img.color = unlocked ? new Color(0.92f, 0.90f, 0.86f, 1f) : new Color(0.45f, 0.45f, 0.45f, 0.55f);
                    img.rect().Stretch();
                    var band = Panel(face.transform, "Band", new Color(0.043f, 0.055f, 0.063f, 0.88f));
                    var bandRt = band.rect();
                    bandRt.anchorMin = new Vector2(0, 0); bandRt.anchorMax = new Vector2(1, 0.40f); bandRt.offsetMin = Vector2.zero; bandRt.offsetMax = Vector2.zero;
                }
                var nm = Label(niche.transform, cd.name.ToUpper(), 15, unlocked ? Hex(0xdcc38a) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                nm.rect().anchorMin = new Vector2(0, 0.22f); nm.rect().anchorMax = new Vector2(1, 0.38f);
                var ro = Label(niche.transform, cd.role, 9, Hex(0xa3895a), TextAnchor.MiddleCenter);
                ro.rect().anchorMin = new Vector2(0, 0.14f); ro.rect().anchorMax = new Vector2(1, 0.22f);
                var st = Label(niche.transform, unlocked ? "FORGED" : "IN THE FORGE", 9, unlocked ? Hex(0xa3895a) : Hex(0x6f6a5e), TextAnchor.MiddleCenter);
                st.rect().anchorMin = new Vector2(0, 0.04f); st.rect().anchorMax = new Vector2(1, 0.12f);
                var b = niche.AddComponent<Button>(); b.targetGraphic = face;
                var cb = b.colors; cb.highlightedColor = new Color(1.12f, 1.08f, 0.9f, 1); cb.pressedColor = new Color(0.75f, 0.65f, 0.42f, 1);
                b.colors = cb;
                cardTints.Add(face.GetComponent<Image>());
                var captured = cd;
                b.onClick.AddListener(() => SelectCard(captured));
                TapTo(crt, () => SelectCard(captured));
            }

            // plate band 87.5-93.4%: BEGIN THE WAKENING + BACK as carved slabs
            CarvedBtn(p.transform, "BEGIN THE WAKENING", 0.26f, 0.62f, 0.065f, 0.125f, true, delegate { SetState(State.Game); });
            CarvedBtn(p.transform, "BACK", 0.04f, 0.24f, 0.065f, 0.125f, true, delegate { SetState(State.Title); });
            return p;
        }GameObject BuildTitle(Transform parent)
        {
            // v226: the UI-MAIN-MENU-CANON plate's style + LAYOUT on the v224 real-UI chassis.
            // AVALON hero at the plate's 7-11% band, carved option slats at the plate's carved-list
            // positions (CONTINUE 58 / NEW JOURNEY 65 / GATES 72 / ACHIEVEMENTS 78 / SETTINGS 85,
            // top-fraction), stone shelf behind the list, bronze engraved labels. Fully real,
            // fully dual-wired — the plate is the style reference, never the screen.
            var p = Panel(parent, "Title", new Color(0.055f, 0.078f, 0.090f, 1f));
            p.transform.Stretch();

            var bgSprite = Art("CINEMATIC-TEASER-KEYART-CANON");
            if (bgSprite != null)
            {
                var bg = new GameObject("KeyArt");
                bg.transform.SetParent(p.transform, false);
                var bi = bg.AddComponent<Image>();
                bi.sprite = bgSprite; bi.preserveAspect = true; bi.color = new Color(0.45f, 0.44f, 0.43f, 1f);
                bi.raycastTarget = false;
                bi.rect().Stretch();
                var shade = Panel(p.transform, "Shade", new Color(0.04f, 0.055f, 0.065f, 0.66f));
                shade.transform.Stretch();
                shade.GetComponent<Image>().raycastTarget = false;
            }

            // AVALON hero — plate band 6.6-11.3% (anchor y = 1 - 0.115 .. 1 - 0.066)
            var title = Label(p.transform, "AVALON", 72, Hex(0xf0e6cf), TextAnchor.MiddleCenter);
            title.rect().anchorMin = new Vector2(0, 0.885f); title.rect().anchorMax = new Vector2(1, 0.985f);
            // rule + sub — plate band 17.6-22.7%
            var ruleCol = Hex(0xa3895a); ruleCol.a = 0.85f;
            var rule = Panel(p.transform, "Rule", ruleCol);
            rule.GetComponent<Image>().raycastTarget = false;
            var rrt = rule.rect();
            rrt.anchorMin = new Vector2(0.30f, 0.815f); rrt.anchorMax = new Vector2(0.70f, 0.819f); rrt.offsetMin = Vector2.zero; rrt.offsetMax = Vector2.zero;
            var sub = Label(p.transform, "THE WAKING GATES", 18, Hex(0xa3895a), TextAnchor.MiddleCenter);
            sub.rect().anchorMin = new Vector2(0, 0.77f); sub.rect().anchorMax = new Vector2(1, 0.815f);

            // stone shelf behind the carved list (plate's lower-third band)
            var shelfEdge = Panel(p.transform, "ShelfEdge", new Color(0.42f, 0.34f, 0.21f, 0.55f));
            var sert = shelfEdge.rect();
            sert.anchorMin = new Vector2(0.235f, 0.085f); sert.anchorMax = new Vector2(0.765f, 0.475f); sert.offsetMin = Vector2.zero; sert.offsetMax = Vector2.zero;
            shelfEdge.GetComponent<Image>().raycastTarget = false;
            var shelf = Panel(shelfEdge.transform, "ShelfFace", new Color(0.055f, 0.075f, 0.086f, 0.88f));
            shelf.transform.Stretch();
            var shrt = shelf.rect();
            shrt.anchorMin = new Vector2(0.012f, 0.03f); shrt.anchorMax = new Vector2(0.988f, 0.97f); shrt.offsetMin = Vector2.zero; shrt.offsetMax = Vector2.zero;
            shelf.GetComponent<Image>().raycastTarget = false;

            bool hasSave = PlayerPrefs.HasKey("avalon.save");
            // carved slats at the plate's own option positions (top-fraction -> anchor)
            CarvedBtn(p.transform, "CONTINUE", 0.30f, 0.70f, 0.415f, 0.465f, hasSave, delegate {
                if (LoadSave())
                {
                    LoadClass(chosen.name); UpdateQuestLine();
                    if (faithUnlocked)
                    {
                        if (beliefFill != null) beliefFill.anchorMax = new Vector2(0.60f, 0.75f);
                        if (beliefLbl != null) beliefLbl.text = "BELIEF \u2014 ALIT";
                    }
                    SetState(State.Game);
                }
            });
            CarvedBtn(p.transform, "NEW JOURNEY", 0.30f, 0.70f, 0.345f, 0.395f, true, delegate { SetState(State.Select); });
            CarvedBtn(p.transform, "GATES", 0.30f, 0.70f, 0.275f, 0.325f, true, delegate { OpenMap(); });
            CarvedBtn(p.transform, "ACHIEVEMENTS", 0.30f, 0.70f, 0.205f, 0.255f, true, delegate { panelAchievements.SetActive(true); });
            CarvedBtn(p.transform, "SETTINGS", 0.30f, 0.70f, 0.135f, 0.185f, true, delegate { panelSettings.SetActive(true); });

#if UNITY_ANDROID && !UNITY_EDITOR
            int stampVc = InstalledVersionCode();
#else
            int stampVc = 0;
#endif
            var stampTxt = Label(p.transform, "SHELL v" + stampVc + " \u2022 " + System.DateTime.Now.ToString("MMM d"), 11, new Color(0.72f, 0.66f, 0.55f, 0.85f), TextAnchor.UpperRight);
            stampTxt.rect().anchorMin = new Vector2(0.78f, 0.012f); stampTxt.rect().anchorMax = new Vector2(0.995f, 0.030f);
            var ver = Label(p.transform, "V 0.2.11  \u00a9 2026 WAKING GATES, INC. ALL RIGHTS RESERVED.", 9, Hex(0x8a8578), TextAnchor.MiddleCenter);
            ver.rect().anchorMin = new Vector2(0, 0.005f); ver.rect().anchorMax = new Vector2(1, 0.04f);
            return p;
        }
