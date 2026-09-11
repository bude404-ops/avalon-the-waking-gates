# THE CINEMATIC PRODUCTION SYSTEM v1.0
### AVALON: THE WAKING GATES — Cinematic Storytelling & Visual Production Doctrine

> **Owner:** BudE404 (Game Director) · **Locked:** Sept 6, 2026
> **Source of truth:** this project alone — docs/, data/, art/approved/, and the deployed build. Nothing else is canon.
> **Prime law:** the project's existing lore, terminology, and artwork are used whenever possible. New artwork is generated ONLY when useful — and must look like it came from the same art department for the same game.

---

## 1. GOVERNING PRINCIPLES

1. **PROJECT = SOURCE OF TRUTH.** No invented names, factions, gods, locations, cosmology, or history. If the project does not define it, the cinematic leaves it OPEN — mystery is the house style anyway (mystery-by-omission doctrine).
2. **PRESERVE THE ARTWORK'S IDENTITY.** Existing canon art is the foundation, brought to life — never replaced by generic animation. The artwork IS the keyframe; the cinematic is the artwork breathing.
3. **THE WORLD IS BIGGER THAN THE PLAYER.** The goal: the player's world feels alive, ancient, and larger than what they currently understand. Every cinematic must leave one unanswered question.
4. **QUIET BETWEEN SPECTACLES.** The rhythm law: spectacle earns its weight only against silence. Between every major spectacle beat there MUST be a quiet beat — atmosphere, discovery, tension, or character moment. (Director's order, Sept 6 2026.)

---

## 2. THE VISUAL STYLE GUIDE (extracted from approved canon)

The fingerprint every frame must carry, drawn from ART_DIRECTION v2.0, ART_STYLE_BIBLE, and the AVALON_DOCTRINE:

| Property | Locked value |
|---|---|
| **Render look** | LOTR-cinematic filmic realism — photorealistic fantasy-epic film still, Weta Workshop practical-grit, IMAX widescreen, anamorphic framing, film grain |
| **NOT** | cartoon, painterly, anime, stylized-low-poly, PBR-photoreal |
| **Lighting** | grounded cinematic; dark moody depths, element-light as the divine source; atmospheric depth haze |
| **Palette law** | unified faction palette: two base colors + one accent family; every glow in frame reads as the SAME element |
| **Flame-only colorway** | the Gate Rune arch is rugged weathered stone, ZERO glow — only the rising flame carries color |
| **Giants/colossi** | primordial raw element (Emergence Law: waist-down terrain-merged, no legs); shrine-crown, world-body, element-hair; zero armor, zero human skin; Gender-Role Binding weapons (M greatsword / F staff) |
| **Pilgrims/sprites** | serious-fey war-priests; Cinder Roads armor — road-worn repair plate, riveted mismatched layers, ash-patina; faces hidden (hood/helm) |
| **Hollow** | the Wasteland look — erased/unmade anatomy, dread by subtraction; T2 anchors on canon champions' armor, T3 echoes canon giant silhouettes |
| **Composition** | half-zoom mid-shot for giants (chest-height, fills central 2/3, min 3 scale anchors); siege framing for god-vs-waves; side profile for god-vs-god |
| **Environments** | the six Gate-named heartlands (Ashfall, Skyrend, Everbloom, Duskmourn, Marenth, Stoneheart) — mortal architecture echoes its giant (crucible hearth towers, storm vanes, petal-crest rooflines, reliquary-niche shrines, fin-buttressed harbors, crystal-crowned quake-villages) |
| **Hard bans** | ZERO text in generated frames; zero emblems/sigils on giants; deity names never in generation prompts; the word 'elven' never in prompts |

### 2.1 Adaptive Generation Protocol
When canon art cannot cover a shot:
1. **Analyze before generating** — pull 3+ approved references of the same class (character/world/Hollow/prop) and extract: shape language, proportions, armor grammar, materials, color relationships, lighting, detail level, mood.
2. **Generate IN the established language** — original composition, existing identity. Never blind-copy an approved image; never import outside styles.
3. **Continuity gate before approval** — silhouette test (recognizable at a glance), palette test (glow sources all one element), material test (Cinder Roads grit, Weta realism), and p-hash comparison against the class's canon set to avoid duplicate compositions.
4. **Fails the gate → correct or discard.** A corrected asset ships only after it passes.

### 2.2 Continuity Priority Ladder
Characters → environments → creatures → architecture → weapons → effects → cinematics → gameplay. The player must never feel they entered a different game between a cinematic and the gameplay it flows into.

---

## 3. STORYTELLING DOCTRINE

- **No exposition dumps.** Reveal the world progressively: visual storytelling, character reactions, environmental clues, scale, symbolism, discovery, dramatic reveals.
- **Minimal but impactful narration** — a few lines per cinematic, VO canon voice (Christopher Neural, −5Hz pitch). Otherwise: silence, sound, and image.
- **The beat grammar** (per Director, Sept 6 2026): 
  `ESTABLISH (atmosphere) → SPECTACLE → QUIET (character/discovery) → TENSION → SPECTACLE → BREATH → CLOSE (open question)`
- Quiet beat types, in rotation: a still landscape that almost changes; a character noticing something off; a scale reveal that lands slowly; a sound from somewhere it should not be.
- **The closing law:** every cinematic ends on a question, a threshold, or a door — never on a full explanation.

---

## 4. THE THREE CINEMATIC TYPES

### 4.1 WORLD / LORE CINEMATICS — the myth in motion
Major worldbuilding: the Waking Gates, the pilgrimage, the Hollow's origin, the Gate Rite. 60–120s. Built primarily from STAGE2 world cards + EMERGENCE canon + Hollow canon.
- **Proven example (live):** the deployed 9-shot teaser (`cinematic.html`) — Ken Burns drift, synth score, letterbox, VO. This engine style is the house base.
- **In-system candidates:** "The Sleeping Gods" (land IS the gods — STAGE2 landscapes that are anatomy), "The Rite" (a pilgrim lights a Gate; the god rises — EMERGENCE canon), "The Dream That Died" (Hollow origin — T1→T2→T3 progression canon).

### 4.2 STORY CINEMATICS — the road between chapters
20–45s slices woven into quests/campaign beats. Pilgrim-scale: the Warden, the Keeper, the first steps of a road. Character moments carry them — a hood adjusted, a reliquary raised, a mark that catches light.
- Built from: PILGRIM-*-CINDERROADS sets, PROP canon (Reliquary, Oathblade), PET-PIXIE, STAGE2 mortal-heartland cards.

### 4.3 ENCOUNTER / BOSS CINEMATICS — the declaration
5–15s pre-fight reveals establishing importance, personality, scale. The god/giant is introduced by EMERGENCE (risen from terrain, never walking); Hollow by subtraction and wrongness.
- Source: the 12 colossus EMERGENCE canon images + Hollow tier canon.
- **Mandatory transition:** cinematic ends on the exact framing the gameplay camera opens on (god-vs-god side profile / god-vs-waves siege front, per the Battle Camera Doctrine) so the cut IS the door into control.

---

## 5. ARTWORK-TO-CINEMATIC PIPELINE

**Disposition every asset before touching it:** USE DIRECT / MODIFY / LAYER (depth planes) / EXPAND (extend environment, generate missing edges) / PARK (needs new art via §2.1). Never regenerate what exists.

```
CANON ART → disposition → depth layers (sky/mid/fore) → camera choreography
   → lighting & atmosphere (fog, embers, dust, weather — all element-true)
   → particles & elemental FX (faction palette only)
   → sound design (score + ambience + SFX)
   → VO/text card (only if it earns its line)
   → continuity gate (§2.2 ladder)
   → deploy → hand-off frame matched to gameplay camera
```

**Motion grammar:** slow pushes/pulls, lateral pans, parallax between depth layers, subtle scale drift (the Ken Burns house move), breathing light. Character movement is SUBTLE — cloth sway, ember flicker, eye-light. The artwork's identity leads; animation only serves it.

**Sound grammar:** low pad + room tone always under quiet beats; score enters at spectacle and drops to near-silence after; stingers reserved for reveals. One music transition per cinematic unless the story demands two.

---

## 6. GAMEPLAY CONNECTION

- Cinematic ends → gameplay opens with matching subject, framing, palette, and camera doctrine (echo-duel.html / siege.html / depths-embermere.html are the reference implementations).
- Any character, creature, weapon, or environment introduced in a cinematic must be VISUALLY IDENTICAL when met in play — same silhouette, same palette, same materials (continuity ladder §2.2).
- Menu shell (`index.html`) carries the same mark: rugged Gate Rune, ember field, Cinder Roads chrome — the front door of the game and the front door of every cinematic are the same door.

---

## 7. CANON ASSET INVENTORY (the cinema's raw material — 78 approved)

| Class | Count | Files (pattern) |
|---|---|---|
| Colossus emergence (kings/queens) | 12 | *-EMERGENCE-CANON.png |
| Stage 2 world cards | 24 | STAGE2-*-WORLD-CANON.jpg |
| Pilgrim sets (M/F) | 14 | PILGRIM-*-CINDERROADS-*-CANON.png (+ Mark engraved) |
| Sprite champions (M/F × 6 elements) | 12 | *-SPRITE-MALE/FEMALE-CANON.jpg |
| Hollow bestiary (T1–T3 + champions) | 7 | HOLLOW-*-CANON.jpg |
| Props | 6 | PROP-*-CANON.png |
| Pets / mark / sigil | 3 | PET-PIXIE-*, SIGIL-* |

Every future cinematic cites its source files per shot in the shot list — traceability is part of the continuity gate.

---

## 8. SYSTEM PROOFS (deployed, both wired into the menu GALLERY)

- **`cinematic-system-demo.html` — SYSTEM PROOF 01 (Type A):** a four-beat Ashfall sequence proving the full doctrine: establish (world card) → spectacle (Vaelthorn's emergence) → QUIET (the Kiln Road, character beat) → tension (T3 Echo) → threshold close. Canon assets only; one narration card per beat; ember-true particles; WebAudio score with the spectacle-swell / quiet-drop grammar; beat grammar per §3.
- **`cinematic-encounter.html` — SYSTEM PROOF 02 (Type C):** Morvaine boss-encounter cinematic — canon emergence art, zero regeneration, ends on the match-cut gameplay transition ("the cinematic ends where the gameplay begins — same god, same gate") per §4.3 and §6.
