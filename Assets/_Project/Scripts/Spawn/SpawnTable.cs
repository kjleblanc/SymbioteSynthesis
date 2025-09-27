using System;
using System.Collections.Generic;
using Company.Game.Fusion;
using UnityEngine;

namespace Company.Game.Spawn
{
    [CreateAssetMenu(menuName = "Game/Spawn/Spawn Table", fileName = "SpawnTable")]
    public sealed class SpawnTable : ScriptableObject
    {
        [SerializeField]
        private List<FamilyEntry> familyEntries = new();

        [SerializeField]
        [Min(0)]
        private int startOfRunSpawnCount = 3;

        [SerializeField]
        [Min(0)]
        private int spawnsPerWave = 1;

        [SerializeField]
        [Min(1)]
        private int pityTurnThreshold = 3;

        [SerializeField]
        private bool clearBoardOnRunStart = true;

        [SerializeField]
        [Min(1)]
        private int antiDuplicateLimit = 3;

        public IReadOnlyList<FamilyEntry> FamilyEntries => familyEntries;
        public int StartOfRunSpawnCount => startOfRunSpawnCount;
        public int SpawnsPerWave => spawnsPerWave;
        public int PityTurnThreshold => pityTurnThreshold;
        public bool ClearBoardOnRunStart => clearBoardOnRunStart;
        public int AntiDuplicateLimit => antiDuplicateLimit;

        public void SetStartOfRunSpawnCount(int value)
        {
            startOfRunSpawnCount = Mathf.Max(0, value);
        }

        public void SetSpawnsPerWave(int value)
        {
            spawnsPerWave = Mathf.Max(0, value);
        }

        public void SetPityTurnThreshold(int value)
        {
            pityTurnThreshold = Mathf.Max(1, value);
        }

        public void SetAntiDuplicateLimit(int value)
        {
            antiDuplicateLimit = Mathf.Max(1, value);
        }

        public void SetClearBoardOnRunStart(bool value)
        {
            clearBoardOnRunStart = value;
        }

        public void SetEntries(IEnumerable<FamilyEntry> entries)
        {
            familyEntries.Clear();
            if (entries == null)
            {
                return;
            }

            foreach (FamilyEntry entry in entries)
            {
                familyEntries.Add(entry);
            }
        }

        [Serializable]
        public struct FamilyEntry
        {
            [SerializeField]
            private string familyId;

            [SerializeField]
            private CellTypeDefinition cellType;

            [SerializeField]
            [Min(0)]
            private int weight;

            [SerializeField]
            private string[] pityRecipeIds;

            public FamilyEntry(string familyId, CellTypeDefinition cellType, int weight, IEnumerable<string> pityRecipes)
            {
                this.familyId = string.IsNullOrEmpty(familyId) && cellType != null ? cellType.Family : familyId;
                this.cellType = cellType;
                this.weight = Mathf.Max(0, weight);
                if (pityRecipes != null)
                {
                    List<string> list = new List<string>();
                    foreach (string entry in pityRecipes)
                    {
                        if (!string.IsNullOrEmpty(entry))
                        {
                            list.Add(entry);
                        }
                    }

                    pityRecipeIds = list.ToArray();
                }
                else
                {
                    pityRecipeIds = Array.Empty<string>();
                }
            }

            public string FamilyId => string.IsNullOrEmpty(familyId) && cellType != null ? cellType.Family : familyId;
            public CellTypeDefinition CellType => cellType;
            public int Weight => Mathf.Max(0, weight);
            public IReadOnlyList<string> PityRecipeIds => pityRecipeIds ?? Array.Empty<string>();
        }
    }
}
