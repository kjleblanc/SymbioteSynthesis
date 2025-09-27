using System;
using Company.Game.Waves;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Company.Game.Tests.Playmode
{
    public sealed class WaveSpawnOrderTests
    {
        private GameObject systems;
        private WaveDirector waveDirector;
        private WaveConfig waveBasic;
        private WaveConfig waveArmoredSolo;

        [SetUp]
        public void SetUp()
        {
            systems = new GameObject("WaveSystems");
            waveDirector = systems.AddComponent<WaveDirector>();
            LoadAssets();
        }

        [TearDown]
        public void TearDown()
        {
            if (systems != null)
            {
                UnityEngine.Object.DestroyImmediate(systems);
            }
        }

        [Test]
        public void PrepareWave_SortsByOrder()
        {
            waveDirector.SetWaves(new[] { waveBasic });
            var entries = waveDirector.PrepareWave();
            Assert.AreEqual(2, entries.Count, "Wave should provide two entries.");
            Assert.LessOrEqual(entries[0].SpawnOrder, entries[1].SpawnOrder, "Entries should be sorted by spawn order.");
        }

        [Test]
        public void AdvanceWave_MovesToNextConfig()
        {
            waveDirector.SetWaves(new[] { waveBasic, waveArmoredSolo });
            var firstEntries = waveDirector.PrepareWave();
            Assert.AreEqual("wave_basic", waveDirector.CurrentWave.WaveId);

            bool advanced = waveDirector.AdvanceWave();
            Assert.IsTrue(advanced, "Expected to advance to second wave.");
            var secondEntries = waveDirector.PrepareWave();
            Assert.AreEqual("wave_armored_solo", waveDirector.CurrentWave.WaveId);
            Assert.AreEqual(1, secondEntries.Count, "Second wave should contain single spawn.");
        }

        private void LoadAssets()
        {
            waveBasic = LoadAsset<WaveConfig>("Assets/_Project/Data/Waves/Wave_Basic.asset");
            waveArmoredSolo = LoadAsset<WaveConfig>("Assets/_Project/Data/Waves/Wave_ArmoredSolo.asset");
        }

        private static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            Assert.IsNotNull(asset, $"Missing asset at {path}");
            return asset;
#else
            throw new InvalidOperationException("Assets can only be loaded in editor tests.");
#endif
        }
    }
}
