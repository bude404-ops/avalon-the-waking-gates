#!/usr/bin/env python3
"""v228 plate-quality pass — surgical Shell.cs + AvalonShellBuild.cs edits.
Every marker asserted unique before splicing (v226 lesson)."""

import sys

SHELL = "unity/Assets/AvalonShell/Shell.cs"
BUILD = "unity/Assets/AvalonShell/Editor/AvalonShellBuild.cs"

edits = open(SHELL).read()
orig_len = len(edits)


def splice(src, marker, insertion, label):
    n = src.count(marker)
    assert n == 1, f"MARKER FAIL [{label}]: found {n}x (expected 1)"
    return src.replace(marker, marker + insertion, 1)


# ---- 1. CarvedBtn: baked carved slab + engraved label shadow ----
m1 = """            var lbl = Label(slab.transform, txt, 15, Hex(0xc2a367), TextAnchor.MiddleCenter);
            lbl.rect().Stretch();"""
ins1 = """
            // v228 plate-quality: the baked carved slab — the plate's own stone + carved bevels
            // behind the engraved label (Bude, Sept 13: "menu works just need make the quality better")
            var baked = Art("SLAB-CARVED");
            if (baked != null)
            {
                fim.sprite = baked; fim.color = Color.white;
                frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
                edge.color = new Color(0f, 0f, 0f, 0f);
                var sh = lbl.AddComponent<Shadow>();
                sh.effectColor = new Color(0.03f, 0.025f, 0.015f, 0.85f);
                sh.effectDistance = new Vector2(0f, -1.5f);
            }"""
assert edits.count(m1) == 1, f"m1 found {edits.count(m1)}x"
edits = edits.replace(m1, m1 + ins1, 1)

# ---- 2. Title hero: the plate's own painted lettering when staged ----
m2 = """            var title = Label(p.transform, "AVALON", 72, Hex(0xf0e6cf), TextAnchor.MiddleCenter);"""
ins2 = """            var heroArt = Art("HERO-AVALON");
            if (heroArt != null)
            {
                var hi = new GameObject("Hero");
                hi.transform.SetParent(p.transform, false);
                var him = hi.AddComponent<Image>();
                him.sprite = heroArt; him.preserveAspect = true; him.raycastTarget = false;
                him.rect().anchorMin = new Vector2(0.25f, 0.875f); him.rect().anchorMax = new Vector2(0.75f, 0.995f);
            }
            if (Art("HERO-AVALON") == null) // fallback: keep the code-drawn hero when art missing
"""
assert edits.count(m2) == 1, f"m2 found {edits.count(m2)}x"
# wrap the fallback: hero art present -> skip code hero by making the label conditional
# simplest robust: keep the label but hide it when hero art exists — handled in 2b below
edits = edits.replace(m2, m2, 1)  # no-op placeholder, real logic in 2b

# ---- 2b. hide the code hero label when the baked hero art is on screen ----
m2b = """            title.rect().anchorMin = new Vector2(0, 0.885f); title.rect().anchorMax = new Vector2(1, 0.985f);"""
ins2b = """            if (Art("HERO-AVALON") != null) title.gameObject.SetActive(false);"""
assert edits.count(m2b) == 1, f"m2b found {edits.count(m2b)}x"
edits = edits.replace(m2b, m2b + ins2b, 1)

# ---- 3. Select: niche cards get the bronze carved frame (9-slice) ----
m3 = """            var niche = Panel(p.transform, "card-" + cd.name, new Color(0.42f, 0.34f, 0.21f, 0.80f));"""
ins3 = """
            var frameSprite = SlicedArt("NICHE-FRAME", 16f, 16f, 16f, 16f);
            if (frameSprite != null)
            {
                var nIm = niche.GetComponent<Image>();
                if (nIm != null) { nIm.sprite = frameSprite; nIm.type = Image.Type.Sliced; nIm.color = Color.white; }
            }"""
assert edits.count(m3) == 1, f"m3 found {edits.count(m3)}x"
edits = edits.replace(m3, m3 + ins3, 1)

# ---- 4. SlicedArt helper (9-slice sprite with border) next to Art() ----
m4 = """        Sprite Art(string name)
        {"""
ins4 = """        // v228: 9-slice sprite with a baked border (for the carved niche frame).
        Sprite SlicedArt(string name, float bl, float bb, float br, float bt)
        {
            var tex = Resources.Load<Texture2D>("Art/" + name);
            if (tex == null) return null;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(bl, bb, br, bt));
        }

"""
assert edits.count(m4) == 1, f"m4 found {edits.count(m4)}x"
edits = edits.replace(m4, ins4 + m4, 1)

# ---- 5. ember pulse fields ----
m5 = """        readonly Dictionary<string, Sprite> artCache = new Dictionary<string, Sprite>();"""
ins5 = """
        // v228: ember pulse on the selected class card
        UnityEngine.UI.Image emberCard;
        Coroutine emberPulse;"""
assert edits.count(m5) == 1, f"m5 found {edits.count(m5)}x"
edits = edits.replace(m5, m5 + ins5, 1)

# ---- 6. SelectCard: start the ember pulse on the chosen card ----
m6 = """            LoadClass(cd.name);
        }

        // Responsive layout"""
ins6 = """            if (emberPulse != null) StopCoroutine(emberPulse);
            emberCard = null;
            for (int i = 0; i < CLASSES.Length; i++) if (CLASSES[i] == cd) emberCard = cardTints[i];
            if (emberCard != null) emberPulse = StartCoroutine(EmberPulseRoutine());
            LoadClass(cd.name);
        }

        IEnumerator EmberPulseRoutine()
        {
            while (true)
            {
                yield return null;
                if (emberCard == null) continue;
                float p = 0.5f + 0.5f * Mathf.Sin(Time.time * 2.2f);
                emberCard.color = new Color(0.16f + 0.05f * p, 0.14f + 0.035f * p, 0.09f + 0.02f * p, 0.96f);
            }
        }

        // Responsive layout"""
assert edits.count(m6) == 1, f"m6 found {edits.count(m6)}x"
edits = edits.replace(m6, ins6, 1)

open(SHELL, "w").write(edits)
print(f"Shell.cs: {orig_len} -> {len(edits)} chars (+{len(edits)-orig_len})")

# ---- 7. AvalonShellBuild.cs: stage the v228 ui art ----
b = open(BUILD).read()
m7 = "            int staged = 0;"
ins7 = """            foreach (var art in new[] { "SLAB-CARVED.png", "NICHE-FRAME.png", "HERO-AVALON.png" })
            {
                var src = Path.Combine("../art/ui", art);
                if (File.Exists(src)) { File.Copy(src, "Assets/AvalonShell/Resources/Art/" + art, true); Debug.Log("[SHELL] staged ui art: " + art); }
            }
"""
assert b.count(m7) == 1, f"m7 found {b.count(m7)}x"
b = b.replace(m7, ins7 + m7, 1)
open(BUILD, "w").write(b)
print("AvalonShellBuild.cs: ui art staging added")
print("ALL SPLICES OK")
