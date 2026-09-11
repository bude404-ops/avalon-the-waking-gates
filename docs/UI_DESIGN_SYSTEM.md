# UI DESIGN SYSTEM v1 — THE CINEMATIC MYTHIC RELIC
*Canon law for every interface in AVALON: THE WAKING GATES. Directive: BudE404, Sept 6 2026.*

## THE ONE RULE
The UI does not sit on top of the game — it is another layer of the game world.
2D artwork tells the mythology. 3D models make the world tangible. The UI connects the two.

## SOURCE OF TRUTH
Vault lore, art, models, and data are the only canon. Never invent names, factions, lore, or visuals for an interface. If a system is undefined, the UI leaves it out — the menu does not speak for the road.

## THE TWO MEDIUMS
**3D for what is tangible:** character showcases, equipment previews, combat, boss reveals, creature showcases, relic inspection, rotatable displays, the hub environment.
**2D for what is myth:** storytelling, lore, codex, portraits, loading screens, worldbuilding, quest illustrations, atmospheric transitions, cinematic backgrounds.
Never force one medium to do the other's job.

## SCREEN LAWS
- **MAIN HUB (GATES)** — a living 3D scene: the player's character at the Gate, ember field breathing, camera in slow drift. Navigation frames the scene; the character and the Gate never leave view.
- **CHARACTER (ROSTER)** — the actual 3D model is the centerpiece: drag-rotate, pinch-zoom, camera presets (PORTRAIT / PROFILE / FULL), ability previews from the real animation kit. Info plates frame the model, never cover it. Selecting a COLOSSUS triggers the boss reveal (letterbox → low-angle push → rumble → name → ENTER THE FIELD → gameplay).
- **WORLD** — the eight roads exist as flames in the 3D scene, not buttons on a list. Tap a flame → camera eases to it → 2D story card (realm, route, flame color) → WALK THE ROAD → gameplay.
- **COMBAT** — battlefield dominates, UI minimal, primary controls in thumb reach (see echo-duel HUD doctrine).
- **CODEX** — the 2D archive: illustrated plates of canon art, lore by omission, records unlock through play. Full-plate viewing for each record.
- **FIELD** — battle select as canon-art cards linking to the live prototypes.

## MATERIAL LANGUAGE
Dark refined stone · ancient metal edges · engraved gold type · chamfered (megalithic) corners, never rounded plastic · translucent relic-glass panels · restrained ember glow (color = elemental feedback, not decoration).
**NOT every panel is stone.** Premium, sophisticated, restrained. Energy accents flow through symbols; relic activation is the interaction metaphor.

## INTERACTION LAWS
- Touch-first: large targets (54px+ primary), thumb-zone dock, safe-area insets.
- Tactile: every press physically depresses (`.pressed`), ticks with stone sound, vibrates where supported.
- Tertiary numbers live on HOLD-TO-REVEAL tooltips — the face stays clean.
- Motion is cinematic, not arcade: unfold, fade-rise, slow drift, breathing flames. No bounces.
- Sound: soft stone tick (press), low swell (commit), double chime (screen change).

## IMPLEMENTATION
- `docs/ui/mythos-ui.css` — tokens + materials + components (buttons, cards, bars, tooltips, unfold, art veil).
- `docs/ui/mythos-ui.js` — MUI: tactile press, hold-reveal tooltips, parallax (pointer + gyro), unfold orchestration, WebAudio stone-and-flame sounds.
- `docs/game-ui.html` — the full living interface: 3D hub (Warden + the Gate + embers), character screen with GLB previews and boss reveal, 3D world flames, 2D codex archive, field select.
- `docs/index.html` — the menu shell on the same system.

## 3D PRESENTATION GRAMMAR
Camera: wide establishing, low hero portrait, profile, free orbit. Loaders say SUMMONING. Models drop feet-to-ground and keep the ember key/rim light. Boss reveals end in a direct transition into player control.

## CONTINUITY
Characters → environments → creatures → architecture → weapons → effects → cinematics → gameplay. Equipment shown in UI must be the same geometry that swings in the field. New UI art follows the ART STYLE BIBLE fingerprint — same art department, same game.


## TITLE MENU LAW v1 — LOCKED (Big, Sept 11 2026: 'The title menu i like and want that to be how ours will be')
The approved canon plate is `art/approved/UI-MAIN-MENU-CANON.png`. The real title menu is built to this plate:
- **Title block:** "AVALON" in engraved Celtic knotwork lettering, subtitle "THE WAKING GATES" beneath.
- **Scene behind the menu:** the colossal megalithic Gate standing in cold storm-fog mountains, a processional stone path leading to it, amber braziers lining the path — the braziers are the ONLY warm light in frame (Fire-Color Law holds: natural amber-orange, never the region hue).
- **Palette:** cold desaturated slate; the brazier flames are the single saturated source.
- **Option list (lower third):** CONTINUE, NEW JOURNEY, GATES, SETTINGS — rendered as carved stone buttons with bronze knotwork borders; no modern flat-glass UI anywhere.
- **Atmosphere:** mist drifts over the UI edges — the menu sits INSIDE the world, never floats on top of it (The One Rule).
- **Orientation:** 9:16 portrait first (S22 Ultra is the reference device), horizontal reflow per the orientation law.
This plate governs the actual Unity title menu build (scene layout, lighting, camera drift, button styling). The class-select and gameplay-HUD concept plates are NOT yet locked — verdicts pending.