# AVALON — MOBILE CONTROL & COMBAT FLOW BIBLE v1
Codified Sept 13, 2026 (Big: "we need to research and figure the best ways for a mobile game for it to flow nicely")
Reference class: Genshin Impact / Zenless Zone Zero mobile (the proven mobile-first standard) + Vainglory/Unity floating-joystick UX research + Unity built-ins (Input System, Cinemachine — Tool Law compliant, zero external assets).

## THE LAW: ONE THUMB MOVES, ONE THUMB FIGHTS, THE SCREEN STAYS CLEAN

### 1. MOVEMENT — floating virtual joystick (replaces tap-to-walk as primary)
- **Floating joystick, left half of the screen**: the joystick spawns WHERE the thumb lands (no fixed thumb-cramp anchor — the #1 mobile UX finding: fixed joysticks force eyes off the action; floating keeps thumb wherever it naturally rests).
- Hold = walk, drag magnitude = speed (walk → run at full extension; run adds a second CMU clip later, walk for now).
- **Quick tap (no drag) still walks-to-point** — tap-to-walk stays as a free exploration convenience on the same gesture logic that already works (tap vs hold disambiguation at ~22px drag threshold, already proven in v213).
- Dead zone ~15% of radius so micro-jitters never move the Sovereign.

### 2. CAMERA — right-half drag + auto-follow
- Drag anywhere on the RIGHT half (above the button cluster) = orbit yaw/pitch. Pinch = zoom (both halves).
- **Auto-follow**: while moving, the camera eases back behind the Sovereign (Cinemachine 3rd-person follow, damping ~1.2s) — no manual orbit needed just to travel. Manual drag overrides for ~3s, then auto-follow resumes.
- **Lock-on reframes**: when locked on a foe, the camera keeps both in frame (shoulder offset toward the target).

### 3. COMBAT — fixed button cluster, bottom-right
- **ATTACK** — biggest button, bottom-right corner (primary thumb's home). Rapid taps chain; the chain buffer holds inputs ~0.2s so mobile latency never eats a combo.
- **Magnet attacks**: on press, if a foe is within lunge range, the Sovereign steps INTO the strike — mobile players don't circle-strafe, they tap and expect it to land.
- **DODGE** — above/left of ATTACK. Dodge-roll with i-frames (the CMU roll clip when it lands; sidestep blend for now), ~0.6s cooldown, no stamina bar — the canon has no numbers in combat; cooldown shows as the button's bronze fill, felt not read.
- **SOFT LOCK, no button**: the nearest foe inside a forward cone gets soft-locked on approach; the Sovereign auto-faces them at attack range. **Tap a foe = hard lock** (ZZZ standard). Hard lock persists until the foe falls or you tap elsewhere.
- **BELIEVE (reliquary skill)** — the BELIEF meter's skill button, above the cluster; charges by deeds (oath choices, waymark kindling — the quest layer feeds it), fires the class-lineage ability when alit. One charge, earned not bought — the canon's anti-grind soul.

### 4. FEEL RULES (what makes mobile combat "flow nicely" — research-backed)
- **Input buffering** (~0.2s): taps during an anim queue into the next chain — the difference between "responsive" and "laggy" on touch.
- **Hit-stop** (~60ms) on strikes that connect — weight without numbers.
- **Telegraphs, not stats**: foes flash/flare before heavy strikes (corpse-light bloom for Hollow tide) — readable at phone size and small-screen legible.
- **Thumb-first geometry**: every button ≥ 14mm effective target, corner-anchored so grip-shift never whiffs; buttons bronze-lip outlined (canon), alpha-transparent over the world.
- **Portrait-first, landscape reflowed** (orientation law): the cluster anchors bottom-right in BOTH orientations; the camera/scene absorbs the aspect change.

### 5. WHAT DIES
- Tap-to-walk AS THE ONLY movement (kept as secondary convenience).
- Camera-drag sharing the whole screen with movement taps (the current tap/drag ambiguity is the flow problem Big is feeling).
- Any combat numbers anywhere (HUD law: no numbers in combat).

## ROLL-OUT (staged, APK-reviewable per stage — Big tests each in the shell)
- **Stage A**: floating joystick + tap-to-walk coexistence + camera split (left=move, right=look). No combat change yet.
- **Stage B**: combat cluster (ATTACK/DODGE) + soft/hard lock + magnet strikes vs. the practice target (no enemies needed — a Cinder Gate training brazier takes hits).
- **Stage C**: BELIEVE button wired to the BELIEF meter + hit-stop + telegraph pass — full feel layer before the Mutefolk deploy so the enemies land in a system that already flows.

All Unity built-ins (EventSystem drag handlers, Cinemachine) — zero Tool Law exceptions. CMU clips for any new anims (roll, run) per pipeline law.
