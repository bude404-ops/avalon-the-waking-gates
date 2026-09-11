# AI TOOLCHAIN REGISTRY — Avalon: The Waking Gates
*Canonical tool policy + registry. Owner doctrine: the SMALLEST, most powerful AI toolchain — ONE excellent tool per pipeline slot. No overlapping installs.*

---

## INSTALLATION POLICY (locks every install)

Before installing anything:
1. Research current version
2. Confirm Unity compatibility
3. Confirm licensing (incl. commercial use)
4. Check duplication against installed tools
5. Confirm meaningful advantage
6. Record the reason in this registry

Never install: experimental, abandoned, incompatible, or duplicative tools.
Replacing a tool with a better one BEATS adding a second overlap.

---

## THE PIPELINE (owner-specified doctrine)

**Characters:** 2D concept → AI 3D gen → game-ready topology → rig → skin → animations → weapons/armor → Unity character controller
**Environments:** 2D concept → AI env gen → 3D terrain → modular assets → Unity scene → lighting → NavMesh
**NPCs:** NPC character → AI personality → lore/context → dialogue → voice → actions → memory

---

## BIG'S NARROWING RULING (Sept 9, ~1:37 AM ET): UNITY + ITS ASSETS ONLY

'No only use if its part of unity and its assets and your actually able to use it and see if your unity game engine key allows it'

THE LAW: a tool qualifies ONLY if (1) it is part of Unity or its Asset Store, (2) it can actually be used by us, (3) the Unity license permits it. Cloud services with external accounts/credits are OUT.

**WHAT SURVIVES THE CUT (the whole stack):**
- **Unity built-ins** (every license tier incl. free Personal): Terrain, Shader Graph, VFX Graph, Cinemachine/Timeline, NavMesh, Animation + Animator, HDRP lighting/skybox, URP — the core pipeline needs no plugins at all.
- **UModeler X** — Asset Store package, 100% FREE (since Apr 22 2026), FULL commercial rights, LOCAL in-editor modeling/retopo/UV/rig/skin — no cloud, no account. THE in-editor 3D toolkit. LICENSE VERDICT: allowed on any Unity tier under the Asset Store EULA.
- **Asset Store ASSETS** (models/rigs/animation/audio packs) — the source of our 3D per the Unity-first law. Standard Asset Store EULA = royalty-free commercial use inside builds on every tier. No special key required.
- **glTFast (com.unity.cloud.gltfast)** — Unity's own official package, free, in scope if external GLBs are ever needed.

**REMOVED BY THE RULING (do not install):**
- PicoBerry AI — cloud credits + external account (the UModeler X *toolkit* stays; the cloud gen goes). Revisit only if Big ever grants one exception.
- Mixamo — Adobe external, not Unity.
- Skybox AI (Blockade) — Asset Store plugin exists but requires Blockade cloud subscription → fails the rule.
- Convai, ElevenLabs, Scenario, Meshy, Layer AI, Genies — all external clouds/accounts.
- 404-GEN mesh generation — already retired by the Unity-first ruling.

**CAN-I-USE-IT HONESTY:** the Unity Editor itself cannot run in my sandbox — package installs + editor work happen on Big's Unity seat; I do the asset shortlists, the canon-matching, the C# scripts, and all repo/pipeline work. If hands-on editor automation is ever needed, a GPU pod with Unity headless is possible but needs his license activation — flagged, not set up.

**LICENSE ANSWER:** Unity Personal (free) covers everything we need pre-revenue — Asset Store free packages and assets are commercial-legal on ALL tiers under the standard Asset Store EULA. No engine key gates any of this.

**THE PRACTIONAL PIPELINE NOW:** canon 2D art (art direction + QC bar) → Asset Store models/rigs/anim packs (shortlisted by me against canon keys) → UModeler X in-editor customization (retint, restyle, kit edits) → Unity built-ins (lighting per Reliquary-Region Aura Law, terrain, VFX, animation) → game.

---

## SUPERSEDED — earlier verdict (kept for the record)
## THE VERDICT — 404-GEN vs PicoBerry vs Meshy

**WINNER: PicoBerry AI (inside UModeler X) — $12/mo commercial tier.**

Why: PicoBerry is the only one that fuses BOTH pipeline steps in one place — AI generation AND in-editor cleanup/refine, natively in Unity. It runs the same backend models (Tripo AND Meshy-class) at the lowest commercial price found ($12/mo vs Meshy $20/mo), free tier of 1,000 credits/mo to evaluate (personal-use license only), and its output drops straight into UModeler X's modeling/UV/rigging toolkit with no export/import round-trip.

- **404-GEN stays** — but repositioned: it is not a competing generator, it's OUR pipeline shell (canon 2D → front plate → free TripoSR route → GLB → Unity). $0, proven end-to-end (first Sovereign mesh: Sept 9). Use it for rapid prototyping and QC gates; PicoBerry/Meshy-class tools produce the production meshes.
- **Meshy = the fallback**, not a second install. If PicoBerry's character output disappoints against our canon plates, we swap PicoBerry → Meshy Pro ($20/mo, Meshy 7 with 8K textures + free remeshing, strongest raw mesh quality per 2026 comparisons). One generator slot, one winner, one fallback. Never both.

---

## REGISTRY

*(Priority levels: CRITICAL / HIGH / MEDIUM / OPTIONAL / DO NOT INSTALL. Versions/licensing as researched Sept 9, 2026 — verify at install time per policy.)*

### 1. PicoBerry AI — 3D GENERATION SLOT — CRITICAL
| Field | Value |
|---|---|
| Tool | PicoBerry AI (module of UModeler X) |
| Purpose | Text/image → game-ready 3D models, generated AND refined inside Unity |
| Version | Current 2026 release line (verify build at install) |
| Unity compatibility | Native UModeler X module, Unity 6 |
| Free/Paid | Free tier: 1,000 credits/mo; paid from $12/mo (Starter) |
| AI credits | ~180 credits per image-to-3D (≈ $0.22/model at paid tier) |
| License | Free tier: CC BY 4.0 (personal use); paid: commercial |
| Commercial use | Yes, on paid plan |
| Dependencies | UModeler X (free) |
| Installed? | NO — **install first** |
| Reason | The ONE generator: only tool fusing gen + in-editor cleanup; cheapest commercial tier; runs Tripo + Meshy-class backends |

### 2. UModeler X — MODELING/CLEANUP SLOT — CRITICAL
| Field | Value |
|---|---|
| Tool | UModeler X (Unilogic) |
| Purpose | In-editor 3D modeling, topology cleanup, UV editing, hotspot texturing, painting, rigging, skinning |
| Version | Free since April 22, 2026 (Asset Store release current) |
| Unity compatibility | Unity 6 (Asset Store, official publisher) |
| Free/Paid | FREE |
| AI credits | None (PicoBerry handles gen) |
| License | Free for individuals AND businesses regardless of revenue |
| Commercial use | Yes |
| Dependencies | None |
| Installed? | NO — **install with PicoBerry** |
| Reason | The ONE modeling/cleanup tool; SketchUp-style, covers topology fix + UV + skinning inside the editor; free |

### 3. Unity glTFast (glTF import) — IMPORT SLOT — CRITICAL
| Field | Value |
|---|---|
| Tool | com.unity.cloud.gltfast (Unity official) |
| Purpose | Import GLB/glTF meshes (404-GEN + PicoBerry output) into Unity |
| Version | Latest Unity registry version |
| Unity compatibility | Unity official package, all current versions |
| Free/Paid | FREE |
| AI credits | None |
| License | Unity EULA / open source |
| Commercial use | Yes |
| Dependencies | None |
| Installed? | NO — **install with Unity setup** |
| Reason | Required plumbing: every AI mesh arrives as GLB; no importer = no pipeline |

### 4. Mixamo — RIGGING/ANIMATION SLOT — HIGH
| Field | Value |
|---|---|
| Tool | Adobe Mixamo (web) |
| Purpose | Auto-rig humanoid meshes + full animation library (walk/combat/idle) |
| Version | Current (stable, unchanged for years) |
| Unity compatibility | Exports FBX, imports clean into Unity |
| Free/Paid | FREE (Adobe account) |
| AI credits | None |
| License | Adobe ToS — commercial use allowed |
| Commercial use | Yes |
| Dependencies | Adobe account |
| Installed? | NO — install at first rigged character |
| Reason | The ONE rigging/animation solution: auto-rig beats manual for pipeline speed; UModeler X handles custom skin fixes |

### 5. Convai — NPC AI SLOT — HIGH
| Field | Value |
|---|---|
| Tool | Convai (Unity plugin + cloud) |
| Purpose | NPC personality, lore context, dialogue, actions, memory — the full NPC pillar |
| Version | Current 2026 plugin (verify at install) |
| Unity compatibility | Unity plugin, 2019.4+ through Unity 6 |
| Free/Paid | Free tier for testing; paid from ~$19.90/mo |
| AI credits | Usage-based beyond free tier |
| License | Commercial on paid tiers |
| Commercial use | Yes |
| Dependencies | Convai cloud account |
| Installed? | NO — install at NPC milestone (after character pipeline is proven) |
| Reason | The ONE NPC solution: covers personality→dialogue→memory→actions in one system; built-in voices reduce audio-tool need |

### 6. 404-GEN — OUR PIPELINE SHELL — HIGH (INSTALLED)
| Field | Value |
|---|---|
| Tool | 404-GEN (ours: tools/404gen/ in repo) |
| Purpose | Canon 2D → front plate → free TripoSR route → GLB; rapid prototyping + QC gates |
| Version | v0.1 (Sept 9, 2026 — first mesh generated) |
| Unity compatibility | GLB output via glTFast |
| Free/Paid | FREE |
| AI credits | None (public TripoSR space) |
| License | Ours; TripoSR = Stability MIT-licensed model |
| Commercial use | Yes |
| Dependencies | gradio_client, public HF Spaces |
| Installed? | YES |
| Reason | Pipeline discipline + $0 prototyping; validates every front plate before paid credits are spent; textured upgrade deferred |

### 7. Skybox AI (Blockade Labs) — ENVIRONMENT SLOT — MEDIUM
| Field | Value |
|---|---|
| Tool | Skybox AI Generator by Blockade Labs (native Unity plugin) |
| Purpose | 360° skyboxes/environments from 2D concepts — the 24-realm skies |
| Version | Unity plugin release Dec 2025, Unity 6000.3+ |
| Unity compatibility | Native plugin, Unity 6 |
| Free/Paid | 5 free preview credits; from $20/mo |
| AI credits | Credit-based |
| License | Full commercial rights on paid |
| Commercial use | Yes |
| Dependencies | Blockade Labs account |
| Installed? | NO — install at STAGE2 world pass |
| Reason | The ONE environment-gen tool: turns our locked realm concepts into in-engine skies; deferred until STAGE2 starts so we're not paying for idle months |

### 8. ElevenLabs — AUDIO/VOICE SLOT — MEDIUM (defer)
| Field | Value |
|---|---|
| Tool | ElevenLabs (via Convai integration) |
| Purpose | Character/NPC voices, barks, ambience VO |
| Version | Current (integrated into Convai per Aug 2025 update) |
| Unity compatibility | Through Convai or direct API |
| Free/Paid | Free tier ~10k credits/mo; paid from $5/mo |
| AI credits | Character-based credits |
| License | Commercial on paid |
| Commercial use | Yes |
| Dependencies | Convai (preferred route) |
| Installed? | NO — defer until voice pass |
| Reason | Convai ships built-in voices; ElevenLabs only if Convai's voices fall short of the mythic tone we want |

### 9. Blender — HEAVY RETOPO FALLBACK — OPTIONAL
| Field | Value |
|---|---|
| Tool | Blender |
| Purpose | Heavy retopo/sculpt beyond what UModeler X handles |
| Free/Paid | FREE |
| License | GPL, commercial OK |
| Installed? | NO — only if UModeler X proves insufficient |
| Reason | Keep the chain minimal; UModeler X is the in-editor answer, Blender is the escape hatch |

### 10. Genies — DO NOT INSTALL
| Field | Value |
|---|---|
| Tool | Genies Avatar SDK |
| Purpose | Stylized customizable AI-companion avatars |
| Reason for rejection | Wrong product for us: consumer avatar/companion platform, stylized look — contradicts our locked dark-mythic canon; duplicates the character pipeline we're building ourselves. Free SDK doesn't change the fit. |

### 11. Layer AI — DO NOT INSTALL
| Field | Value |
|---|---|
| Tool | Layer.ai |
| Purpose | Enterprise AI creative platform (images/video/3D/audio, 149+ models) |
| Reason for rejection | Duplicates our existing 2D generation (all canon 2D is already produced in-pipeline at no extra cost); enterprise pricing; no Unity integration; scope creep against the minimal-toolchain doctrine. Revisit ONLY if 2D canon production ever needs trained-style consistency at volume. |

### 12. Meshy — FALLBACK (not installed)
| Field | Value |
|---|---|
| Tool | Meshy 7 |
| Purpose | Textured image-to-3D; strongest raw mesh/texture quality in 2026 comparisons |
| Version | Meshy 7 (8K textures, free remeshing) |
| Unity compatibility | Unity plugin available |
| Free/Paid | Free 100 credits/mo (CC BY 4.0, personal); Pro $20/mo commercial |
| Installed? | NO — the ONE-GENERATOR fallback if PicoBerry output disappoints against canon plates |
| Reason | Held in reserve per the one-slot doctrine; never run alongside PicoBerry |

---

## FINAL TOOLCHAIN (7 active at full build, 3 installed now)

| Slot | Tool | When |
|---|---|---|
| 3D generation | PicoBerry AI ($12/mo) | NOW |
| Modeling/cleanup | UModeler X (free) | NOW |
| GLB import | glTFast (free) | NOW |
| Pipeline shell/prototyping | 404-GEN (free, ours) | INSTALLED |
| Rigging/animation | Mixamo (free) | First rigged character |
| NPC AI | Convai (free tier → paid) | NPC milestone |
| Environment skies | Skybox AI ($20/mo) | STAGE2 world pass |
| Voice (maybe) | ElevenLabs via Convai | Only if needed |

Rejected: Genies, Layer AI, Meshy-as-second-generator, and any tool that overlaps a filled slot.
