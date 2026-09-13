// AVALON CLEAN-SLATE BUILD — stage 1b: BARE + BOOT CANARY (diagnostic)
// Same bare scene as stage 1 (zero game code) but the launcher is a PURE JAVA
// canary activity that: (1) proves the Android layer boots, (2) starts the
// Unity engine on demand, (3) traps any Java crash and shows the stack on
// screen for a screenshot, (4) infers native crashes via launch stamps.
// Why: v201 bare APK crashed on launch on SM-S908U while the old Java-shell
// APK ran fine → the fault is in the ENGINE layer; this build captures the
// exact cause without logcat access.
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

namespace AvalonShell
{
    public static class BareBuild
    {
        public static void Run()
        {
            int vcode = 201;
            int.TryParse(Environment.GetEnvironmentVariable("AVALON_VCODE") ?? "201", out vcode);
            int stage = 1;
            int.TryParse(Environment.GetEnvironmentVariable("AVALON_STAGE") ?? "1", out stage);
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

            // ---- Scene: stage 1 = bare, stage 2 = lit 3D scene (engine proof) ----
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            string scenePath = "Assets/Scenes/Bare.unity";
            Directory.CreateDirectory("Assets/Scenes");
            if (stage >= 2)
            {
                BuildStageScene(scene);
                scenePath = "Assets/Scenes/Stage2.unity";
                Debug.Log("[BARE] stage 2: 3D scene built (camera + ember cube + Spin + label)");
            }
            if (stage >= 3)
            {
                BuildHud();
                scenePath = "Assets/Scenes/Stage3.unity";
                Debug.Log("[BARE] stage 3: HUD canvas layered over the 3D scene");
            }
            EditorSceneManager.SaveScene(scene, scenePath);

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath)));
            var report = BuildPipeline.BuildPlayer(new[] { scenePath }, outPath, BuildTarget.Android, BuildOptions.None);
            var s = report.summary;
            Debug.Log($"[BARE] result={s.result} size={s.totalSize} errors={s.totalErrors}");
            if (s.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                EditorApplication.Exit(1);
            EditorApplication.Exit(0);
        }

        // Stage 2: a real lit 3D scene. Proves engine + renderer + run loop:
        // Big taps RUN UNITY in the canary and should see a slowly rotating
        // amber cube (fire-color law) on a dark void with a status label.
        static void BuildStageScene(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var cam = root.GetComponent<Camera>();
                if (cam != null)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0.04f, 0.04f, 0.06f); // cold dark void
                    cam.transform.position = new Vector3(0f, 1.2f, 4f);
                    cam.transform.LookAt(Vector3.zero);
                }
                var light = root.GetComponent<Light>();
                if (light != null)
                {
                    light.type = LightType.Directional;
                    light.intensity = 1.1f;
                    light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                }
            }

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "EmberCube";
            cube.transform.position = Vector3.zero;
            cube.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            var shader = Shader.Find("Standard");
            var mat = new Material(shader);
            mat.color = new Color(1.0f, 0.45f, 0.10f); // amber-orange mortal fire
            cube.GetComponent<Renderer>().sharedMaterial = mat;
            cube.AddComponent<Spin>();

            var labelObj = new GameObject("Label");
            labelObj.transform.position = new Vector3(0f, 1.9f, 0f);
            var label = labelObj.AddComponent<TextMesh>();
            label.text = "AVALON STAGE 2 - ENGINE LIVE";
            label.characterSize = 0.35f;
            label.fontSize = 48;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = new Color(1f, 0.85f, 0.55f);
            var lr = labelObj.GetComponent<MeshRenderer>();
            if (lr == null) labelObj.AddComponent<MeshRenderer>();
        }

        // Stage 3: ugui HUD overlay over the 3D scene. Proves the Canvas layer
        // renders + scales in both orientations (AutoRotation already set).
        // Amber HUD on cold void - matches the AVALON shell law.
        static void BuildHud()
        {
            var canvasObj = new GameObject("HudCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvasObj.AddComponent<GraphicRaycaster>();

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var amber = new Color(1f, 0.72f, 0.35f);
            var dimAmber = new Color(0.85f, 0.55f, 0.25f);

            var top = MakeHudText(canvasObj.transform, "AVALON // HUD ONLINE", font, 56, amber);
            var rt = top.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f); rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f); rt.anchoredPosition = new Vector2(0f, -90f);
            rt.sizeDelta = new Vector2(1000f, 80f);

            var bottom = MakeHudText(canvasObj.transform, "STAGE 3 OF 4 - ENGINE + HUD", font, 42, dimAmber);
            var rb = bottom.GetComponent<RectTransform>();
            rb.anchorMin = new Vector2(0.5f, 0f); rb.anchorMax = new Vector2(0.5f, 0f);
            rb.pivot = new Vector2(0.5f, 0f); rb.anchoredPosition = new Vector2(0f, 120f);
            rb.sizeDelta = new Vector2(1000f, 70f);
        }

        static Text MakeHudText(Transform parent, string text, Font font, int size, Color color)
        {
            var go = new GameObject(text);
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = font;
            t.fontSize = size;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.text = text;
            return t;
        }
    }

    // Injects the BootCanary source + launcher manifest patch into the gradle
    // project after Unity generates it, before gradle assembles the APK.
    public class CanaryInjector : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 1000; } }

        public void OnPostGenerateGradleAndroidProject(string gradlePath)
        {
            // DIAGNOSTIC BUILDS ONLY. The canary hijacks MAIN/LAUNCHER — that is correct for
            // clean-slate stage builds (AVALON_STAGE set) but WRONG for the real shell
            // (AvalonShellBuild), which must open straight into the game via the stock
            // UnityPlayerGameActivity (v208 lesson: canary leaked into the final APK).
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AVALON_STAGE")))
            {
                Debug.Log("[CANARY] AVALON_STAGE not set — real build, canary injection SKIPPED");
                return;
            }
            string act = "com.unity3d.player.UnityPlayerGameActivity";

            string[] manifests = Directory.GetFiles(gradlePath, "AndroidManifest.xml", SearchOption.AllDirectories);
            foreach (var mf in manifests)
            {
                string xml = File.ReadAllText(mf);
                if (!xml.Contains(act)) continue;

                string patched = StripLauncherIntentFilter(xml, act);
                patched = AddCanaryActivity(patched);

                File.WriteAllText(mf, patched);
                Debug.Log($"[CANARY] patched manifest: {mf}");

                // drop the Java source into the module that owns the manifest (src/main/java)
                string moduleMain = Directory.GetParent(mf).FullName; // .../src/main
                string javaDir = Path.Combine(moduleMain, "java", "com", "bigfoot404", "avalon");
                Directory.CreateDirectory(javaDir);
                string javaPath = Path.Combine(javaDir, "BootCanaryActivity.java");
                File.WriteAllText(javaPath, CanarySource);
                Debug.Log($"[CANARY] injected: {javaPath}");
            }
        }

        // Removes every <intent-filter> that contains android.intent.action.MAIN
        // inside the <activity ...UnityPlayerGameActivity...> element.
        private string StripLauncherIntentFilter(string xml, string activityName)
        {
            int aIdx = xml.IndexOf("android:name=\"" + activityName + "\"");
            if (aIdx < 0) return xml;
            int start = xml.LastIndexOf("<activity", aIdx, StringComparison.Ordinal);
            int end = xml.IndexOf("</activity>", aIdx, StringComparison.Ordinal);
            if (start < 0 || end < 0) return xml;
            string head = xml.Substring(0, start);
            string body = xml.Substring(start, end + "</activity>".Length - start);
            string tail = xml.Substring(end + "</activity>".Length);

            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (true)
            {
                int f = body.IndexOf("<intent-filter", i, StringComparison.Ordinal);
                if (f < 0) { sb.Append(body.Substring(i)); break; }
                int fe = body.IndexOf("</intent-filter>", f, StringComparison.Ordinal);
                if (fe < 0) { sb.Append(body.Substring(i)); break; }
                string block = body.Substring(f, fe + "</intent-filter>".Length - f);
                sb.Append(body.Substring(i, f - i));
                if (!block.Contains("android.intent.action.MAIN"))
                    sb.Append(block); // keep non-launcher filters
                i = fe + "</intent-filter>".Length;
            }
            return head + sb.ToString() + tail;
        }

        private string AddCanaryActivity(string xml)
        {
            if (xml.Contains("BootCanaryActivity")) return xml;
            string block =
                "<activity android:name=\"com.bigfoot404.avalon.BootCanaryActivity\" " +
                "android:exported=\"true\" " +
                "android:theme=\"@android:style/Theme.Black.NoTitleBar.Fullscreen\" " +
                "android:screenOrientation=\"fullSensor\">" +
                "<intent-filter>" +
                "<action android:name=\"android.intent.action.MAIN\"/>" +
                "<category android:name=\"android.intent.category.LAUNCHER\"/>" +
                "</intent-filter>" +
                "</activity>";
            int appEnd = xml.LastIndexOf("</application>");
            if (appEnd < 0) return xml;
            return xml.Substring(0, appEnd) + block + xml.Substring(appEnd);
        }

        private const string CanarySource = @"
package com.bigfoot404.avalon;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Typeface;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.ScrollView;
import android.widget.TextView;
import java.io.File;
import java.io.FileWriter;
import java.io.FileInputStream;
import java.text.SimpleDateFormat;
import java.util.Date;

public class BootCanaryActivity extends Activity {
    private static final String CRASH_FILE = ""unity-crash.txt"";
    private static final String LAUNCH_FILE = ""unity-launch.txt"";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Thread.setDefaultUncaughtExceptionHandler(new Thread.UncaughtExceptionHandler() {
            @Override public void uncaughtException(Thread t, Throwable e) {
                try {
                    File f = new File(getExternalFilesDir(null), CRASH_FILE);
                    FileWriter w = new FileWriter(f, false);
                    w.write(""TIME: "" + new Date() + ""\nTHREAD: "" + t + ""\n\n"");
                    w.write(e.toString() + ""\n\n"");
                    Throwable r = e;
                    while (r != null) {
                        for (StackTraceElement el : r.getStackTrace()) w.write(""    at "" + el + ""\n"");
                        r = r.getCause();
                        if (r != null) w.write(""CAUSED BY: "" + r + ""\n"");
                    }
                    w.close();
                } catch (Throwable ignored) {}
                try {
                    Intent i = new Intent(BootCanaryActivity.this, BootCanaryActivity.class);
                    i.putExtra(""show_crash"", true);
                    i.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                    startActivity(i);
                } catch (Throwable ignored) {}
                Runtime.getRuntime().exit(10);
            }
        });

        if (getIntent() != null && getIntent().getBooleanExtra(""show_crash"", false)) {
            setContentView(crashScreen());
        } else {
            setContentView(mainScreen());
        }
    }

    private View mainScreen() {
        LinearLayout root = panel();
        root.addView(label(""AVALON"", 34, Color.WHITE, true));
        root.addView(label(""BASE OK — Android layer healthy"", 17, 0xFF7CFC9B, true));
        root.addView(label(deviceInfo(), 12, 0xFFAAAAAA, false));
        root.addView(gap(28));
        Button start = button(""START UNITY ENGINE →"");
        start.setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) {
                writeStamp(LAUNCH_FILE);
                try {
                    Class<?> unity = Class.forName(""com.unity3d.player.UnityPlayerGameActivity"");
                    Intent i = new Intent(BootCanaryActivity.this, unity);
                    startActivity(i);
                } catch (Throwable e) {
                    statusScreen(""Could not even load the Unity activity class:\n\n"" + e);
                }
            }
        });
        root.addView(start);
        root.addView(gap(12));
        Button view = button(""VIEW LAST CRASH / STATUS"");
        view.setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) { setContentView(statusScreen()); }
        });
        root.addView(view);
        root.addView(gap(20));
        root.addView(label(""If the app dies after START, reopen it and tap VIEW LAST CRASH"", 11, 0xFF888888, false));
        ScrollView sc = new ScrollView(this);
        sc.setFillViewport(true);
        sc.addView(root);
        return sc;
    }

    private View crashScreen() {
        LinearLayout root = panel();
        root.addView(label(""UNITY CRASH CAPTURED"", 20, 0xFFFF6B6B, true));
        root.addView(label(""SCREENSHOT THIS WHOLE SCREEN AND SEND IT TO THE BOT"", 13, Color.WHITE, true));
        root.addView(gap(8));
        String txt = read(CRASH_FILE, ""(no crash file text)"");
        root.addView(label(txt, 11, 0xFFDDDDDD, false));
        root.addView(gap(12));
        Button back = button(""← BACK"");
        back.setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) { setContentView(mainScreen()); }
        });
        root.addView(back);
        ScrollView sc = new ScrollView(this);
        sc.setFillViewport(true);
        sc.addView(root);
        return sc;
    }

    private View statusScreen() { return statusScreen(null); }

    private View statusScreen(String preface) {
        LinearLayout root = panel();
        if (preface != null) {
            root.addView(label(preface, 14, 0xFFDDDDDD, false));
            root.addView(gap(8));
        }
        String crash = read(CRASH_FILE, null);
        if (crash != null) {
            root.addView(label(""JAVA CRASH ON FILE:"", 16, 0xFFFF6B6B, true));
            root.addView(gap(6));
            root.addView(label(crash, 11, 0xFFDDDDDD, false));
            root.addView(gap(6));
            root.addView(label(""SCREENSHOT THIS AND SEND IT TO THE BOT"", 13, Color.WHITE, true));
        } else {
            String stamp = read(LAUNCH_FILE, null);
            if (stamp != null) {
                root.addView(label(""Unity was started at "" + stamp + "" and died with NO Java crash."", 15, 0xFFFFB84D, true));
                root.addView(gap(6));
                root.addView(label(""That means a NATIVE engine crash (libunity/libil2cpp). SCREENSHOT THIS AND SEND IT TO THE BOT."", 13, Color.WHITE, true));
            } else {
                root.addView(label(""No crash data yet. Tap START UNITY ENGINE and see what happens."", 14, 0xFFAAAAAA, false));
            }
        }
        root.addView(gap(12));
        Button back = button(""← BACK"");
        back.setOnClickListener(new View.OnClickListener() {
            @Override public void onClick(View v) { setContentView(mainScreen()); }
        });
        root.addView(back);
        ScrollView sc = new ScrollView(this);
        sc.setFillViewport(true);
        sc.addView(root);
        return sc;
    }

    // ---- helpers ----
    private LinearLayout panel() {
        LinearLayout p = new LinearLayout(this);
        p.setOrientation(LinearLayout.VERTICAL);
        p.setBackgroundColor(Color.rgb(8, 10, 14));
        p.setPadding(40, 60, 40, 40);
        return p;
    }
    private TextView label(String t, float sp, int color, boolean bold) {
        TextView tv = new TextView(this);
        tv.setText(t);
        tv.setTextSize(sp);
        tv.setTextColor(color);
        if (bold) tv.setTypeface(Typeface.DEFAULT_BOLD);
        tv.setLineSpacing(2f, 1f);
        return tv;
    }
    private Button button(String t) {
        Button b = new Button(this);
        b.setText(t);
        b.setTextSize(16);
        return b;
    }
    private View gap(int px) {
        View v = new View(this);
        v.setLayoutParams(new LinearLayout.LayoutParams(1, px));
        return v;
    }
    private String deviceInfo() {
        String abi = Build.SUPPORTED_ABIS != null && Build.SUPPORTED_ABIS.length > 0 ? Build.SUPPORTED_ABIS[0] : ""?"";
        return Build.MANUFACTURER + "" "" + Build.MODEL + "" — Android "" + Build.VERSION.RELEASE
            + "" (API "" + Build.VERSION.SDK_INT + "")\n""
            + ""ABI: "" + abi + "" | Device: "" + Build.DEVICE;
    }
    private void writeStamp(String name) {
        try {
            File f = new File(getExternalFilesDir(null), name);
            FileWriter w = new FileWriter(f, false);
            w.write(new SimpleDateFormat(""HH:mm:ss"").format(new Date()));
            w.close();
        } catch (Throwable ignored) {}
    }
    private String read(String name, String fallback) {
        try {
            File f = new File(getExternalFilesDir(null), name);
            if (!f.exists()) return fallback;
            byte[] b = new byte[(int) f.length()];
            FileInputStream in = new FileInputStream(f);
            int n = in.read(b);
            in.close();
            return new String(b, 0, n).trim();
        } catch (Throwable t) { return fallback; }
    }
}
";
    }
}
