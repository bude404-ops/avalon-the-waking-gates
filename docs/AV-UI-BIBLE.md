# AVALON — UI BIBLE
Genre-peer research distilled into AVALON's own canon UI system. Companion to `docs/AV-UI-LAYOUT-SPEC.md`.

## Peer research — what the best in genre do

### God of War 2018 / Ragnarök (Santa Monica — 80.lv deep dive)
- **Single-column left menu, huge right-side preview.** Navigation never forks; every screen reads the same way.
- **One message at a time.** All HUD messaging runs through a single queue — the screen never gets loud.
- **Combat HUD anchored bottom-left** (health/abilities/rage); everything else is a quiet UI message.
- **Font scale accessibility** — 2018 shipped fonts too small; Ragnarök built dynamic layouts so text scales without breaking layout. LESSON: design the layout around the text, not vice versa.
- **Unified conventions** — identical tab/paging language across pause menu, vendors, inventory.

### Elden Ring / FromSoftware
- **Minimal, elegant HUD**: health/FP/stamina top-LEFT in thin bars; tools bottom-left. Rest of screen belongs to the world.
- **Serif display type on dark translucent panels** — quiet, aged, mythic. Menus = one horizontal tab row + one vertical list. Nothing else.
- **Status effects as small glyph icons** — no paragraph text in combat.
- Restraint is the aesthetic: ornament lives in type and one gold accent, not in borders everywhere.

### Ghost of Tsushima
- **Near-zero HUD.** No persistent clutter; guidance is diegetic (wind, foxes, birds).
- **Location title cards** — beautiful typographic moments when entering regions. Establishes place with zero UI.
- **Radial submenu** for gear swipes; the world is always the star.
- LESSON: the less UI, the more mythic the world feels.

### Dark Souls
- **HUD fades when idle** — screen returns to pure world after seconds of stillness.
- Bonfire menus are list-first, minimal, same visual grammar as the world.

### AC Valhalla (as counter-example)
- Ornate Norse knotwork everywhere — impressive at first, fatiguing over 100 hours. LESSON: ornament as ACCENT (dividers, sigils, corners), never as full chrome. We take the mythic tone, not the maximalism.

## THE AVALON UI SYSTEM — the synthesis
Three laws, all canon-compliant (cold desaturated world, two-layer light, region colorways):

### LAW 1 — THE LANTERN IS THE HUD
The Lantern anchor is our signature HUD centerpiece (GoW has rage, we have BELIEF):
- **Belief meter = the lantern's glow.** The meter is literally a lantern glyph that fills with light. Unlit → guttering → lit reads at a glance, no numbers in combat.
- Health is a thin slate bar (Elden Ring line-weight) under the class crest, top-left.
- Ability slots bottom-center, each a **gate-rune sigil** — sealed runes are unlit, unlocking = the rune ignites. Progression language = THE FLAME IS NEVER SEEN UNVESSELLED, even in UI.
- Region aura colorways govern accents (storm slate / oxide red / bone grey / crimson / tide teal / deep moss) — lantern accent always matches home realm.

### LAW 2 — QUIET, ONE MESSAGE, FADE TO WORLD
- GoW-style single message queue: one quest/journal/update line at a time, top-center, small serif caption, fades after 4s.
- Dark Souls-style **HUD fade**: after ~6s idle, all HUD alpha → 15%. Touch wakes it. The world (the actual product) is the show.
- Quest tracker = one line, no counters, no checklist chrome.

### LAW 3 — SINGLE-COLUMN MENUS, MYTHIC TYPE
- GoW-style single-column left menu + right preview pane (character/gear/lore) — one navigation grammar everywhere (pause menu, skill paths, codex).
- Elden Ring type discipline: serif headers (gate-gold), quiet grey body, ALL-CAPS tracking for section labels. Ornament ONLY as: knotwork divider lines (2px, low alpha), corner brackets on panels, and class sigil gate-runes.
- REGION TITLE CARDS (Tsushima law): entering a realm = a full-screen typographic moment — realm name in serif caps, one-line epigraph, region accent underline, 2.5s fade. This is our "wow" screen and it costs nothing.
- Title screen: dark stage, title serif, ONE button. No menus on boot.

## Screen flow
TITLE (one button) → CLASS SELECT (cards, model live behind) → REALM TITLE CARD → IN-GAME (Law 1 HUD) → CHARACTER TAB (single-column skill paths, right preview).

## Shell implementation notes (v3, runtime Shell.cs)
- CanvasScaler ScaleWithScreenSize 1920×1080 landscape, match 0.5 — kills the "spread out" scaling bug.
- defaultInterfaceOrientation = LandscapeLeft.
- HUD-fade coroutine (idle 6s → alpha 0.15) on panelGame.
- Message queue label top-center (single line, queued, 4s life).
- Region title card on entering Game state.
- Model-load hardening: fallback Resources.LoadAll scan + on-screen diagnostic line if no model staged.
