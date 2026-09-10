# AVALON MAPS — Parallel Production Track (Sept 10, 2026)

**Big's order:** start generating playable maps while the character pipeline runs.
**Verdict:** YES — parallel track, starting with Map #1. Characters and maps share the
same doctrine stack, the same generation engine (404-GEN), and the same verdict loop.

---

## 1. MAP #1 — THE COLD LANTERN (ASHFALL)

Already fully designed: `docs/COLD_LANTERN_MAP_ARCHITECTURE.md` (the vertical-slice
design test — beat table, space taxonomy, two-scale payoff moments all locked).

**First playable space = beats 1–4:**
1. EMBERHOLLOW — town square, the brazier landmark, Keeper NPC spot (SAFE)
2. THE ROAD — Charcoal Forest stretch, fallen-log bridge, first combat space (THREAT)
3. RUINED SHRINE — brazier rite, lore tablet, the Gate PULSE reveal (DISCOVERY)
4. WATCHTOWER APPROACH — drake strafing set-piece lane (THREAT)

Set in **ASHFALL** (oxide-red lantern hue, Ember King's realm) — which pairs the first
playable map with the **RAVAGER**, the first class through the character pipeline.
**First playable demo = the Ravager walking the Cold Lantern map.** The two tracks
converge on one vertical slice.

## 2. PRODUCTION ROUTES (Unity-Only Law compliant, verified)

| Layer | Tool | Status |
|---|---|---|
| Terrain | **Gaia for Unity 6** (Asset Store, free version available, v2.5 June 2026, standard EULA) | verified listing |
| Landmark props (Cinder Gate, shrine brazier, Watchtower, standing stones, megalith arch) | **404-GEN** from clean-plate references — same prop pipeline as the weapons | plates roll next, Big's verdicts gate |
| Biome fill (trees, rocks, grass, road) | Asset Store biome kits — shortlist against: Mythic World Law reads, mobile poly budget, NO baked-in runes/glyphs (script laws), tintable to cold-desaturated + one hue | shortlist next |
| Bespoke megalithic structures | UModeler X (free, commercial OK) | on-seat assembly |
| Assembly | Seat (first map by hand while AUTO-FORGE proves; subsequent realms via batch) | spec-driven |

**Knotwork note:** all carved surfaces follow the rune law — knotwork/spiral PATTERNS
only, gate-rune as the sole mark where canon allows. Asset Store kits with baked
fantasy runes get their runes stripped or are rejected.

## 3. THE APK PLAYABLE PREVIEW (review surface, not the game)

Same doctrine as the model viewer: he reviews in the APK, not GIFs. The live shell
gets a **walkable pilgrim-scale scene** of the Emberhollow stretch:
- WASD + touch controls, third-person camera behind the character GLB
- Procedural three.js terrain + megalithic props staged per the beat spec
- Lantern-Region Aura lighting (cold desaturated ambient, oxide-red lantern glow as
  the only rich source — Lantern-Region Aura Law)
- Loads the current Aedan/Ravager model so the character pipeline output has a world
  to walk in
- This is the FEEL check while the Unity build catches up — not the shipped game

## 4. WHAT I BUILD, IN ORDER
1. **Landmark clean-plates** (2D) for 404-GEN: Cinder Gate, shrine brazier,
   Watchtower, standing-stone set, megalith arch — verdicts in the DM as usual
2. **Map 1 build spec**: exact spaces, dimensions, prop manifest, POI placement,
   combat-space sizing (≤12m rule), silhouette-test checklist per the
   Double-Duty Geometry Rule
3. **APK walkable preview** of beats 1–2 (square + first road stretch)
4. **Biome kit shortlist** with per-kit compliance notes
5. **Realm starter layouts** for the other five realms (world-map layer) — only after
   Map 1 wins its verdict

## 5. THE SIX REALM MAPS (sequence after Map 1)
Ashfall (Cold Lantern) → Skyrend (Sovereign start) → Stoneheart → Duskmourn →
Marenth → Everbloom. Each realm's map follows the Gate Rite loop from the doctrine:
arrival in a wounded land → pilgrimage → relight the Gates → the Rite wakes the god
→ the realm battle. Same map, two scales, every realm.

*Divisions of labor: me = plates, specs, preview, shortlists, QC; seat = terrain,
kit assembly, first map build; Big = verdicts, as always.*
