using System;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Discovery;
using Company.Game.Fusion;
using Company.Game.RNG;
using UnityEngine;

namespace Company.Game.Spawn
{
    [DisallowMultipleComponent]
    public sealed class SpawnDirector : MonoBehaviour
    {
        [SerializeField]
        private HexBoardService boardService;

        [SerializeField]
        private DiscoveryLogService discoveryLogService;

        [SerializeField]
        private RNGService rngService;

        [SerializeField]
        private SpawnTable spawnTable;

        private readonly List<HexBoardService.AxialCoord> _allCoords = new();
        private readonly List<HexBoardService.AxialCoord> _freeCoords = new();
        private readonly List<SpawnTable.FamilyEntry> _eligibleEntries = new();
        private readonly List<int> _weightBuffer = new();
        private readonly Dictionary<CellTypeDefinition, int> _activeCounts = new();
        private readonly System.Random _fallbackRandom = new System.Random(1);

        private bool _coordinatesCached;
        private bool _runStarted;
        private bool _discoverySinceLastSpawn = true;
        private int _turnsWithoutDiscovery;
        private int _lastPersistentDiscoveryCount;

        private void OnEnable()
        {
            SubscribeDiscoveryLog();
            InvalidateCoordinateCache();
        }

        private void Start()
        {
            if (!_runStarted)
            {
                BeginRun();
            }
        }

        private void OnDisable()
        {
            UnsubscribeDiscoveryLog();
        }

        public void Configure(HexBoardService board, RNGService rng, SpawnTable table, DiscoveryLogService discovery)
        {
            if (boardService != board)
            {
                boardService = board;
                InvalidateCoordinateCache();
            }

            rngService = rng;
            spawnTable = table;

            if (discoveryLogService != discovery)
            {
                UnsubscribeDiscoveryLog();
                discoveryLogService = discovery;
                SubscribeDiscoveryLog();
            }
        }

        public void BeginRun()
        {
            if (boardService == null || spawnTable == null)
            {
                Debug.LogWarning("SpawnDirector missing dependencies for run start.");
                return;
            }

            InvalidateCoordinateCache();
            CacheBoardCoordinates();

            if (spawnTable.ClearBoardOnRunStart)
            {
                boardService.ClearBoard();
            }

            _runStarted = true;
            _turnsWithoutDiscovery = 0;
            _discoverySinceLastSpawn = true;
            _lastPersistentDiscoveryCount = GetPersistentDiscoveryCount();

            SpawnBatch(spawnTable.StartOfRunSpawnCount, false);
        }

        public void HandleWaveAdvanced()
        {
            if (!_runStarted || spawnTable == null)
            {
                return;
            }

            if (_discoverySinceLastSpawn)
            {
                _turnsWithoutDiscovery = 0;
            }
            else
            {
                _turnsWithoutDiscovery++;
            }

            bool requireUnlogged = spawnTable.PityTurnThreshold > 0 &&
                                   _turnsWithoutDiscovery >= spawnTable.PityTurnThreshold;

            SpawnBatch(spawnTable.SpawnsPerWave, requireUnlogged);
        }

        private void SpawnBatch(int desiredCount, bool requireUnlogged)
        {
            if (boardService == null || spawnTable == null || desiredCount <= 0)
            {
                return;
            }

            CacheBoardCoordinates();
            BuildFreeCoordinateList();

            if (_freeCoords.Count == 0)
            {
                return;
            }

            RebuildActiveCounts();

            System.Random random = rngService != null ? rngService.GetRandom(RngCategory.Spawn) : _fallbackRandom;
            int spawned = 0;

            while (spawned < desiredCount && _freeCoords.Count > 0)
            {
                SpawnTable.FamilyEntry entry = SelectEntry(random, requireUnlogged);
                CellTypeDefinition cellType = entry.CellType;
                if (cellType == null)
                {
                    break;
                }

                HexBoardService.AxialCoord coord = TakeRandomCoordinate(random);
                HexCellOccupant occupant = CreateOccupant(cellType);
                if (!boardService.TryPlace(occupant, coord))
                {
                    DestroyOccupant(occupant);
                    break;
                }

                IncrementCount(cellType);
                spawned++;
            }

            if (spawned > 0)
            {
                _discoverySinceLastSpawn = false;
            }
        }

        private SpawnTable.FamilyEntry SelectEntry(System.Random random, bool requireUnlogged)
        {
            IReadOnlyList<SpawnTable.FamilyEntry> entries = spawnTable.FamilyEntries;
            _eligibleEntries.Clear();
            _weightBuffer.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                SpawnTable.FamilyEntry entry = entries[i];
                CellTypeDefinition type = entry.CellType;
                if (type == null || type.Tier != 1)
                {
                    continue;
                }

                int limit = spawnTable.AntiDuplicateLimit;
                if (limit > 0 && GetActiveCount(type) >= limit)
                {
                    continue;
                }

                bool pityEligible = IsEntryPityEligible(entry);
                if (requireUnlogged && !pityEligible)
                {
                    continue;
                }

                int weight = entry.Weight;
                if (weight <= 0)
                {
                    continue;
                }

                _eligibleEntries.Add(entry);
                _weightBuffer.Add(weight);
            }

            if (_eligibleEntries.Count == 0)
            {
                if (requireUnlogged)
                {
                    return SelectEntry(random, false);
                }

                return default;
            }

            int totalWeight = 0;
            for (int i = 0; i < _weightBuffer.Count; i++)
            {
                totalWeight += _weightBuffer[i];
            }

            if (totalWeight <= 0)
            {
                return default;
            }

            int roll = random.Next(totalWeight);
            for (int i = 0; i < _weightBuffer.Count; i++)
            {
                int weight = _weightBuffer[i];
                if (roll < weight)
                {
                    return _eligibleEntries[i];
                }

                roll -= weight;
            }

            return _eligibleEntries[_eligibleEntries.Count - 1];
        }

        private void RebuildActiveCounts()
        {
            _activeCounts.Clear();
            if (boardService == null)
            {
                return;
            }

            foreach (KeyValuePair<HexBoardService.AxialCoord, HexCellOccupant> pair in boardService.Occupants)
            {
                HexCellOccupant occupant = pair.Value;
                if (occupant?.CellType == null)
                {
                    continue;
                }

                if (_activeCounts.TryGetValue(occupant.CellType, out int count))
                {
                    _activeCounts[occupant.CellType] = count + 1;
                }
                else
                {
                    _activeCounts[occupant.CellType] = 1;
                }
            }
        }

        private int GetActiveCount(CellTypeDefinition type)
        {
            if (type == null)
            {
                return 0;
            }

            return _activeCounts.TryGetValue(type, out int count) ? count : 0;
        }

        private void IncrementCount(CellTypeDefinition type)
        {
            if (type == null)
            {
                return;
            }

            if (_activeCounts.TryGetValue(type, out int count))
            {
                _activeCounts[type] = count + 1;
            }
            else
            {
                _activeCounts[type] = 1;
            }
        }

        private bool IsEntryPityEligible(SpawnTable.FamilyEntry entry)
        {
            IReadOnlyList<string> recipes = entry.PityRecipeIds;
            if (recipes == null || recipes.Count == 0)
            {
                return false;
            }

            if (discoveryLogService == null)
            {
                return true;
            }

            for (int i = 0; i < recipes.Count; i++)
            {
                string recipeId = recipes[i];
                if (string.IsNullOrEmpty(recipeId))
                {
                    continue;
                }

                if (!IsRecipeDiscovered(recipeId))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRecipeDiscovered(string recipeId)
        {
            if (discoveryLogService == null || string.IsNullOrEmpty(recipeId))
            {
                return false;
            }

            return Contains(discoveryLogService.PersistentDiscoveries, recipeId) ||
                   Contains(discoveryLogService.RunDiscoveries, recipeId);
        }

        private static bool Contains(IReadOnlyCollection<string> collection, string value)
        {
            if (collection == null)
            {
                return false;
            }

            foreach (string entry in collection)
            {
                if (string.Equals(entry, value, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private void CacheBoardCoordinates()
        {
            if (_coordinatesCached || boardService == null)
            {
                return;
            }

            _allCoords.Clear();
            int radius = Mathf.Max(1, boardService.Radius);
            for (int q = -radius; q <= radius; q++)
            {
                int rMin = Mathf.Max(-radius, -q - radius);
                int rMax = Mathf.Min(radius, -q + radius);
                for (int r = rMin; r <= rMax; r++)
                {
                    _allCoords.Add(new HexBoardService.AxialCoord(q, r));
                }
            }

            _coordinatesCached = true;
        }

        private void InvalidateCoordinateCache()
        {
            _coordinatesCached = false;
        }

        private void BuildFreeCoordinateList()
        {
            _freeCoords.Clear();
            if (boardService == null)
            {
                return;
            }

            for (int i = 0; i < _allCoords.Count; i++)
            {
                HexBoardService.AxialCoord coord = _allCoords[i];
                if (!boardService.TryGetOccupant(coord, out HexCellOccupant occupant) || occupant == null)
                {
                    _freeCoords.Add(coord);
                }
            }
        }

        private HexBoardService.AxialCoord TakeRandomCoordinate(System.Random random)
        {
            int index = random.Next(_freeCoords.Count);
            HexBoardService.AxialCoord coord = _freeCoords[index];
            int last = _freeCoords.Count - 1;
            _freeCoords[index] = _freeCoords[last];
            _freeCoords.RemoveAt(last);
            return coord;
        }

        private HexCellOccupant CreateOccupant(CellTypeDefinition type)
        {
            GameObject go = new GameObject(type != null ? $"Cell_{type.CellId}" : "Cell_Tier1");
            if (boardService != null)
            {
                go.transform.SetParent(boardService.transform, false);
            }

            HexCellOccupant occupant = go.AddComponent<HexCellOccupant>();
            occupant.Initialize(type);
            go.SetActive(true);
            return occupant;
        }

        private static void DestroyOccupant(HexCellOccupant occupant)
        {
            if (occupant == null)
            {
                return;
            }

            GameObject go = occupant.gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(go);
                return;
            }
#endif
            UnityEngine.Object.Destroy(go);
        }

        private void SubscribeDiscoveryLog()
        {
            if (discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged += OnDiscoveryChanged;
                _lastPersistentDiscoveryCount = GetPersistentDiscoveryCount();
            }
        }

        private void UnsubscribeDiscoveryLog()
        {
            if (discoveryLogService != null)
            {
                discoveryLogService.DiscoveryChanged -= OnDiscoveryChanged;
            }
        }

        private void OnDiscoveryChanged(IReadOnlyCollection<string> persistent)
        {
            int newCount = persistent?.Count ?? GetPersistentDiscoveryCount();
            if (newCount > _lastPersistentDiscoveryCount)
            {
                _turnsWithoutDiscovery = 0;
                _discoverySinceLastSpawn = true;
            }

            _lastPersistentDiscoveryCount = newCount;
        }

        private int GetPersistentDiscoveryCount()
        {
            if (discoveryLogService?.PersistentDiscoveries == null)
            {
                return 0;
            }

            return discoveryLogService.PersistentDiscoveries.Count;
        }
    }
}
