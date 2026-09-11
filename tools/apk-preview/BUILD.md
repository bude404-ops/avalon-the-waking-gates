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

## v0.5 (Sept 9, Big's verdict: batch locks + character naming)
- WORLD FAUNA 12/12 + WYVERN V2 + STAGE2 24/24 + X LOGO all canon-locked (see CANON-LOCKS.md)
- THE TWELVE NAMED (Big: 'get them actual names based around their class and region and gender and then have what class they are'): docs/CHARACTER-NAMES.md — gallery now shows NAME + class line for all 12 heroes (Aedan/Seren Stormcrown — Sovereign of Skyrend, etc.)
- versionCode 5 / versionName 0.5-test; same debug keystore (updates over v0.3/v0.4)

## v0.6 (Sept 9, Big's ask: view the live 3D model in the APK)
- LIVE 3D MODELS menu item: model3d.html — three.js r147 (UMD build, offline in assets/vendor/) WebGL viewer, drag-rotate/pinch-zoom (OrbitControls), idle loop autoplay (AnimationMixer)
- AEDAN-RIGGED-V3.glb bundled (models/): manual distance-based vertex binding (top-4 inv-sq over 20 bones) — bone-heat auto-weights fail silently on non-manifold AI meshes, which was the root cause of every frozen idle render
- GLB probe-verified: weighted arm vertex deforms 1.9% body height at breathe peak; render diff 61K pixels between rest and peak frames
- versionCode 6 / versionName 0.6-test; same debug keystore (updates over v0.5)
- v0.6 R2 FIX: model didn0027t load on device 2014 WebView blocks XHR/fetch from file:// origins, so GLTFLoader couldn0027t pull the GLB. MainActivity now sets setAllowFileAccessFromFileURLs(true) + setAllowUniversalAccessFromFileURLs(true). GLB itself verified healthy in-APK (skin 20 joints, 1 idle anim, JOINTS_0/WEIGHTS_0 present).
- v0.6 R3 (still failing on device): file-origin flags insufficient 2014 shell now serves ALL assets via shouldInterceptRequest under loadDataWithBaseURL(https://avalon.local/) with full MIME map (glb = model/gltf-binary); model3d.html gains an on-screen diagnostic readout (window.onerror + vendor onerror + GLB fetch progress + loader error text) so any failure prints the exact reason mid-screen. Same versionCode 6, over-installs R1/R2.

## BUILD CHANNELS DOCTRINE (Sept 9, Big's call: split dev vs demo APKs)

Two channels from here on — never mixed:

1. TEST channel (what ships today): AVALON-WAKING-GATES-TEST-vX.Y.apk
   - Debug keystore, versionName suffix -test, diagnostic readouts ON (red error text, HUD stage lines)
   - Carries experimental/in-flight content (pending verdicts, test models, broken-on-purpose probes)
   - Audience: Big's device + inner circle only. Fast iteration — rebuild freely.

2. DEMO channel (when the game is demo-ready): AVALON-WAKING-GATES-DEMO-vX.Y.apk
   - Built ONLY from canon-locked content (approved/ art, locked bestiary, locked models) — no pending pieces
   - Diagnostics OFF: debug HUD hidden via ?demo=1 flag / demo build constant in index.html (window.DEMO_MODE — build script swaps it), clean entry, polished nav
   - Stable version bumps only; never ship a demo build same-day as its content lock
   - Audience: outsiders, playtesters, potential partners. Third channel (Play Store release signing) deferred until commercial decision.

Build rule: demo builds cut from the same repo but require an explicit "demo-ready" pass — I sweep assets for pending/ or non-canon entries and refuse the build if any are referenced.

## v0.7 (Sept 9, Big's ask: game UI/layout "as if it was the real game")
- NEW: GAME UI — THE REAL THING menu card → ui/ui-index.html hub with 6 canon-styled game screens:
  ui-title (teaser keyart bg, game menu), ui-charselect (the Twelve, tap for lore+kit), ui-hud (vitals/compass/quest tracker/ability bar with storm-slate lantern slot over Campaign I still), ui-map (Six Realms SVG map + location marker + realm art thumbs), ui-skills (Aedan kit lineage tree, anti-grind copy), ui-inventory (First Crafts loadout + materials)
- art additions: teaser keyart, Campaign I still, 6 STAGE2 realm images (~2.5MB)
- versionCode 7 / 0.7-test; published to web at mcontwitter-glitch.github.io/avalon-3d-viewer/ui/ (repo avalon-3d-viewer carries ui/ + art subset, 3D viewer root links to it)
- mobile 3D still unfixed on device (R3 diagnostic readout shipped — awaiting the red line); web viewer + all UI pages confirmed working

## v0.9.3 (Sept 10 — PIPELINE v2 EXECUTED: Big's verdicts C + MOCAP)
- AEDAN V7 REBUILT under pipeline v2: 468K original -> weld+decimate 147K (silhouette-exact, NO voxel smoothing), normal+AO baked from the original sculpt (2048px), CMU skeleton refit to mesh joints, REAL MOCAP walk (CMU 07_01, in-place) + chest-led idle, GLB+FBX (models/generated/AEDAN-V7-MO.*)
- SOVEREIGN SPEAR ATTACHED: leaf-blade spear built from the canon weapon plate, bone-parented to RightHand, holds through walk+idle (models/generated/AEDAN-V7-MO-ARMED.*)
- TURNAROUND SHEETS: Sovereign M front/side/back rolled from canon (art/pending/TURNAROUND-SOVEREIGN-M.png) — template for the Twelve
- CLEAN WEAPON PLATE: REF-SHEET-WEAPON-SOVEREIGN-V3-CLEAN.png (character stripped, weapon only) — Big flagged the old plate had the character in frame; V3 awaiting verdict, all 6 plates re-cut to clean standard if it reads right
- MODEL VIEWER: loads AEDAN-V7-MO-ARMED.glb; IDLE + WALK buttons (real mocap)
- versionCode 13 / versionName 0.9.3-test; same debug keystore

## v0.10.0 (Sept 10 — LIVE SHELL: the APK now updates itself, per Big's request)
- HOT CONTENT LAYER: on every launch the app fetches live/content.json from the Pages host
  (mcontwitter-glitch.github.io/avalon-3d-viewer/live/). If content_version changed, it silently
  mirrors the listed files into filesDir/live and reloads — new models/screens/art appear with
  NO install. Interceptor serve order: filesDir/live -> bundled assets (www/ then flat).
- SHELL SELF-UPDATE: content.json also carries an apk pointer (versionCode/versionName/url).
  If the installed versionCode is older, the app downloads the new APK in the background and
  fires the system install screen — ONE tap ("Install"). Requires the one-time
  "install unknown apps" allowance for the AVALON app (Android asks on first prompt).
- ApkProvider.java: minimal ContentProvider serving the downloaded APK to the package installer
  (no androidx dependency — repo has no gradle, dex is built with R8 directly).
- New permissions: INTERNET, REQUEST_INSTALL_PACKAGES.
- PUBLISHING: tools/apk-preview/publish_live.sh [optional_apk] — content-only updates are one
  command; shell updates = build new APK (bump versionCode) then publish with the apk path.
- versionCode 14 / versionName 0.10.0-live; last manual install (this one). v0.9.3 had an
  asset-path packaging bug (flattened assets -> ASSET READ FAIL) — superseded/fixed here.

## v107+ — UNITY SIGNING BASE (Sept 11, 2026)
Unity CI builds (forge-build.yml) sign with `unity/AVALON-DEMO.keystore` — this is now the PERMANENT signing base for all AVALON Unity APKs. It does NOT match the old web-shell debug.keystore (v0.10.0 vcode 14): Android will silently fail to install v107 over the old shell. Owners must uninstall the old app ONCE before installing the first Unity build. All future Unity builds share this keystore → one-tap self-updates, no more uninstalls. Never rotate this key.
