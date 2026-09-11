# AVALON — THE UI BIBLE
## Cinematic Mythic Relic · the awakened interface (v1, Sept 6 2026)

Direction (BudE404): the UI is an ancient, powerful artifact brought into a modern mobile
interface. The artwork stays the centerpiece. Materials are used selectively — bronze,
glass, carved notch-work — never stone everywhere. Nothing invented: all names, factions,
places, abilities come from the vault.

Live build: `game-ui.html`

---

## THE THREE LEVELS OF INFORMATION
1. **PRIMARY** — the artwork. Full-bleed living world (parallax breathes), character identity.
2. **SECONDARY** — translucent relic panels: stats, abilities, mission info, equipment.
3. **TERTIARY** — hold-to-reveal tooltips, micro-labels (letter-spaced caps). Never competes.

## MATERIALS (selective, never heavy)
- **Relic-bronze rails** — 1px borders at ~42% opacity, engraved inner groove (the ::before).
- **Obsidian glass** — rgba(12,8,4,.62) + backdrop blur 10px; panels float over art.
- **Carved notch corners** — chamfer clip-paths (12px outer / 9px inner), NO rounded plastic.
- **Ember energy** — feedback only: the active-nav underline, FAITH flame, current route ring.
- **Grain + vignette** — the artifact is old; every screen breathes with film grain.

## TYPOGRAPHY
- **Display** — Cinzel (fallback Georgia): titles, names, buttons. Weight 700, tracking .05em.
- **Body** — Cormorant Garamond italic: epithets, flavor, descriptions (the myth voice).
- **Micro** — Cinzel 600, 9–10px, tracking .2em uppercase: labels, nav.
- Hierarchy test: name (24px+) > epithet (15px italic) > stat (13px) > micro-label (10px).

## ICONOGRAPHY
Inline SVG, stroke 1.6, relic-bronze. The Gate Rune (trilithon arch + flame) is the master
icon — the hub sigil, the WARD button, the faction node flame, the Mark relic all share it.
Icons are engraved (stroke), never filled blobs — except the flame.

## THE GRID
8pt spacing. 12px screen margins (thumb-safe). Bottom nav max item 86px.
Bottom dock zone: 104px above nav (relic-dock, mode-card ride above the rail).

## NAVIGATION
- **Bottom rail** (frequent): HUB · WORLD · ROSTER · RELICS · CODEX · SETTINGS.
- **Contextual** (secondary): top-right carved 9-dot menu → WORLD · EVENTS · SETTINGS.
- **Identity** (top-left): the Gate Rune sigil = player profile. FAITH chip top-right.
- **The world map transitions straight into battle** (See → Select → Understand → Act).

## SCREEN GRAMMAR (one artifact, many functions)
- **HUB** — Vaelthorn's emergence as the living world; name plate + chips (status/tier/kit)
  bottom-left; ANSWER THE GATE CTA; art uncovered by any full-screen panel.
- **WORLD (THE ROADS)** — heartland dawn map; 8 route nodes (canon routes/realm names) with
  faction flame colors; states: done ✦ / current (ember ring) / locked (dimmed). Chapter
  card at bottom → straight into the Echo Duel.
- **ROSTER** — canon colossi cards (emergence art), status dots in element colors; tap →
  CHARACTER screen (shared shell).
- **CHARACTER** — hero art center 18%; stat rail left (VESSEL/FAITH bars + MIGHT/REACH);
  ability dock right (QUAKE/SURGE/VOLLEY with hold-tooltips); relic dock bottom (Mark,
  Cold Reliquary, Way-Daggers + one empty socket — relics are earned, never sold).
- **RELICS** — same chamfered cards, EARNED·NEVER SOLD law on the header.
- **CODEX** — six records; THE CINEMATICS deep-links to the story film. "Told in play."
- **SETTINGS** — carved dials: cinematic camera, ember particles, stone voice (haptics),
  reduced effects, language. Same frame language, quiet environment.

## COMBAT (See → Select → Understand → Act)
- **See** — the world fills the screen; minimal top: VS unit chips w/ thin HP bars + compact
  objective strip ("ECHO DUEL · THE GATE OF EMBERS").
- **Select** — ember range rings mark what's targetable.
- **Understand** — contextual reveal panel slides in (name, line, VESSEL/STAGGER/PATTERN) —
  never a modal wall.
- **Act** — ability tray in thumb reach: big WARD + SWING/SURGE/VOLLEY/QUAKE with cooldown
  veils; FAITH counter floats above the tray. Live combat: echo-duel.html / siege.html.

## MOTION & FEEL
- Screens cross-fade .5s; panels unfold (translateY + fade .28s).
- The world has parallax (pointer/gyro) — the artifact holds still, the world breathes.
- Press feedback: scale .93–.96 + stone-tick (WebAudio) + 9ms haptic.
- Ember glow = attention: current route, active nav, FAITH. Nothing else glows.

## FLEXIBILITY LAW
Screens are data-driven (FACTIONS / COLOSSI arrays at the top of game-ui.html). New faction,
colossus, relic, or route = add one object; the interface reshapes itself. The system admits
new screens by copying the .relic frame + typographic levels — no new visual language needed.
