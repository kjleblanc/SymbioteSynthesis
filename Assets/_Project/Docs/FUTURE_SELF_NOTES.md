# FUTURE-SELF NOTES

## Glossary
- **Family:** Color group (Red/Blue/Purple…).
- **Trait:** Combat modifier (Rending = armor pen on hit; Armored = flat DR).
- **Tier:** Power level; Tier2 from merging Tier1s.
- **Discovery:** First-time fusion result recognition.

## Decisions Made
- Board: Hex (Point-Top), sticky, no gravity.
- Initiative: SPD-based tick; ties by unit id.
- Spawn pity: Trigger after 3 turns without new Discovery (+35% bias once).

## Known Issues & Top Risks
- Visual clutter at high unit counts → VFX throttle & pooling.
- Colorblind conflicts → icons + palettes.
- Fusion tutorial clarity → first-merge overlay & recipe hints.
- Discovery persistence currently JSON-only; add checksum/versioning before shipping.

## Next 3 High-Impact Tasks
1) Integrate RNGService facade so CombatResolver/WaveDirector draw from RunSeed deterministically.
2) Build Discovery Collection screen polish (sorting, trait icons, tooltips) and hook to meta progression UI.
3) Author combat VFX/audio placeholders and wire into CombatResolver events for clarity.

## CHANGELOG Entry Stub
v0.0.1 — Initial docs & production plan committed; seeds and specs defined.

---
**You Did:** Updated notes & changelog.  
**Expected:** New devs bootstrap fast.  
**Quick Test:** New dev reads notes → can explain pillars.  
**If Broken:** Expand glossary; cross-link to GDD.  
**Log Note:** Anchored to canonical; seeds & rules reiterated.
