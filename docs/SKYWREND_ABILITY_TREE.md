# SKYWREND — Faction Ability Tree (v1 CANON)

> Unlocked by pledging the flame Mark to the Skyrend deity (Haeldor / Sylwenna).
> Resource: FAITH. Free re-pledge — progress persists. All effects are BORROWED storm — plain kit at rest, glow is the effect, never an engraving.

## Storm Doctrine
Skyrend rites are about **charge as connection**: enemies carry STATIC CHARGES, and the tree connects them — arcing, chaining, teleporting between charged foes. Control the web of charges and you control the field.

**Passive — Storm-Kin:** +move speed during rain/storm zones; Static Charges you apply last 3s longer.

## Tier 1 — The Front (Mark awakened)
1. **STATIC TOUCH** — 5 FAITH. Melee hits apply 1 Static Charge (stacks to 5). At 5 stacks, charge arcs to the nearest other charged enemy.
2. **SQUALL WARD** — 8 FAITH. Wind shell for 6s; deflects ranged attacks; on break, knocks adjacent enemies back and applies 1 Charge.
3. **THIN AIR** — Passive. Regen and stamina recovery faster at elevation (walls, ridges, platforms).

## Tier 2 — The Current
4. **BOLT STEP** — 12 FAITH. Instant dash TO a charged enemy (travel through the arc, not through space).
5. **THUNDERCLAP PALM** — 15 FAITH. Concussive shockwave from the open palm — knockdown cone; grounded enemies gain 2 Charges.
6. **ARC MEND** — 10 FAITH. Drain all Charges from one enemy; restore 5 stamina + 5 FAITH per charge consumed.

## Tier 3 — The Squall
7. **TEMPEST EDGE** — 18 FAITH. Weapon crackles with quiet storm-light for 10s: +30% damage, melee applies 2 Charges.
8. **STORM VEIL** — 14 FAITH. Body becomes wind-wrapped for 3s — projectiles miss, movement is free.
9. **RINGING MARK** — 20 FAITH. Ground rune that rings like a bell — enemies crossing it are stunned and take arc damage from every charged enemy in the fight.

## Tier 4 — Capstone
10. **WAKING TEMPEST** — 35 FAITH, ultimate, 90s cooldown. 12s as the god's vessel: storm aura auto-arcs lightning between all charged enemies, Ashfall-cost discount 50% on Skyrend rites, dodge speed +25%. Ends with a thunderclap knockdown.

## Stat pins (engine source of truth)
- Static Charge: no DoT — connection currency. Arc = 4 damage per linked charge pair, 0.5s tick.
- FAITH pool 100 base +10/tier; ult = only hard-cooldown rite.
- Synergies: Wayfarer (Bolt Step chains), Cantor (RINGING MARK overlaps bell resonance), Keeper (ARC MEND restores FAITH — lantern keeper economy).
- Visual budget: arc = line particle (one shader), charges = small spark mote on enemy, aura = existing ember-aura recolored storm slate. No new meshes/skeletons.
