# Testing & Telemetry Plan

## Smoke tests per scene
- Boot loads menu; MainMenu start run; Game 1 merge + 1 combat; Results shows summary.

## Automated Play-Mode Tests
- Fusion recipes: same-family + cross-family.
- 3-move rule: cannot exceed 3 merges.
- Wave spawn order: Wave1 then Wave2 timed by `WaveDirector`.

## Analytics (optional, local MVP)
- Events (JSON append): `run_start`, `run_end`, `turn_start`, `turn_end`, `merge_performed{inA,inB,out}`, `discovery_unlocked{id}`, `wave_cleared{n}`, `death_reason`.
- File: `persistentDataPath/Logs/telemetry.jsonl`.

## Crash Repro Checklist
- Repro steps; seed; board snapshot (serialize axial coords & units).
- Attach Editor log; profiler capture 300 frames.

## Profiling Notes
- Sample hot paths: `CombatResolver.Tick`, `FusionService.Resolve`.
- No allocations in per-frame loops; cache `List<>`.

---
**Paths/Files:** `Assets/_Project/Docs/TestPlan.md`

**You Did:** Wrote tests & events.  
**Expected:** CI or local runner passes.  
**Quick Test:** Run EditMode & PlayMode suites; expect green.  
**If Broken:** Bisect failing test; attach seed.  
**Log Note:** Determinism captured via seed + board snapshot.
