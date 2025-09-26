# Orchestration Playbook — Initial Task Cards

## TC-001 — Hex Board & Drag-Merge Input
**Director Brief:** Implement a hex grid with sticky, non-falling cells. LMB drag A→B to attempt merge; enforce board bounds & feedback.

**Acceptance**
- Place/move cells; invalid merges blocked; highlight valid target; 3 merges tracked in UI.

**Dependencies:** None (bootstraps systems).

**Coding Agent Handoff**
- Files to create:
  - `Assets/_Project/Scripts/Board/HexBoardService.cs`
  - `Assets/_Project/Scripts/Input/InputMergeService.cs`
  - `Assets/_Project/Scripts/Game/TurnController.cs` (Merge state only)
  - `Assets/_Project/Scenes/Game.unity`
- Tests:
  - `Assets/_Project/Scripts/Tests/Playmode/TC001_MergeTests.cs`
- Classes: as in §7.

**Expected Result:** Drag on grid snaps; `TryMerge` invoked; merge count increments up to 3.

**Quick Test:** Spawn two Tier1 Red; drag to merge; ensure `count=1` and result appears.

**Rollback:** Disable `InputMergeService` prefab; revert scene prefab changes.

**Visuals Agent Prompt Packet**
- Board tile (flat lab hex): 128×128; seed 9301.
- Cell placeholders (T1/T2 silhouettes per family): seeds 9310–9315.
- Merge VFX: `vfx_merge_squelch.prefab` 0.18s; seed 9400.

**Human Integrator Steps**
- Menu: *GameObject → 2D Object → Hexagonal Tilemap (Point-Top).* Grid cell size 1,1.
- Import Settings: sprites PPU=128, Filter=Point, Compression=Normal.
- Drag `HexBoardRoot` prefab into `Game.unity`.
- Add `InputMergeService` to an empty Systems GO; wire refs to board & UI.

**Verification**
- **You Did:** Built to PC; merged 3 times; 4th blocked.
- **Expected:** No GC spikes on drag.
- **If Broken:** Check event system; input capture.
- **Log Note:** Determinism unaffected (no RNG yet).

---

## TC-002 — Fusion Core (2+2=3) + Discovery Tab
**Director Brief:** Implement `FusionService` with recipe lookup; on first new result, log to run Discovery and persist to meta Collection.

**Acceptance**
- Same-family T1+T1→T2; cross-family according to recipes; Discovery updates UI.

**Dependencies:** TC-001.

**Coding Agent Handoff**
- Files:
  - `Scripts/Fusion/FusionService.cs`
  - `Scripts/Discovery/DiscoveryLogService.cs`
  - `Data/Cells/*.asset`, `Data/Traits/*.asset`, `Data/Fusions/*.asset`
  - UI: `UI/Discovery/DiscoveryPanel.prefab`
- Tests:
  - `Tests/Playmode/FusionRecipeTests.cs`, `Tests/Editmode/DiscoveryTests.cs`

**Expected:** Merge fires VFX, spawns output, updates log.

**Quick Test:** Red+Red→Pincer; Red+Blue→Armored Claw (Purple).

**Rollback:** Disable recipe assets; restore backup of `DiscoveryLog.json`.

**Visuals**
- Trait icons: Rending, Armored (seeds 501–502).
- Fusion “pop” VFX seed 9450.
- Discovery UI widget frames seeds 9600–9602.

**Human Steps**
- Create ScriptableObjects via *Assets → Create → Game → CellType/Trait/FusionRecipe*.
- Place in `Assets/_Project/Data/{Cells,Traits,Fusions}`.
- Wire `DiscoveryPanel.prefab` into HUD; bind to `DiscoveryLogService`.

**Verification**
- **You Did:** 2 recipes validated; 1 cross-recipe logged.
- **Expected:** No duplicate log entries in a single run.
- **If Broken:** Check recipe tags; ensure HashSet for log.
- **Log Note:** Discovery deterministic per seed; persistence isolated.

---

## TC-003 — Combat Phase on Same Board + Enemy Waves
**Director Brief:** Add `CombatResolver` with tick/initiative; enemies spawn per `WaveConfig`; include basic traits (Rending/Armored).

**Acceptance**
- On end of merges, combat runs; units act by SPD; traits modify damage/mitigation.

**Dependencies:** TC-001, TC-002.

**Coding Agent Handoff**
- Files:
  - `Scripts/Combat/CombatResolver.cs`
  - `Scripts/Combat/Traits/Rending.cs`, `Armored.cs`
  - `Scripts/Waves/WaveDirector.cs`
  - `Data/Enemies/*.asset`, `Data/Waves/Wave_*.asset`
  - Prefabs: `Prefabs/Enemies/EnemyBasic.prefab`
- Tests:
  - `Tests/Playmode/CombatOrderTests.cs`, `WaveSpawnOrderTests.cs`

**Expected:** Units and enemies trade attacks; wave clear → Cleanup.

**Quick Test:** One unit kills one enemy within 5 sec at wave1.

**Rollback:** Disable `WaveDirector` in scene; revert `CombatResolver` binding.

**Visuals**
- Enemy silhouettes (seeds 700–705).
- Hit VFX seed 9500; damage numbers sprite font.

**Human Steps**
- Import enemy prefabs to `Prefabs/Enemies`.
- Add `WaveDirector` to Systems; assign `WaveConfig` assets.
- Link `CombatResolver` to board entities via serialized lists.

**Verification**
- **You Did:** Ran 3 waves; elites at 4/8 spawn positions correct.
- **Expected:** GC under 1KB/frame; profiler clean.
- **If Broken:** Check SPD tick; ensure pooling for damage numbers.
- **Log Note:** Trait hooks isolated; future traits won’t alter core loop.
