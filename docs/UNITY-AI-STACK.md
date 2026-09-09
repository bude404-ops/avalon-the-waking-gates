# AVALON: THE WAKING GATES — UNITY AI PRODUCTION STACK
Research date: Sept 9, 2026. Compiled per Big's directive: research first, no blind installs.
Every tool verified against: Unity 6 compat, maintenance, cost, credits, commercial license, mobile, external account, cloud dependency, API/automation, Git-friendliness, pipeline fit.

---

## THE VERDICT STACK (what we actually use)

| Production stage | PRIMARY tool | Cost | Backup/secondary |
|---|---|---|---|
| 2D concept / canon art | Base44 generation (current system) | current | Scenario (paid, style-locked) |
| Image→3D mesh (prototype) | 404-GEN runner (TripoSR route) — PROVEN | FREE | Hunyuan3D-2.1 (needs paid GPU) |
| Image/Text→3D + refine IN Unity | PicoBerry AI + UModeler X | FREE base; $12/mo Starter | Meshy Pro ($19/mo) later |
| 3D cleanup / retopo / kitbash | UModeler X (Unity-native) | FREE | Blender (free) |
| Rigging humanoids | Mixamo auto-rig | FREE | ActorCore autorig (free) |
| Animation | Mixamo library + DeepMotion video-mocap | FREE + $15/mo | Cascadeur (free tier) |
| Textures / PBR materials | Scenario (train on OUR canon art) | ~€29/mo | Meshy textures |
| Skyboxes / HDRI lighting | Blockade Labs Skybox AI + Unity cubemap generator | free tier / cheap | Unity AI built-in |
| Level blockout | UModeler X + MCP (AI-agent driven) | FREE | Unity AI agent |
| NPC dialogue (ship) | Hand-written + lore docs (no cloud) | FREE | Convai (prototype only) |
| NPC voice acting | ElevenLabs offline TTS | $5/mo Starter | xVASynth (free/offline) |
| Sound effects | Stable Audio Open (local) | FREE | ElevenLabs SFX |
| Music | AIVA / Suno (paid tier = commercial rights) | ~$10-30/mo | hand-composed |
| Coding assistance | Unity AI (in-editor agent, Open Beta) | $10/mo Personal | Cursor/Claude (current) |

Zero-cost bootable stack TODAY: UModeler X + Mixamo + 404-GEN free route + Stable Audio Open + xVASynth. Payment ladder at the bottom.

---

## 1. 404-GEN (our pipeline — INSTALLED & PROVEN)
Proved Sept 9: front-view canon plate → GLB in ~5s, 112k faces, zero cost (tools/404gen/). TripoSR route = shape-only; Hunyuan3D-2.1 (textured) behind ~$9/mo HF PRO.
- Mesh: TripoSR good silhouette, needs retopo+texture. Hunyuan 2.1 = near-game-grade textured.
- Formats: GLB native → FBX convert for Mixamo/Unity. Unity 6: fine. Automation: fully scriptable (ran itself).
- Characters: humanoid shapes read, face detail lost — canon art stays source of truth. Creatures: strong (Gate-Worm/Drake will be good tests). Weapons/props: the sweet spot.
- Commercial: permissive outputs, no account needed on current route.
VERDICT: keep as free prototyping lane; upgrade to textured service when budget opens.

## 2. GENIES — verdict: SKIP for production
Avatar SDK free on Asset Store, maintained, lightweight. BUT it generates Genies-style UGC avatars in THEIR style system — cannot import or respect our locked Mythic Celtic canon, gear laws, class kits. Built for apps whose product IS avatar customization. Not better than 404-GEN/Meshy for our characters — decisive factor: canon lock. Revisit only if we ever want UGC player avatars.

## 3. PICOBERRY AI + UMODELER X — verdict: PRIMARY UNITY-NATIVE 3D TOOL
Strongest find of the research. UModeler X (full Unity-native modeling: blockout, retopo, UV, kitbash) went 100% FREE April 22, 2026. PicoBerry = their AI gen service on top: text→3D + image→3D, TEXTURED, any device, export straight to Unity.
- Pricing (July 2026): free tier + Starter $12/mo, Pro $32/mo, Max $96/mo. Purchased credits don't expire.
- Killer feature: gen output drops into Unity as EDITABLE UModeler meshes — fix proportions, clean topology, adjust kit pieces inside the editor. No Blender round-trip. Solves 404-GEN's cleanup gap natively.
- Automation: async API (text/image→3D, multiple engines, one API) slots into our pipeline. UModeler X shipped a dedicated MCP server Aug 14, 2026 — AI agents can DRIVE the modeling tools directly.
- Unity 6: native plugin, actively developed, 30k+ devs. Commercial licensing on paid tiers (verify at signup).
VERDICT: Unity-native primary. Test plan: free tier on a weapon/prop first (Ashfall leaf-blade), then Starter $12/mo when it earns it.

## 4. SKYBOX AI (Blockade Labs) — verdict: YES for skyboxes/HDRI
Text→360° equirect panoramas + 32-bit HDRI (real lighting data), Sketch mode, official Unity integration.
- For Avalon: six realm skyboxes (Skyrend storm, Ashfall caldera, Stoneheart, Duskmourn black-crimson, Marenth tide, Everbloom) per realm colorway. No-script prompt law applies here too.
- Free tier limited; paid for commercial + HDRI + API. Unity AI's cubemap generator = zero-account fallback.
- NOT a world-geometry tool — skyboxes and lighting only.
VERDICT: realm skyboxes + HDRI lighting passes. Unity cubemap fallback.

## 5. LAYER AI — verdict: SKIP; SCENARIO is the real tool
Layer.ai's current status unclear (no strong 2026 signal) — do not buy. Category leader = SCENARIO:
- Game-asset-grade 2D (characters, props, environments, concept) AND PBR 3D meshes exportable to Unity.
- Killer feature: TRAIN A CUSTOM MODEL ON OUR OWN CANON VAULT — every generated piece inherits Big's style automatically. The style-consistency engine for 2D production art (UI, item icons, environment concepts).
- Full commercial license + IP ownership on paid plans; Unity plugin; maintained. ~€29/mo individual.
VERDICT: the 2D/PBR style-lock tool, trained on the canon vault. Start when 2D production begins (post-STAGE2).

## 6. CONVAI — verdict: HOLD (prototype later, never core)
Cloud AI-NPC: dialogue, 500+ voices / 65+ languages, lip sync, scene perception, MCP, Unity incl. Quest/WebGL. Inworld the main alternative (~$20/mo, free plan available).
- Blocker is architectural: runtime LLM dialogue = recurring per-player cloud costs forever + internet at play time. GDC 2026 consensus: these costs are real.
- Our lore model is the opposite: crafted authored dialogue (Vera speaks in half-truths — WRITTEN character, not emergent chat).
VERDICT: not part of core NPC system. Revisit only for a 'talk to a god' marketing demo.

## 7. AI VOICE / AUDIO — verdict: offline generation, zero runtime dependency
- ELEVENLABS (voice acting): best-in-class TTS, generate OFFLINE WAVs at production time — no runtime cost, no internet dependency, commercial allowed. Starter $5/mo. Use for Vera + class voices.
- XVASYNTH (free, offline, open source): stylized NPC voices, zero-cost bench option.
- STABLE AUDIO OPEN (free, local): text→SFX, no API cost; MCP server exists for automated SFX gen. Creature calls + foley prototyping (Cŵn Annwn howls, Gate-Worm groans).
- MUSIC: AIVA (Pro = commercial game soundtrack rights) or Suno (commercial rights on paid plans). Ambient realm tracks.
- ASSET STORE wrappers: most store voice/audio assets wrap the same cloud APIs — buy nothing; source services are better.

---

## ASSET STORE SWEEP (22 categories → deduped verdicts)
1-2. Image/Text→3D: PicoBerry + UModeler X (native, editable) > Meshy (better meshes, cloud, $19/mo) > our free TripoSR lane. Genies skipped (style lock).
3-5. AI char/creature/weapon gen: same 3D tools; characters/creatures from OUR canon art → image→3D (kit integrity preserved); weapons/props = strongest gen category.
6-7. AI rigging/skinning: Mixamo (free, proven — meshes already head there) + ActorCore (free autorig, paid anim library). Nothing store-bought beats free+proven.
8-9. AI animation: Mixamo library (free) + DeepMotion Animate 3D (video→mocap, $15/mo) for custom moves; Cascadeur (free tier, physics-assisted) for two-scales god cinematics later. Tripo's built-in animation = gimmick tier.
10-11. AI texture/material: Scenario (custom-trained PBR: albedo→height/rough/metal, canon palette) > Meshy textures. Unity AI material gen for quick-and-dirty.
12. AI environment: Blockade Skybox AI (skybox+HDRI) + UModeler X MCP blockout (6-hour Dust II build proves the workflow).
13-14. AI terrain/level: terrain from real assets; blockouts via UModeler X + AI agent MCP; Unity AI agent assists scene assembly. Nothing store-bought needed.
15-16. AI NPCs/dialogue: Convai/Inworld — HOLD per above. Hand-written lore dialogue ships the game.
17. AI voice: ElevenLabs offline gen.
18-19. AI SFX/music: Stable Audio Open (free local) + AIVA/Suno (paid for commercial).
20. AI coding: Unity AI (Open Beta, in-editor agentic assistant + generators, $10/mo Personal incl. 1,000 credits) — only Unity-official option; our Cursor/Claude flow already covers script gen. Optional seat, not urgent.
21. AI gameplay assistance: nothing shipped worth buying — pre-production demos. Skip.
22. AI testing/QA: Unity Automated QA exists but not AI-relevant. Skip; manual passes for now.

---

## PAYMENT LADDER (unlock only when a phase needs it)
- $0 NOW: UModeler X, Mixamo, 404-GEN free route, Stable Audio Open, xVASynth, Unity cubemap gen — the whole prototype phase runs free.
- $12/mo PicoBerry Starter — first 3D production unlock (commercial license, textured, editable in-Unity).
- $5/mo ElevenLabs Starter — Vera + class voice acting, offline generated.
- $10/mo Unity AI Personal — in-editor agent IF Cursor flow feels slow inside Unity.
- ~€29/mo Scenario — when 2D production art begins (post-STAGE2), trained on canon vault.
- $15/mo DeepMotion — when custom animation/mocap needed.
- Skybox/HDRI: Blockade paid or Unity AI credits when realm lighting passes start.

## LEGAL / PIPELINE NOTES (all cloud gen tools)
- Meshy FREE tier = CC-BY non-private (NOT clean for commercial) — commercial needs paid plan. PicoBerry/Scenario/ElevenLabs similar: commercial rights on paid tiers. NEVER ship free-tier output in a commercial build.
- All tools export GLB/FBX/PNG/WAV = Git-friendly (LFS for binaries), all fit vault → commit → verdict protocol.
- No-script / no-deity-name prompt laws apply to EVERY generation prompt in EVERY tool — the law stack carries.
- 404-GEN runner already automates the free lane; PicoBerry's async API slots into the same runner on upgrade.
