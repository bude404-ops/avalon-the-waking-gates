# MARENTH — Faction Ability Tree (v1 CANON)

> Unlocked by pledging the flame Mark to THE MARENTH PATRON GOD — THE TIDE QUEEN (Ylsanne).
> Resource: FAITH. Free re-pledge. Borrowed tide — plain kit at rest.

## Tide Doctrine
Marenth rites are about **momentum as a meter**: the pilgrim carries a personal TIDE that RISES with aggression (attack, move) and CRASHES on command into surges, knockdowns, and heals. The wave is always coming — decide when it lands.

**Passive — Tide-Kin:** +speed in water/rain; tide builds 10% faster in wet zones.

## Tier 1 — The Shallows
1. **RIPPLE TOUCH** — 5 FAITH. Melee adds +1 to your TIDE meter (max 10). Every hit makes you 2% faster and 2% harder-hitting.
2. **TIDE WARD** — 8 FAITH. Sheer of water for 6s; absorbs hits; on break, a wave pushes all adjacent enemies back 3m.
3. **DEEP BLESSING** — Passive. Regenerate slowly near water, wells, fountains, and sea-shrines.

## Tier 2 — The Swell
4. **CURRENT STEP** — 12 FAITH. Dash that flows THROUGH enemies (no collision); each enemy flowed through adds +2 TIDE.
5. **TIDAL PALM** — 15 FAITH. Cone of pressurized water from the open palm — knockdown + 2s wet (wet enemies take +15% from the tide).
6. **SURGE MEND** — 10 FAITH. Crash your tide early: heal 3 HP per TIDE consumed, meter resets.

## Tier 3 — The Break
7. **CORAL EDGE** — 18 FAITH. Weapon rimmed with quiet pearl-teal light for 10s: +30% damage, melee builds TIDE twice as fast.
8. **MIST VEIL** — 14 FAITH. Fog-form 3s — untargetable; exiting releases a spray that wets everything nearby.
9. **WHIRLPOOL MARK** — 20 FAITH. Ground rune: a pool that pulls enemies to its center every second.

## Tier 4 — Capstone
10. **WAKING WAVE** — 35 FAITH, ultimate, 90s cooldown. 12s as the god's vessel: your meter burns to 10 instantly and a tide crashes outward in ALL directions — massive knockup + damage; allies in the wave are healed instead. For the duration your tide never falls below 5. Ends with the water receding — dragging grounded enemies.

## Stat pins (engine source of truth)
- TIDE meter: 0-10, +2%/stack speed AND damage; decay -1 per 3s out of combat.
- FAITH pool 100 base +10/tier; ult = only hard-cooldown rite.
- Synergies: Wayfarer (CURRENT STEP + dash = tide machine), Vigil (WHIRLPOOL MARK holds enemies on shield), Smith (meter feeds 2H wind-up heavies).
- Visual budget: tide = existing water shader + spray particles, ward = translucent shell (Cinder Ward recolor), whirlpool = ground decal + spiral particles. No new meshes/skeletons.
