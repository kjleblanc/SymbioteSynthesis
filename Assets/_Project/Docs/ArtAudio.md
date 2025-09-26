# Art/Audio Direction

## Hex Readability Constraints
- Cell sprite fits inside hex with 6–8 px padding at 1080p; outline 1–2 px.
- Tier pips at top-right; trait badges bottom-left.
- Merge/combat VFX capped ≤200ms burst; alpha-premultiplied.

## Style Lanes
- **Bio-punk Painterly** — Organic brushes, saturated accents; juicy merge VFX. (+Memorable; −Harder to batch/scale.)
- **Clean Lab Diegetic UI (chosen for MVP)** — Clinical UI frames, flat-shaded organisms; high readability. (+Fast export; +UI-diegetic Discovery; −Less “grit.”)

## Visuals Agent Prompt Kit (global)
- **Global prompt:** “Bio-punk lab aesthetic, flat-shaded organisms in petri-hex UI, high contrast edges, limited palette per family, clean vector UI frames, no gore.”
- **Seeds:** Use `GLOBAL_SEED: 41073`; per-asset seeds listed below.
- **Negatives:** “gore, photoreal, text labels, busy backgrounds.”

## Assets (names/specs)
- **Cell Sprites** `Assets/_Project/Art/Cells/{Family}/{Tier}/`
  - Families (Tier1→Tier2 examples):
    - Red/Claw: `claw_t1.png` (seed 101), `pincer_t2.png` (seed 102)
    - Blue/Carapace: `carapace_t1.png` (201), `bulwark_t2.png` (202)
    - Purple/Hybrid: cross recipes: `armored_claw_t2.png` (301)
  - Import: 256×256, PPU=128, Filter=Point, Compression=Normal Quality, Pivot=Center, Padding=4 px.
- **Trait Icons** `Assets/_Project/Art/UI/Traits/`
  - `trait_rending.png` (seed 501), `trait_armored.png` (502) — 64×64, PPU=64.
- **VFX** `Assets/_Project/Art/VFX/`
  - Merge: `vfx_merge_squelch.prefab` — 0.18s.
  - Hit: `vfx_hit_clink.prefab` — 0.12s with spark.
  - Damage numbers: TMP text (if used), or bitmap numbers sprites.
- **UI/HUD** `Assets/_Project/Art/UI/HUD/`
  - Tier pips (1–3), Move counter, Wave meter, Discovery tab frames.
- **Marketing** `Assets/_Project/Art/Marketing/`
  - Capsule 616×353, Key 1920×1080, Thumb 512×512 templates.

## Audio Prompts/Specs
- Merge SFX “wet squelch, short, no low-end mud” seed 801; WAV 44.1kHz/16-bit; -3dB peak.
- Armored hit “clink with short tail” seed 802.
- Battle ambience loop 60–90s; OGG ~160kbps; seamless loop.
- Music loop: 2:00, minor scale, BPM 90; seed 820.

---
**You Did:** Picked style lane; provided prompt kit with seeds.  
**Expected:** Visuals Agent can batch-produce assets with consistent specs.  
**Quick Test:** Import one cell, one trait, one VFX; check scale & clarity.  
**If Broken:** Adjust PPU/padding; re-export.  
**Log Note:** Prevented style drift via global prompt, seeds, and folder rules.
