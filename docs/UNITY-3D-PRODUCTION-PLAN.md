# UNITY 3D PRODUCTION PLAN — THE TWELVE CLASS MODELS (Sept 9 2026)

**Doctrine:** UNITY-ONLY TOOL LAW + UNITY-FIRST 3D LAW govern — every model comes from Unity built-ins + Asset Store (rigged humanoid bases, modular armor/weapon packs, animation sets), customized in-editor with UModeler X (Asset Store, free, LOCAL in-editor modeling/retopo/UV/rig/skin — full commercial license), QC'd side-by-side against the locked 2D canon keys (art/approved/CLASS-*-CANON.png — the art-direction bible). The 404-GEN image-to-3D generation route is RETIRED. Unity Editor runs on Big's seat — this repo carries the sourcing shortlists, ref sheets, specs, C#, and QC checklists; in-editor assembly happens at the workstation.

**Reference sheets (Stage B plates):** art/pending/REF-SHEET-CLASS-*-V1.png — A-pose 3-view turnaround (front/side/back), flat neutral lighting, kit-matched to canon. Use as the modeling/customization target + the QC bar alongside the canon key art.

## A. BASE MODELS (rigged humanoid) — search shortlist
All twelve models share ONE humanoid rig standard (universal naming for animation packs): search terms — `rigged humanoid base`, `stylized character base mesh`, `modular humanoid`. Pick ONE male-skeleton base + ONE female-skeleton base and reuse for all twelve (keeps animation sets universal):
- Male: broad-shouldered adjustable topology (retopo to 3 build archetypes: statuesque Sovereign / massive Ravager / stocky Warden; + wiry Veilborn, spare Weaver, feral Wildborn).
- Female: curvy hourglass base (per the female frame law — full bust, cinched waist, rounded hips, realistic proportions), morph targets per class sizing (statuesque / tall broad / stocky strong / lean compact / slim spare / athletic feral). NEVER athletic-flat; zero visible muscle definition.

## B. ARMOR + WEAPON PACKS — search shortlist (matched against canon keys)
Canon kit grammar for ALL: solid CLOSED engraved bronze breastplate, high collar, NO chest cutout; knotwork + La Tène spiral engravings; sleeveless; battle-kilt panels; torcs; cloaks; leaf-blade weapons. Search terms:
- Armor: `Celtic armor`, `bronze cuirass`, `modular fantasy armor kit`, `knotwork ornament pack`, `cloth simulation cloak`
- Weapons: `Celtic spear`, `two-handed greatsword`, `warhammer`, `dual daggers leaf blade`, `wooden gnarled staff`, `claw gauntlets`
- Per-class accents (material swaps only — one kit, six colorways): storm slate (Sovereign) / oxide red (Ravager) / bone grey (Warden) / crimson (Veilborn) / tide teal (Weaver) / deep moss (Wildborn).
- FEMALE SET: same packs, fitted-cut adjustments + battle-skirts over leggings + tasteful exposed shoulders/upper back per the female armor spec; 50/50 functional/provocative split.
- HAIR (Hairstyle-to-Class Law — M+F share the class read): braided crown-lock w/ bronze rings (Sovereign) / war-shaved sides + top-knot (Ravager) / cropped + pinned plaits (Warden) / long straight half-veiling (Veilborn) / unbound flowing w/ cord-strands + bronze beads (Weaver) / matted dreads w/ moss (Wildborn). Search: `modular hair pack`, `dreads stylized`, `braided hair games`.

## C. ASSEMBLY SPEC (per model, on Big's seat)
1. Base skeleton → UModeler X retopo to class build archetype.
2. Kit: breastplate (closed, engraved normal-map from knotwork tile), kilt, boots, torc, cloak (cloth sim or bone chain).
3. Weapon from pack → leaf-blade reshaping in UModeler X where needed.
4. Materials: cold muted palette, weathering masks (story-wear), ONE class accent colorway as the quiet cloth/trim accent.
5. QC GATE: render turnaround vs REF-SHEET + canon key side-by-side — kit-integrity (no extra weapons), female law stack where applicable, silhouette match. Fail = adjust, re-QC. Vault pass renders to art/production/ for the canon record.

## D. "ONES THAT NEED IT" — custom 3D work queue (beyond stock)
- ALL SIX GODS: not humanoid-stockable — colossal primordial element colossi (torso-up, terrain-fused, elemental faces w/ features per God Face Law v2). Custom UModeler X sculpts + VFX (particle elements) built from the locked god canon keys. Highest custom-work priority for the Cinematic/Two-Scales feature.
- TITAN GUARDIANS + Warden/Keeper champions: UModeler X customization from giant-stock bases, matched to the STAGE2 world canon.
- HOLLOW BESTIARY T1-T3: the void-black-face shader (near-zero albedo + wisp particle per ART_DIRECTION §VOID BLACK) is a custom shader job; bodies can start from creature packs, faces must be custom (void law).
- CLASS MODELS: stock-covered (B above) — no custom sculpting needed beyond retopo/kit adjustments.

## E. ANIMATION SETS (shared rig)
Search: `melee combat animation set`, `spear animations`, `greatsword animations`, `locomotion pack`. Class grammar: Sovereign pole discipline / Ravager momentum fury / Warden weight / Veilborn paired-dagger flow / Weaver staff-cast / Wildborn claw rush. Giant-scale god forms reuse the colossus set from COLOSSUS_COMBAT_SPEC.
