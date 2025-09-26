# GDD (MVP-level)

## Mechanics
- **Drag-merge:** A→B; valid if recipe or same-family Tier1→Tier2.
- **Fusion outcomes:**
  - Same family Tier1 + Tier1 → Tier2 of that family.
  - Cross-family Tier1 + Tier1 → Mixed color/type (scriptable recipe).
- **Traits inherit:** union of component traits; resolve conflicts via `Trait.CombinePolicy`.
- **Stats (per Unit):** HP, ATK, SPD (ticks), RANGE, ARMOR PEN, TAGS.
- **Enemy waves:** Spawn at predefined hexes; simple AI: nearest hostile targeting, tie-break by lowest HP.

## Controls
- Mouse: LMB drag; RMB cancel; Wheel=zoom.
- Gamepad (optional later): left stick move cursor; A pick; A drop.

## Difficulty & Pacing
- Waves 1–10; elites at 4 & 8.
- ATK/HP scaling +5–10% per wave; elite +25%.
- Spawn pity: if no new Discovery in 3 turns, bias to unlogged result by +35% for 1 roll.
- Anti-dup: avoid >3 identical Tier1 on board.

## UX Flows
- Tutorial pop after first valid drag.
- Discovery tab auto-logs on first seen result; run view vs. meta Collection view.
- Map: choose 1 of 2–3 nodes; preview rewards (enemy type pattern or event).

## Accessibility
- Colorblind palettes (Deuter/Protan/Tritan) + high-contrast outlines.
- Shape language for families (icon frames).
- No flashing >3Hz; camera shake toggle; remap basics.

## Content Rating & Localization
- Teen (stylized bio-punk, non-gore).
- Localization: opt-in post-MVP; structure UI text in `ScriptableUIStrings.asset`.

---
**Paths/Files:** `Assets/_Project/Docs/GDD.md`

**You Did:** Saved GDD with subsections.  
**Expected:** Systems are unambiguous for coding.  
**Quick Test:** Verify pity & anti-dup rules are present.  
**If Broken:** File diff to last good; fix.  
**Log Note:** Constraints reference canonical to prevent drift.
