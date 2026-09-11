# MYTHOS GATES: ASCENSION — ONE WORLD, TWO SCALES
## Map, Dungeon & Mission Architecture (v1 — Cold Reliquary as design test)

The core design question, always: **"What can we do with scale that another fantasy RPG cannot?"**

---

## 1. THE FIVE SCALE-ONLY MECHANICS (our moat)

These are the things a normal RPG literally cannot do. Every mission must use at least two.

1. **RECONNAISSANCE PAYOFF** — Knowledge gained at pilgrim scale becomes tactical intel at giant scale. The cellar you explored hides the Hollow nest you now crush in one stomp. The player's MEMORY of the small world is the giant's minimap.
2. **EMOTIONAL INVERSION** — The horror you fled from becomes the pest you crush. (The Erased Drake beat: unfightable at 2 feet, trivial at 100 feet. Revenge is a scale mechanic.)
3. **LANDMARK MEMORY** — Places you LIVED IN become things you HOLD. The shrine you light candles inside fits in the god's palm. No other genre can make you feel homesickness for a building you can accidentally step on.
4. **PROTECTION INVERSION** — The world that sheltered you (roofs, caves, the town walls) now needs YOU as its shelter. The player becomes the thing they were hiding behind.
5. **PHYSICAL CONTINUITY** — One geometry, two games. This is also our production cheat: content is built ONCE and played TWICE. A mission that costs one map gives two experiences.

---

## 2. ANALYSIS OF THE BRIEF — WHERE I PUSH BACK

The brief is 90% right. Six honest critiques:

**2.1 The frequency problem.** If Giant Mode only fires at "major story moments," the player is at pilgrim scale 90% of the time — and our most marketable fantasy (100ft god) becomes a rare cutscene. *Improvement:* **GLIMPSE SYSTEM** — between full ascensions, the pilgrim can channel brief "glimpses" of the god: a single colossal hand manifesting to block a collapse, a god-scale stomp that staggers everything near, one fire-lance strike per ritual site. Glimpses cost FAITH, last 3 seconds, and keep the giant emotionally present in pilgrim gameplay. Full ascension stays rare and sacred; glimpses keep it alive.

**2.2 Mobile scope risk.** An 8-layer handcrafted mission (Emberhollow → Echo arena) is a AAA pipeline. On mobile with our team it would take months per mission. *Improvement:* **BEAT DISCIPLINE** — every layer is a 3–6 minute beat, not a zone. Reuse biome kits (one forest kit, one town kit, one shrine kit). Major missions = max 6 beats + 1 transformation + 1 boss. The Cold Reliquary vertical slice below proves the shape.

**2.3 Detail becomes noise at scale.** Pilgrim-scale needs density (a building is a dungeon); that same density is unreadable clutter from 100 feet up on a phone screen. *Improvement:* **DOUBLE-DUTY GEOMETRY RULE** — every hero asset is designed at pilgrim scale FIRST, then must pass a silhouette test at giant scale: unique profile, one glow accent, instantly nameable shape ("that's the watchtower, that's the shrine"). If it can't be named from the god's eye view, redesign it.

**2.4 The Erased Drake placement is broken as written.** A drake at pilgrim scale right before the Gate would be trivial the moment you transform. *Fix:* the Drake is deliberately UNFIGHTABLE at pilgrim scale — a shelter/dodge/timing set-piece (it strafes the road; you shelter survivors under the old watchtower; you lure it to crash by baiting its dive onto the beacon). Then it RETURNS at giant scale — now killable in three swats. Same creature, two roles, one emotional arc: terror → revenge.

**2.5 World-change: scripted over systemic.** Spreading fire and systemic collapse are expensive and unpredictable on phone CPUs. *Improvement:* world-change moments are DETERMINISTIC SET-PIECES triggered by objective beats (tower collapse, lava burst opening a lane, bridge break) — authored, readable, cheap. The player still feels authorship ("I smashed the tower onto the spawn pit") because the trigger is their action.

**2.6 Friendly structure damage.** "Accidentally damage friendly structures" is great drama but brutal UX if one stray slam kills evacuees. *Improvement:* **CARE METER, not permadeath** — stray hits on the town/evacuees build Hollow-exploited wounds (corruption spawns from your own collateral). Punishing enough to make you careful, never run-ending.

---

## 3. SPACE TAXONOMY

| TYPE | FEEL | RULES | MOBILE RULE |
|---|---|---|---|
| SAFE | towns, shrines, camps | No enemies, NPC talk, rituals, banking | One tap = fast travel back once unlocked |
| THREAT | corruption zones, ambushes, elites | Telegraphs always visible, no instant kills | Combat spaces ≤ 12m across, one exit |
| DISCOVERY | secrets, lore, relics | Optional, never blocks progress, reliquary reveals | Max 1 hidden thing per screen |

**Not every room is a combat arena.** Target mission mix: 40% SAFE/DISCOVERY, 60% THREAT at pilgrim scale.

---

## 4. THE COLD RELIQUARY — MISSION ARCHITECTURE

**Premise:** The lantern-flame of Emberhollow has gone cold. Hollow corruption creeps down from the Ember Gate's battlefield. You — a lantern-sworn pilgrim — must carry the last Cold Reliquary to the Gate, evacuate the town, and wake the sleeping fire: Vaelthorn, the god beneath the mountain.

| # | BEAT | SCALE | SPACE | OBJECTIVE | WORLD-CHANGE | LANDMARK |
|---|---|---|---|---|---|---|
| 1 | EMBERHOLLOW | Pilgrim | SAFE | Take the Cold Reliquary from the Keeper; learn the rite | Corruption wisps visible at town edge | The town square brazier (giant-scale: a spark at your feet) |
| 2 | CHARCOAL FOREST | Pilgrim | THREAT+DISCOVERY | Cross via the fallen-log bridge; first small combat; find the hunter's cache | Reliquary light repels corruption blobs as you pass | Fallen log bridge (giant-scale: a twig) |
| 3 | RUINED SHRINE | Pilgrim | DISCOVERY | Light the shrine brazier; read the lore tablet ("the Sleeping Fire below") | Lighting it makes the distant Gate PULSE visibly for the first time | The shrine (giant-scale: fits in your palm) |
| 4 | DRAKE CROSSING | Pilgrim | THREAT set-piece | UNFIGHTABLE Erased Drake strafes the road: sprint cover-to-cover, shelter survivors under the Watchtower | The Drake scorches a new lane through the forest (opens giant-scale shortcut) | The Watchtower (giant-scale: knee-high, smashable) |
| 5 | EVACUATION ROAD | Pilgrim | SAFE→THREAT | Escort 6 survivors along the road to the Gate; Hollow ambush the column; the lit reliquary holds them off | Ambush craters become giant-scale terrain hazards | The road ribbon |
| 6 | CINDER GATE RITE | Sprite→Giant | SAFE (sacred) | Stand in the Gate's foot-circle; perform the rite | ASCENSION: camera pulls up, you become the god | THE GATE — visible from beat 1, the whole mission's compass |
| 7 | GIANT BATTLEFIELD | Giant | THREAT (open war) | Hold three lanes (forest choke, lava ford, cliff ridge); the Drake returns — now killable (revenge beat); smash the Watchtower onto the spawn pit; PROTECT the evacuee column still moving (care meter) | Your own smashing reshapes lanes: craters, collapses, lava bursts | EVERYTHING from beats 1–5, now at your ankle |
| 8 | ECHO OF THE FORGOTTEN ARENA | Giant | BOSS | The Hollow's stolen giant rises from the caldera — T3 duel, phases at 66%/33%, arena = the caldera floor with lava hazard rings | Boss slams reshape the arena floor | The Gate at your back, the town far behind |

**Flow shape:** SAFE → DISCOVERY → THREAT(survival) → SAFE(escort) → TRANSFORM → WAR → DUEL. The emotional arc: curiosity → dread → duty → apotheosis → wrath → judgment.

**Same world, two scales — the payoff moments:**
- At giant scale you SEE the entire path you walked — town at your heel, forest at your ankle, the shrine you lit a candle in resting in the palm-line of your footprint.
- The Watchtower you sheltered under becomes the weapon you topple.
- The Drake that hunted you becomes the kill that closes the loop.

---

## 5. HANDCRAFTED vs PROCEDURAL

**Handcrafted (identity-critical):** major towns, story dungeons, shrines, Gates, boss arenas, major landmarks. Also: all mission set-pieces (drake strafes, collapses).

**Procedural (fill + replay):** minor caves, Hollow nests, side ruins, random encounters, small optional dungeons — all built from the biome kits, all Discovery-tier. Procedural content NEVER contains story beats or Gates (Gates are always authored landmarks).

**Army composition at giant scale is procedural; the battlefield lanes are authored.**

---

## 6. MOBILE READABILITY RULES

1. **One Landmark Rule** — at any position, exactly one landmark dominates the view (Gate, tower, shrine). No vista contains two competing silhouettes.
2. **Compass landmark** — the active objective's landmark is always visible or direction-glowing (the Gate pulse).
3. **No dead ends without rewards** — every wrong turn holds a Discovery.
4. **Loops not lines** — shortcut unlocks kill backtracking (drake-scorched lane in beat 4 becomes the giant shortcut in beat 7).
5. **Combat readability** — sprite arenas ≤ 12m; giant lanes 40–80m wide; every hazard telegraphs ≥ 1 second.
6. **Silhouette-first assets** — double-duty geometry rule from §2.3.

---

## 7. THE PROTOTYPE

`web-prototype-cold-reliquary.html` — a playable vertical slice of this architecture: one continuous map (Emberhollow → forest → shrine → watchtower → road → Ember Gate → battlefield → caldera), played FIRST as the pilgrim (escort, survival, discovery beats) then the GATE RITE wakes the giant on THE SAME GEOMETRY — landmarks you walked now at your feet, evacuees to protect, the Drake returned for its revenge beat, the Echo of the Forgotten duel.

