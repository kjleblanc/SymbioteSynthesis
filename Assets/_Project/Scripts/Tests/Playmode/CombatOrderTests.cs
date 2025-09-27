using System;
using System.Collections;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Combat;
using Company.Game.Fusion;
using Company.Game.Waves;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Company.Game.Tests.Playmode
{
    public sealed class CombatOrderTests
    {
        private GameObject systems;
        private HexBoardService boardService;
        private CombatResolver combatResolver;
        private WaveDirector waveDirector;
        private List<GameObject> spawnedPieces;

        private CellTypeDefinition redTier1;
        private CellTypeDefinition blueTier1;
        private WaveConfig waveBasic;
        private WaveConfig waveArmoredSolo;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            systems = new GameObject("CombatSystems");
            spawnedPieces = new List<GameObject>();

            boardService = systems.AddComponent<HexBoardService>();
            combatResolver = systems.AddComponent<CombatResolver>();
            waveDirector = systems.AddComponent<WaveDirector>();
            combatResolver.Configure(boardService, waveDirector);

            LoadAssets();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (spawnedPieces != null)
            {
                for (int i = 0; i < spawnedPieces.Count; i++)
                {
                    if (spawnedPieces[i] != null)
                    {
                        UnityEngine.Object.DestroyImmediate(spawnedPieces[i]);
                    }
                }

                spawnedPieces.Clear();
            }

            if (systems != null)
            {
                UnityEngine.Object.DestroyImmediate(systems);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator FastPlayerActsFirst()
        {
            boardService.ClearBoard();
            waveDirector.SetWaves(new[] { waveBasic });

            SpawnOccupant(redTier1, 0, 0);
            SpawnOccupant(blueTier1, 1, 0);

            CombatResolver.CombatResult result = combatResolver.ResolveCurrentWave();
            Assert.IsTrue(result.Events.Count > 0, "Combat should emit at least one event.");
            Assert.AreEqual("Player_0_0", result.Events[0].AttackerId, "Fastest unit should strike first.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator RendingPiercesArmored()
        {
            boardService.ClearBoard();
            waveDirector.SetWaves(new[] { waveArmoredSolo });

            SpawnOccupant(redTier1, 0, 0);

            CombatResolver.CombatResult result = combatResolver.ResolveCurrentWave();
            Assert.IsTrue(result.Events.Count > 0, "Combat should emit events against armored enemy.");
            CombatResolver.CombatEvent firstEvent = result.Events[0];
            Assert.AreEqual("Enemy_enemy_armored_0", firstEvent.DefenderId, "Attack should target armored enemy.");
            Assert.AreEqual(redTier1.Attack, firstEvent.Damage, "Rending should negate armored reduction.");
            yield return null;
        }

        private void SpawnOccupant(CellTypeDefinition type, int q, int r)
        {
            GameObject go = new GameObject($"Unit_{q}_{r}");
            spawnedPieces.Add(go);
            HexCellOccupant occupant = go.AddComponent<HexCellOccupant>();
            occupant.Initialize(type);
            HexBoardService.AxialCoord coord = new HexBoardService.AxialCoord(q, r);
            bool placed = boardService.TryPlace(occupant, coord);
            Assert.IsTrue(placed, $"Failed to place occupant at {coord}.");
        }

        private void LoadAssets()
        {
            redTier1 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Red_Claw_T1.asset");
            blueTier1 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Blue_Carapace_T1.asset");
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
            throw new InvalidOperationException("Assets can only be loaded in editor playmode tests.");
#endif
        }
    }
}
