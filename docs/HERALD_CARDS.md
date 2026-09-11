# SACRED HERALD + LUMINARY PRODUCTION CARDS
Canon: element-formed heralds (class VII) + the Luminary line (class VIII). Kept by BudE404 Sept 5 2026.
Art law: "A herald is the element wearing the beast — the gods' primordial grammar at fauna scale."
3D pipeline: SINGLE-SUBJECT 3D-INPUT RULE (one figure, one view, flat studio grey, no scale anchors in the 3D ref — anchors live in the scene compositions only).

---

## 1. MOON STAG — Everbloom
- **Element:** deep moss bloom (sap-light)
- **Body:** bark-wood hide, moss, sap-light seams; antlers are living branches with blossoms sprouting from the body
- **Omen:** seeing one at dusk = the forest consents to your passage. Guardian of the ancient groves.
- **Behavior:** never attacks unless a Everbloom shrine is profaned
- **3D ref:** `art/concepts/sacred-heralds/3D-MOON-STAG.png`
- **3D notes:** quad-deer rig, idle graze + head-tilt omen animation; blossom sprites attach points along spine/antlers; emissive sap-seam mask

## 2. MIST WOLF — Duskmourn (the Gates)
- **Element:** crimson mist
- **Body:** solid dark-smoke head/shoulders, haunches and tail dissolve into fog streams; eyes = two crimson lights
- **Omen:** walks beside the Marked = the land vouches for the vow
- **Behavior:** guides through barrow-fog; never attacks pilgrims
- **3D ref:** `art/concepts/sacred-heralds/3D-MIST-WOLF.png`
- **3D notes:** wolf rig; rear fog = alpha-mapped particle trail (geometry solid through shoulders, dissolves via vertex alpha to tail); guide-path AI state

## 3. STORM EAGLE — Skyrend
- **Element:** storm slate storm (thunderhead + lightning veins)
- **Body:** slate-grey solidified storm-cloud, cloud-packed feathers, lightning veins through wings
- **Omen:** its cry answered by thunder = a god heard the prayer
- **Behavior:** divine messenger; carries omens between storm-shrines
- **3D ref:** `art/concepts/sacred-heralds/3D-STORM-EAGLE.png`
- **3D notes:** full wingspread ref for wing-bone rig; perched + soaring flight sets; lightning = animated emissive shader on feather-edge mask

## 4. FAEWILD STEED — cross-realm (the seams)
- **Element:** pale realm-light (moonstone-glass)
- **Body:** semi-translucent pearly light-glass, mane/tail = drifting motes, hooves trail light seams
- **Omen:** refuses riders the vow does not trust; mount earned by great deeds
- **3D notes:** horse rig (reuse pilgrim-mount skeleton); translucent glass shader + trailing mote particles; gallop + seam-crossing vanish animation
- **3D ref:** `art/concepts/sacred-heralds/3D-FAEWILD-STEED.png`

## 5. WORLD SERPENT — Marenth (sacred lakes)
- **Element:** pearl-teal tide (living water)
- **Body:** dark flowing water shaped into coils, teal light within, foam-and-spray mane, shrine stones grown into the brow
- **Omen:** when a lake goes quiet, the serpent is listening
- **3D ref:** `art/concepts/sacred-heralds/3D-WORLD-SERPENT.png`
- **3D notes:** long-chain spine rig (S-coil pose ref); water-body = flowing-surface shader + foam crest particles; rise-from-lake event animation

## 6. SILVER SWAN — the meres
- **Element:** radiant silver light (pearl-teal + moon-silver)
- **Body:** made of soft light, feather edges dissolve to motes, liquid light in its wake
- **Omen:** a swan on the mere = the water can change you (transformation)
- **3D ref:** `art/concepts/sacred-heralds/3D-SILVER-SWAN.png`
- **3D notes:** swan rig, glide + wings-half-spread display; emissive-only material option (reads as pure light at distance)

## 7. HORNED PRIMORDIAL — Stoneheart
- **Element:** bedrock + white-violet geode crystal
- **Body:** ancient bedrock strata plates, cracks glow pale amethyst, geode-crystal antlers like mountain roots, moss and shrine ruins on shoulders
- **Omen:** where it grazes, the wilderness is still wild
- **Behavior:** the primordial wilderness walking; do not disturb
- **3D ref:** `art/concepts/sacred-heralds/3D-HORNED-PRIMORDIAL.png`
- **3D notes:** largest herald mesh; heavy quadruped rig, slow idle + head-sweep; geode antlers = separate crystal material w/ internal glow shader

## 8. ELDER DRAGON — the Depths
- **Element:** bark-stone + ember-gold molten seams
- **Body:** deep-earth plates cracked with molten light through every seam, wings like folded cliff strata, heat shimmer
- **Omen:** not fought for loot — an encounter is an event; older than the gates
- **3D ref:** `art/concepts/sacred-heralds/3D-ELDER-DRAGON.png`
- **3D notes:** T3-scale world-event mesh; dragon rig (wings + tail chain); molten seams = emissive crack mask + heat-shimmer post pass; event staging: coiled around crater terrain

## 9. LUMINARY (canon — Ashfall) — companion line
- **Element:** living ember (faction-reskinnable: fire / lightning / bloom / mist / water / crystal)
- **Body:** formed ENTIRELY of its element — molten-light skin, flame hair/wings, drift-sparks; wears the Cold Reliquary (human-craft bronze, kept deliberately: made thing, named flame)
- **Function:** pet-relic fusion — "the flame illuminates, the reliquary it wears protects"; bare-flame vs reliquary-donned = earned progress badge
- **3D ref:** `art/concepts/sacred-heralds/3D-LUMINARY-CANON.png`
- **3D notes:** smallest rig (hover bob + idle orbit animations); emissive-heavy material; reliquary = separate mesh slot (attire toggle); faction reskin = element color + hair/wing texture swap on ONE mesh

---
*Faction strip refs: `LUMINARY-FACTION-STRIP-6-v2.png` (reskin palette source). Scene compositions (pilgrim scale anchors): `*-v2.png` files.*
