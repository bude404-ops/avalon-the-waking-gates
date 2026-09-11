# MYTHOS GATES: ASCENSION — GAME DESIGN DOCUMENT (GDD)
**Version:** 2.0  
**Date:** August 19, 2026  
**Status:** Production-Ready  
**Platform:** Mobile (iOS/Android) with cross-platform expansion capability  
**Genre:** Mythological Tactical Dungeon-Crawler / Action-RPG  
**Visual Style:** 2.5D Grim Dark Mythological Fantasy  

---

## 1. GAME OVERVIEW

### 1.1 Logline
Choose a deity. Project your Avatar through the Mythos Gates. Cross into enemy Realms, fight through divine dungeons, and battle mythological horrors. Earn Belief and Influence to grow your Avatar's power. Survive the Ascension.

### 1.2 Core Loop
Select Deity → Project Avatar → Enter Dungeon Route (home, enemy, or Earth) → Fight Through Rooms (Combat + Hazards + Lore) → Defeat Boss → Earn Belief/Influence → Return to Hub → Upgrade Avatar/Unlock Abilities → Select Harder Route

### 1.3 Core Pillars
1. **One Avatar, One Deity** — Single-avatar mastery. You command the Avatar of a god. No squad mechanics. The Deity is safe; the Avatar fights.
2. **Mythological Authenticity** — Every deity, creature, and Realm is drawn from real mythology, refracted through grim-dark divine lens.
3. **Route Variety** — Same dungeon plays completely differently per deity. 28 deities = 28 playthroughs per route.
4. **Earth Campaigns** — All missions take place in each faction's mythological homeland on ancient Earth. Neutral ground, no faction advantage. Faction realms are event-only.
5. **Sacred Stakes** — No jokes. No meta. Ancient civilizations handling sacred catastrophe.
6. **Progressive Depth** — Easy to pick up, deep to master. Combat is readable on mobile but rewards skill.

---

## 2. COMBAT SYSTEM

### 2.1 Combat Format
- **Perspective:** 2.5D isometric camera (slight angle, looking down at the battlefield)
- **Scale:** Avatars are GOD-SCALE everywhere (hundreds of feet tall). Camera frames the Avatar and 4-8 enemies at a time. Environmental scale cues (tiny buildings, mountains at waist height, clouds at chest level) communicate colossal size.
- **Movement:** Touch-stick or tap-to-move with auto-facing
- **Attacks:** Auto-attack (basic) + 3 Active Abilities + 1 Ultimate
- **Passive:** 1 innate Passive that defines the Avatar's playstyle identity

### 2.2 Ability Kit Structure
Every Avatar has:
| Slot | Type | Description |
|------|------|-------------|
| Passive | Innate | Always active, defines core identity |
| Ability 1 | Active | Bread-and-butter combat ability (low cooldown) |
| Ability 2 | Active | Tactical/positioning ability (medium cooldown) |
| Ability 3 | Active | Heavy impact ability (long cooldown) |
| Ultimate | Active | Room-clearing or boss-phase ability (charged via combat) |

### 2.3 Combat Roles (4 Roles)
| Role | Playstyle | Key Stat |
|------|----------|----------|
| Warrior | Tanky frontline, block, absorb, zone control, lifesteal | Armor/HP |
| Caster | Ranged AoE, ability combos, terrain manipulation, CC | Divine Energy |
| Archer | Long-range sustained DPS, mark stacking, kiting, pierce | Attack Speed |
| Assassin | Burst, stealth, backstab, execute, mobility | Crit/Speed |

See `docs/ROLE_IDENTITY_SYSTEM.md` for full role design.

### 2.4 Resource System
- **Divine Energy (DE):** Primary combat resource. Regenerates over time + on hit. Abilities cost DE.
- **Ultimate Charge:** Fills via dealing/taking damage. When full, Ultimate is available.
- **Belief:** Meta-progression resource. Earned from victories, followers, realm control. Spent to level Avatar stats.
- **Influence:** Meta-progression resource. Earned from missions, battles, territory. Spent to unlock/upgrade abilities.
- **No Mana/No Cooldown-only:** FAITH gates ability use (the Macer's Loop — attacking and killing build it, abilities spend it; there is no other resource), not arbitrary cooldowns. Some abilities have mechanical cooldowns for balance.

### 2.5 Damage Types
| Damage Type | Description | Counter |
|-------------|-------------|---------|
| Divine | Raw divine power | Divine Resistance |
| Storm | Lightning, thunder, wind | Storm Warding |
| Solar | Sun-fire, radiance, light | Solar Shielding |
| Frost | Ice, cold, freeze | Frost Immunity |
| Nature | Root, thorn, growth, poison | Nature Warding |
| Spirit | Soul, phase, mirror | Spirit Binding |
| Infernal | Hellfire, ash, chain | Infernal Sealing |
| Void | Hollow anti-light, erasure | Void Anchoring (rare) |

### 2.6 Status Effects
| Effect | Mechanic |
|--------|----------|
| Stagger | Enemy cannot act for 1.5s |
| Knockback | Enemy pushed back |
| Slow | Movement -40% for 3s |
| Burn | DoT — damage over time |
| Freeze | Immobilized 2s, shatters for bonus damage |
| Stun | Cannot act for 2.5s |
| Marked | Takes +25% damage from all sources |
| Corrupted | Healing reversed (takes damage instead of healing) |
| Hollowed | Identity drain — abilities cost +50% FAITH for 4s |

### 2.7 Enemy AI Tiers
| Tier | Behavior |
|------|----------|
| Trash | Swarm, basic attack, die fast |
| Elite | Telegraphed attacks, 2-3 ability rotations, moderate HP |
| Mini-Boss | Phase mechanics, enrage timers, requires ability use |
| Boss | Multi-phase, room mechanics, lore-driven encounters |
| World Boss | Server-event scale, unique mechanics, legendary |



## 3. PROGRESSION SYSTEM

### 3.1 Avatar Progression
- **Level:** 1-50 per Avatar. Gained by spending Belief earned through route completion, combat, and lore pickups.
- **Level Bonuses:** +HP, +DE, +Damage per level. No skill trees (keep it mobile-simple).
- **Ascension Rank:** After level 50, Avatars enter Ascension tiers (1-10). Each tier unlocks a passive enhancement slot. Ascension requires high Influence.

### 3.2 Belief System
- **Belief** is the spiritual fuel that powers the Avatar's connection to its Deity
- **Architecture:** Hybrid — Faction Base (+10%) + Deity Faith (+20% lore-specific trigger) + Fallback (+5%)
- **All triggers are SOLO-ONLY** — no ally-dependent mechanics
- **9 Faith Triggers:** Endurance, Conduit, Dominion, Fracture, Disruption, Range, Counter, Shadow, Bulwark
- **32 Deity Faiths:** each of the 32 live kits carries a unique faith identity (roster: data/deities/ — e.g., Shemris the Glasswind, heat-shimmer goddess of the Meridian Court)
- **Faction Belief Config:** Each faction has passive bonus, unique trigger, 3 tiered buffs, and Pantheon Resonance
- **Max Belief per win:** 145 (base + passive + faith + trigger + resonance)
- **See:** docs/FACTION_BUFFS.md (faction bonuses) + data/factions/ (live roster)

- **Spent on:** Avatar level-ups (HP, DE, ATK, DEF, SPD — free allocation, no forced paths)
- **Full config:** docs/lore/07-BELIEF-SYSTEM.md + docs/lore/08-FACTION-BELIEF-CONFIG.md
- **Data files:** data/belief-system.json + data/faction-belief-config.json

### 3.3 Influence System
- **Influence** is the territorial and political power the Avatar accumulates
- Earned from: completing missions, winning battles, spreading the Deity's domain, defeating rival Avatars
- Spent on: unlocking new abilities, upgrading existing abilities, relic enhancement, Ascension tier unlocks
- **Full config:** data/ascension-system.json (influence_system section)

### 3.4 Relic System
- Relics are route rewards — divine items tied to mythology
- 3 Relic slots per Avatar: Weapon, Armor, Artifact
- Relics have rarity tiers: Common, Rare, Epic, Legendary, Mythic
- Relics provide stat bonuses + sometimes modify ability behavior
- Relics are deity-locked (Aten Ra relics only work on Aten Ra Avatars)

### 3.5 Gate Shard Currency
- **Gate Shards:** Earned from completing routes, defeating bosses, daily challenges
- Spent on: Relic rerolls, cosmetic unlocks, new deity contracts, route keys
- **Lore Shards:** Rare currency from lore pickups — used to unlock story chapters and realm codex entries

### 3.6 Hub System
- The **Crossroads** — a neutral space between Realms where the player manages their roster
- Features: Deity selection, Avatar customization, Relic management, Route selection, Codex (lore library), Daily Challenges
- Visual: A shattered Mythos Gate plaza with portals to each Realm and to Earth

### 3.6.1 Deity Unlock System
- **Starting Deity:** Player picks 1 of 7 starter deities (one per faction, all Warriors — simplest mechanics)
- **Campaign Unlocks:** 21 deities unlock through Acts I-III (3 per faction per Act chapter)
- **Faction Mastery:** 3 deities unlock by leveling starter to 10/20/30
- **Gate Shard Purchase:** Any locked deity can be unlocked early (250-1,000 Gate Shards)
- **All 28 deities are 100% free to unlock** — no paywall on gameplay content
- **See:** `docs/DEITY_UNLOCK_SYSTEM.md` for full details
- **Data:** `data/deity-unlock-system.json`

### 3.7 Death and Respawn
- **Avatar Death:** The Avatar falls, not the Deity. The Gate returns the divine essence to the home Realm.
- **Respawn:** Player respawns at their Deity's domain after a cooldown period
- **Penalties:** Loss of 10-20% of unspent Belief and Influence; cooldown timer before re-deployment
- **Preserved:** Avatar level, unlocked abilities, relics, and all progression
- **Permadeath Tiers:** Only in Gate Failure difficulty (Tier 5) does death carry heavier consequences

---

## 4. CAMPAIGN STRUCTURE

### 4.1 Route Architecture
7 playable routes, one per Realm, plus Earth as neutral dungeon territory:

| # | Route Name | Realm | Gate State |
|---|-----------|-------|------------|
| 1 | Sun-Scale Verdict Descent | Aten Ra | Unstable (sun-bleeding) |
| 2 | Thunder-Oath Root Gauntlet | Asgardian | Ruin Gate (oath memory active) |
| 3 | Laurel-Sky Hubris Trial | Olympian | Stable (oracle vapor leak) |
| 4 | Torii-Moon Mirror Road | Kami | Distortion (ritual rerouting) |
| 5 | Silver-Root Geas Labyrinth | Tuatha | Sealed (fae time bleed) |
| 6 | Choir-Vault Discord Ascent | Empyrean | Wound Gate (radiance leak) |
| 7 | Black-Iron Debt Descent | Infernal Dominion | Distortion (contract-bound) |
| 8 | Earth — Hollow Corridor | Earth (Neutral) | Multiple damaged Gates |

### 4.2 Route Structure
Each route contains:
- Entry Gate (transition screen — Avatar projection sequence)
- 7+ Room Nodes minimum
- Lughching paths (player chooses direction)
- Hazard rooms (Realm-specific traps)
- Combat rooms (enemy encounters)
- Shrine nodes (healing/buffs)
- Treasure rooms (relics/currency)
- Elite encounter (mini-boss)
- Lore reveal chains (story pickups)
- Boss chamber (Realm-specific boss)
- Expansion hooks (teasers for future content)

### 4.3 Mission Types
| Type | Description |
|------|-------------|
| Standard Route | Full dungeon crawl, boss at end |
| Threat Mission | Hollow/Beast Realm incursion — shorter, higher pressure |
| Gate Event | Time-limited event with unique mechanics |
| World Boss | Server-wide encounter, requires coordination |
| Lore Detour | Optional side-path with story rewards, minimal combat |
| Elite Lock | Room that requires specific role/ability to progress |
| Earth Dungeon | Neutral-territory Hollow invasion — balanced, no faction advantage |
| Realm Raid | Limited-time event in a faction realm — high risk, high reward |

### 4.4 Difficulty Tiers
| Tier | Name | Modifier |
|------|------|----------|
| 1 | Gate Breath | Base stats, learning mode |
| 2 | Gate Echo | +25% enemy HP/damage |
| 3 | Gate Strain | +50% enemy HP/damage, new enemy types |
| 4 | Gate Collapse | +75% enemy HP/damage, boss phases added |
| 5 | Gate Failure | +100% enemy HP/damage, heavier death penalties |

### 4.5 Campaign Chapters (19 Chapters)
- **Act I (Ch 1-6):** The First Reopening — Aten Ra + Asgardian + Olympian routes
- **Act II (Ch 7-13):** Deeper Descent — Tuatha + Kami + Empyrean routes + first World Boss + Earth dungeons unlock
- **Act III (Ch 14-19):** The Black Iron Bargain — Infernal Dominion + final World Bosses + Hollow climax on Earth

### 4.6 Campaign Gameplay Design
**See:** `docs/CAMPAIGN_GAMEPLAY_DESIGN.md` for full mission design

**Key Design Principles:**
1. Every mission uses a role mechanic (Warrior blocks, Caster combos, Archer marks, Assassin executes)
2. Each chapter teaches one deity's ability kit through a specific challenge
3. Bosses test specific abilities — each boss counters one playstyle and rewards another
4. Difficulty tiers 1-5 scale with deity level (Tier 5 = Level 30 builds only)
5. Faction unlock chapters (Ch 3, 4, 5, 7, 9, 11, 14) have 3 deity-specific challenges
6. Each deity has at least one "signature campaign moment" where their unique ability shines
7. Weapon skill tree paths are tested — Path A (burst) vs Path B (sustain) in Ch8 and Ch15
8. Faction buffs are tested — each faction unlock chapter forces the buff mechanic

**Mission Objectives by Role:**
- Warrior: Hold the Line, Survive, Protect — tests blocking, zone control, armor stacking
- Caster: Clear All, Chain Combos, Destroy — tests AoE, ability chaining, terrain manipulation
- Archer: Hunt Target, Kill Before Escape, Mark & Detonate — tests kiting, mark stacking, pierce
- Assassin: Kill Before Timer, Stealth Infiltration, Execute All — tests burst, stealth, execute

---

## 5. THREAT LAYER

### 5.1 The Hollow
- Primary cross-route antagonist
- Not a playable faction — an anti-civilization
- Consumes memory, identity, terrain law
- Punishes overextension, corrupts objectives
- Imitates consumed source-culture forms
- Visual: Void-mist bodies, white fracture cracks, anti-light absorption, Gate stone fragments as only solid anchors
- Primary invasion point: Earth (through damaged Gates)

### 5.2 Threat Categories
| Category | Role | Mechanics |
|----------|------|-----------|
| Hollow | Primary pressure | Identity drain, void corruption, anti-light |
| Forgotten | Memory loss | Erasure aura, frost spread, half-dissolved forms |
| Beast Realm | Wilderness threat | Pack mechanics, primal aggression |
| Gateborn | Anomaly threat | Portal mechanics, terrain distortion, mutations |
| World Bosses | Legendary encounters | Server events, unique mechanics |

### 5.3 World Bosses (4)
1. **Leviathan of the First Flood** — 200+ft primordial sea serpent, living stormwater body
2. **Hollow World-Wound Behemoth** — Colossal Hollow entity, void-mist colossus
3. **The Gate Guardian** — 300+ft living-stone sentinel, fused with broken Gate
4. **The Forgotten Giant** — 250+ft frost-iron Deity, walking erasure, half-erased face

---

## 6. FACTION STAT SYSTEM

### 6.1 Stat Categories
| Stat | Range | Effect |
|------|-------|--------|
| Health (HP) | 1000-5000 | Damage capacity |
| Divine Energy (DE) | 100-300 | Ability resource pool |
| Attack (ATK) | 150-800 | Base damage scaling |
| Defense (DEF) | 50-400 | Damage reduction |
| Speed (SPD) | 1-10 | Movement + attack speed |
| Crit Rate | 5%-35% | Critical hit chance |
| Crit Damage | 150%-300% | Critical hit multiplier |
| Penetration | 0-200 | Ignores enemy DEF |
| Recovery | 0-50 HP/s | Passive HP regen |
| Area | 1-10 | AoE radius modifier |

### 6.2 Stat Budget Rule
Total stat budget per Avatar = 1000 points (base). Role determines distribution:
- Defender: HP/DEF heavy
- Assassin: SPD/Crit heavy
- Artillery: ATK/Area heavy
- Battery: DE/Recovery heavy
- (etc. per role)



## 7. VISUAL SCALE

### 7.1 God-Scale Rule (Option A)
All Avatars remain GOD-SCALE everywhere, regardless of Realm. Both the Avatar and enemies should appear COLOSSAL — hundreds of feet tall.

Scale cues to communicate god-scale:
- Tiny city buildings, temples, and structures near the fighters' feet
- Mountains in the background that only reach the fighter's waist
- Clouds at chest level of the fighters
- Rivers with tiny boats in the far background
- Birds as barely visible specks near the fighters' knees
- Impact craters that crush multiple buildings
- Weapons as wide as city avenues

### 7.2 Power Visual Indicators (Not Size)
Power differences between Realms are shown through:
- **Aura intensity:** Full radiance on Earth — Gate energy amplifies all divine power equally
- **Ability charge availability:** Standard on Earth — no realm modifiers
- **Cooldown length:** Standard on Earth — no realm modifiers
- **Particle effects:** Full intensity on Earth — Gate amplifies all VFX equally
- **NOT physical size** — the Avatar is always colossal

---

## 8. ECONOMY

### 8.1 Currencies
| Currency | Source | Use |
|----------|--------|-----|
| Belief | Victories, followers, realm control | Avatar level-ups (base stats) |
| Influence | Missions, battles, territory | Ability unlocks and upgrades |
| Gate Shards | Routes, bosses, daily challenges | Relic rerolls, cosmetics, route keys |
| Lore Shards | Lore pickups | Story chapter unlocks, codex entries |

### 8.2 Economy Flow
1. Play routes → earn Belief + Influence + Gate Shards
2. Spend Belief → level Avatar → increase base stats
3. Spend Influence → unlock abilities → expand combat options
4. Spend Gate Shards → reroll relics → optimize build
5. Higher difficulty routes → greater rewards (but greater risk)
6. Enemy Realm routes → +50% Belief/Influence (but Avatar is weakened)

---

## 9. PLAYABLE CHARACTER SUMMARY (Avalon: the two scales)

| Aspect | Detail |
|--------|--------|
| Champion scale | One of THE SIX CLASSES (Sovereign–Spear / Ravager–Greatsword / Warden–Warhammer / Veilborn–Twin Blades / Weaver–Staff / Wildborn–Claws; docs/CLASS-SYSTEM.md v2, 12 locked canon models M+F), bond + Luminary, behind-back third-person camera |
| God scale | The woken colossus itself, played directly at the Gate — stationary per the Emergence Law, front-view cinematic framing |
| Power source | FAITH (the divine-bond loop — attacking/killing builds it, abilities spend it) + persistent TRIBUTE economy |
| Progression | Class trees, relic tiers, Mark re-attunement — curated, zero random rolls |
| Death | The champion falls; the god never walks |

---

## 10. ENDGAME — SOLO-FIRST (Avalon canon; supersedes guilds/world-boss/gear endgame)
**No guilds, no servers to race, no gear to grind.** AVALON is solo-first end to end — one pilgrim, one Luminary, the long descent.

### 10.1 The Depths (the grind space)
Procedural dungeon layer beneath each realm. Tribute flows RICHEST here — the dungeon-masher's home, the place you go to keep the loop hot between chapters. Farming is OPTIONAL: every tree node and relic tier is also earnable through the campaign. The Depths never gate the story.

### 10.2 Waking Siege (replayability, god-scale)
Replayable Hollow-tide defenses at the Gates — waves scale, tribute scales, the god you woke stands and answers. The Masher's Loop at god scale.

### 10.3 Echo Duels (god vs god)
Replayable duels at the Six Gates — your woken colossus against the Echo of another (recorded god-form — solo-first stays true, no live opponent).

### 10.4 Champion Hunts (side quests, lore-forward)
The legendary Hollow of the bestiary (T2/T3) surface through side quests — 'found, not told' per the quest doctrine. Their felled tribute funds deep tree unlocks.

### 10.5 The Tribute System (progression economy — canon per BudE404, Sept 6: 'in-agree on the gear, the grind part sucks and stats would suck and we would be building more gear etc.')
- **NO stat-gear treadmill**: zero random rolls, zero affix loot, zero gear power creep. There is NO drop RNG in AVALON.
- **Two drop currencies from kills**: FAITH motes (in-run resource, unchanged — the Macer's Loop) + **TRIBUTE** (persistent currency) — occasional, tied to notable kills (elites, first-clears, THE OATHLESS (oath-forsaken elite hollows), siege waves), reading as physical offerings to the god (tribute-reef lore: centuries of pilgrim offerings fused into the god's strata).
- **Tribute spends at SHRINES**: class-tree nodes (builders/spenders), relic tiers (Cold Reliquary line), Mark re-attunement rites, cosmetic rune flame colorways.
- **Curated loadout, zero randomness**: WEAPON (swappable kits — the six class trees Blade/Dagger/Hammer/Reliquary/Song/Tower; weapons are separate hand-bone meshes for exactly this), ARMOR (one set per role per gender — the canon 12; faction = rune colorway), RELIC (deed-earned), skill-tree branches.
- **Power curve = trees + relic tiers + Mark choices — never drops.**
