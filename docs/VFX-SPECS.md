# VFX SPECIFICATIONS — Phase 4 (post rigging/animation), per Big's production order

## Global laws that govern all VFX
- TWO-LAYER LIGHT: cold desaturated ambient; the ONE rich light source per scene carries the color (Lantern-Region Aura Law).
- LANTERN-REGION AURA: the Luminary's glow = the region's aura (Skyrend storm slate / Ashfall oxide red / Stoneheart bone grey / Duskmourn crimson / Marenth tide teal / Everbloom deep moss). Aedan/Skyrend = STORM SLATE.
- MUTED PALETTE: no saturated RGB bloom; effects read painterly, cold, serious — magic in Avalon is old and quiet, never neon.
- FLAME IS NEVER SEEN UNVESSELLED (Lantern Law): no free-floating fire orbs; light lives in crafted vessels.

## Aedan Stormcrown (Sovereign, Skyrend) — flagship VFX set
1. AURA: faint slate-grey shimmer on armor edges at low health/rage — shader: fresnel edge glow, storm-slate, 0.15 intensity floor.
2. LANTERN GLOW: his belt-lantern (prop, attach-time) emits the only warm-point light — slate, not gold.
3. SKILL VFX (Sovereign = command/storm): rally banner-call (knotwork ring pulse at feet), spear-strike (slate arc trail, no glow particles), crown-lock hair wisps pick up ambient static in storms.
4. WEATHER TIE: in Skyrend scenes, his cloak responds to storm wind (cloth sim, subtle).

## Per-class effect grammar (for the remaining Eleven — rolled after rigging standard locks)
- Ravager/Ashfall: oxide-red ember drift on weapon impacts, ash kick-up.
- Warden/Stoneheart: bone-grey dust shockwaves, glyph-carve ground rings.
- Veilborn/Duskmourn: crimson mist step-trail, lantern light dies where he walks (Lantern Law inverse — Hollow adjacency).
- Weaver/Marenth: tide-teal thread ribbons on casts, wet-sheen reflections.
- Wildborn/Everbloom: deep-moss spore motes, foliage regrowth footprint (anti-grind: cosmetic only).

## Unity implementation notes (when the seat exists)
- Shader Graph: void-black shader (Hollows), fresnel aura shader (classes), lit-mist volumetrics (fog-first staging law).
- VFX Graph: ember/mist/mote systems, all palette-locked to region auras.
- All effects must pass the muted-palette QC against canon key art before ship.
