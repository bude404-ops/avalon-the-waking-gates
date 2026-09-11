# Map Design System — Mythos Gates: Ascension

**Version:** 1.0.0  
**Date:** Aug 31, 2026  
**Total Maps:** 19 Campaign Maps + 3 Threat Maps + 4 World Boss Arenas + 5 Post-Game Maps + 1 Hub = 32 Maps  

---

## MAP TAXONOMY

### Map Types

| Type | Count | Purpose |
|------|--------|---------|
| Campaign Map | 19 | Story progression — one per chapter |
| Threat Map | 3 | Non-faction enemy encounters (Hollow, Beast, Gateborn) |
| World Boss Arena | 4 | Server-wide boss encounters |
| Post-Game Map | 5 | Earth dungeons, realm raids, daily/weekly challenges |
| Hub Map | 1 | The Crossroads — post-game social/command center |

### Map Structure

Every map consists of two layers:

**1. Campaign Map Layer (Chapter Select)**
- Mobile chapter-select view showing the realm overview
- Route path with chapter nodes
- Faction color palette and architecture
- God-scale environmental cues (tiny buildings, mountains at waist height)

**2. Tactical Map Layer (Battle Board)**
- 2.5D isometric battle board
- 5-15 rooms per map (varies by chapter)
- Room types: Combat, Hazard, Elite, Treasure, Shrine, Boss, Lore
- Branching paths (Lughch A/B) on most maps
- Faction-specific hazards per environment

### Room Types

| Room Type | Purpose | Icon |
|-----------|---------|------|
| Combat | Fight enemies | ⚔️ |
| Hazard | Environmental danger + enemies | ⚡ |
| Elite | Mini-boss encounter | 👹 |
| Treasure | Relic/loot room (1 guardian) | 💎 |
| Shrine | Heal + buff station | 🏛️ |
| Boss | Chapter boss arena | 💀 |
| Lore | Story reveal pickup | 📜 |

---

## SCALE RULES

- **Avatar Scale:** God-scale (hundreds of feet tall) in ALL maps
- **Camera:** 2.5D isometric — slight angle, looking down at battlefield
- **Framing:** Camera frames Avatar + 4-8 enemies at a time
- **Environmental Cues:** Tiny buildings, mountains at waist height, clouds at chest level
- **Room Size:** Large enough for 1 Avatar + 8-10 enemies + hazard zones

---

## ACT I — THE FIRST REOPENING (Ch 1-6)

### MAP 1: Nile Delta — Ch1 Gate Fracture (Tutorial)
| Property | Value |
|----------|-------|
| **Faction** | Aten Ra |
| **Environment** | Nile Delta — papyrus marshes, reed boats, sun-baked riverbanks |
| **Gate State** | Unstable (sun-bleeding) |
| **Rooms** | 3 (tutorial — short) |
| **Hazards** | None (tutorial) |
| **Enemies** | 3-5 Hollow Drifters (basic melee, low HP) |
| **Boss** | None — tutorial ends with lore reveal |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Gold, sandstone, Nile-blue, papyrus green. Sun-bleached riverbanks with Gate-fractures in the silt. Marsh reeds, mudbrick settlements. Tiny human structures at Avatar's feet. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Riverbank (learn movement + basic attack)] → [Room 2: Reed Maze (learn first ability)] → [Room 3: Gate Fracture (lore reveal)]
```

---

### MAP 2: Desert Necropolis — Ch2 First Blood
| Property | Value |
|----------|-------|
| **Faction** | Aten Ra |
| **Environment** | Pyramid complexes consumed by void-mist, cracked sarcophagi, funerary temples |
| **Gate State** | Unstable (void-bleeding) |
| **Rooms** | 7 (5-7 combat rooms + 1 mini-boss + 1 treasure) |
| **Hazards** | Solar radiation zones (10 ATK/s to standing deity) |
| **Enemies** | Hollow Drifters, Khepri Scarabs, Uraeus Serpents |
| **Boss** | Hollow Husk (consumed Egyptian guardian) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Sandstone necropolis, cracked sarcophagi, void-mist pouring from tomb entrances. Gold accents fading to charcoal. Gate-fractures in pyramid surfaces. Tiny funerary temples at Avatar's feet. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Tomb Entrance (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Sarcophagi Hall (hazard + combat)] → [Room 3: Offering Chamber (treasure)] → [Room 4: Duat Threshold (elite mini-boss)] → [Boss: Hollow Husk]
  Lughch B → [Room 2: Priest Tunnel (combat)] → [Room 3: False Door (lore)] → [Room 4: Canopic Vault (treasure)] → [Boss: Hollow Husk]
```

---

### MAP 3: Temple Complex — Ch3 Aten Ra Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Aten Ra |
| **Environment** | Colossal obelisks, cracked sandstone columns, sun-disc reliefs bleeding white energy |
| **Gate State** | Unstable (solar flare) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Solar beams sweep room (150 ATK), solar radiation (10 ATK/s) |
| **Enemies** | Solar Judges (counter-attackers), Uraeus Serpents (poison), Khepri Scarabs (swarmers) |
| **Boss** | Misweighed Dawn Judge |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Sandstone temple complex, massive obelisks with Gate-fractures, sun-disc reliefs with white energy bleeding through cracks. Hypostyle halls with columns taller than the Avatar. Solar beam hazards from cracked ceiling. Gold + amber + deep blue + void-black. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Sun-Bleached Antechamber (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Lotus Verdict Hall (hazard + combat)] → [Room 3: Solar Scale Chamber (elite: Misweighed Judge mini)] → [Room 5: Ma'at Shrine (shrine)] → [Boss: Misweighed Dawn Judge]
  Lughch B → [Room 2: Nile Underground (combat + rising water hazard)] → [Room 3: Forgotten Pharaoh's Tomb (treasure)] → [Room 4: Duat Threshold (combat + lore)] → [Boss: Misweighed Dawn Judge]
```

---

### MAP 4: Storm-Carved Oathground — Ch4 Asgardian Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Asgardian |
| **Environment** | Frozen fjord, longhouses cracking under Gate-pressure, rune stones bleeding white energy |
| **Gate State** | Ruin Gate (oath memory active) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Lightning strikes random (250 ATK, 1s telegraph), frost patches (slow), wind pushes (movement -30%) |
| **Enemies** | Rime Raiders, Thunder-Bridge Jotun, Valkyr Storm-Callers |
| **Boss** | Forgotten Standard Bearer |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Frozen fjord, ice-choked waters, black cliffs, longhouses with turf roofs cracking. Runic standing stones bleeding white energy. Ship graves consumed by void-mist. Ice blue + iron gray + deep green + void-black. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Broken Bridge Approach (combat + lightning hazard)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Oath-Stone Hall (hazard + combat)] → [Room 3: Storm Riven Pass (combat + wind/ice hazard)] → [Elite: Forgotten Standard Bearer (Phase 1)] → [Room 5: Shield-Wall Shrine (shrine)] → [Boss: Forgotten Standard Bearer (Phase 2)]
  Lughch B → [Room 2: Frozen Root Tunnels (combat + frost)] → [Room 3: Draugr Mound (treasure)] → [Room 4: Thunder Vault (combat + lore)] → [Boss: Forgotten Standard Bearer (Phase 2)]
```

---

### MAP 5: Laurel-Sky Citadel — Ch5 Olympian Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Olympian |
| **Environment** | Marble temples with shattered columns, amphitheaters with void-mist, Parthenon stones floating in anti-gravity |
| **Gate State** | Stable (oracle vapor leak) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Anti-gravity zones (float up, lose control), falling marble debris (200 ATK, telegraphed) |
| **Enemies** | Hoplite Phantoms (shielded), Harpies (flying), Gorgon Echoes (petrification gaze) |
| **Boss** | The Oracle's Silence (Hollow-corrupted Greek oracle) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Marble white temples, shattered columns, amphitheaters with void-mist. Parthenon stones floating in anti-gravity. Olive groves cracking. Aegean turquoise visible in distance. Marble white + turquoise + laurel green + bronze gold + void-black. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Laurel Approach (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Oracle Vapor Hall (hazard + combat)] → [Room 3: Trial Arena (elite)] → [Room 5: Victory Shrine (shrine)] → [Boss: The Oracle's Silence]
  Lughch B → [Room 2: Bronze Forge Ruins (combat)] → [Room 3: Cloud Terrace (anti-gravity hazard + combat)] → [Room 4: Laurel Garden (treasure)] → [Boss: The Oracle's Silence]
```

---

### MAP 6: The Great Pyramid — Ch6 Act I Boss
| Property | Value |
|----------|-------|
| **Faction** | Cross-faction (Aten Ra, Asgardian, Olympian) |
| **Environment** | Inside the Great Pyramid — corridors twisted by Gate energy, King's Chamber with Gate nexus |
| **Gate State** | Critical (Gate nexus active) |
| **Rooms** | 5 (approach + 4 boss phases) |
| **Hazards** | Solar radiation, lightning chains, anti-gravity zones (one per boss phase) |
| **Enemies** | Mixed (all Act I faction enemies) |
| **Boss** | The Gatekeeper (4-phase boss — Solar/Storm/Divine/Fusion) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Interior of the Great Pyramid, but corrupted. Sandstone corridors twisted by Gate energy — walls bend, floors crack. The King's Chamber is vast, with the Gate nexus pulsing at the center. Each boss phase changes the room: Phase 1 = solar gold, Phase 2 = ice blue, Phase 3 = marble white, Phase 4 = all three mixed. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Descending Corridor (combat — mixed enemies)] → [Room 2: Grand Gallery (hazard + combat)] → [Room 3: Antechamber (shrine — last heal before boss)] → [Boss: The Gatekeeper (4 phases, room transforms per phase)]
```

---

## ACT II — DEEPER DESCENT (Ch 7-13)

### MAP 7: Sídhe-Root Crossings — Ch7 Tuatha Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Tuatha |
| **Environment** | Green cliffs above the Atlantic, Celtic ringforts cracking, standing stones bleeding white energy |
| **Gate State** | Active (time-bleed) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Time-bleed zones (movement slowed 40%), thorn patches (50 ATK/s, root traps) |
| **Enemies** | Fae Wraiths (teleport), Bog Beasts (tanky), Will-o'-Wisps (swarm, fast) |
| **Boss** | The Time-Eater (Hollow entity that slows time) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Moss green, mist gray, Atlantic blue, ogham white. Celtic ringforts with wattle walls, druidic stone circles disrupted by void-fissures, ogham standing stones floating in anti-gravity. Misted coastline. Ancient oaks. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Misted Coastline (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Sacred Grove (hazard + combat — thorn patches)] → [Room 3: Stone Circle (elite)] → [Room 5: Spring Shrine (shrine)] → [Boss: The Time-Eater]
  Lughch B → [Room 2: Hill Fort (combat)] → [Room 3: Ogham Chamber (time-bleed hazard + combat)] → [Room 4: Fae Mound (treasure)] → [Boss: The Time-Eater]
```

---

### MAP 8: Ancient Forest — Ch8 Wild Hunt
| Property | Value |
|----------|-------|
| **Faction** | Tuatha |
| **Environment** | Ancient forest consumed by void-mist, trees turned to void-crystal, fae paths leading nowhere |
| **Gate State** | Active (beast corruption) |
| **Rooms** | 11 (10 wave survival rooms + boss) |
| **Hazards** | Void-crystal shards (100 ATK if touched), disorienting mist (vision -3m) |
| **Enemies** | 10 waves of Hollow-possessed beasts (increasing difficulty) |
| **Boss** | The Horned Shadow (Hollow-possessed divine stag) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Moss green + mist gray + void-black. Ancient forest with trees turned to black void-crystal. Fae paths that loop. Disorienting mist. Tiny Celtic ruins at Avatar's feet. |

**Room Layout:**
```
[Entry Gate] → [Wave 1-3: Forest Edge (combat waves)] → [Wave 4-6: Deep Forest (combat waves + void-crystal hazard)] → [Shrine: Sacred Clearing (heal between waves 6 and 7)] → [Wave 7-10: Heart of the Forest (combat waves)] → [Boss: The Horned Shadow]
```

---

### MAP 9: Torii-Moon Mirror Road — Ch9 Kami Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Kami |
| **Environment** | Vermilion torii gates in surf, cherry trees cracking with void-energy, stone braziers flickering anti-light |
| **Gate State** | Active (reflection distortion) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Mirror corridors (reflections attack), boiling sand (50 ATK/s) |
| **Enemies** | Oni Echoes (heavy melee), Yurei (floating, phase through walls), Kitsune (illusion, teleport) |
| **Boss** | The Mirror-Self (Hollow that copies your deity's abilities) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Vermilion red, lacquer black, cherry pink, cedar green. Thatched shrine village, lacquered pagodas, torii gates in surf. Stone braziers flickering anti-light. Cherry trees cracking with void-energy. Bamboo forest. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Coastal Shrine Village (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Bamboo Forest Temple (hazard + combat)] → [Room 3: Mirror Hall (elite)] → [Room 5: Bell Shrine (shrine)] → [Boss: The Mirror-Self]
  Lughch B → [Room 2: Mountain Pass (combat + mist)] → [Room 3: Waterfall Shrine (treasure)] → [Room 4: Imperial Shrine (mirror hazard + combat)] → [Boss: The Mirror-Self]
```

---

### MAP 10: Ancient Shrine Complex — Ch10 Spirit Realm Breach
| Property | Value |
|----------|-------|
| **Faction** | Kami |
| **Environment** | Vermilion torii corridor streaked with void-cracks, sacred mirror halls showing Hollow reflections |
| **Gate State** | Critical (phase breach) |
| **Rooms** | 8 (dual-realm mission — enemies phase between visible/invisible) |
| **Hazards** | Phase shift zones (enemies invisible), spirit fog (vision -5m) |
| **Enemies** | Phased enemies (visible/invisible switching) |
| **Boss** | The Phase Wraith (exists in both realms simultaneously) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Vermilion + lacquer black + void-black. Torii corridor with void-cracks. Mirror halls where every reflection shows the Hollow. Phased rooms where the environment shifts between physical (solid, colored) and spirit (translucent, void-mist) states. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Torii Approach (combat — enemies phase in/out)] → [Room 2: Mirror Hall (dual-realm combat)] → [Room 3: Phase Garden (hazard — enemies invisible)] → [Room 4: Bell Tower (lore — spirit realm explanation)] → [Room 5: Inner Shrine (shrine)] → [Room 6: Phase Corridor (dual-realm combat)] → [Room 7: Spirit Bridge (combat)] → [Boss: The Phase Wraith]
```

---

### MAP 11: Choir-Vault Discord — Ch11 Empyrean Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Empyrean |
| **Environment** | Mudbrick stepped ziggurat, irrigation channels cracked by Gate-fractures, reed houses collapsing into void-mist |
| **Gate State** | Active (discord) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Silence fields (abilities disabled), sound wave pulses (200 ATK, knockback) |
| **Enemies** | Fallen Cherubim (flying, holy damage), Discordant Hymns (AoE sound waves), Void Seraphim (anti-light) |
| **Boss** | The Dissonance (Hollow-corrupted angelic entity) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Sun-baked clay, lapis lazuli, gold leaf, white radiance, void-black. Stepped ziggurat with cracked mudbrick. Irrigation channels running anti-light. Cuneiform tablets floating in anti-gravity. Hanging gardens with void-crystal plants. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Ziggurat Steps (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Hanging Gardens (hazard + combat)] → [Room 3: Choir Chamber (elite)] → [Room 5: Hymn Shrine (shrine)] → [Boss: The Dissonance]
  Lughch B → [Room 2: Tablet Archive (combat + silence field hazard)] → [Room 3: White Temple (treasure)] → [Room 4: Star Map Room (lore)] → [Boss: The Dissonance]
```

---

### MAP 12: Crumbled Ziggurat — Ch12 The Holy Fall
| Property | Value |
|----------|-------|
| **Faction** | Empyrean |
| **Environment** | Crumbling ziggurat, hanging gardens dead, void-mist pouring from cracks |
| **Gate State** | Critical (fallen deity) |
| **Rooms** | 8 (escort mission — protect weakened NPC) |
| **Hazards** | Holy ground zones (buff for Empyrean, debuff for others), collapsing floors |
| **Enemies** | Hollow swarm (protect NPC from reaching them) |
| **Boss** | The Fallen Seraph (Hollow-corrupted Empyrean warrior — flies above melee) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Sun-baked clay + lapis + void-black. Crumbling ziggurat with dead hanging gardens. Void-mist from every crack. Collapsing floors. Holy ground zones glowing faint white-gold (Empyrean buff zones). |

**Room Layout:**
```
[Entry Gate] → [Room 1: Ziggurat Base (combat — escort NPC)] → [Room 2: Dead Gardens (hazard + combat)] → [Room 3: Collapsed Archive (combat — collapsing floor)] → [Room 4: Holy Ground (shrine — Empyrean buff)] → [Room 5: Ascending Ramp (combat — aerial enemies)] → [Room 6: Inner Temple (combat + lore)] → [Room 7: Summit Approach (combat)] → [Boss: The Fallen Seraph (aerial boss)]
```

---

### MAP 13: Hollow Corridor — Ch13 Act II Boss
| Property | Value |
|----------|-------|
| **Faction** | Cross-faction (Tuatha, Kami, Empyrean) |
| **Environment** | Neutral territory — a corridor through pure void-space between Earth and the Hollow dimension |
| **Gate State** | Critical (world boss emergence) |
| **Rooms** | 6 (approach + 5 boss phases) |
| **Hazards** | Void pools (instant debuff if touched), anti-light zones (drain abilities) |
| **Enemies** | Mixed (all Act II faction enemies) |
| **Boss** | The Consumer (5-phase World Boss — devours your abilities) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Pure void-space. No faction architecture. No terrain — just floating Gate stone fragments in a void corridor. White fracture lines in the void. Anti-light pulsing. The most alien environment in the game. Pure charcoal-black + white fracture lines. No color. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Void Approach (combat — mixed enemies)] → [Room 2: Fracture Bridge (hazard — void pools)] → [Room 3: Gate Stone Platform (shrine — last heal)] → [Boss: The Consumer (5 phases — each phase the room gets darker as abilities are devoured)]
```

---

## ACT III — THE BLACK IRON BARGAIN (Ch 14-19)

### MAP 14: Black-Iron Debt Court — Ch14 Infernal Dominion Faction Unlock
| Property | Value |
|----------|-------|
| **Faction** | Infernal |
| **Environment** | Burned city ruins, descending passages into Kur, obsidian doors cracked, flame-lit tunnels |
| **Gate State** | Active (debt binding) |
| **Rooms** | 8 (3 deity challenges + elite + shrine + boss) |
| **Hazards** | Debt zones (HP drain per second), sulfur vents (50 ATK/s, poison) |
| **Enemies** | Contract Bound (high armor), Sin Eaters (heal from your damage), Void Lawyers (CC immune) |
| **Boss** | The Debt Collector (Hollow entity that demands payment) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Blood red, obsidian black, sulfur yellow, ash gray, void-black. Burned city ruins, descending into the underworld. Obsidian gates. Flame-lit tunnels. The river Hubur running black. Destroyed palaces of ancient dead. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Cursed Ruins (combat)] → [Lughch A / Lughch B]
  Lughch A → [Room 2: Debt Halls (hazard + combat — debt zones)] → [Room 3: Contract Archive (elite)] → [Room 5: Ash Shrine (shrine)] → [Boss: The Debt Collector]
  Lughch B → [Room 2: Underworld Gates (combat + sulfur)] → [Room 3: Throne of Ash (treasure)] → [Room 4: River Hubur (lore)] → [Boss: The Debt Collector]
```

---

### MAP 15: Contract-Scorched Court — Ch15 The Bargain
| Property | Value |
|----------|-------|
| **Faction** | Infernal |
| **Environment** | Vast halls of contracts carved in stone, each one cracked by Gate energy |
| **Gate State** | Active (choice binding) |
| **Rooms** | 8 (choice-based mission — 2 paths) |
| **Hazards** | Path of Power: sacrifice zones (lose HP for buff). Path of Resistance: normal hazards |
| **Enemies** | Path of Power: aggressive enemies (faster, more dangerous). Path of Resistance: tanky enemies (slower, more HP) |
| **Boss** | The Pact-Breaker (different mechanics per path) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Blood red + obsidian + sulfur. Vast contract halls with stone tablets carved into walls, each cracked by Gate energy. Two distinct paths: Power Path = red-hot, aggressive lighting. Resistance Path = cold, dark, endurance lighting. The same architecture, different atmosphere. |

**Room Layout:**
```
[Entry Gate] → [Room 1: Bargain Hall (lore — choice presented)] → [Path of Power / Path of Resistance]
  Path of Power → [Room 2: Sacrifice Chamber (sacrifice HP for buff)] → [Room 3: Aggression Gauntlet (fast combat)] → [Room 4: Power Shrine (shrine — buff)] → [Boss: The Pact-Breaker (DPS check)]
  Path of Resistance → [Room 2: Endurance Hall (tanky combat)] → [Room 3: Slow Descent (hazard + combat)] → [Room 4: Safe Shrine (shrine — normal heal)] → [Boss: The Pact-Breaker (endurance test)]
```

---

### MAP 16: Multiple Gate Nexus Points — Ch16 The Hollowed Pantheon
| Property | Value |
|----------|-------|
| **Faction** | Cross-faction (all 7) |
| **Environment** | 3 separate arenas — each in a different faction's territory, all corrupted |
| **Gate State** | Critical (multi-point breach) |
| **Rooms** | 3 (boss rush — 3 Hollow-corrupted deity echoes) |
| **Hazards** | Per arena: Aten Ra = solar radiation, Asgardian = frost zones, Kami = mirror corridors |
| **Enemies** | None — boss rush only |
| **Bosses** | Hollow Echo colossi (the god-shaped shells with no god inside — T3 bestiary canon) |
| **Art Status** | ⚠️ Needs tactical map art (3 separate arenas) |
| **Art Direction** | Three distinct arenas: (1) Corrupted Aten Ra temple — gold + void-black, (2) Corrupted Asgardian oathground — ice blue + void-black, (3) Corrupted Kami shrine — vermilion + void-black. Each arena looks like the faction's territory but consumed by Hollow. |

**Room Layout:**
```
[Boss 1 Arena: Corrupted Aten Ra Temple] → [Transition: Void Corridor] → [Boss 2 Arena: Corrupted Asgardian Oathground] → [Transition: Void Corridor] → [Boss 3 Arena: Corrupted Kami Shrine]
```

---

### MAP 17: The Mythos Gate — Ch17 The Last Gate
| Property | Value |
|----------|-------|
| **Faction** | Cross-faction (all 7) |
| **Environment** | The Mythos Gate itself — the source of the Hollow invasion, at the center of Earth |
| **Gate State** | Maximum (the Gate itself) |
| **Rooms** | 15+ (the longest mission — no checkpoints) |
| **Hazards** | All faction hazards cycling, void pools, anti-light zones, collapsing terrain |
| **Enemies** | Mixed (all factions, all threat types) |
| **Mini-Bosses** | 4 Hollow champions (one per role mechanic test) |
| **Boss** | None (the Gate itself is the challenge — survive 15 rooms) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | The interior of the Mythos Gate itself. A massive structure of Gate stone — dark grey, cracked, pulsing with anti-light. The rooms shift through all 7 faction aesthetics as you progress — Aten Ra gold → Asgardian ice → Olympian marble → Kami vermilion → Tuatha moss → Empyrean lapis → Infernal blood red. Each section is the faction's architecture, but corrupted and consumed. The final rooms are pure void — no faction identity, just Gate stone and void-mist. |

**Room Layout:**
```
[Entry] → [Rooms 1-2: Aten Ra section (solar hazards)] → [Mini-Boss 1: Hollow Warrior Champion] → [Rooms 3-4: Asgardian section (frost/lightning)] → [Rooms 5-6: Olympian section (anti-gravity)] → [Mini-Boss 2: Hollow Caster Champion] → [Rooms 7-8: Kami section (mirror/phase)] → [Rooms 9-10: Tuatha section (time-bleed/thorn)] → [Mini-Boss 3: Hollow Archer Champion] → [Rooms 11-12: Empyrean section (silence)] → [Rooms 13-14: Infernal section (debt zones)] → [Mini-Boss 4: Hollow Assassin Champion] → [Room 15: The Gate Core (pure void)]
```

---

### MAP 18: The Throne of Contracts — Ch18 The Black Iron Bargain
| Property | Value |
|----------|-------|
| **Faction** | Cross-faction |
| **Environment** | The deepest depth — a throne of volcanic glass and void-crystal |
| **Gate State** | Maximum (final confrontation) |
| **Rooms** | 4 (approach + 4-phase boss) |
| **Hazards** | All hazards active simultaneously in final phase |
| **Enemies** | None — boss only |
| **Boss** | The Hollow King (4-phase final boss — uses all 7 faction buffs + all 4 role mechanics) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | The lowest depth. A throne of volcanic glass (obsidian) and void-crystal (black + white fracture lines). The room is vast — cathedral scale. Anti-fire burns from the floor. The Gate nexus is visible as a massive crack in the ceiling — the sky is void. Blood red + obsidian + void-black + white fracture lines. The most dramatic environment in the game. |

**Room Layout:**
```
[Entry: Descent Shaft (lore — the Hollow King's offer)] → [Room 2: Throne Approach (shrine — final heal)] → [Boss: The Hollow King (4 phases — room transforms: Phase 1 = all 7 faction colors, Phase 2 = role-specific lighting, Phase 3 = weapon-path colors, Phase 4 = pure void)]
```

---

### MAP 19: The Crossroads — Ch19 Epilogue (Hub)
| Property | Value |
|----------|-------|
| **Faction** | Neutral |
| **Environment** | A neutral hub at the intersection of all 7 faction territories |
| **Gate State** | Stable (post-game) |
| **Rooms** | 1 (hub — social/command center) |
| **Hazards** | None |
| **Enemies** | None |
| **Boss** | None |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | The Crossroads — a vast neutral plaza where all 7 faction architectural styles meet. Each direction shows a different faction's gate: north = Aten Ra gold, east = Asgardian ice, south = Olympian marble, west = Kami vermilion, plus Tuatha, Empyrean, and Infernal gates in the cardinal diagonals. The center has the reopened Mythos Gate — now stable, glowing faintly. Neutral stone palette with all 7 faction accent colors. |

**Layout:**
```
[Hub Center: The Reopened Gate (post-game portal)] → [7 Faction Gates (one per direction)] → [Post-Game Boards: Earth Dungeons, Realm Raids, World Bosses, Daily/Weekly, New Game+]
```

---

## THREAT MAPS (3)

### THREAT MAP 1: Hollow Breach
| Property | Value |
|----------|-------|
| **Type** | Endless wave survival |
| **Environment** | A breach point where Hollow pour through — pure void-mist terrain |
| **Hazards** | Void pools (instant debuff), expanding breach (arena shrinks over time) |
| **Enemies** | All Hollow types — increasing difficulty |
| **Boss** | Every 10 waves: random Hollow elite |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Pure void-space. No architecture. No terrain — just floating Gate stone fragments. White fracture lines. The breach expands from the center, consuming the playable area. Charcoal-black + white fracture lines only. |

---

### THREAT MAP 2: Beast Realm Hunt
| Property | Value |
|----------|-------|
| **Type** | Hunt mission — track and kill specific beasts |
| **Environment** | Primal wilderness — pre-Gate terrain, not aligned to any faction |
| **Hazards** | Pack ambush zones, primal terrain (swamps, cliffs, dense forest) |
| **Enemies** | Beast Realm creatures — pack mechanics, primal aggression |
| **Boss** | Beast Realm Maneater (hunt target) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Wild, primal terrain. No architecture. Ancient forest, swamp, cliff edges. Pre-civilization earth. Moss green, earth brown, primal red. |

---

### THREAT MAP 3: Gateborn Anomaly
| Property | Value |
|----------|-------|
| **Type** | Anomaly containment — close Gate fractures before they expand |
| **Environment** | Terrain distorted by Gate energy — gravity shifts, portals, mutations |
| **Hazards** | Gravity inversion zones, portal traps (teleport to random room), terrain distortion (walls move) |
| **Enemies** | Gateborn creatures — portal mechanics, terrain distortion, mutations |
| **Boss** | Gateborn Colossus (anomaly source) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Terrain that is WRONG. Gravity shifts — floors become ceilings. Portals to other rooms. Walls that move. Stone that breathes. The most disorienting environment. Dark grey + purple + white fracture lines. |

---

## WORLD BOSS ARENAS (4)

### WB ARENA 1: The Drowned Deep
| Property | Value |
|----------|-------|
| **World Boss** | Leviathan of the First Flood (200+ft primordial sea serpent) |
| **Environment** | Flooded terrain — standing in ocean water at Avatar's ankles, storm above |
| **Hazards** | Tidal waves (300 ATK, knockback), whirlpools (pull + drown), lightning from storm |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Ocean arena. Standing in chest-deep (for humans) water at Avatar's feet. Storm above — lightning, rain, wind. The Leviathan rises from the deep. Deep blue + storm gray + white foam + void-black. |

---

### WB ARENA 2: The World-Wound
| Property | Value |
|----------|-------|
| **World Boss** | Hollow World-Wound Behemoth (colossal Hollow entity) |
| **Environment** | A wound in reality — the ground is cracked open, void-mist pouring from the fissure |
| **Hazards** | Expanding void fissure (arena shrinks), anti-light pulses (drain abilities) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | A cracked-open battlefield. The ground is split by a massive void fissure. Void-mist pours from the crack. The Behemoth emerges from the fissure. Charcoal-black + white fracture lines + dark earth. |

---

### WB ARENA 3: The Broken Gate
| Property | Value |
|----------|-------|
| **World Boss** | The Gate Guardian (300+ft living-stone sentinel, fused with broken Gate) |
| **Environment** | The ruins of a Mythos Gate — massive stone fragments, the Gate arch cracked in half |
| **Hazards** | Falling Gate stone debris (300 ATK), Gate energy pulses (200 ATK room-wide) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | The ruins of a Mythos Gate. Massive stone fragments scattered. The Gate arch cracked in half, pulsing with anti-light. The Gate Guardian is fused with the broken Gate — it IS the Gate. Dark grey stone + white fracture lines + void-black. |

---

### WB ARENA 4: The Forgotten Wastes
| Property | Value |
|----------|-------|
| **World Boss** | The Forgotten Giant (250+ft frost-iron Deity, walking erasure) |
| **Environment** | Grey dusty wasteland — remnants of an erased civilization |
| **Hazards** | Erasure zones (lose abilities temporarily), frost spread (slow + damage) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Grey dusty wasteland. Broken architecture matching no known faction — structures that look almost familiar but wrong. Erasing fog. Ash falling upward. Grey + dust + faint unidentifiable color + void-black. |

---

## POST-GAME MAPS (5)

### POST-GAME MAP 1: Earth Dungeon — Neutral City
| Property | Value |
|----------|-------|
| **Type** | Repeatable dungeon — randomized room order, randomized enemy spawns |
| **Environment** | A modern Earth city partially consumed by the Gate — buildings fused with Gate stone |
| **Hazards** | Randomized (one faction hazard per run) |
| **Enemies** | Randomized (all factions) |
| **Boss** | Randomized (one faction boss per run) |
| **Art Status** | ⚠️ Needs tactical map art |
| **Art Direction** | Modern city architecture (tiny buildings at Avatar's feet) fused with Gate stone. Skyscrapers cracked, streets split by Gate fractures. A mix of modern grey concrete + Gate stone dark grey + void-mist. All 7 faction colors bleeding through where the Gate touches. |

---

### POST-GAME MAP 2: Realm Raid — Faction-Specific
| Property | Value |
|----------|-------|
| **Type** | Limited-time event — faction-specific raid with unique mechanics |
| **Environment** | The faction's home Realm (not Earth — the actual mythological dimension) |
| **Hazards** | Faction-specific, escalated to raid difficulty |
| **Enemies** | Faction enemies + unique raid-only enemies |
| **Boss** | Raid-specific boss (different from campaign boss) |
| **Art Status** | ⚠️ Needs tactical map art (7 variants — one per faction) |
| **Art Direction** | Each faction's home Realm in its purest form — no Earth contamination, no Hollow corruption. The mythological dimension as it was BEFORE the Gate opened. Aten Ra = pure gold desert paradise. Asgardian = pure ice realm with Yggdrasil. Olympian = pure marble Olympus. Kami = pure spirit realm. Tuatha = pure green Otherworld. Empyrean = pure radiance heaven. Infernal = pure underworld. |

---

### POST-GAME MAP 3: Daily Challenge
| Property | Value |
|----------|-------|
| **Type** | Daily rotating challenge — one faction per day, one modifier per day |
| **Environment** | Rotates through all 19 campaign maps |
| **Hazards** | Daily modifier (e.g., "all damage doubled," "no healing," "enemies enrage at 50%") |
| **Enemies** | Campaign enemies with daily modifier |
| **Boss** | Campaign boss with daily modifier |
| **Art Status** | ✅ Reuses campaign map art |

---

### POST-GAME MAP 4: Weekly Challenge
| Property | Value |
|----------|-------|
| **Type** | Weekly gauntlet — all 19 chapters in sequence, one life |
| **Environment** | All 19 campaign maps in order |
| **Hazards** | Escalating — each chapter adds one permanent modifier |
| **Enemies** | All enemies, escalating |
| **Boss** | All 19 bosses, escalating |
| **Art Status** | ✅ Reuses campaign map art |

---

### POST-GAME MAP 5: New Game+ — The Reopened Gate
| Property | Value |
|----------|-------|
| **Type** | Full campaign replay with permanent modifiers |
| **Environment** | All 19 campaign maps — but visually altered (void corruption spread further) |
| **Hazards** | All hazards + additional void hazards |
| **Enemies** | All enemies + void-enhanced variants |
| **Boss** | All bosses + void-enhanced final phase |
| **Art Status** | ⚠️ Needs NG+ variant art (void-corrupted versions of campaign maps) |
| **Art Direction** | Same campaign maps but with more void-mist, more white fracture lines, more Gate stone visible. The world is further consumed. Same architecture, darker palette, more void-black. |

---

## EXISTING ART INVENTORY

### Battlefields (8 Approved)
| ID | Name | Status |
|----|------|--------|
| MG-BATTLEFIELD-001 | The First Reopening Gate | ✅ Approved |
| MG-BATTLEFIELD-002 | Solar Pylon Observatory (Aten Ra) | ✅ Approved |
| MG-BATTLEFIELD-003 | Storm Oath Bridge (Asgardian) | ✅ Approved |
| MG-BATTLEFIELD-004 | Marble Sky Arena (Olympian) | ✅ Approved |
| MG-BATTLEFIELD-005 | Moon Grove Root Labyrinth (Tuatha) | ✅ Approved |
| MG-BATTLEFIELD-006 | Black Iron Court (Infernal) | ✅ Approved |
| MG-BATTLEFIELD-010 | Mirror Lake Sanctum (Kami) | ✅ Approved |
| MG-BATTLEFIELD-012 | Broken Heaven Engine (Empyrean) | ✅ Approved |

### Map Concepts (65 files)
- MG-MAP-000 through MG-MAP-055 (56 files) — early concept map art
- MG-GATE-xxx (7 files) — faction Gate concept art
- MG-COMBAT-EXAMPLE-xxx (2 files) — combat example renders

### Missing Battlefield Art
| Map | Battlefield Art Needed |
|-----|----------------------|
| MAP 1: Nile Delta | ❌ Tutorial battlefield |
| MAP 6: Great Pyramid (Act I Boss) | ❌ Boss arena |
| MAP 8: Ancient Forest | ❌ Wave survival arena |
| MAP 10: Shrine Complex | ❌ Dual-realm battlefield |
| MAP 13: Hollow Corridor (Act II Boss) | ❌ World boss arena |
| MAP 16: Gate Nexus (Boss Rush) | ❌ 3 corrupted arenas |
| MAP 17: The Mythos Gate | ❌ 15-room gauntlet |
| MAP 18: Throne of Contracts (Final Boss) | ❌ Final boss arena |
| MAP 19: The Crossroads (Hub) | ❌ Hub map |
| Threat Map 1: Hollow Breach | ❌ Endless arena |
| Threat Map 2: Beast Realm Hunt | ❌ Hunt arena |
| Threat Map 3: Gateborn Anomaly | ❌ Anomaly arena |
| WB Arena 1-4 | ❌ All 4 world boss arenas |
| Post-Game: Earth Dungeon | ❌ City-dungeon |
| Post-Game: Realm Raid (x7) | ❌ 7 home-realm variants |

### Art Priority Order
1. All 19 campaign map battlefields (one per chapter)
2. 4 world boss arenas
3. 3 threat maps
4. 1 hub map (The Crossroads)
5. Post-game map variants

---

## MAP MECHANICS SUMMARY

### Faction-Specific Hazards by Map

| Faction | Hazard | Maps Used In |
|---------|--------|-------------|
| Aten Ra | Solar radiation (10 ATK/s) | MAP 1, 2, 3, 6, 16, 17 |
| Asgardian | Lightning (250 ATK random) + Frost (slow) | MAP 4, 6, 16, 17 |
| Olympian | Anti-gravity (float, lose control) | MAP 5, 6, 17 |
| Kami | Mirror corridors (reflections attack) | MAP 9, 10, 16, 17 |
| Tuatha | Time-bleed (movement slowed 40%) | MAP 7, 8, 17 |
| Empyrean | Silence fields (abilities disabled) | MAP 11, 12, 17 |
| Infernal | Debt zones (HP drain per second) | MAP 14, 15, 17 |

### Branching Path System
- Most campaign maps have a Lughch A/B split
- Lughch A = standard combat path
- Lughch B = hazard-heavy path with better treasure
- Both paths converge at the boss
- Exception: Tutorial (MAP 1), Act Bosses (MAP 6, 13, 18), Boss Rush (MAP 16), Hub (MAP 19) — no branching

### Shrine System
- Every campaign map has 1 shrine room (except tutorial and boss rushes)
- Shrine = full HP heal + temporary buff (30-60 seconds)
- Buffs: ATK +20%, DEF +30%, or faction-specific buff boost
- Shrine appears BEFORE the boss room

---

## RELATIONSHIP TO EXISTING DOCS

| Document | Relationship |
|----------|-------------|
| `docs/CAMPAIGN_MAPS.md` | Defines the 7 realm route maps + 3 threat + 4 WB arenas. This doc expands to all 19 chapters. |
| `docs/MYTHOS_GATES_DUNGEON_ROUTE_REGISTRY.md` | Defines the 7 dungeon routes. This doc adds chapter-specific tactical maps. |
| `docs/CAMPAIGN_GAMEPLAY_DESIGN.md` | Defines 19 chapters with locations and bosses. This doc provides the map layouts for each. |
| `docs/CAMPAIGN_VISUAL_LORE_AUDIT.md` | Defines faction environments. This doc maps them to playable tactical maps. |
| `docs/COMBAT_SYSTEM_SPEC.md` | Defines combat mechanics. This doc defines where they happen. |
| `art/mission-packages/` (280 files) | Mission-level content. This doc provides the map framework. |

