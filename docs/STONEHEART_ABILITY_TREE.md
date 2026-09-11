# STONEHEART — Faction Ability Tree (v1 CANON)

> Unlocked by pledging the flame Mark to THE STONEHEART PATRON GOD — THE IRON KING (Grathwyn).
> Resource: FAITH. Free re-pledge. Borrowed stone — plain kit at rest.

## Stone Doctrine
Stoneheart rites are about **crystal as debt**: CRYSTAL SHARDS planted in enemies and ground are stored damage — the tree shatters them on command for burst, reflection, and terrain. Everything you strike owes you a break.

**Passive — Stone-Kin:** +poise (resist knockback); shards you plant last 3s longer.

## Tier 1 — The Seam
1. **SHARD TOUCH** — 5 FAITH. Melee plants a Crystal Shard in the enemy (max 5) — grows quietly until shattered.
2. **GEODE WARD** — 8 FAITH. Crystal shell for 6s; reflects 20% of absorbed damage back to attackers; on break, shards spray: 1 shard planted in each adjacent enemy.
3. **MOUNTAIN BLESSING** — Passive. Regenerate slowly near bare stone, ruins, and mountain shrines.

## Tier 2 — The Fault
4. **TREMOR STEP** — 12 FAITH. Dash that ends in a stomp — mini-quake staggers enemies in a small radius.
5. **QUARTZ PALM** — 15 FAITH. Cone of crystal splinters from the open hand — piercing line damage; each enemy hit gains 1 shard.
6. **SHARD MEND** — 10 FAITH. Shatter all shards on one enemy: 5 damage per shard, heal 2 HP per shard.

## Tier 3 — The Strata
7. **CRYSTAL EDGE** — 18 FAITH. Weapon veined with soft geode light for 10s: +30% damage, melee plants 2 shards.
8. **STONE VEIL** — 14 FAITH. Petrify-form 2s — a statue: immune to all damage, immobile; exiting shatters the shell outward (small knockback).
9. **FAULTLINE MARK** — 20 FAITH. Ground rune: a crack line — when triggered, crystal spikes erupt along it, knocking enemies up.

## Tier 4 — Capstone
10. **WAKING MOUNTAIN** — 35 FAITH, ultimate, 90s cooldown. 12s as the god's vessel: you ROOT — unmovable, unbreakable (75% damage reduction) — and the land answers: crystal spikes radiate outward on a pulse, ALL shards on the field shatter at once, and enemies striking you plant shards on themselves. Ends with the ground settling — everything still standing is knocked down.

## Stat pins (engine source of truth)
- Shard: 5 damage on shatter; 5 max per enemy; 12s lifetime.
- FAITH pool 100 base +10/tier; ult = only hard-cooldown rite.
- Synergies: Vigil (rooted ult + shield = the Gate itself), Smith (SHARD MEND lifesteal loop on hammer arcs), Keeper (GEODE WARD reflect keeps the reliquary lit).
- Visual budget: shard = small crystal prop/mote, spikes = ground decal + crystalline particle column (one shader), petrify = texture swap to stone material, quakes = camera shake (already built for Giant mode). No new meshes/skeletons.
