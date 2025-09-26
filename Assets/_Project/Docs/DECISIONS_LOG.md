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
