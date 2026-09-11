# Cinematic Style Research — Teaser + In-Game Cutscenes
> Researched Sept 10, 2026 for Big. Decision doc: what cinematic language fits AVALON, what's feasible under the Unity-Only Tool Law.

## PART 1 — TEASER STYLE

### The four teaser archetypes
1. **SLOW-BURN MOOD TEASER** (Elden Ring reveal, Bloodborne "Prepare to Die") — 60-90s, almost no cuts, dread built through sound + one central image. No gameplay. Ends on title + date. Highest prestige-per-dollar; lives entirely on art direction + sound.
2. **CINEMATIC CG TRAILER** (Blizzard, CDPR style) — outsourced studio CG, $500k+. **OFF THE TABLE** for us: external studios violate the Unity-Only Tool Law and the budget doesn't exist.
3. **IN-ENGINE CAPTURE TEASER** (Hellblade II, GoW Ragnarok promo) — real-time renders captured from the actual build. Zero visual continuity gap with the product. Requires the game engine to already look finished — becomes our option as the Forge finishes models/lighting.
4. **STILLS + VO MONTAGE** (Zelda TotK drip-marketing, indie standard) — Ken Burns moves over key art, VO anchor, sound design carries it. Cheapest, fastest, and **already proven in-house** (our 9-shot teaser + the 6 new storyboard frames).

### Pacing craft that matters (2025-26 standard)
- **Contrast dynamics**: the teaser's job is ONE loud moment. Everything before it must be quiet. Quiet → loud → silence → title.
- **Cut acceleration**: shots start at 3-4s, compress toward 1s at the peak, then a full-still title card. Never cut fast early.
- **Sound**: braams are dead-cliché. Current standard = low sub-drone bed + diegetic-first foley (stone, wind, flame, breath) + one musical swell. Silence is the sharpest tool in the kit.
- **VO discipline**: 3 lines max in a teaser (our "The Lantern Kindles" treatment already obeys this).
- **Mobile-first**: most viewers watch on a phone in 9:16 crop. Compose center-weighted; test the crop.

### AVALON FIT
The locked treatment ("The Lantern Kindles") IS archetype 1 built with archetype 4 tooling. **Recommendation: keep it.** Phase the teaser program:
- **Now**: stills + VO + sound design (6 storyboard frames + 7 approved cinematic stills = 13 shots of ammo, all canon-locked).
- **After the 12-model Forge batch**: re-cut as in-engine capture (archetype 3) for launch-window trailer. Same structure, upgraded surface. No creative rework needed.

## PART 2 — IN-GAME CUTSCENES

### Pre-rendered vs real-time (industry verdict)
The industry has converged on **real-time in-engine cutscenes** (God of War 2018/Ragnarok, Ghost of Tsushima, Hellblade II): pre-rendered now looks *worse* than gameplay in remasters, breaks visual continuity, can't adapt to player state (gear, time of day), and bloats build size. Real-time = same assets, same renderer, instant iteration.

### Camera grammar options
1. **ONE-SHOT NO-CUT** (GoW 2018) — the camera never cuts for the whole game. Max immersion, max authoring cost, heavy mocap requirements. Not for us as a default.
2. **CLASSICAL FILM GRAMMAR** (shot/reverse-shot, establishing → coverage) — authored cutscenes in film language. Industry default; cheapest to direct well.
3. **IN-WORLD STAGING** (FromSoft, Tsushima's ambient scenes) — camera stays in gameplay mode; NPCs speak in world; no mode switch at all. Cheapest, zero continuity gap, fits our player-respect law.
4. **LETTERBOX 2.39:1** (Hellblade, Asura's Wrath) — cinematic bars drop when a cutscene starts, real-time underneath. Instant "this is a movie moment" signal without going pre-rendered.

### Player-respect requirements (our law stack already rules)
- **Every cutscene skippable** (tap to skip, confirm on hold) + pausable. Anti-grind law: no unskippable exposition.
- **Strategic-only cinematics** (already doctrine): cinematics fire at god-moments, realm crossings, gate openings — never for fetch quests.
- **Environmental storytelling over exposition**: the world shows the lore (dead lanterns, Hollow sites); cutscenes never narrate what the world can show.
- **Subtitle-first**: default on, no voice dub requirement for lore text (VO only at the highest beats).

### AVALON FIT — THE CUTSCENE BIBLE (recommended law)
- **Real-time in-engine, always.** Letterbox bars drop in, 2.39:1 crop, gameplay renderer underneath.
- **Two grammar tiers**: CLASSICAL film grammar as the workhorse; ONE-SHOT reserved for god moments — the Two-Scales law is the money shot: when the player *becomes* the god, the camera stays unbroken, no cut, primordial scale.
- **Diegetic-first sound**: no score until a god or a gate appears. Lantern hum, wind, stone = the ambience.
- **Light law holds**: cold desaturated base, the lantern the only warm source — cutscenes are no exception. Crimsons only in Duskmourn beats.
- **In-world staging for NPC dialog**: quest talk happens in gameplay camera (tier 3), cutscene mode reserved for the mythic beats. This keeps cinematics rare and therefore huge.

### Unity implementation path (all inside the Tool Law)
- **Timeline** (built-in): the cutscene authoring spine — activation tracks, signals, skip logic via `PlayableDirector` + input.
- **Cinemachine** (built-in): authored camera moves, blends, one-shot virtual cameras with hard constraints.
- **CMU mocap + Asset Store motion packs**: same pipeline as the Forge — no new tooling needed.
- **Post-processing stack** (built-in): the cold grade + vignette; letterbox via camera aspect override + UI bars.
- **Cost note**: this adds ZERO external tools. The cutscene capability rides on what the Forge already builds.

## RECOMMENDATION SUMMARY
1. Teaser: slow-burn mood + stills/VO now, re-cut in-engine at launch. Treatment unchanged.
2. Cutscenes: real-time, letterboxed, classical grammar + one-shot god moments, skippable always, in-world NPC talk, strategic-only.
3. Everything builds inside Timeline/Cinemachine on the existing Forge output — no new tool law exceptions.
