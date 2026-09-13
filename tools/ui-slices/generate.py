#!/usr/bin/env python3
"""
AVALON v228 — V2 menu asset generator (Bude's new refs, Sept 13:
"this one was the title menu it needs to be based from" / "this one is the
character menu we are going for").

New grammar sampled from the refs:
  slab face  RGB (57,55,50)   dark warm stone
  edge lines RGB (155,142,118) warm tan bevel
  labels     RGB (240,235,222) parchment-white, engraved bright

Outputs (art/ui/):
  SLAB-CARVED.png   960x160  text-free carved slab (stretch-rendered behind
                                 runtime Cinzel labels — text stays crisp)
  NICHE-FRAME.png   320x420  warm dark stone niche frame, 9-slice border 14px,
                                 near-black interior (class portraits render inside)
  HERO-AVALON.png   the title ref's own painted AVALON lettering band, cropped
"""
from PIL import Image, ImageFilter
import numpy as np
import os

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.join(HERE, "..", "..")
REF_TITLE = os.path.join(ROOT, "incoming_menu_ref.jpg")          # pre-vault source
OUT = os.path.join(ROOT, "art", "ui")
os.makedirs(OUT, exist_ok=True)

RNG = np.random.default_rng(2204)

FACE = np.array([57, 55, 50], dtype=np.float32)          # slab face (sampled)
EDGE = np.array([155, 142, 118], dtype=np.float32)       # tan bevel line (sampled)
PARCH = np.array([240, 235, 222], dtype=np.float32)      # label color (sampled)
INTERIOR_TOP = np.array([16, 16, 15], dtype=np.float32)
INTERIOR_BOT = np.array([9, 9, 9], dtype=np.float32)


def stone(w, h, base=FACE, var=3.0, top_boost=1.10, bot_dim=0.85):
    """Warm dark stone: base + fine noise + vertical light gradient."""
    tex = np.ones((h, w, 3), dtype=np.float32) * base
    tex += RNG.normal(0, var, (h, w, 3))
    gy = np.linspace(top_boost, bot_dim, h)[:, None, None]
    tex *= gy
    # gentle horizontal end vignette
    gx = np.ones(w)
    gx[:36] = np.linspace(0.90, 1.0, 36)
    gx[-36:] = np.linspace(1.0, 0.90, 36)
    tex *= gx[None, :, None]
    return np.clip(tex, 0, 255)


def carve_slab(w=960, h=160):
    pad = 8
    W, H = w + pad * 2, h + pad * 2
    img = np.zeros((H, W, 4), dtype=np.float32)
    face = stone(w, h)
    img[pad:pad + h, pad:pad + w, :3] = face
    img[pad:pad + h, pad:pad + w, 3] = 255.0
    # tan bevel edge lines (2px) — lit top edge slightly brighter
    img[pad + 1:pad + 3, pad + 3:pad + w - 3, :3] = EDGE * 1.12
    img[pad + h - 3:pad + h - 1, pad + 3:pad + w - 3, :3] = EDGE * 0.92
    img[pad + 3:pad + h - 3, pad + 1:pad + 3, :3] = EDGE * 0.85
    img[pad + 3:pad + h - 3, pad + w - 3:pad + w - 1, :3] = EDGE * 0.80
    # engraved inner shading just inside the edges (chamfer)
    for i, f in enumerate(np.linspace(0.78, 1.0, 5)[1:-1]):
        img[pad + 3 + i, pad + 3:pad + w - 3, :3] *= f        # top chamfer darker
        img[pad + h - 4 - i, pad + 3:pad + w - 3, :3] *= f
    # outer dark keyline (carve contour)
    key = np.array([20, 19, 16], dtype=np.float32)
    img[pad - 1:pad + 1, pad - 1:pad + w + 1, :3] = key
    img[pad + h - 1:pad + h + 1, pad - 1:pad + w + 1, :3] = key
    img[pad - 1:pad + h + 1, pad - 1:pad + 1, :3] = key
    img[pad - 1:pad + h + 1, pad + w - 1:pad + w + 1, :3] = key
    img[pad - 1:pad + h + 1, pad - 1:pad + w + 1, 3] = 255.0
    # soft drop shadow under the slab
    sh = np.zeros((H, W), dtype=np.float32)
    sh[pad + 5:, 3:W - 3] = 80.0
    sh = np.asarray(Image.fromarray(sh.astype(np.uint8)).filter(ImageFilter.GaussianBlur(3.5)), dtype=np.float32)
    a = img[:, :, 3]
    a[pad + 5:, 3:W - 3] = np.maximum(a[pad + 5:, 3:W - 3], sh[pad + 5:, 3:W - 3] / 80 * 110)
    return Image.fromarray(np.clip(img, 0, 255).astype(np.uint8))


def carve_niche(w=320, h=420, border=14):
    """Warm dark stone frame, 9-slice border=14px; near-black interior stretches."""
    img = stone(w, h, base=FACE * 0.92, var=2.6)
    # interior: near-black vertical gradient
    for y in range(border, h - border):
        t = (y - border) / max(1, h - 2 * border)
        img[y, border:w - border, :] = INTERIOR_TOP + (INTERIOR_BOT - INTERIOR_TOP) * t
    img[border:h - border, border:w - border] += RNG.normal(0, 1.2, (h - 2 * border, w - 2 * border, 1))
    # inner tan edge (frame meets interior)
    for i in range(2):
        img[border + i, border:w - border] = EDGE * 0.95
        img[h - border - 1 - i, border:w - border] = EDGE * 0.80
        img[border:h - border, border + i] = EDGE * 0.88
        img[border:h - border, w - border - 1 - i] = EDGE * 0.76
    # frame top highlight + bottom shade
    img[2:border - 2, 2:w - 2] *= np.linspace(1.28, 1.0, border - 4)[:, None, None]
    img[h - border + 2:h - 2, 2:w - 2] *= np.linspace(1.0, 0.72, border - 4)[:, None, None]
    # outer keyline
    img[0:2, :] = np.array([20, 19, 16]); img[h - 2:h, :] = np.array([20, 19, 16])
    img[:, 0:2] = np.array([20, 19, 16]); img[:, w - 2:w] = np.array([20, 19, 16])
    return Image.fromarray(np.clip(img, 0, 255).astype(np.uint8))


def hero_crop(src):
    """The title ref's own painted AVALON lettering band."""
    im = np.asarray(Image.open(src).convert("RGB"), dtype=np.uint8)
    return Image.fromarray(im[56:134, 276:740])


if __name__ == "__main__":
    title_src = REF_TITLE if os.path.exists(REF_TITLE) else os.path.join(ROOT, "art", "approved", "UI-MAIN-MENU-V2-CANON.png")
    carve_slab().save(os.path.join(OUT, "SLAB-CARVED.png"))
    carve_niche().save(os.path.join(OUT, "NICHE-FRAME.png"))
    hero_crop(title_src).save(os.path.join(OUT, "HERO-AVALON.png"))
    for f in ["SLAB-CARVED.png", "NICHE-FRAME.png", "HERO-AVALON.png"]:
        p = os.path.join(OUT, f)
        print(f, Image.open(p).size, f"{os.path.getsize(p)//1024}KB")
