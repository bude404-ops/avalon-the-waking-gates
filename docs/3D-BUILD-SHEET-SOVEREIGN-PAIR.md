# 3D BUILD SHEET — SOVEREIGN M+F (BUILD #1, THE FLAGSHIP PAIR)

First assembly of the 3D era. This pair LOCKS THE SHARED-BASE DECISION for all 12 models — same skeleton, same scale pass, same material conventions. Everything below runs in the Unity seat (Unity Personal free + UModeler X free from the Asset Store). QC references: canon keys art/approved/CLASS-SOVEREIGN-CANON.png + CLASS-SOVEREIGN-F-CANON.png, locked ref plates art/approved/REF-SHEET-CLASS-SOVEREIGN-{M,F}-CANON.png.

## STEP 0 — PROJECT SETUP (one-time)
1. Install Unity 2022.3 LTS (Personal license, free) — URP template.
2. Asset Store -> install UModeler X (free, full commercial): in-editor modeling/retopo/UV/skin.
3. Folder structure: Assets/Models/Classes/Sovereign/{M,F} | Assets/Props/Weapons | Assets/Materials/Classes.

## STEP 1 — BASE MODEL (locks the decision for all 12)
- Asset Store search: "modular fantasy character base rigged humanoid" / "stylized fantasy character full rig" — prioritize: full body + face rig, Humanoid-configured, retarget-friendly bone naming, M/F body variants.
- Pick ONE base skeleton family for all 12 classes (shared animation sets later = one retarget, not twelve).
- Import, configure Humanoid rig, verify hands empty (weaponless plate law) and proportions match the ref plate silhouettes (M: tall statuesque; F: tall statuesque hourglass per the body law stack — full bust via fitted top, cinched waist, rounded hips, realistic never exaggerated).

## STEP 2 — SOVEREIGN M ASSEMBLY SPEC
Build grammar: STATUESQUE, TALL, COMMANDING. Whisper accent: STORM SLATE (Skyrend).
1. Chest: solid CLOSED ceremonial breastplate — full coverage, HIGH COLLAR LINE, no chest cutout ever (solid-breastplate law), FULL knotwork + La Tene spiral engraving across the chest — the regal maximal plate (unmistakably distinct from Ravager's boar-head and Warden's bands).
2. Arms: sleeveless — bare arms, simple bronze bracers; physique visible (70/30 armored pin-up).
3. Waist/legs: armored waistband + layered battle-kilt panels over trousers, tall boots.
4. Cloak + jewelry: slate mantle/cloak (storm-slate whisper in the cloak only), bronze torc at the neck, knotwork PATTERNS only — the gate-rune stays the only mark.
5. Hair: braided crown-lock with bronze rings — regal, never wild (HAIRSTYLE-TO-CLASS law).
6. Face: mature weathered, commanding calm expression, thick hair + beard (Mythic Celtic men's law).

## STEP 3 — SOVEREIGN F ASSEMBLY SPEC
Build grammar: TALL STATUESQUE HOURGLASS, COMMANDING. Whisper accent: STORM SLATE.
1. Chest: fitted engraved bronze ceremonial cuirass — elegant fitted armor emphasizing the waist (50/50 functional/provocative split), plunging neckline tasteful (cleavage-present law), zero visible muscle definition — strength via shape only.
2. Arms: bare upper arms + ornate bronze bracers; exposed shoulders/upper back per the female armor spec.
3. Waist/legs: cinched armored belt, battle-skirt panels + leggings, tall boots.
4. Cloak + jewelry: slate cloak w/ storm-slate whisper, torc, ornate bronze jewelry.
5. Hair: braided crown-lock w/ bronze rings (class hair law — M+F share the class read), thick flowing.
6. Face: pretty natural, soft clean features, NO weathering on faces (Female Face Law), mature 25-50 per-class read, commanding calm — never uniform happy (expression-to-class).

## STEP 4 — MATERIALS
- Bronze/dark-iron palette, heavily desaturated — muted, never shiny gold reads.
- ONE whisper accent: storm slate, in cloak/mantle only.
- Weathering texture on gear only (story-wear), never on skin.
- Cold two-layer light for QC preview shots (cold ambient + one rich source). Final in-engine lighting per the Reliquary-Region Aura Law when placed in Skyrend.

## STEP 5 — WEAPON PROP (SEPARATE — never parented to the rig by default)
- Sovereign spear: leaf-shaped blade head (Celtic leaf-blade law), engraved dark-iron + bronze collar with knotwork, leather grip, carved wood shaft.
- Standalone prop at Assets/Props/Weapons/SovereignSpear; attach via hand socket at scale per the WIELDER-SCALE LAW (spear = wielder height). Ref: art/approved/REF-SHEET-WEAPON-SOVEREIGN-CANON.png.

## STEP 6 — QC GATE (both models)
Render each from: front A-pose (match the ref plate), 3/4 hero view, side profile.
- Side-by-side against the canon key + ref plate — any mismatch = rework, never "close enough."
- Chestplate reads unmistakable at silhouette distance (chestplate-distinction law).
- Kit-integrity: ZERO extra weapons/gear beyond spec.
- Zero script/runes beyond the gate-rune mark.

## STEP 7 — WHEN DONE
Push the Unity project (or export .fbx/.glb + textures) — I QC from here against the canon files, then we roll straight to Ravager M+F (the boar-head breastplate is the hero piece there; ref art/approved/REF404-GEN-CLASS-RAVAGER-M-CANON.png).
