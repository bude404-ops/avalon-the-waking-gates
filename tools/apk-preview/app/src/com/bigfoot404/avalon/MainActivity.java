package com.bigfoot404.avalon;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.view.WindowManager;
import android.webkit.WebResourceRequest;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import org.json.JSONArray;
import org.json.JSONObject;

public class MainActivity extends Activity {
    // Live content root (hot-update layer). Files listed in live/content.json
    // get mirrored into filesDir/live and served ahead of bundled assets.
    static final String LIVE_BASE =
        "https://mcontwitter-glitch.github.io/avalon-3d-viewer/live/";
    static final long RECHECK_MS = 10 * 60 * 1000L; // recheck at most every 10 min

    private WebView webView;
    private File liveDir;
    private SharedPreferences prefs;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        prefs = getSharedPreferences("live", Context.MODE_PRIVATE);
        liveDir = new File(getFilesDir(), "live");
        liveDir.mkdirs();
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                             WindowManager.LayoutParams.FLAG_FULLSCREEN);
        webView = new WebView(this);
        WebSettings s = webView.getSettings();
        s.setJavaScriptEnabled(true);
        s.setDomStorageEnabled(true);
        s.setAllowFileAccess(true);
        s.setMediaPlaybackRequiresUserGesture(false);
        s.setLoadWithOverviewMode(true);
        s.setUseWideViewPort(true);
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view, WebResourceRequest request) {
                String path = request.getUrl().getPath();
                if (path != null && path.startsWith("/")) path = path.substring(1);
                if (path == null || path.isEmpty()) path = "index.html";
                // 1) live hot-update layer (freshest)
                File lf = new File(liveDir, path);
                if (lf.isFile()) {
                    try {
                        return new WebResourceResponse(mimeFor(path), null,
                                new FileInputStream(lf));
                    } catch (IOException e) { /* fall through */ }
                }
                // 2) bundled assets: www/ layout, then flat
                for (String p : new String[]{"www/" + path, path}) {
                    try {
                        return new WebResourceResponse(mimeFor(path), null, getAssets().open(p));
                    } catch (IOException e) { /* next */ }
                }
                return null;
            }
        });
        webView.setBackgroundColor(0xFF0A0B0D);
        setContentView(webView);
        String html = readFirstAsset(new String[]{"www/index.html", "index.html"});
        webView.loadDataWithBaseURL("https://avalon.local/", html, "text/html", "utf-8", null);
        hideSystemUI();
        new Thread(this::checkLive).start();
    }

    @Override
    protected void onStart() {
        super.onStart();
        long last = prefs.getLong("last_check", 0);
        if (System.currentTimeMillis() - last > RECHECK_MS) {
            new Thread(this::checkLive).start();
        }
    }

    /** Pulls content.json: silently hot-syncs listed files; one-tap installs new shells. */
    private void checkLive() {
        prefs.edit().putLong("last_check", System.currentTimeMillis()).apply();
        try {
            JSONObject cfg = httpJson(LIVE_BASE + "content.json");
            if (cfg == null) return;

            // ---- content hot-sync (silent, no install) ----
            String remoteVer = cfg.optString("content_version", "");
            String localVer = prefs.getString("content_version", "");
            if (!remoteVer.isEmpty() && !remoteVer.equals(localVer)) {
                JSONArray files = cfg.optJSONArray("files");
                boolean ok = true;
                if (files != null) {
                    for (int i = 0; i < files.length(); i++) {
                        String f = files.optString(i, "");
                        if (f.isEmpty() || f.contains("..")) continue;
                        if (!httpToFile(LIVE_BASE + f, new File(liveDir, f))) { ok = false; break; }
                    }
                }
                if (ok) {
                    prefs.edit().putString("content_version", remoteVer).apply();
                    runOnUiThread(() -> webView.reload());
                }
            }

            // ---- shell (APK) self-update: one tap ----
            JSONObject apk = cfg.optJSONObject("apk");
            if (apk != null) {
                int remoteCode = apk.optInt("versionCode", 0);
                String url = apk.optString("url", LIVE_BASE + "AVALON-WAKING-GATES-TEST-LATEST.apk");
                PackageInfo info = getPackageManager().getPackageInfo(getPackageName(), 0);
                int myCode = info.versionCode; // sufficient: versionCodes stay < 2^31
                if (remoteCode > myCode && !url.isEmpty()) {
                    File apkFile = new File(getFilesDir(), "avalon-update.apk");
                    if (httpToFile(url, apkFile)) {
                        Intent i = new Intent(Intent.ACTION_VIEW);
                        i.setDataAndType(
                            Uri.parse("content://com.bigfoot404.avalon.apk/update.apk"),
                            "application/vnd.android.package-archive");
                        i.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION
                                 | Intent.FLAG_ACTIVITY_NEW_TASK);
                        startActivity(i);
                    }
                }
            }
        } catch (Exception e) {
            // offline or host down: bundled content serves fine
        }
    }

    // ---------------- small http helpers ----------------
    private HttpURLConnection open(String urlStr) throws IOException {
        HttpURLConnection c = (HttpURLConnection) new URL(urlStr).openConnection();
        c.setConnectTimeout(15000);
        c.setReadTimeout(30000);
        c.setInstanceFollowRedirects(true);
        return c;
    }

    private JSONObject httpJson(String urlStr) {
        try {
            HttpURLConnection c = open(urlStr);
            if (c.getResponseCode() != 200) return null;
            ByteArrayOutputStream buf = readAll(c.getInputStream());
            return new JSONObject(buf.toString("UTF-8"));
        } catch (Exception e) { return null; }
    }

    private boolean httpToFile(String urlStr, File out) {
        try {
            HttpURLConnection c = open(urlStr);
            if (c.getResponseCode() != 200) return false;
            File parent = out.getParentFile();
            if (parent != null) parent.mkdirs();
            File tmp = new File(out.getAbsolutePath() + ".part");
            FileOutputStream fo = new FileOutputStream(tmp);
            InputStream in = c.getInputStream();
            byte[] b = new byte[16384];
            int n;
            while ((n = in.read(b)) > 0) fo.write(b, 0, n);
            fo.close();
            in.close();
            if (tmp.length() < 1024) { tmp.delete(); return false; }
            if (out.exists()) out.delete();
            return tmp.renameTo(out);
        } catch (Exception e) {
            return false;
        }
    }

    private ByteArrayOutputStream readAll(InputStream in) throws IOException {
        ByteArrayOutputStream buf = new ByteArrayOutputStream();
        byte[] b = new byte[8192];
        int n;
        while ((n = in.read(b)) > 0) buf.write(b, 0, n);
        in.close();
        return buf;
    }

    // ---------------- asset helpers ----------------
    private String readFirstAsset(String[] paths) {
        for (String p : paths) {
            try {
                return readAll(getAssets().open(p)).toString("UTF-8");
            } catch (IOException e) { /* next */ }
        }
        return "<html><body style='background:#0a0b0d;color:#c8c4bc'>ASSET READ FAIL</body></html>";
    }

    private String mimeFor(String path) {
        String p = path.toLowerCase();
        if (p.endsWith(".html")) return "text/html";
        if (p.endsWith(".js")) return "text/javascript";
        if (p.endsWith(".css")) return "text/css";
        if (p.endsWith(".glb")) return "model/gltf-binary";
        if (p.endsWith(".png")) return "image/png";
        if (p.endsWith(".jpg") || p.endsWith(".jpeg")) return "image/jpeg";
        if (p.endsWith(".mp4")) return "video/mp4";
        if (p.endsWith(".svg")) return "image/svg+xml";
        return "application/octet-stream";
    }

    private void hideSystemUI() {
        View d = getWindow().getDecorView();
        d.setSystemUiVisibility(View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
    }
}
