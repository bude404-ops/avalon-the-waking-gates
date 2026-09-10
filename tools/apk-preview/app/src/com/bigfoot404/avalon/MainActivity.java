package com.bigfoot404.avalon;

import android.app.Activity;
import android.content.res.AssetManager;
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
import java.io.IOException;
import java.io.InputStream;

public class MainActivity extends Activity {
    private WebView webView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                             WindowManager.LayoutParams.FLAG_FULLSCREEN);
        webView = new WebView(this);
        WebSettings s = webView.getSettings();
        s.setJavaScriptEnabled(true);
        s.setDomStorageEnabled(true);
        s.setAllowFileAccess(true);
        s.setAllowFileAccessFromFileURLs(true);
        s.setAllowUniversalAccessFromFileURLs(true);
        s.setLoadWithOverviewMode(true);
        s.setUseWideViewPort(true);
        s.setMediaPlaybackRequiresUserGesture(false);
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view, WebResourceRequest request) {
                String path = request.getUrl().getPath();
                if (path != null && path.startsWith("/")) path = path.substring(1);
                if (path == null || path.isEmpty()) path = "index.html";
                if (!path.startsWith("www/")) path = "www/" + path;
                try {
                    InputStream in = getAssets().open(path);
                    return new WebResourceResponse(mimeFor(path), null, in);
                } catch (IOException e) {
                    return null;
                }
            }
        });
        webView.setBackgroundColor(0xFF0A0B0D);
        setContentView(webView);
        String html = readAsset("www/index.html");
        webView.loadDataWithBaseURL("https://avalon.local/", html, "text/html", "utf-8", null);
        hideSystemUI();
    }

    private String readAsset(String path) {
        try {
            InputStream in = getAssets().open(path);
            ByteArrayOutputStream buf = new ByteArrayOutputStream();
            byte[] b = new byte[8192];
            int n;
            while ((n = in.read(b)) > 0) buf.write(b, 0, n);
            in.close();
            return buf.toString("UTF-8");
        } catch (IOException e) {
            return "<html><body style='background:#0a0b0d;color:#c8c4bc'>ASSET READ FAIL</body></html>";
        }
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
