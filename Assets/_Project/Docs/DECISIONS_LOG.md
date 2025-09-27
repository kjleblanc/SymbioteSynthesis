# Symbiote Synthesis — DECISIONS LOG

**Purpose:** A running, append-only log of consequential decisions. Each entry records context, decision, impact, and rollback plan.

> Format
- **Date:** YYYY-MM-DD
- **Topic:** Short label
- **Decision:** What we chose
- **Rationale:** Why we chose it (tradeoffs)
- **Impact:** Code, art, production, risks
- **Rollback:** How to undo if needed
- **Links:** Files/PRs/tests

---

- **Date:** 2025-09-26
  - **Topic:** Rendering
  - **Decision:** Use Built-in 2D for MVP; defer URP until justified.
  - **Rationale:** Faster iteration, fewer moving parts; readability-first.
  - **Impact:** No 2D lights at MVP; simpler import pipeline.
  - **Rollback:** Switch to URP via §10 steps; audit materials.
  - **Links:** `Assets/_Project/Docs/GDD.md`, `Assets/_Project/Docs/PROJECT_RULES.md`

- **Date:** 2025-09-26
  - **Topic:** Core Economy
  - **Decision:** Space is the economy; three merges per turn; no currencies.
  - **Rationale:** Differentiation; focus on board tension and discovery.
  - **Impact:** UI needs merge counter; balance levers shift to spawn/waves.
  - **Rollback:** Add currency only post-MVP; requires new UX.
  - **Links:** `Assets/_Project/Docs/CoreLoop.md`, `Assets/_Project/Docs/MVP.md`

- **Date:** 2025-09-26
  - **Topic:** Style Lane (MVP)
  - **Decision:** **Clean Lab Diegetic UI**; flat-shaded organisms; high readability.
  - **Rationale:** Fast export, clear icons; production-friendly.
  - **Impact:** Visual prompts + seeds defined; VFX capped.
  - **Rollback:** Pivot to Bio-punk Painterly; re-export with prompt kit variants.
  - **Links:** `Assets/_Project/Docs/ArtAudio.md`

- **Date:** 2025-09-26
  - **Topic:** Determinism
  - **Decision:** Seeded RNG for runs; generative seeds recorded alongside prompts.
  - **Rationale:** Reproducibility & debugging.
  - **Impact:** `RNGService` categories; seed input UI at run start.
  - **Rollback:** None; keep deterministic backbone.
  - **Links:** `Assets/_Project/Docs/TDD.md`, `Assets/_Project/Docs/FUTURE_SELF_NOTES.md`

- **Date:** 2025-09-26
  - **Topic:** Scope
  - **Decision:** MoSCoW MVP with explicit “Won’t” list.
  - **Rationale:** Prevent creep; ship in 6 weeks.
  - **Impact:** No multiplayer/mobile/localization at MVP.
  - **Rollback:** Re-open items post-1.0.
  - **Links:** `Assets/_Project/Docs/MVP.md`, `Assets/_Project/Docs/Roadmap.md`
- **Date:** 2025-09-26
  - **Topic:** Seed Source
  - **Decision:** Centralize deterministic gameplay RNG via `RunSeed` ScriptableObject at `Assets/_Project/Data/Seeds/RunSeed.asset`.
  - **Rationale:** Single source of truth keeps future RNGService integrations deterministic per guardrails.
  - **Impact:** All future systems pull seed value from this asset; tests can stub or override it consistently.
  - **Rollback:** Replace asset/script path and update dependent services; regenerate asset GUID if moving locations.
  - **Links:** `Assets/_Project/Scripts/Seeding/RunSeed.cs`, `Assets/_Project/Data/Seeds/RunSeed.asset`
- **Date:** 2025-09-26
  - **Topic:** Hex Board Bootstrap
  - **Decision:** Adopt axial (q,r) coordinates with radius bounds and auto-bootstrapped occupants for sticky board merges.
  - **Rationale:** Axial math keeps neighbor checks simple; bootstrapping lets scenes wire placeholder units without runtime setup scripts.
  - **Impact:** Designers set `initialQ/initialR` on `HexCellOccupant`; TurnController enforces three merges; InputMergeService surfaces validity for UI.
  - **Rollback:** Swap to offset coords or manual placement by refactoring `HexBoardService` and occupant serialization; rewire scene references accordingly.
  - **Links:** `Assets/_Project/Scripts/Board/HexBoardService.cs`, `Assets/_Project/Scripts/Board/HexCellOccupant.cs`, `Assets/_Project/Scenes/Game.unity`

- **Date:** 2025-09-27
  - **Topic:** Rendering Cleanup
  - **Decision:** Removed URP package + 2D lights; locked project to Built-in 2D renderer with orthographic main camera.
  - **Rationale:** Align with MVP constraint to avoid SRP overhead and ensure deterministic lighting between platforms.
  - **Impact:** `Packages/manifest.json`, `ProjectSettings/GraphicsSettings.asset`, and `Game.unity` updated; URP assets excluded going forward.
  - **Rollback:** Reinstall URP package, restore SRP asset references, recreate Global Light 2D in scene.
  - **Links:** `Packages/manifest.json`, `ProjectSettings/GraphicsSettings.asset`, `Assets/_Project/Scenes/Game.unity`

- **Date:** 2025-09-27
  - **Topic:** Fusion & Discovery Persistence
  - **Decision:** Introduced FusionService + DiscoveryLogService with JSON persistence keyed by recipeId per deterministic run seed.
  - **Rationale:** Guarantees reproducible fusion outputs and allows collection progress to persist between sessions without duplicates.
  - **Impact:** Added fusion/trait/cell ScriptableObjects, discovery UI prefab, and tests; Game scene wires services for merges/combat handoff.
  - **Rollback:** Remove recipe assets from FusionService list, delete `DiscoveryLog.json`, and disable DiscoveryPanel prefab.
  - **Links:** `Assets/_Project/Scripts/Fusion/FusionService.cs`, `Assets/_Project/Scripts/Discovery/DiscoveryLogService.cs`, `Assets/_Project/UI/Discovery/DiscoveryPanel.prefab`, `Assets/_Project/Scenes/Game.unity`
