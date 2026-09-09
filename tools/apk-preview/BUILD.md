# AVALON Preview APK — Test Build v0.1

WebView-shell test APK for runtime/mobile preview of the current canon content. The real game build comes from the Unity seat per the Unity-first law; this shell is for phone-side preview and testing only.

## Contents (all offline, bundled in assets/www/)
- HOME — title splash + menu
- CINEMATIC TEASER — the full teaser reel (9 shots + studio/title cards)
- CANON ART VAULT — 23 locked canon keys: 12 champions, 6 gods, 4 Hollow Tide elites, Vera
- THE WORLD — the share-ready game overview

## Build (from repo root)
- App source: `tools/apk-preview/app/` (MainActivity WebView shell, minSdk 24, target 34)
- Toolchain: JDK 17 + Android build-tools 34 + platform android-34
- `javac -classpath android.jar app/src/.../MainActivity.java` → `d8` → `aapt2 compile/link -A assets` → inject classes.dex → `zipalign` → `apksigner sign`
- Keystore: self-signed debug keystore (CN=BIG Entertainment, O=Bigfoot404 LLC) — NOT for store distribution

## Distribution
APK ships as a GitHub Release asset on this repo (Base44 file storage blocks .apk uploads).
