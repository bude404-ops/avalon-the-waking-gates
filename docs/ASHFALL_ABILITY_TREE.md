# ASHFALL — Faction Ability Tree (Draft v1)

> Unlocked by pledging the flame Mark to THE ASHFALL PATRON GOD — THE EMBER KING (Vaelthorn).
> Resource: FAITH. Free re-pledge — progress persists per faction tree.

## Ember Doctrine
Ashfall rites are about **burn as economy**: enemies carry Cinder Stacks (burn), and half the tree *spends* what the other half *applies*. Aggression heals, momentum burns, and the capstone turns the pilgrim into the kiln itself.

**Passive — Ember-Kin:** +fire resistance; Cinder Stacks you apply last 3s longer.

## Tier 1 — The Kindling (Mark awakened)
1. **EMBER-TOUCH** — 5 FAITH. Melee hits apply 1 Cinder Stack (burn DoT, stacks to 5).
2. **CINDER WARD** — 8 FAITH. Ash shell absorbs damage for 6s; when it breaks, applies 2 Cinder Stacks to all adjacent enemies.
3. **KILN'S WARMTH** — Passive. Regenerate slowly while near braziers, shrines, or any lit flame.

## Tier 2 — The Heat
4. **ASHFALL STEP** — 12 FAITH. Short dash leaving an ember trail; enemies crossing it gain Cinder Stacks.
5. **FURNACE PALM** — 15 FAITH. Cone of flame from the free hand — the Keeper's grammar (power lives in the open palm).
6. **SPARK MEND** — 10 FAITH. Detonate all Cinder Stacks on one enemy; heal 2 HP per stack consumed.

## Tier 3 — The Blaze
7. **MOLTEN EDGE** — 18 FAITH. Weapon wreathed in quiet ember-light for 10s: +30% damage, melee applies 2 stacks. (Weapon stays honest iron — the fire is borrowed, not stamped.)
8. **CINDER VEIL** — 14 FAITH. Collapse into drifting ash for 3s — enemies lose you; leaving the veil releases a spark burst.
9. **PYRE MARK** — 20 FAITH. Plant a small flame rune on the ground; enemies crossing it detonate their stacks.

## Tier 4 — Capstone
10. **WAKING FURNACE** — 35 FAITH, ultimate. For 12s the pilgrim burns as the god's vessel: ember aura applies stacks to everything nearby, all Ashfall rites cost 50% less, damage taken reduced 25%. Ends with an ash-burst knockback.

## Tuning notes
- Full tree = 10 rites (within the ~10-12/faction cap; 36 builds stay readable).
- Cinder Stack = 1.5 HP / 2s per stack, max 5 stacks.
- Synergy hooks: Wayfarer (mobility + ember trail), Smith (Molten Edge on 2H hammers), Vigil (Cinder Ward + shield turtle).
- Shadow of the god: WAKING FURNACE is the pilgrim-scale echo of the Waking Siege — borrowed flame, never owned.

---

# COHERENCE AUDIT v1 (post-keep)

## Lore fit — all rites trace to canon
- FAITH economy is already the doctrine resource (banked at shrines, earned through the pilgrimage) — every cost in the tree draws from it. No new currency.
- The flame Mark on the breastplate is the pledge mechanism (Build System v2: FACTION = ABILITIES, unlocked by the Mark). The tree reads as the god answering the pledge — no new lore machinery.
- The pilgrim's identity laws hold: no permanent elemental mutation. Ashfall effects are BORROWED flame — channelled during rites, gone when they end. "Borrowed, never owned" is the doctrine line, matching the vessel role in the Gate Rite.
- WAKING FURNACE = the pilgrim-scale echo of the Waking Siege, consistent with "the pilgrim is the conductor, not the god."
- Cinder Stacks are Hollow-corruption being burned away — purging nightmare-blight with the god's flame fits the Hollow doctrine (the blight is a god's nightmare; fire is Ashfall's answer).

## Gameplay fit — hooks into existing loops
- FAITH costs (5-35) sit inside the shrine-banking loop: light rites are spammable between banks, the ult requires deliberate saving. No infinite-sustain loop (stacks need melee application).
- Every rite is usable in BOTH scales: Pilgrim dungeon crawl (the Depths) and Giant siege modes — the tree never assumes a mode.
- Order synergies are real, not cosmetic: Wayfarer's dash extends Ashfall Step trail; Smith's 2H swing applies Molten Edge in an arc; Vigil's shield holds enemies inside Pyre Mark radius; Cantor's Bell-Staff resonance can extend stack duration (future cross-tree node).

## Stat baseline (v1 tuning anchors)
- FAITH pool: 100 base, +10 per pilgrim tier (shrine-banked, slow regen out of combat only).
- Cinder Stack: 1.5 HP / 2s per stack, max 5 stacks, 8s base duration (Ember-Kin: +3s).
- WAKING FURNACE: 12s duration, 90s cooldown, 35 FAITH — the only rite on a hard cooldown (ult discipline).
- All numbers are placeholders pinned in one table for engine implementation; single source of truth in this doc.

## Visual budget — what we can actually render
Every rite uses effects we already have or can build cheaply — no new models, no new skeletons, no wing-words:
- EMBER-TOUCH / stacks: ember particle on hit + small burn puff on the enemy. Reuses the ember-mote particle system (already built for the Luminary pet).
- CINDER WARD: soft ash-grey shell shader around the pilgrim, cracks ember on break. One material.
- ASHFALL STEP: dash + ember trail decal/particles — same tech as the existing movement glow in the Giant mode.
- FURNACE PALM: cone-shaped particle burst from the free hand — matches the Keeper's palm-orb grammar already canonized in the armor art.
- MOLTEN EDGE: weapon emissive texture swap + ember drip particles. Weapon stays plain iron at rest — flame-only glow law respected (glow is the borrowed effect, not an engraving).
- CINDER VEIL: pilgrim opacity drop + drifting ash particle shroud. No skeleton change, cheap.
- PYRE MARK: ground decal rune + flame wall particles — same decal system as shrine braziers.
- WAKING FURNACE: full-body ember aura + light radius + screen-edge heat shimmer. The most expensive single effect, but it's one ultimate — 12s, on cooldown, one shader + particles. Mobile-safe.
- NOTHING in the tree requires a new mesh, new animation skeleton, or any art that breaks the solo/weapon/armor canon. All effects are temporary flame — the pilgrim's kit stays honest human-craft iron at rest, per the doctrine.

## Verdict: tree KEPT with this audit as the binding spec. Engine implementation pulls numbers only from this doc.
