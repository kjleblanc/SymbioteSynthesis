using System;
using System.Collections.Generic;
using Company.Game.Combat;
using UnityEngine;

namespace Company.Game.Waves
{
    [CreateAssetMenu(menuName = "Game/Waves/Wave Config", fileName = "WaveConfig")]
    public sealed class WaveConfig : ScriptableObject
    {
        [SerializeField]
        private string waveId = "wave";

        [SerializeField]
        private List<SpawnEntry> enemies = new();

        public string WaveId => waveId;
        public IReadOnlyList<SpawnEntry> Enemies => enemies;

        public IReadOnlyList<SpawnEntry> GetOrderedEntries()
        {
            enemies.Sort((a, b) =>
            {
                int orderCompare = a.spawnOrder.CompareTo(b.spawnOrder);
                if (orderCompare != 0)
                {
                    return orderCompare;
                }

                return string.CompareOrdinal(a.enemy != null ? a.enemy.EnemyId : string.Empty,
                    b.enemy != null ? b.enemy.EnemyId : string.Empty);
            });

            return enemies;
        }

        [Serializable]
        public sealed class SpawnEntry
        {
            [SerializeField]
            internal EnemyDefinition enemy;

            [SerializeField]
            internal int spawnOrder;

            [SerializeField]
            internal int q;

            [SerializeField]
            internal int r;

            public EnemyDefinition Enemy => enemy;
            public int SpawnOrder => spawnOrder;
            public int Q => q;
            public int R => r;
        }
    }
}
