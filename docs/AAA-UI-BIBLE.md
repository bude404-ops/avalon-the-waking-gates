# AAA-UI-BIBLE — AVALON: The Waking Gates

Bude's mandate, Sept 14 2026 (verbatim basis): "Build the menu UI to AAA-quality presentation,
not a basic collection of buttons and text."

## THE ORDER OF WORK
composition -> hierarchy -> materials -> lighting -> animation -> interaction -> polish.
Never "more decoration." Every screen intentional and production-ready.

## THE LAWS
1. FOUNDATION BUILDS ONLY — no pasted art carries UI (v230 law). The reference art is the
   style bible, never the asset.
2. ONE CENTRALIZED DESIGN SYSTEM — every menu and submenu wears the same shared components:
   PanelChrome (dark glass + bronze sliced border + corner diamonds), Heading (engraved
   Cinzel, carve + sheen), CloseBtn (carved slab + code-drawn glyph, dual-wired),
   EngravedOption, CarvedSlab, NicheFrame, ClassSigil, IconSprite set. No screen invents
   its own materials.
3. TYPOGRAPHY HIERARCHY — title (beveled 3-layer silver stack) > heading (engraved bronze
   Cinzel 17) > body (parchment 13) > stat (muted 9-11). Never one flat size everywhere.
4. CUSTOM ICONS ONLY — code-drawn glyphs (close/back/map/...). No placeholder symbols.
5. PHYSICAL BUTTONS — press-depress, hover tint, glow pulse on selection, transition
   states on every interactive element. Dual-wired input always (EventSystem + TapRouter).
6. ALIVE SCREENS — cinematic backdrop + parallax drift + ember motes + fog; every panel
   enters with the same motion (PanelIn: 0.30s smooth scale 0.94->1 + fade).
7. ATMOSPHERE — particles, light movement, environmental motion are part of the material
   budget, not decoration bolted on.
8. RESPONSIVE — anchor-driven layout, portrait/landscape relayout, touch targets sized
   for thumb reach without oversized UI.
9. CONSISTENT WORLD — character/world imagery is COMPOSED INTO screens (sigils in the
   niches, the drawn map, the mist worlds), never dropped behind the UI as a poster.

## THE SHIP GATE — polish pass before any UI is called complete
1. Alignment  2. Spacing  3. Typography  4. Icon consistency  5. Contrast  6. Depth
7. Lighting  8. Animation timing  9. Touch/controller usability  10. Overall AAA presentation

## ROADMAP (approved direction, in order)
- v232 (this commit): centralized design system — PanelChrome + Heading + Icon set +
  CloseBtn + PanelIn entrance motion + ember motes, applied to Settings / Achievements /
  Map (the last unstyled screens).
- Next: controller/keyboard focus navigation (gamepad dpad + arrow keys moving the option
  list with ember focus glow) — extends the dual-path input law.
- Next: deeper parallax (backdrop layer offset drift vs fog vs motes, keyed to device
  tilt where available).
- Ongoing: every new screen (inventory, abilities, campaign) ships IN the design system,
  never beside it.
