# THE AVALON CUTSCENE BIBLE
> Codified Sept 10, 2026 — Big's order ("do that for the cutscene bible"). Companion law to docs/AV-UI-BIBLE.md and docs/CINEMATIC-STYLE-RESEARCH.md. Governs every scripted cinematic moment in AVALON — The Waking Gates.

## LAW 1 — REAL-TIME, ALWAYS
All cutscenes render **in-engine, real-time**. Never pre-rendered, never outsourced CG. The gameplay renderer, the canon assets, the live lighting stack — one visual language from first frame to last. Cutscenes adapt to player state (gear, realm, time of day) because they ARE the game.

## LAW 2 — THE LETTERBOX SIGNAL
Cinematic mode = letterbox bars drop in, camera crops to **2.39:1 widescreen**, gameplay HUD (the lantern) fades quiet. Bars up = gameplay; bars down = myth. The mode switch is the storytelling signal.

## LAW 3 — TWO CAMERA TIERS
- **CLASSICAL GRAMMAR** (the workhorse): authored shot/reverse-shot, establishing → coverage, film language. Used for realm crossings, gate openings, lore beats.
- **THE ONE-SHOT** (the god moment): camera NEVER cuts. Reserved exclusively for primordial beats — and the Two-Scales law is its throne: **when the player becomes the god, the camera stays unbroken.** One take, primordial scale, no edit can touch it.

## LAW 4 — IN-WORLD STAGING FOR NPC TALK
Quest dialog, shop talk, rumors — all stay in the **gameplay camera** (in-world staging, FromSoft/Tsushima style). NPCs speak in the world; the player stays the player. Cutscene mode is reserved for the mythic beats, so when the bars drop, it MEANS something.

## LAW 5 — SKIPPABLE, PAUSABLE, ALWAYS
Every cutscene: tap-to-skip (hold to confirm), pausable, resumable. **No unskippable exposition ever** — anti-grind law holds inside cinematics. Lore the world can show does not get narrated.

## LAW 6 — STRATEGIC-ONLY CINEMATICS
Cutscenes fire only at the mythic beats: god encounters, gate/seal events, realm first-crossings, campaign turning points. Never for fetch quests, never as tutorials. Rarity = weight.

## LAW 7 — SOUND: DIEGETIC-FIRST, NO PREMATURE SCORE
Lantern hum, wind, stone, breath — the ambience carries cutscenes. **No orchestral score until a god or a gate appears.** When the music finally hits, it is earned. Silence stays the sharpest tool.

## LAW 8 — THE LIGHT LAW HOLDS
Cold desaturated base; the **lantern the only warm source** (Lantern-Region Aura hue per realm). Duskmourn beats alone may burn crimson. Cutscenes get no lighting exceptions — the two-layer law is the cinema.

## LAW 9 — SUBTITLE-FIRST TEXT
Lore text and dialog read clean without VO. Voice acting lives only at the highest beats (gods, campaign openers); subtitles default on everywhere.

## IMPLEMENTATION SPINE (Unity-Only Tool Law — zero exceptions)
- **Timeline** — the authoring spine: activation tracks, signals, skip via `PlayableDirector`.
- **Cinemachine** — authored cameras, blends, one-shot virtual cams with hard constraints.
- **CMU mocap + Asset Store motion packs** — same pipeline as the Forge (root-locked).
- **Built-in post stack** — cold grade, vignette, aspect override + UI bars for the letterbox.
- **Zero new tools.** The cutscene capability rides free on Forge output.

## QC GATE
Before any cutscene ships: bars drop clean, skip works mid-line, no score before its earned moment, lantern the only warm light, and the one-shot used ONLY where a god stands. If a beat can be told in-world instead — it must be.
