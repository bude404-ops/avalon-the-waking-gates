# DUSKMOURN — Faction Ability Tree (v1 CANON)

> Unlocked by pledging the flame Mark to THE DUSKMOURN PATRON GOD — THE DEATH KING (Morvaine).
> Resource: FAITH. Free re-pledge. Borrowed gloom — plain kit at rest.

## Dusk Doctrine
Duskmourn rites are about **gloom as erosion**: enemies carry GLOOM stacks that dim their strength and vision. The pilgrim is a lantern in the dark — weakening what approaches, healing by consuming sorrow.

**Passive — Dusk-Kin:** +night vision; Gloom you apply lasts 3s longer.

## Tier 1 — The Wick
1. **GLOAM TOUCH** — 5 FAITH. Melee applies 1 Gloom stack (max 5): enemy deals 4% less damage per stack.
2. **LANTERN WARD** — 8 FAITH. Ghost-light shell for 6s; on break, releases 3 wisps that terrify adjacent enemies (flee 2s).
3. **CANDLE'S MEMORY** — Passive. Regenerate slowly near lanterns, graves, and shrines.

## Tier 2 — The Dimming
4. **SHADOW STEP** — 12 FAITH. Dash THROUGH enemies (phase — no collision); enemies passed through gain 1 Gloom.
5. **WISP PALM** — 15 FAITH. Cone of cold crimson wisp-light — terror: enemies flee or freeze in place 2s.
6. **GRIEF MEND** — 10 FAITH. Consume all Gloom on one enemy; +2% damage per stack to you for 8s.

## Tier 3 — The Veil
7. **UMBRA EDGE** — 18 FAITH. Weapon trailing soft crimson light for 10s: +30% damage, melee applies 2 Gloom.
8. **DUSK VEIL** — 14 FAITH. Shadow-form 3s — invisible, next attack from it is a guaranteed critical.
9. **BARROW MARK** — 20 FAITH. Ground rune: a circle of gloom — enemies inside are slowed 30% and their attacks have a 20% miss chance.

## Tier 4 — Capstone
10. **WAKING LANTERN** — 35 FAITH, ultimate, 90s cooldown. 12s as the god's vessel: you become the lantern — all Gloom in a wide radius is pulled toward you (enemies dragged), enemies feared, allies wreathed in protective wisp-light. Ends with the light flaring — knockback + all consumed Gloom dealt as damage.

## Stat pins (engine source of truth)
- Gloom: -4% enemy damage per stack, max 5; no DoT — erosion currency.
- FAITH pool 100 base +10/tier; ult = only hard-cooldown rite.
- Synergies: Wayfarer (SHADOW STEP phase-dodge chains), Cantor (BARROW MARK + bell stun lock), Vigil (gloom miss-chance + shield = wall).
- Visual budget: wisp = floating light particle (Luminary-mote recolor), gloom = purple dimming shader on enemy, fear = existing AI flee state, veil = opacity drop. No new meshes/skeletons.
