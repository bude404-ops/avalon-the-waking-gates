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
3. **V1 superseded for the full set:** all twelve REF404-GEN-CLASS-*-V2.png replace V1 (V1 preserved in git history). The six female V2 plates + four approved male V2 plates are the SAME approved designs, weapons removed. The V2 class plates and V2 weapon plates are LOCKED (see CANON-LOCKS.md).

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

## WEAPON PROP REFERENCE PLATES — V1 (Sept 9, Big: 'we need to make the weapons 3d reference sheets aswell with same view and only one model of it so its visually read')

Six weapon plates rolled — ONE weapon per plate, same neutral flat-light 404-GEN discipline as the class plates, each class's kit rendered as a single readable model (Veilborn's twin leaf-blades = one blade; Wildborn's paired claw gauntlets = one gauntlet). Zero script (knotwork/spiral patterns only, rune-law compliant), unlit Weaver lantern (Lantern Law), quiet realm-accent whisper per class (storm slate / oxide red / bone grey / crimson / tide teal / deep moss).

| Class | Weapon prop | Plate (pending verdict) |
|---|---|---|
| Sovereign | leaf-blade spear (engraved bronze head, leather grip, bronze collar band) | art/pending/REF-SHEET-WEAPON-SOVEREIGN-V1.png |
| Ravager | massive battered greatsword (engraved dark iron, bronze crossguard, disc pommel) | art/pending/REF-SHEET-WEAPON-RAVAGER-V1.png |
| Warden | heavy banded-bronze warhammer (knotwork faces, cord-wrapped haft) | art/pending/REF-SHEET-WEAPON-WARDEN-V1.png |
| Veilborn | single leaf-blade dagger (dark bronze, cord grip, antler pommel) — twin kit, one model | art/pending/REF-SHEET-WEAPON-VEILBORN-V1.png |
| Weaver | gnarled lantern-staff (twisted branch, round bronze lantern cage, unlit) | art/pending/REF-SHEET-WEAPON-WEAVER-V1.png |
| Wildborn | single claw gauntlet (four curved iron claws over bronze-and-hide plate) — paired kit, one model | art/pending/REF-SHEET-WEAPON-WILDBORN-V1.png |

P-hash QC: min hamming 28/256 across the set (Weaver V1 first roll was a near-dupe of the Sovereign spear at 8/256 — re-rolled with the twisted-branch silhouette; superseded roll not vaulted). Verdicts pending — on Keep, each plate becomes the modeling/QC reference for its Unity weapon prop.

## WEAPON PROP REFERENCE SHEETS — V1 (Sept 9, Big: 'make the weapons 3d reference sheets aswell with same view and only one model of it so its visually read')
Per the V2 Weaponless Plate Law (character plates carry zero weapons), each class kit's weapon now gets its OWN single-model 404-GEN Stage B plate: one weapon per plate, consistent side-profile view, neutral grey studio grade, flat lighting, Mythic Celtic craft grammar (bronze/dark iron, La Tène knotwork patterns, cord/leather/bone), realm accent whisper, unlit lanterns (Weaver staff), zero script. Rolled V1 (art/pending/REF-SHEET-WEAPON-*-V1.png), p-hash verified unique (all 6): Sovereign leaf-blade spear / Ravager greatsword / Warden warhammer / Veilborn leaf-blade dagger (twin kit = identical pair, single model shown) / Weaver lantern-staff / Wildborn claw gauntlet (pair = identical, single model shown). Pending Big's verdicts.

## WEAPON PROP REFERENCE PLATES — V2 (Sept 9, Big: 'Weapon need to be in same art style I believe and make so it will be proper sizes for the characters when we attach')

V1 superseded (studio-plate look, no scale read). V2 laws per Big's direction:
1. **CANON-STYLE LAW** — weapon plates render in the SAME hand-painted dark-fantasy oil-paint style as the character canon art (painterly brushwork, muted desaturated palette, cold two-layer light) — never glossy studio-product photography.
2. **WIELDER-SCALE LAW** — every weapon plate carries a faint translucent featureless grey SILHOUETTE of its class's wielder build at TRUE relative scale (Sovereign tall statuesque / Ravager tall broad / Warden stocky strong / Veilborn lean compact / Weaver slim spare / Wildborn athletic) so Unity attachment gets correct proportions: spear = wielder height; greatsword = ground-to-chest; warhammer haft = to shoulder, head = chest width; dagger = forearm length; lantern-staff = a full head taller than wielder; claw gauntlet = forearm-and-hand size. Silhouette never holds the weapon, no face, no hands.

Files (art/pending/REF-SHEET-WEAPON-{CLASS}-V2.png): Sovereign leaf-blade spear / Ravager battered greatsword / Warden block-head banded warhammer / Veilborn single leaf-blade dagger (twin kit, one model) / Weaver gnarled lantern-staff (unlit, Lantern Law) / Wildborn single claw gauntlet (paired kit, one model). Zero script holds (knotwork/spiral patterns only). P-hash QC: first Warden V2 roll drifted into the greatsword silhouette (18/256 + 1.8% pixel-diff) — re-rolled with the no-blade block-head lock; final set min hamming 24/256. Verdicts pending.

## WEAPON PROP REFERENCE PLATES — V2 (Sept 9, Big: 'Weapon need to be in same art style... and make so it will be proper sizes for the characters when we attach')

V1 superseded (studio-product-shot style + no scale read). V2 re-rolls fix both: (1) **SAME ART STYLE** — painted in the character-canon oil-paint style on the same light neutral background as the REF404-GEN character plates; (2) **TRUE-SCALE WIELDER SILHOUETTE** — each plate carries a faint featureless grey silhouette of the class's build at true relative scale, so Unity attachment gets proportions right (spear = tall as the Sovereign; greatsword = ground-to-chest on a tall broad build; warhammer = shoulder-height, head chest-wide; dagger = forearm-length; lantern-staff = taller than the slim figure's shoulder; gauntlet = knuckles-to-mid-forearm).

| Class | Plate (pending verdict) |
|---|---|
| Sovereign | art/pending/REF-SHEET-WEAPON-SOVEREIGN-V2.png |
| Ravager | art/pending/REF-SHEET-WEAPON-RAVAGER-V2.png |
| Warden | art/pending/REF-SHEET-WEAPON-WARDEN-V2.png |
| Veilborn | art/pending/REF-SHEET-WEAPON-VEILBORN-V2.png (horizontal lay) |
| Weaver | art/pending/REF-SHEET-WEAPON-WEAVER-V2.png (horizontal lay, big cage lantern) |
| Wildborn | art/pending/REF-SHEET-WEAPON-WILDBORN-V2.png |

QC: uniform light-neutral backgrounds across the set (matching character plates); shape-IoU check confirms distinct weapon silhouettes (upright trio shares only the standing scale-figure by design); tall-pole pair (spear/staff) broken by laying the staff horizontal. V1 preserved in git history.

## ATTACH-TIME SCALE LAW (Sept 9 ~12:34 PM, Big: 'Are we able to resize them when they are a model to fit regardless of 2d art?')

YES — codified as doctrine: the 2D weapon plates are ART REFERENCE (form, style, materials, detail); TRUE SCALE IS SET IN-ENGINE. Every weapon prop is imported at native size and then resized freely at attach time in Unity — per-class prop scale + hand-bone attach offsets tuned against the rigged character model in-engine, then locked as the class kit's attach preset. The wielder silhouettes in the 2D plates are INDICATIVE of intended proportions, never binding on the 3D transform. No 2D re-roll is required to solve a scale problem; scale is a free variable at the 3D layer. QC on attached props = visual side-by-side vs the canon key art in-engine, not vs the ref plate's silhouette.
