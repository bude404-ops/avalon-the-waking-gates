# MYTHOS GATES: ASCENSION — CORE GAMEPLAY STRUCTURE (v1)
## Both forms are the game. The loop is the game. Full Ascension is the climax.

Built ON established lore (source of truth: docs/lore/MYTHOS_WORLD_LORE.md, GATE_MECHANICS_GAMEPLAY_LAYER.md, COLD_RELIQUARY_MAP_ARCHITECTURE.md — nothing here rewrites canon).

**THE SYNTHESIS (the one new idea):** the three tiers of Giant presence are the three degrees of Gate opening. The Gate layer already established that a deity's mortal manifestation is capped by its Gate's state — so:

| TIER | GATE STATE | WHAT CROSSES | COST | DURATION |
|---|---|---|---|---|
| **1. GLIMPSE** | Gate nearly closed | A single limb/force — a hand, a foot, an eye, one fire-lance | 10–20 FAITH | 3–5 seconds |
| **2. MANIFESTATION** | Gate cracked open (RITE STONE) | The full colossal body, thinly — tethered to the stone's radius | 40–60 FAITH | 45–90 seconds |
| **3. ASCENSION** | Gate ACTIVE (at the Gate itself) | The TRUE colossal form, full essence, battlefield strength | 100+ FAITH + full rite | Minutes — mission-scale |

One lore answer, three gameplay tiers. Frequency problem solved by canon, not by arbitrary respawn timers.

---

## THE CORE LOOP (mission grammar)

1. **EXPLORE AS SPRITE** — investigation, NPCs, secrets, small-scale Hollow, environmental puzzles. The world is enormous: house = dungeon, tree = bridge, crack = entrance.
2. **DISCOVER A PROBLEM** — something that CANNOT be solved at pilgrim scale (a boulder, a corrupted structure, an elite, a ravine). The game TELLS you: blades glance off, doors don't move, "THIS IS BEYOND YOUR SCALE."
3. **BUILD DIVINE CONNECTION** — bank FAITH (kills, rescue, rituals, discoveries), find the rite stone / shrine / channel-stone. This step IS gameplay — never a menu.
4. **MANIFEST** — the tier appropriate to the problem. Glimpse for obstacles; Manifestation for structures/elites; Ascension for battlefields.
5. **SOLVE WHAT ONLY A GOD CAN** — destroy, reshape, break, protect, alter the battlefield.
6. **RETURN TO SPRITE** — and EXPLORE THE CONSEQUENCES: wreckage becomes a discovery site (loot, lore, opened passages). The giant's actions generate new sprite content. **This beat is mandatory in every manifestation** — it's where the two scales kiss.
7. **DISCOVER SOMETHING DEEPER** — every loop reveals one new lore layer (Gates, Hollow, deities, the Forgotten One). The world progressively opens.

**→ FULL ASCENSION as the mission/region climax** — extended giant gameplay: battlefields, armies, colossal enemies, evacuation/protection, world-shaping stakes.

**Loop pacing per mission: E–M–E minimum; E–G–E–M–E typical; A only at region climaxes.** A standard mission alternates forms 2–3 times. Giant is a recurring pillar (every mission) but NEVER the default state (sprite remains the traversal/investigation identity).

---

## THE THREE TIERS — design rules

### 1. GLIMPSE (the everyday miracle)
- A colossal hand clears the rock; a god-foot stamps the Hollow flat; a single eye opens in the storm and burns a corrupted wisp
- Contextual and contextual ONLY — offered when the situation is worthy (blocked path, crushing loss, a moment of wonder). Never on cooldown rotation; the game PROPOSES it, the player invokes it
- Camera: brief scale-shock — pull wide for the 3 seconds so the hand looks HUGE against the sprite world
- Failsafe: costs faith, so spamming starves your manifestation economy

### 2. MANIFESTATION (the 60-second god)
- Trigger: RITE STONES — ancient channel-stones placed within a region (each near its Gate's influence radius — the tether law holds: you manifest where the Gate can still reach)
- 45–90 seconds, visible timer = the essence link draining (body opacity thins as time runs — the form is spent)
- Objectives sized for the window: ONE structure destroyed, ONE elite felled, ONE breach opened, ONE village shield held
- **Early completion returns the unspent essence as faith** — speed is rewarded with economy
- The land is ALTERED: rubble, craters, opened walls — and beat 6 turns that alteration into new sprite content

### 3. ASCENSION (the climax)
- Only at an ACTIVE Gate, via full rite (charge + warden + cleanliness, per the Gate layer)
- Battlefields per the Map Architecture doc: lanes, destructibles, hazards, distributed objectives, care meter, colossal duel
- Longest encounters, biggest story consequences — region-shaking, not mission-shaking

---

## SPRITE VERB SET (the investigation kit)
exploration · NPC dialog & investigation · personal combat w/ dodge+PARRY (parry = perfect-timing counter that refunds FAITH — skill feeds the divine economy) · environmental puzzles · secrets (reliquary reveals) · lore discovery · rescue · rituals · faith-building · small-scale Hollow · scouting future battlefields (recon payoff)

## GIANT VERB SET (the force-of-nature kit)
**THE RULE: the Giant is NOT sprite stats ×100. The giant gets a different INTERACTION SET:**
- POSITIONING matters (lanes, hazards, link management) — not dodging
- AREA attacks (sweeps, slams, elemental waves) — never single-target duels
- ENVIRONMENTAL DESTRUCTION (structures, bridges, walls — with collateral care)
- BATTLEFIELD CONTROL (crater chokepoints, toppled-tree area denial, lava routing)
- TERRAIN MANIPULATION (dam a stream, collapse a cliff onto a column)
- ARMY-swiping (crowds, not elites — elites are COLOSSI, and colossi are duels)
- PROTECTION (shielding evacuees with your own body)
- The design test every giant beat must pass: **"What can I do to this battlefield BECAUSE I am enormous?"** If the answer is "kill it faster," the beat fails.

## ANTI-PATTERN CHECKLIST
✗ Giant = sprite combat with bigger numbers
✗ Glimpses on rotation/cool-down spam
✗ Manifestation sites that don't change the world
✗ Skipping the return-to-sprite consequence beat
✗ Disconnected giant arenas (all battlefields are walked-first geography)
✗ Ascensions more common than region climaxes

## FAITH ECONOMY (the fuel of the loop)
- Parries +10 · Hollow kills +10–15 · rescues +25 · discoveries +30 · rituals +40
- Glimpse 10–20 · Manifestation 40–60 · Ascension 100+ charge
- A clean mission alternates forms 2–3 times on the faith it EARNED that mission — the economy itself enforces "recurring but not constant."

## PROTOTYPE
`web-prototype-three-tiers.html` — the full loop playable: sprite investigation → glimpse (colossal hand clears the crushed bridge) → manifestation (60s god-smash of a corrupted nest too big for a sprite) → RETURN TO SPRITE and explore the wreckage (new lore revealed in the rubble — the two scales kiss) → ascension at the Gate (battlefield waves + mini-colossal). Every tier is invoked by the player, costs faith, and changes the world.
