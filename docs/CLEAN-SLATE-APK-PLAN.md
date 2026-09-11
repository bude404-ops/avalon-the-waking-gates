# CLEAN-SLATE APK REBUILD — STEPWISE LADDER (Big's order, Sept 11 2026)

> "Big pause on the apk file, what ever you keep trying isnt working need a new approach or start over with its build" →
> "Ok ya we just need to get that apk fixed so its viewable to see whats being made"

**Target device (Big's, LATEST confirmation, supersedes any earlier device info):** Samsung Galaxy S22 Ultra SM-S908U (Android 14, ARM64).

NOTE: an S23-targeted duplicate pipeline (clean-apk.yml + AvalonClean/CleanApkBuild.cs) was
removed Sept 11 — the ONLY live pipeline is clean-slate-apk.yml (BareBuild.cs, vcode 201 series).

## Doctrine

The v0–v113 shell stack failed blind-testing on one device. The rebuild goes stepwise:
**each build adds exactly ONE layer**, and Big tests on his S23 before the next layer ships.
No more blind multi-layer guesses.

## Build settings (every step inherits)

- Unity 6000.5.11f1 (CI container, headless batch, license via vaulted creds)
- IL2CPP, **ARM64 only** (S23), minSdk 26, targetSdk 34, GLES3 only (v113 Vulkan-crash fix holds)
- Package `com.bigfoot404.avalon` — same as old shell, so new builds install OVER it, no uninstall
- SIGN GUARANTEE: AVALON-DEMO keystore (CN=AVALON Bigfoot404) re-sign + cert DN check + launcher verify
- Delivery: direct file to Big's DM from CI (no links — links don't work on his phone)
- Workflow: `.github/workflows/clean-apk.yml` (isolated from forge-build.yml)

## The ladder

| Build | Layer added | Content | Test gate |
|---|---|---|---|
| **v114** | none — bare stock | static primitives only (stone ring + amber brazier, ZERO app code) | opens + shows the brazier shrine |
| **v115** | + canon gallery | approved canon art plates on textured quads + swipe (first runtime code, UI only) | browses canon art |
| **v116** | + shell HUD | THE RELIQUARY HUD frame | HUD renders both orientations |
| **v117** | + self-updater | live-channel content sync + one-tap APK update | content update flows silently |

After v117 the shell is back to full function, then 3D content (Forge prefabs) layers in
per the class batch queue (Ravager first).

## v114 status

Dispatched Sept 11 2026 (vcode 114). Bare scene = camera, light, 6 stone slabs, 1 emissive
amber brazier cube. If it opens clean, the Unity-6-on-S23 base chain (IL2CPP/GLES3/sign/
launcher) is PROVEN and every failure past that point is attributable to one layer.
