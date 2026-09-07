# PILGRIM COMBAT SYSTEM v1 — The Avalon Fusion

## CLASS ARMOR LAW (Sept 7 2026, Big: 'armors to fit their class aswell so assassin type is leathers casters are some armor but robes')

Armor type follows class role — the Cinder Roads aesthetic (road-worn, riveted, ash-patina) stays, but the material grammar is fixed per class:

| Class | Size | Weapon | Armor Grammar |
|---|---|---|---|
| Wayfarer | SMALL | twin way-daggers | LEATHERS — hooded stitched leather, quiet cloth, zero metal clang. Assassin-type. |
| Warden | MEDIUM | sword + shield | mid plate — the road-knight kit (existing GLB canon) |
| Keeper | MEDIUM | flail + lantern | medium lamellar + harness — the wardener's rig |
| Cantor | TALL/LEAN | bell-staff | ROBES + light armor — reinforced mantle, bracers over cloth. Caster-type. |
| Vigil | TALL | greatbow | ranger leathers + longcloth — hunter quiet |
| Smith | HEAVYSET | greathammer | heavy plate + smith's apron — forge-built, stagger-built |

Law: light classes wear quiet, heavy classes wear metal. No class wears armor outside its grammar. Pilgrim armor is Avalon human-craft: mortal smithing, road-worn repair-plate.
BudE404 directive (Sept 6 2026): mix Diablo Immortal + Genshin into our own style, wrapped in Avalon lore, then fix combat + stats to match.

## The Three Layers

### Layer 1 — DIABLO IMMORTAL (the action foundation)
- **Auto-face lock-on**: every attack auto-faces the nearest Hollow — no thumb gymnastics, joystick only for movement/exploration
- **Skill bar**: 3 ability slots + ultimate on cooldowns (cooldown speed scales with TEMPO)
- **Wave structure**: Thrall waves → elite (Warden Hollow) → boss (risen Echo/god-scale per Emergence Law)
- **Loot-lite**: tribute relics from the Depths (relic-gift altar already canon in Depths prototype)

### Layer 2 — GENSHIN (the reaction layer)
- **Elemental identity**: the pilgrim channels ONE element through the Mark (ember/storm/bloom/dusk/tide/stone) — re-attuned at camp/shrine (solo-first answer to Genshin's team swap). THE LANTERN IS THE LUMINARIES (BudE404, Sept 6): the lantern is a living being, never a tool or oil-swapped prop — the Luminary's flame mirrors whatever element the Mark carries, because the flame is kin to that element
- **ELEMENTAL REACTIONS**: status-on-status = burst effects:
  - ember + tide = STEAMVEIL (blind + area denial)
  - storm + stone = SHATTER (guard break)
  - bloom + dusk = SPOREBLOOM (chain damage)
  - ember + storm = ASHSTORM (damage-over-time whirl)
  - tide + stone = LODESTONE (pull/anchor)
  - bloom + tide = ROOTMERE (root + heal)
- Reactions do the big damage — re-attuning the Mark answers enemy resistances
- **Stamina dodge** with i-frames (separate from abilities)

### Layer 3 — AVALON (the lore wrapper — ours alone)
- **FAITH ECONOMY LAW v2 — THE MACER'S LOOP** (BudE404, Sept 6: 'that Diablo dungeon masher feel so instead of mana we just have faith and abilities use faith and attacking and killing builds it back up'): FAITH is the ONLY resource — there is NO mana, NO passive regen bar. THE LOOP: attacking BUILDS faith (every landed hit feeds the flame), killing BUILDS it bigger (mote burst), the WARD converts caught damage INTO faith (the more you're sieged, the more you can answer — unchanged canon), and abilities SPEND it. The pilgrim is always swinging to afford the next miracle — aggression is the economy. This is the Diablo dungeon-masher loop transplanted whole: hatred/essence/fury/spirit -> FAITH.
- **WEAPON/SKILL TREES FIT THE LOOP**: every weapon tree node and ability is either a BUILDER (more faith per hit/kill/parry — e.g. a Warden parry node that deepens the +12 catch bonus, a Smith node that adds burn-ticks that generate faith) or a SPENDER (faith-cost miracles — surges, reactions, relic verbs), gated by FAITH not long cooldowns. Cooldowns stay SHORT (TEMPO shrinks them) — the resource gate is the faith cost. Kill-burst values scale so chaining packs keeps the bar full (the masher's momentum high).
- **FAITH DROPS** (BudE404, Sept 6 — the loot feel): defeated Hollow RELEASE their faith as golden motes that visibly float and stream INTO THE LUMINARY, as if the living lantern collects them — the Diablo-gold-drop satisfaction, but the "gold" is belief and the "purse" is your companion. The Luminary gathers; the FAITH lands when the motes arrive (bar ticks per mote, soft chime), not on the kill frame. Lore-perfect: the flame feeds on deeds.
- **THE TRIBUTE SYSTEM — progression economy** (BudE404 canon, Sept 6 night: 'in-agree on the gear, the grind part sucks and stats would suck and we would be building more gear etc.'): NO stat-gear treadmill — zero random rolls, zero affix loot, zero gear power creep; there is NO drop RNG in AVALON. Kills drop TWO currencies: FAITH motes (in-run — unchanged) and TRIBUTE (persistent — occasional, tied to notable kills: elites, first-clears, Warden Hollows, siege waves; reads as physical offerings to the god, tribute-reef lore). TRIBUTE spends at SHRINES: class-tree nodes (builders/spenders), relic tiers (Cold Lantern line), Mark re-attunement rites, cosmetic rune colorways. Loadout is CURATED: WEAPON (swappable kits — Blade/Dagger/Hammer/Lantern/Song/Tower class trees; weapons are separate hand-bone meshes for exactly this), ARMOR (one set per role per gender — the canon 12; faction = rune colorway), RELIC (deed-earned). Power curve = trees + relic tiers + Mark choices — never drops. The DEPTHS = the grind space where tribute flows richest (optional — never gates the story).
- **Debt economy** (canon from combat runtime): Hollow ACCRUE DEBT when struck; at threshold a Hollow REVENANTS (enrages) — kill fast or get swarmed by rage.
- **Void tether**: Hollow are void-element, reaction-IMMUNE until their tether is SEVERED. CANON ROLES (per the ONE COMPANION TWO LIGHTS ruling): the LUMINARY REVEALS the tether — its glow is discovery/navigation light, NEVER combat power — and the PILGRIM severs what it reveals (strike the shown tether-point). The lantern's combat-adjacent role stays PROTECTIVE only: while the Luminary shines, void-chill does not drain VIGOR. The lantern never attacks.
- **Emergence Law**: god-scale = stationary siege combat (unchanged canon).

## The Stat Block

### Pilgrim
| Stat | Meaning |
|---|---|
| VIGOR | health |
| FAITH | the ONLY resource — abilities spend it; landed hits, kills (mote bursts), ward catches build it — no mana, no passive regen |
| MIGHT | melee/combo/finisher damage |
| WIT | ability + reaction damage |
| GUARD | damage mitigation (Cinder Roads plate) |
| TEMPO | cooldown speed + dodge cost |

### Hollow
| Type | Role | Notes |
|---|---|---|
| Thrall | swarm melee | low vigor, lunge telegraph |
| Chorister | caster | applies VOID-TETHER (reaction immunity aura) |
| Warden Hollow | elite blocker | parry-break or bash to open |
| Revenant | any type enraged | debt threshold crossed: +speed +damage |

### Slice baseline numbers (Warden-M vs Hollow Thrall)
- VIGOR 100 · FAITH 35 start · strike 22 · heavy 46 (cd 2.4s) · parry window 1.1s (+12 FAITH on catch) · bash 16 + knockback (cd 4s)
- Thrall: VIGOR 34 · lunge 9 · approach 2.4→3.0
- WARD: absorbs any caught lunge → +8 FAITH (already live in slice)

## Implementation order
1. Auto-face lock-on + cooldown HUD (slice patch 2 — replaces free-joystick combat)
2. TEMPO/GUARD stat wiring + elite Warden Hollow with guard-break
3. Void-tether severing (lantern toggle mid-fight)
4. Mark re-attunement (camp/shrine) + first 2 reactions (ember+storm, ember+tide); Luminary flame mirrors the Mark; Luminary tether-REVEAL + pilgrim-sever loop
5. TRIBUTE economy (Unity): TributeMeter (notable-kill drops), ShrineVendor (spend: tree nodes / relic tiers / re-attunement), Depths drop-tables

## SIZE LAW (+ BODY-TYPE LAW: distinct builds per class, not just heights — see PILGRIM_DESIGN_DOCTRINE.md) — weight classes as role language (BudE404, Sept 7 2026)
Weapon role = silhouette. Size is readable depth: you SEE what a unit does before it swings.

### Pilgrim weight classes
| Class | Weapon | Size class | Combat identity |
|---|---|---|---|
| Wayfarer | twin way-daggers | SMALL | fastest TEMPO, faith builds in a stream of small hits, longest/fasted step-dodge |
| Warden | sword + shield | MEDIUM | balanced read |
| Keeper | flail + lantern | MEDIUM | mid |
| Cantor | bell-staff | TALL/LEAN | reach advantage at pole length |
| Vigil | greatbow | TALL | ranged reads |
| Smith | greathammer | HEAVYSET (largest) | few huge hits, faith in bursts, every swing staggers |

### Mechanic scaling (per weight class)
1. TEMPO + dodge distance + i-frame window scale with size (small = far/fast dodge; heavy trades dodge for poise)
2. Faith-per-hit tuned per class — SAME Macer's Loop economy, different rhythm (stream vs burst)
3. Poise/stagger thresholds scale with size — big units shrug off hits that fold small ones

### Hollow size variants
Same-tier variants (elder-hunched vs broad-smith ERASED) = scale + speed verb + attack set.
T1 trash comes in lean and broad flavors. Wave variety WITHOUT new models. Reads instantly in a mob.

### Implementation notes
- Hitboxes scale with size class (Unity capsule/collider sizing per kit)
- Enemy variant spawn tables: per-realm bestiary units carry a sizeVariant field (lean | broad | standard)
- Pilgrim kits: proportions land at model-roll time (remaining kits roll per the Canon-Reference Art Law)
