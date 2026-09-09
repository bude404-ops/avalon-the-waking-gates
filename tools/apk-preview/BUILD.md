# AVALON Preview APK — Test Build v0.1

WebView-shell test APK for runtime/mobile preview of the current canon content. The real game build comes from the Unity seat per the Unity-first law; this shell is for phone-side preview and testing only.

## Contents (all offline, bundled in assets/www/)
- HOME — title splash + menu
- CINEMATIC TEASER — the full teaser reel (9 shots + studio/title cards)
- CANON ART VAULT — 33 canon keys: 12 champions, 6 gods, 4 Hollow Tide elites, 7 Avalon Creatures, Vera
- THE WORLD — the share-ready game overview

## Build (from repo root)
- App source: `tools/apk-preview/app/` (MainActivity WebView shell, minSdk 24, target 34)
- Toolchain: JDK 17 + Android build-tools 34 + platform android-34
- `javac -classpath android.jar app/src/.../MainActivity.java` → `d8` → `aapt2 compile/link -A assets` → inject classes.dex → `zipalign` → `apksigner sign`
- Keystore: self-signed debug keystore (CN=BIG Entertainment, O=Bigfoot404 LLC) — NOT for store distribution

## Distribution
APK ships as a GitHub Release asset on this repo (Base44 file storage blocks .apk uploads).

## v0.3 (Sept 9, Big's notes: Vera zoom / elite Hollows / Avalon Creatures)
- VERA ZOOM FIX: gallery figure now `.fullview` (object-fit:contain, 3:4) — full art visible, no crop
- ELITE HOLLOWS FIXED: v0.2 build lost 3 of 4 elite images in a bad assets sync — all four now bundled (Champion V8-FACELAW, Erased Drake, Gate-Worm, Echo)
- AVALON CREATURES: fauna section renamed + expanded to all seven canon fauna (Cwn Annwn, Afanc, Questing Beast, White Hart, Barghest, Wyvern, Strata Giant), each captioned 'AVALON CREATURE'
- Keystore: v0.1-0.2 debug.keystore was lost with a staging wipe; v0.3 is signed by a NEW self-signed debug key (same DN). v0.3 does NOT update-install over v0.2 — uninstall the old build first. Keystore now committed at tools/apk-preview/debug.keystore so this never recurs.
