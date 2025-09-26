# TDD (Unity 6)

## ScriptableObjects (schemas)
- **CellType.asset** → `id(string)`, `family(enum: Red/Blue/Purple/…)`, `tier(int)`, `baseStats{hp,atk,spd,range,armorPen}`, `allowedTraits(List<TraitId>)`, `sprite(SpriteRef)`, `mergeTags(HashSet<string>)`.
- **Trait.asset** → `id`, `keywords`, `stackingRules`, `hooks{OnHit,OnDamageTaken,OnTurnStart}`.
- **FusionRecipe.asset** → `inputATags`, `inputBTags`, `outputCell(CellTypeRef)`, `discoveryFlag`, `rarity(weight)`.
- **EnemyType.asset** → `id`, `stats`, `behavior(enum:Chase/Ranged)`, `spawnWeight`.
- **WaveConfig.asset** → `entries[List{enemyType, boardHex, turnOffset}]`, `scalingRules`.
- **MapNode.asset** → `nodeType(enum:Battle/Elite/Event)`, `weights`, `outcomes`.
- **SpawnTable.asset** → `tier1Weights(dict{family:weight})`, `pityRules`, `antiDupRules`.

## Systems & APIs (class outlines)
- `HexBoardService`: place/validate/adjacency; grid axial coords; `TryPlace(CellType, q,r)`.
- `InputMergeService`: drag interactions, `TryMerge(from,to)`.
- `FusionService`: `Resolve(Cell a, Cell b)` -> Cell with recipe lookup & trait combine.
- `TurnController`: enforces 3 merges; states: Spawn/Merge/Combat/Cleanup/Map.
- `CombatResolver`: tick loop; initiative by SPD; target selection; damage application; trait hooks.
- `SpawnDirector`: seeded RNG; spawn Tier1s; pity/anti-dup.
- `WaveDirector`: schedule enemies per wave; elites.
- `DiscoveryLogService`: session log + meta persistence.
- `SaveService`: run+meta unlocks (JSON in `Application.persistentDataPath`).
- `MapController`: branching map nodes; choice UI.
- `RNGService`: seeded `System.Random` facade; categories (spawn, recipe, loot).

## Scenes/Prefabs
- **Scenes/Game.unity:** `HexBoardRoot`, `Unit.prefab`, `Enemy.prefab`, `VFX_Merge.prefab`, `UI_HUD.prefab`.
- **Prefab paths:** `Assets/_Project/Prefabs/{Units,Enemies,VFX,UI}`.

## Performance budgets
- 1080p/60 on mid-range laptop; GC < 1KB/frame during combat; avoid LINQ in hot loops.

## Plugins Manifest (optional)
- **2D Tilemap Extras** — Version: latest compatible; License: MIT; Source: Unity Open-Source; Purpose: hex helpers; Setup: import package; API Facade: `Company.Game.Hex.HexHelpers`; Risks: API drift; Removal: copy needed code to local.
- **DOTween (free)** — Version: latest; License: MIT; Purpose: UI polish; Setup: install, setup; Facade: `Company.Game.Tweener`; Risks: overuse; Removal: replace via facade no-op.

---
**Files/Paths:** `Assets/_Project/Docs/TDD.md` (+ stub classes later under `Assets/_Project/Scripts/*`)

**You Did:** Wrote schemas & class outlines.  
**Expected:** Coding Agent can scaffold ScriptableObjects & services.  
**Quick Test:** Create one `CellType.asset`; serialize; no null refs.  
**If Broken:** Add `[CreateAssetMenu]`; fix namespaces.  
**Log Note:** Determinism via `RNGService`; opt-in plugins wrapped.
