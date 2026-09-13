# AVALON — Game Shell UI Layout Spec

Living spec for the game shell UI. The Unity review shell (`unity/Assets/AvalonShell/Shell.cs`)
is the living mock — every screen below is staged there as the pipeline ships classes.

## Canon UI Style (locked)

- **Palette:** cold slate backgrounds `#0d0e10` / panels `#0a0b0d @ 88%`, bronze text `#a3895a`, cream `#e6ddca`, muted grey `#8a8578`, locked grey `#6f6a5e`.
- **Lighting read:** two-layer cold light. UI never glows warmer than the world.
- **Ornament:** bronze knotwork + La Tène spiral frames (patterns only — gate-rune is the only mark, per rune law).
- **Type:** single mythic serif family; ALL-CAPS for titles, role lines in bronze, lore in muted grey.

## Screens

### 1. Boot / Title — TITLE MENU LAW v1 (LOCKED, Big Sept 11: 'The title menu i like and want that to be how ours will be')
- Governing plate: UI-MAIN-MENU-CANON.png. "AVALON" engraved knotwork hero, "THE WAKING GATES" sub, Gate-in-storm-fog key art behind, mist over UI edges.
- Carved stone option list, lower third: CONTINUE, NEW JOURNEY, GATES, SETTINGS (bronze knotwork borders, no modern flat-glass).
- NEW JOURNEY + GATES → Character Select; CONTINUE + SETTINGS sealed until saves/options ship.
- Version + copyright line at the plate base: "V 0.2.11 © 2026 WAKING GATES, INC. ALL RIGHTS RESERVED." (bump per versionCode).

### 2. Character Select — CLASS SELECT LAW v1 (LOCKED, Big Sept 11)
- AVALON overline top, "CHOOSE YOUR CLASS" header (per plate).
- Six class niches in a carved stone-knotwork frame (UI-CLASS-SELECT-CANON.png governs): portrait-first 2-col x 3-row grid, actual canon class art in the niches, selected niche bronze-lipped.
- Locked classes show "IN THE FORGE"; forged classes show "FORGED" (bronze).
- Card: class name / role line (bronze) / lore blurb (grey) / realm pairing:
  Sovereign→Skyrend, Ravager→Ashfall, Warden→Stoneheart, Veilborn→Duskmourn, Weaver→Marenth, Wildborn→Everbloom.
- Live model on the Unity stage behind the cards; card tap swaps the model instantly.
- Selected card reads bronze-tinted.
- BEGIN THE WAKENING → In-Game view (Big's button ruling, Sept 11: the Character Select button reads BEGIN THE WAKENING).

### 3. In-Game HUD
- **Top-left crest:** class name (cream) + LEVEL line + realm.
- **Health bar:** slate fill `#6b788c`, label in-panel. The Ravenkin companion carries the reliquary vessel — the game's relic-carrier (stages in the HUD art as the companion system ships).
- **Belief meter:** bronze fill `#a3895a`, under health. NO numbers in combat — reliquary-glow read per AV-UI-BIBLE Law 1. Abilities draw on BELIEF, not stamina — restored through shrines, story choices, divine bonds; never ground out. Health stays physical, also numeral-free.
- **Top-right:** THE RELIQUARY — UNLIT (flame state per Reliquary Law; alights when entrusted).
- **Quest line TOP-CENTER per canon plate:** "REACH THE FIRST GATE" (Law 2 — one line, single queue).
- **Region title card (Law 3):** on entering game — "SKYREND" + epigraph "The gate remembers who lit the flame." + storm-slate underline, 2.5s hold then fade.
- **Ability bar:** 4 slots bottom-center, SEALED until skill trees ship.
- **Region banner (future):** entering a realm splashes the realm name + colorway whisper (Reliquary-Region Aura Law).
- **Controls (GAMEPLAY LAYER 1 — tap-to-walk, Cold Reliquary slice):** tap the ground = walk order (Sovereign walks the route via the root-locked walk clip); drag = orbit; pinch/scroll = zoom. IDLE/WALK review toggles RETIRED.
- **CHARACTER button:** bottom-right → skill tab over the game view.

### 4. Character / Skill Tab
- Right-side panel: CHARACTER — SKILL PATHS.
- Slots: SKILL PATH I–III, MASTER CLASS, DIVINE BOND, RELIC — sealed until progression ships.
- Future: tap a path → tree view; level-up gates unlock nodes (anti-grind law: no stat inflation, mastery/cosmetic).

### 5. Map (BUILT — map screen pass)
- MAP button in the HUD opens the map screen: Cold Reliquary — Ashfall slice live (approved canon plate MAP1), quest pin line under the map.
- Landmark pins (Cinder Gate, shrine brazier, watchtower) land with the pin pass; realm starter maps stage as each plate is approved.

### 6. Relics (staged next)
- Relic inventory: the Luminary line — meaningful artifacts only, never stat loot.

### 7. Settings (staged next)
- Audio mix, controls, data. No FOMO mechanics anywhere (Progression & Player-Respect Law).

## Pipeline Notes
- All shell UI is runtime-built uGUI (headless CI needs no scene authoring).
- versionCode cadence: 101 = v1 review shell, 102+ = game-structure builds; stable demo keystore `unity/AVALON-DEMO.keystore` (alias `avalon`) so every build updates over the last. Production moves to a held-secret key.
- Each new class prefab lands in Character Select same-day the Forge ships it.
