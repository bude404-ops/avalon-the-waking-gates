> **⚠️ PANTHEON LAW SUPERSESSION (Sept 8 2026, Big: 'we only make 6 gods 3 male 3 females') — swept Sept 9.** The god roster is SIX PATRON GODS, one per realm, 3 male + 3 female: **ASHFALL — THE EMBER KING (Vaelthorn)** · **SKYREND — THE STORM QUEEN (Sylwenna)** · **STONEHEART — THE IRON KING (Grathwyn)** · **MARENTH — THE TIDE QUEEN (Ylsanne)** · **EVERBLOOM — THE LIFE MOTHER (Mirielle)** · **DUSKMOURN — THE DEATH KING (Morvaine)** — canon art: art/approved/GOD-<realm>-*-CANON.png. The retired second giant per faction (**Vessamaine, Haeldor, Thevraine, Senneth, Nerovane, Bergrune**) is RECAST as REALM TITANS — guardian colossi of the world sites (Avatar Doctrine cover: gate-crossing vessels and realm wardens, never worshipped, never pantheon). Sprite champions (Warden/Keeper cast) remain champion NPCs. All kits/numbers below apply unchanged — to the six playable god forms at giant scale, and to the titan guardians + champions as NPC units. Legacy 24-deity phrasing below is historical.

# Mythos Gates: Ascension — Combat Balance Document

**Version:** 1.0.0
**Date:** August 29, 2026
**Status:** LOCKED
**Approved by:** BudE404 (Creative Director), BIGagent404

---

## 1. Role Stat Templates

ROSTER NOTE (Sept 9 sweep): the deity roster is now SIX pantheon gods (3M+3F, Gender-Role Binding: kings = Warrior chassis, queens = Caster chassis) — the Warrior/Caster templates govern the six playable god forms; Archer/Assassin templates govern champion NPCs and titan guardians. All formulas, scaling, and enemy stats below are unchanged and inherited by the current roster. Each role has a distinct playstyle — no role is objectively "better."

### Warrior (7 deities)
| Stat | Value | Design |
|------|-------|--------|
| HP | 48-52 | Highest survivability |
| ATK | 10-11 | Medium damage |
| DEF | 18-19 | Highest defense |
| SPD | 2 | Slow, deliberate |
| RNG | 2 | Melee range |
| Dodge | 35 | Medium evasion |
| Parry | 40 | Highest damage reduction |
| Acc | 81-83 | Medium accuracy |

**Faith Trigger:** Win while staying above 50% HP the entire fight.

### Caster (7 deities)
| Stat | Value | Design |
|------|-------|--------|
| HP | 38-42 | Fragile |
| ATK | 11-12 | High ability damage |
| DEF | 9 | Low defense |
| SPD | 3 | Medium mobility |
| RNG | 3 | Medium range |
| Dodge | 28-32 | Low evasion |
| Parry | 15 | Very low damage reduction |
| Acc | 84-86 | High accuracy |

**Faith Trigger:** Win after executing 3+ ability combo chains.

### Archer (7 deities)
| Stat | Value | Design |
|------|-------|--------|
| HP | 36-42 | Lowest HP |
| ATK | 10-11 | Medium damage |
| DEF | 8 | Lowest defense |
| SPD | 3 | High mobility |
| RNG | 4 | Longest range |
| Dodge | 53-57 | High evasion |
| Parry | 10 | Very low damage reduction |
| Acc | 87-88 | Highest accuracy |

**Faith Trigger:** Win while controlling 60%+ of battlefield nodes.

### Assassin (7 deities)
| Stat | Value | Design |
|------|-------|--------|
| HP | 40-44 | Medium survivability |
| ATK | 13-14 | Highest burst damage |
| DEF | 12 | Medium defense |
| SPD | 2 | Low mobility (positioning matters) |
| RNG | 2 | Melee range |
| Dodge | 58-62 | Highest evasion |
| Parry | 25 | Medium damage reduction |
| Acc | 80-81 | Lowest accuracy (high risk, high reward) |

**Faith Trigger:** Win after breaking 3+ enemy armor/defense stacks.

---

## 2. Stat Spread Audit

All stat spreads within each role are ≤4 points (except HP for Archers at 6, which is intentional — fragility tradeoff).

| Role | HP Spread | ATK Spread | DEF Spread | Dodge Spread | Parry Spread | Acc Spread |
|------|-----------|------------|------------|---------------|--------------|------------|
| Warrior | 4 (48-52) | 1 (10-11) | 1 (18-19) | 0 (35) | 0 (40) | 2 (81-83) |
| Caster | 4 (38-42) | 1 (11-12) | 0 (9) | 4 (28-32) | 0 (15) | 2 (84-86) |
| Archer | 6 (36-42) | 1 (10-11) | 0 (8) | 4 (53-57) | 0 (10) | 1 (87-88) |
| Assassin | 4 (40-44) | 1 (13-14) | 0 (12) | 4 (58-62) | 0 (25) | 1 (80-81) |

**Per-Faction Balance:** Each faction has exactly 1 of each role (Warrior, Caster, Archer, Assassin). No faction has a stat advantage — the ±2 variations are distributed evenly.

---

## 3. Dodge/Parry Resolution Math

### Formulas
```
Dodge% = DefenderDodge / (DefenderDodge + AttackerAccuracy)
Parry% = DefenderParry / (DefenderParry + AttackerPower)
```

If dodge succeeds → 0% damage (full evade).
If dodge fails but parry succeeds → 50% damage.
If both fail → 100% damage (minus armor reduction).

Armor reduction = DEF / (DEF + 100)

### Expected Damage (Base %) by Role vs Enemy Archetype

| Role | vs Swarmer | vs Brute | vs Champion | vs EnemyDeity |
|------|-----------|----------|-------------|---------------|
| Warrior | 34% | 40% | 44% | 49% |
| Caster | 42% | 50% | 55% | 60% |
| Archer | 34% | 41% | 47% | 51% |
| Assassin | 28% | 34% | 39% | 44% |

**Design Intent:**
- Warriors take the least damage from all sources (tank role fulfilled)
- Assassins are surprisingly durable due to high dodge (but fragile if dodge fails — no parry backup)
- Casters take the most damage (glass cannon — must kill before being killed)
- Archers are evasive like assassins but take more damage when hit (low parry)

---

## 4. Enemy Base Stats (L1)

| Archetype | HP | ATK | Acc | Power | MoveSpeed | AttackRange | AttackCD |
|-----------|-----|-----|-----|-------|-----------|-------------|----------|
| Swarmer | 30 | 5 | 50 | 8 | 400 | 100 | 1.5s |
| Brute | 150 | 15 | 55 | 18 | 200 | 150 | 3.0s |
| Hunter | 50 | 10 | 70 | 10 | 250 | 600 | 2.5s |
| Controller | 80 | 8 | 55 | 10 | 300 | 400 | 2.0s |
| Disruptor | 70 | 7 | 65 | 9 | 350 | 300 | 1.8s |
| Guardian | 200 | 12 | 55 | 15 | 150 | 120 | 2.5s |
| Executioner | 100 | 20 | 60 | 22 | 300 | 150 | 3.5s |
| Elite | 120 | 14 | 65 | 16 | 280 | 200 | 2.0s |
| Champion | 300 | 18 | 70 | 20 | 250 | 180 | 2.2s |
| EnemyDeity | 500 | 25 | 80 | 28 | 300 | 250 | 1.5s |

---

## 5. Progression Scaling (L1 → L60)

### Formula
```
Stat(Level) = BaseStat × (1 + ScalingRate × (Level - 1))
```

### Deity Scaling Rates
| Stat | Rate per Level | Multiplier at L60 | Design Intent |
|------|---------------|-------------------|---------------|
| HP | +12% | 8.08x | Significant HP growth |
| ATK | +10% | 6.90x | Strong damage growth |
| DEF | +8% | 5.72x | Steady defense growth |
| SPD | +3% | 2.77x | Slow speed growth (positioning stays relevant) |
| Dodge | +6% | 4.54x | Matches enemy accuracy scaling |
| Parry | +6% | 4.54x | Matches enemy power scaling |
| Accuracy | +6% | 4.54x | Matches enemy dodge scaling |

### Enemy Scaling Rates (MUST match deity rates)
| Stat | Rate per Level | Design Intent |
|------|---------------|---------------|
| Enemy HP | +12% | Matches deity HP (TTK stays constant) |
| Enemy ATK | +10% | Matches deity ATK (lethality stays constant) |
| Enemy Acc | +6% | Matches deity Dodge/Parry (mitigation % stays constant) |
| Enemy Power | +6% | Matches deity Dodge/Parry (mitigation % stays constant) |

### Balance Verification

Because deity defensive stats (Dodge, Parry) and enemy offensive stats (Accuracy, Power) scale at identical rates, the dodge/parry percentages remain **constant across all levels**:

| Level | Warrior Dodge% vs EnemyDeity | Warrior Parry% vs EnemyDeity |
|-------|------------------------------|------------------------------|
| 1 | 30% | 59% |
| 15 | 30% | 59% |
| 30 | 30% | 59% |
| 45 | 30% | 59% |
| 60 | 30% | 59% |

This ensures the combat feels the same at L1 and L60 — the numbers get bigger but the ratios stay balanced.

---

## 6. Ability Cost Balance

### Energy Costs (Divine Energy, max 100)
| Ability Slot | Cost Range | Regain Rate |
|-------------|------------|-------------|
| Basic Attack | 0 (free) | +5 per hit |
| Ability 1 | 20-30 | — |
| Ability 2 | 35-40 | — |
| Signature | 65-70 | — |
| Ultimate | 0 (uses Belief) | — |

**Energy Regeneration:** +2/sec passive + 5 per basic attack + 50% of damage taken

### Belief Bar (max 100)
| Source | Belief Gain |
|--------|-------------|
| Basic Attack | +2 |
| Ability Use | +8 |
| Dodge Dash | +3 |
| Enemy Kill | +10 |
| Passive Proc | +5 |
| Faith Trigger (on win) | +20 |

**Belief Decay:** -0.5/sec after 5s of no combat action

**Ultimate:** Requires 100% Belief. Consumes entire bar on use. One per battle.

### Cooldown Ranges
| Ability Slot | CD Range | Design |
|-------------|----------|--------|
| Ability 1 | 4-8s | Frequent tactical use |
| Ability 2 | 8-10s | Medium frequency |
| Signature | 16-18s | Once per major engagement |
| Ultimate | ∞ (1 per battle) | Belief-gated |
| Dodge Button | 3-5s | Frequent repositioning |

---

## 7. Gear & Rarity Caps

| Rarity | Max Level | Stat Budget | Gear Tier Cap |
|--------|----------|-------------|---------------|
| Rare | 50 | 0 bonus | Epic |
| Epic | 55 | +4 stat budget | Legendary |
| Legendary | 60 | +8 stat budget | Mythic |
| Mythic | 60 | +12 stat budget | Mythic |

**Stat budget allocation (Legendary example):**
- +4 HP, +1 ATK, +1 Energy, +2 Crit
- OR +2 HP, +1 ATK, +1 Energy, +2 Armor, +2 Resistance

Gear bonuses are capped to prevent breaking the balance curve. No gear can add more than +2 to any single stat per tier.

---

## 8. Combat Power Bands

### Deity Combat Power (sum of all stats at L1)
| Role | Power Range | Average |
|------|------------|---------|
| Warrior | 240-250 | 245 |
| Caster | 210-220 | 215 |
| Archer | 215-225 | 220 |
| Assassin | 230-240 | 235 |

Warriors have the highest raw stats (tanky), Casters the lowest (glass cannon). This is intentional — Casters compensate through ability damage multipliers (3x base ATK per ability vs 1x for basic attacks).

### Combat Power at L60 (scaled)
| Role | Power Range |
|------|------------|
| Warrior | 1,500-1,600 |
| Caster | 1,350-1,420 |
| Archer | 1,380-1,450 |
| Assassin | 1,450-1,520 |

Ratios stay constant — no role out-scales another at high levels.

---

## 9. Fixes Applied (Aug 29 2026 Balance Pass)

| Deity | Issue | Fix |
|-------|-------|-----|
| Jophiel | ATK=14, RNG=3 (too high for Assassin) | ATK→13, RNG→2 |
| Lucifer | HP=44, ATK=13 (too high for Caster) | HP→42, ATK→12 |
| Asmodeus | DEF=17 (too low for Warrior) | DEF→19 |
| Artemis | HP=36 (too low) | HP→38 |
| All 28 | Missing Dodge/Parry stats | Added per role template |

---

## 10. Wave Balance

| Wave Type | Enemy Count | Archetypes | Design |
|-----------|------------|------------|--------|
| Trash | 5-15 | Swarmer, Disruptor | Killed by basic attacks + AoE |
| Elite | 2-4 | Hunter, Controller, Elite | Requires ability usage |
| Boss | 1 (+adds) | Champion, EnemyDeity, Guardian | Full rotation + positioning |

**Time-to-Kill Target:**
- Trash mob: 2-4 basic attack hits (1-2 seconds)
- Elite: 1-2 ability rotations (8-12 seconds)
- Boss: Full encounter (30-60 seconds with all abilities)

---

**Document Status:** LOCKED — All balance formulas, stat templates, and scaling rates are final.
**Next Review:** Only if playtesting reveals issues. Formula design prevents level-based imbalance by construction.
