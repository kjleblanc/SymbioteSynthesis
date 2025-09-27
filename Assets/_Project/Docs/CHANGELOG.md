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

## v0.0.2 — 2025-09-27
- Removed URP dependencies and Global Light 2D to comply with Built-in 2D rendering mandate; restored orthographic main camera.
- Authored fusion/discovery data ScriptableObjects (cells, traits, recipes) and services wiring deterministic merge outputs to discovery persistence and UI panel.
- Added combat and wave scaffolding with placeholder enemies, prefabs, and initiative-based resolution to satisfy TC-003 setup for future tuning.
- 2025-09-27 — TC-002 Fusion Core + Discovery & TC-003 Combat/Wave scaffolds.
  - You Did: Implemented FusionService/DiscoveryLogService with persistent logging, created DiscoveryPanel prefab, seeded trait/cell/fusion assets, and introduced CombatResolver + WaveDirector with basic enemy data.
  - Expected: Red T1 + Red T1 upgrades to Red T2 and logs once; Red T1 + Blue T1 yields Purple Armored Claw; end-of-turn combat iterates through configured waves using initiative order.
  - Quick Test: Run FusionRecipeTests and DiscoveryTests in Unity Test Runner; trigger a merge in Game scene and observe Discovery panel update; end turn to watch CombatResolver advance Wave_Basic then Wave_ArmoredSolo.
  - If Broken: Verify FusionService recipe list assignments, ensure DiscoveryLogService has write access to persistentDataPath, and confirm WaveDirector references real WaveConfig assets (guid 643d62df383b4101ba757f20cc72632a / 21b6435cf0154621871fdd7372b842af).
  - Log Note: Discovery persistence serialized to JSON under SymbioteSynthesis/Meta; combat traits Rending/Armored currently static hooks awaiting future behavior expansions.

---
### Entry Template
- Date (YYYY-MM-DD)
- Summary of change
- Impacted docs / systems
- “You Did / Expected / Quick Test / If Broken / Log Note” (if relevant)
