using System.Collections.Generic;
using Company.Game.Combat;
using Company.Game.Spawn;
using UnityEngine;

namespace Company.Game.Waves
{
    [DisallowMultipleComponent]
    public sealed class WaveDirector : MonoBehaviour
    {
        [SerializeField]
        private List<WaveConfig> waves = new();

        [SerializeField]
        private SpawnDirector spawnDirector;

        private int currentWaveIndex;

        public WaveConfig CurrentWave => waves.Count == 0 ? null : waves[Mathf.Clamp(currentWaveIndex, 0, waves.Count - 1)];

        public IReadOnlyList<WaveConfig.SpawnEntry> PrepareWave()
        {
            if (CurrentWave == null)
            {
                return System.Array.Empty<WaveConfig.SpawnEntry>();
            }

            return CurrentWave.GetOrderedEntries();
        }

        public bool AdvanceWave()
        {
            if (currentWaveIndex + 1 < waves.Count)
            {
                currentWaveIndex++;
                spawnDirector?.HandleWaveAdvanced();
                return true;
            }

            return false;
        }

        public void ResetWaves()
        {
            currentWaveIndex = 0;
            spawnDirector?.BeginRun();
        }

        public void SetWaves(IEnumerable<WaveConfig> configs)
        {
            waves.Clear();
            if (configs == null)
            {
                return;
            }

            foreach (WaveConfig config in configs)
            {
                if (config != null)
                {
                    waves.Add(config);
                }
            }

            currentWaveIndex = 0;
            spawnDirector?.BeginRun();
        }

        public void ConfigureSpawnDirector(SpawnDirector director)
        {
            spawnDirector = director;
        }
    }
}
