# UNITY 3D MODEL BUILD MANIFEST — THE 12 CLASS MODELS (Sept 9 2026)

Per the UNITY-ONLY TOOL LAW: every 3D asset comes from **Unity built-ins + Asset Store (UModeler X for in-editor modeling/retopo/rig/skin)**, assembled on Big's seat (Unity Personal, free, commercial OK; Asset Store EULA = commercial use on all tiers). This manifest is the build order; the 404-GEN reference sheets (art/pending/REF404-GEN-CLASS-*-V2.png (weaponless plates per the Ref-Sheet Laws; V1 superseded)) are the modeling/QC reference plates; the locked class canon keys (art/approved/CLASS-*-CANON.png) are the art-direction bible and QC bar.

## PIPELINE PER MODEL
1. **Base** — rigged humanoid base from Asset Store (pick ONE consistent base for all 12: same skeleton = shared animation sets).
2. **Blockout** — UModeler X: silhouette pass against the REF404-GEN plate (build grammar: statuesque / massive / stocky / wiry / spare / feral).
3. **Armor pass** — modular Celtic-bronze pieces matched to the canon key (solid closed breastplate, high collar, sleeveless arms, kilt panels, torc, cloak).
4. **Material pass** — muted desaturated bronze palette; ONE whisper accent per class (storm slate / oxide red / bone grey / crimson / tide teal / deep moss); weathering on gear, never on faces (Female Face Law).
5. **Hair pass** — class hairstyle signature (HAIRSTYLE-TO-CLASS LAW): Sovereign braided crown-lock w/ bronze rings, Ravager war-shaved + braided top-knot, Warden cropped/pinned plaits, Veilborn long straight half-covering, Weaver unbound w/ cord-strands + beads, Wildborn matted dreads w/ moss.
6. **Weapon pass** — kit-integrity: exactly ONE signature kit, no extras (Spear / Greatsword / Warhammer / Twin leaf-blades / Staff w/ UNLIT lantern housing / Claw gauntlets).
7. **QC GATE** — side-by-side against canon key + ref sheet before it ships. Any mismatch = rework, never "close enough."

## BODY LAW STACK (applies to all 12)
- **Males:** broad/powerful per class build, 70/30 armored pin-up treatment — physique visible through fitted armor, sleeveless arms, solid closed breastplate (NO chest cutout, high collar line).
- **Females:** curvy hourglass anchor on every frame (full bust, cinched waist, rounded hips — realistic, never exaggerated; NEVER overweight/plump), mature 25-50 per-class age read, pretty natural faces (no rough face weathering, no anime, no glamour), 50/50 functional/provocative attire split, bust presented via fitted plunging-neckline tops (tasteful, never explicit), strength via shape only (zero visible muscle definition).
- All: real skin, weathering lives in gear, no modern reads.

## ASSET STORE SOURCING NOTES (shortlist direction)
- Base skeletons: search "modular fantasy character base rigged humanoid" — prioritize packs with full body + face rigs and retarget-friendly naming.
- Armor: "Celtic armor pack" / "bronze age armor modular" — anything close gets UModeler X re-cut to the knotwork + La Tene spec (spiral/knotwork PATTERNS only — the gate-rune stays the only mark).
- Cloaks/cloth: UModeler X cloth or Asset Store cloth sims; whisper-accent tint per class.
- Weapons: leaf-shaped blades required (Celtic leaf-blade law); bronze/dark-iron, leather grips, carved wood.
- Animations: one shared melee set retargeted to the common skeleton + class-specific idle/attack flavors.

## BUILD ORDER (pairs share a skeleton/scale pass)
1. Sovereign M + F (flagship pair — locks the shared-base decision)
2. Ravager M + F
3. Warden M + F
4. Veilborn M + F
5. Weaver M + F
6. Wildborn M + F

## AFTER THE 12
- Vera + key NPC re-dresses (canon NPC roster governs; attire per Mythic Celtic doctrine).
- Colossi/gods: NOT character models — scale-props + VFX (Primordial Law: torso-up, fused to terrain; Unity terrain/VFX work, not rigs).
- Hollows: creature packs matched against bestiary canon keys.

---

## REF-SHEET LAWS — V2 UPDATE (Sept 9, Big's verdict feedback)

1. **WEAPONLESS PLATE LAW (Big: 'make sure there arent weapons on them for the 3d reference sheets images'):** 404-GEN Stage B plates render the character ONLY — hands completely empty, zero weapon props, zero sheathed weapons, zero held/worn weapons (incl. Wildborn claw gauntlets and Weaver lantern-staff). Weapons are modeled as SEPARATE attachable 3D props per the class kit (kit-integrity still governs the canon key art — the keys keep their weapons; the plates don't).
2. **MALE CHESTPLATE-DISTINCTION LAW (Big: Warden + Veilborn 'look too identical' to the others):** each male class carries an unmistakably distinct chest treatment — Sovereign: solid engraved ceremonial breastplate, high collar (unchanged) · Ravager: massive battered engraved plate, torn mantle (unchanged) · Wildborn: minimal hide-and-bronze harness, bare chest (unchanged) · Weaver: simple shoulder-plates + layered robes (unchanged) · **Warden M: NEW — double-thick bronze breastplate of overlapping horizontal reinforcing bands over mail, deep-stamped knotwork ridge lines, heavy shoulder yoke, mail skirt — the wall-of-defense read** · **Veilborn M: NEW — narrow light dark-bronze cuirass almost swallowed by layered cord-and-leather harness wraps, sparse stamps, cloth silhouette — the ghost read.** F-set carries the same distinction logic (Warden-F banded heavy, Veilborn-F wrapped minimal) for set consistency.
3. **V1 superseded for the full set:** all twelve REF404-GEN-CLASS-*-V2.png replace V1 (V1 preserved in git history). The six female V2 plates + four approved male V2 plates are the SAME approved designs, weapons removed. Verdicts pending on the V2 batch.

---

## V2 REVISION LAWS (Sept 9 2026, Big's review of the V1 plates)

### REF-SHEET WEAPONLESS LAW (Big: "make sure there arent weapons on them for the 3d reference sheets images")
404-GEN reference plates are BODY/ARMOR reference only — ZERO weapons in frame (no held weapons, no sheathed weapons, no staves, no claw gauntlets). Hands relaxed and empty. Weapons are SEPARATE 3D PROPS in Unity (one per class kit, built from the canon key art; kit-integrity still governs the weapon prop itself). The weaponless plate keeps the modeling pass clean and lets the weapon asset be attached/detached independently. All 12 plates re-rolled V2 weaponless (art/pending/REF404-GEN-CLASS-*-V2.png); V1 superseded.

### MALE CHESTPLATE-DISTINCTION LAW (Big: "the sovereign, ravager and wild one and weaver are good for the males the other two look too identical")
The male kits must be silhouettes-first — no two chestplates read the same. Locked distinctions:
- SOVEREIGN: solid closed ceremonial breastplate, FULL knotwork + La Tène engraving across the chest — the regal maximal plate.
- RAVAGER: MASSIVE battered single-piece plate, heavy scarred knotwork — the biggest chestplate, war-worn brutal read.
- WARDEN: double-thick overlapping horizontal reinforcing BRONZE BANDS over mail, layered shoulder yoke, mail skirt — the wall-of-defense read (V2 fix).
- VEILBORN: narrow light dark-bronze cuirass swallowed by cord-and-leather harness wraps, cloth-built silhouette — the ghost read (V2 fix).
- WEAVER: simple engraved bronze shoulder-plates + layered robes — the minimal mystic read.
- WILDBORN: bronze-and-hide harness over bare chest — the feral minimal read.
QC GATE ADDITION: before any plate ships, its chestplate treatment must be unmistakable at silhouette distance vs the other five.
