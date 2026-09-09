# LEVEL DESIGN — campaign route, dungeon layouts, battle arenas (v1 canon)
Basis: CAMPAIGN-ROUTE-MAP-v1 + six Depths layout maps + seven Gate arena keyshots (BudE404 level-design pass, Sept 5 2026).
Zero text in the art — this doc holds the names and the build specs.

## THE PILGRIMAGE (campaign route)
Order of chapters (matches questline docs; cross-region consequence wiring noted there):
1. ASHFALL — The Cold Mere (tutorial realm: Cinder Gate, rite-tool mastery)
2. SKYREND — The Unread Sky
3. EVERBLOOM — The Harvest Debt
4. DUSKMOURN — The Lantern Ledger
5. MARENTH — The Tide's Accounting
6. STONEHEART — The Unmoved Promise
Each chapter = 5-6 quests + the Gate Rite finale (roll-call rite, T3 Echo duel, pilgrim-scale final phase). After all six: THE GATE CYCLE endgame in the Depths (procedural god-dream dungeons, relic-gift loot).

## DEPTHS DUNGEON LAYOUTS (SHEET-DUNGEON-LAYOUTS-6-v1.jpg)
Shared layout grammar (Diablo lane per Quest Doctrine): entrance stair from the realm surface → branching corridor network with TWO loop routes → side chambers (fever mobs = pressure lane) → mini-boss hall on one wing → central HEART chamber (relic altar; sworn-drake territory lane) → deepest end drains to GREY de-coloration (Hollow lane — the nightmare's edge). Pilgrim lantern dots mark the golden path. Procedural build: the loops and wing chambers reshuffle; the heart chamber + grey end are fixed poles.

Per-realm set dressing: Ashfall forge-cathedral/magma heart · Skyrend sky-bell chambers/bell-chain bridges · Everbloom root corridors/rot gardens · Duskmourn barrow-shelf catacombs/votive lantern heart · Marenth flooded halls/tidal dais · Stoneheart tilted strata slabs/half-carved vow-stone heart.

## DEPTHS CONTENT MODEL — one BIOME per realm, infinite dungeons
The six layout maps are NOT six single levels — each is its realm's dungeon GRAMMAR (the biome kit the procedural builder constructs from). The Depths are an ENDLESS DESCENT per realm:
- Every run = a freshly generated dungeon from that realm's grammar (loops + wings reshuffle; heart chamber + grey-end pole fixed).
- DEPTH TIERS (Gate Cycle endgame): descending tier number = deeper, sicker, denser fever, richer relic-gift loot, and the grey de-coloration creeps closer to the entrance the deeper you go. The deepest tier of a realm borders the Wound.
- Chamber families per realm give the generator vocabulary variety — e.g. Ashfall mixes forge-cathedral, magma cavern, drowned-forge, and ash-gallery chamber families into one descent.
- Challenge poles: sworn drake (territorial lane) guards the heart chamber each run; the mini-boss wing rotates through the realm's T1/T2 Hollow pool.

## GATE BATTLE ARENAS (SHEET-GATE-ARENAS-7-v1.jpg)
Arena grammar (Waking Siege + Stationary God laws): the Gate is the arena's heart and the thing under attack. RISE POINT = ritual stone ring directly before the Gate — the god emerges THERE and never moves. Hollow waves spawn at the field's far edges and march ON the Gate. Terrain toys + hazards radiate outward. Player defends by channeling the god: palm surges, environment smashing, Gate Rite ultimate. Camera doctrine: god-vs-waves = front view facing the player-god; god-vs-god = side view.

| Arena | Rise point | Field hazards | Terrain anchors |
|---|---|---|---|
| CINDER GATE | cracked shrine-stone circle | magma fissures, lava channels | ash dunes |
| SQUALL GATE | storm-bell stone ring | lightning glass scars | wind-vanes, bell towers |
| BLOOM GATE | living-root stone ring | vine snares, pollen banks | root ramparts |
| DUSK GATE | lantern-shrine ring | mist concealment banks | votive braziers |
| DEEP GATE | tide-marker ring on shallows | tide channels, whirlpools | sea-stacks, drowned ruins |
| STONE GATE | monolith-stub ring | fault cracks, rolling boulders | strata terraces, geode outcrops |

THE WOUND (T3 finale arena — ARENA-THE-WOUND-ECHO-v1.png): a grey de-colored plain where the paint itself gives out; the arch is built of ERODED PIECES OF ALL SIX GATES stacked into one impossible arch (the Hollow steals identity), zero glow anywhere; de-coloration rings spread outward; the only color in the frame is the pilgrim's lantern flame. This is the Echo of the Forgotten's rise point — matches echo-duel.html: stationary duel → drained field → the wound → THE NAMING.

## BUILD SPECS
- Arena scale: duel gap ~17 units between rise points (echo-duel.html tuning).
- Dungeons: hand-authored heart chambers + grey-end pole rooms; procedural reshuffle of corridor loops (Depth seed per run).
- Wave doctrine: Hollow spawn beyond fog line, never inside ritual ring; Gate integrity is the fail state (siege.html).
- Hazard timing: arena hazards pulse on the wave cadence (4.2s wave rhythm from echo-duel Phase II).
