# Symbiote Synthesis — CHANGELOG

All notable changes to this project will be documented in this file. Keep entries short and objective. One version per section.

## v0.0.1 — 2025-09-26
- Initial production docs authored and committed.
- Canonical pillars, loop, MVP scope, seeds/specs established.
- Project rules and multi-agent workflow written.
- Roadmap drafted (6-week MVP push).
- 2025-09-26 — TC-001 hex board & merge scaffold.
  - You Did: Implemented HexBoardService/InputMergeService/TurnController merge loop, wired Game scene with hex tilemap + placeholders, authored RunSeed asset and playmode tests.
  - Expected: Drag Tier1 placeholder A→B merges up to three times; TurnController blocks fourth attempt.
  - Quick Test: Open Game.unity, drag placeholder cell three times, confirm counter + run TC001_MergeTests.
  - If Broken: Disable InputMergeService in Systems GO, verify HexBoardService placement via tests, then rewire scene references.
  - Log Note: RNG hook pending; RunSeed asset ready for future RNGService integration.

---
### Entry Template
- Date (YYYY-MM-DD)
- Summary of change
- Impacted docs / systems
- “You Did / Expected / Quick Test / If Broken / Log Note” (if relevant)
