# AVALON: THE WAKING GATES

A dark-fantasy action RPG set in the Ancient Mythic Age — a primordial world shaped by six patron gods, where megalithic monuments dwarf everything mortals build. You wake in one of six realms, bound to one of six classes and one patron deity. No loot treadmills — progression is mastery, story, exploration, and divine connection, and it's permanent: your victories reshape the map. The endgame is the Two Scales: you start as a mortal hero and finish commanding primordial gods in world-shaping battles against the Unmaking.

**Owner:** BudE404 (creative director — all canon locks through his verdict gate)
**Ops:** BIGagent404 · Bigfoot404 LLC
**Studio brand:** BIG Entertainment

---

## WHAT WE'RE USING (the toolchain)

| Stage | Tool | Role |
|---|---|---|
| Canon art | AI image generation (BIGagent404) | All 2D canon keys — classes, gods, NPCs, enemies, worlds |
| 3D models | **Unity Asset Store (Unity-first)** | Store assets matched to locked canon art → retint/restyle in-engine |
| Rig + animation | Asset Store packs + in-engine | Rigged bases + animation sets from store assets |
| Engine | **Unity** | World, lighting, VFX, abilities, cinematics, build target |
| Version control | GitHub (this repo) | Everything committed + pushed on every milestone |

**Pipeline, end to end:**
`Locked 2D canon art (art direction) → Unity Asset Store (models/rigs/anims) → Unity (retint/light/animate/VFX) → play`

**Engine doctrine:** Unity is THE engine (decided Sept 2026). Legacy Godot scaffolding is retired under `legacy-godot/` and stays for history only.

**In-engine lighting law:** two-layer light — cold desaturated ambient, the lantern-flame (or one realm accent) the only rich source. Region colorways (Skyrend storm slate, Ashfall oxide red, Stoneheart bone grey, Duskmourn crimson-and-black, Marenth tide teal, Everbloom deep moss) are Unity lighting/post-processing settings, not art problems.

---

## THE VERDICT PROTOCOL (how canon locks)

1. BIGagent404 rolls a piece under the full law stack → p-hash uniqueness check → vaulted to `art/pending/` → sent to BudE404's court.
2. **NOTHING locks without an explicit verdict word** — "Keep", "Approved", "Lock it". Preference comments ("I like this one") are notes, not locks.
3. One piece in court at a time — no next roll until the current one gets its verdict.
4. On Keep: file promoted to `art/approved/` with `-CANON` suffix, committed, pushed, logged in `docs/CANON-LOCKS.md` and the vault notes.

Full pipeline + prompt-law documentation: **`docs/PIPELINES.md`** (single source of truth).

---

## CURRENT CANON STATE (Sept 9, 2026)

**Character roster — CANON-COMPLETE:**
- **12 hero classes** — 6 classes (Sovereign, Ravager, Warden, Veilborn, Weaver, Wildborn) × M/F, Mythic Celtic armor doctrine, realm-native staging, class hair signatures
- **6 patron gods** — 3M/3F (Storm, Ember, Bloom kings; Tide, Dusk, Stone mothers), primordial law, torso-up terrain forms, matched finger counts
- **NPCs** — Vera (lorekeeper), Ravenkin (Luminary-bearer, corvid talon-hands/feet)

**Enemy pass (in progress):**
- **T1 fodder tier — COMPLETE:** Mutefolk, Shade-Beasts, Grimlights (neutral stone-and-mist staging, fog canon, zero script, unlit lanterns)
- **T2 Unmade Champion** — hooded V2 in court
- Queue: Erased Drake → Gate-Worm → Echo of the Forgotten → Camelot fauna

**World pass queued:** STAGE2 24-realm mythic-world pass (world renders at myth tier — never simple villages).

**Branding:** X-account game logo V1 in court. Unity-first 3D ruling (404-GEN retired).

---

## REPO MAP

- `art/approved/` — **CANON.** Locked pieces only (`*-CANON.*`)
- `art/pending/` — live court pieces awaiting verdict (kept to a minimum)
- `art/superseded/` — rolled-but-rejected variants, kept for reference/history
- `art/cinematic/`, `art/concepts/`, `art/production/` — supporting art by purpose
- `docs/` — the law stack + design bibles. Start with `AVALON_DOCTRINE.md` (master law), `ART_DIRECTION.md`, `PIPELINES.md`, `PROJECT_STATUS.md`
- `docs/QUESTLINES/` — six realm questlines, five-beat grammar
- `unity-staging/` — Unity-side staging assets + prototypes
- `legacy-godot/` — retired Godot scaffolding (history only, do not build)
- Root `*.html` — playable web prototypes (GitHub Pages: budE404-ops.github.io/avalon-the-waking-gates/)

**House rule:** every task or milestone completes with a commit + push to `main`. The repo is the record.
