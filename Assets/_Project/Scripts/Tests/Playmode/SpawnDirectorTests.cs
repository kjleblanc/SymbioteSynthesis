using System.Collections;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Discovery;
using Company.Game.Fusion;
using Company.Game.RNG;
using Company.Game.Spawn;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Company.Game.Tests.Playmode
{
    public sealed class SpawnDirectorTests
    {
        private GameObject systems;
        private HexBoardService boardService;
        private DiscoveryLogService discoveryLogService;
        private RNGService rngService;
        private SpawnDirector spawnDirector;
        private SpawnTable spawnTable;
        private RunSeed runSeed;

        private CellTypeDefinition redTier1;
        private CellTypeDefinition blueTier1;
        private CellTypeDefinition purpleTier1;

        private string tempDirectory;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            systems = new GameObject("Systems");
            boardService = systems.AddComponent<HexBoardService>();
            discoveryLogService = systems.AddComponent<DiscoveryLogService>();
            rngService = systems.AddComponent<RNGService>();
            spawnDirector = systems.AddComponent<SpawnDirector>();

            runSeed = ScriptableObject.CreateInstance<RunSeed>();
            runSeed.value = 31415;
            rngService.ConfigureSeedAsset(runSeed);
            rngService.Reseed();

            tempDirectory = System.IO.Path.Combine(Application.temporaryCachePath, $"SpawnDirectorTests_{System.Guid.NewGuid():N}");
            discoveryLogService.ConfigureOverrideDirectory(tempDirectory);
            discoveryLogService.EnsureLoaded();

            spawnTable = ScriptableObject.CreateInstance<SpawnTable>();
            spawnDirector.Configure(boardService, rngService, spawnTable, discoveryLogService);

            LoadCells();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (!string.IsNullOrEmpty(tempDirectory) && System.IO.Directory.Exists(tempDirectory))
            {
                System.IO.Directory.Delete(tempDirectory, true);
            }

            if (spawnTable != null)
            {
                ScriptableObject.DestroyImmediate(spawnTable);
            }

            if (runSeed != null)
            {
                ScriptableObject.DestroyImmediate(runSeed);
            }

            if (systems != null)
            {
                Object.DestroyImmediate(systems);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Pity_EnsuresUnloggedRecipeAfterThreeDrySpawns()
        {
            spawnTable.SetStartOfRunSpawnCount(0);
            spawnTable.SetSpawnsPerWave(1);
            spawnTable.SetAntiDuplicateLimit(3);
            spawnTable.SetPityTurnThreshold(3);
            spawnTable.SetEntries(new[]
            {
                new SpawnTable.FamilyEntry("Red", redTier1, 6, new[] { "fusion_red_t1" }),
                new SpawnTable.FamilyEntry("Blue", blueTier1, 1, new[] { "fusion_blue_t1" }),
            });

            spawnDirector.BeginRun();
            yield return null;

            for (int i = 0; i < 3; i++)
            {
                spawnDirector.HandleWaveAdvanced();
                yield return null;
            }

            spawnDirector.HandleWaveAdvanced();
            yield return null;

            int blueCount = CountCells(blueTier1);
            Assert.GreaterOrEqual(blueCount, 1, "Pity rule should guarantee an unlogged recipe spawn.");
        }

        [UnityTest]
        public IEnumerator AntiDuplicate_CapsAtThreeCopiesPerTierOne()
        {
            spawnTable.SetStartOfRunSpawnCount(6);
            spawnTable.SetSpawnsPerWave(0);
            spawnTable.SetAntiDuplicateLimit(3);
            spawnTable.SetPityTurnThreshold(3);
            spawnTable.SetEntries(new[]
            {
                new SpawnTable.FamilyEntry("Red", redTier1, 5, new[] { "fusion_red_t1" }),
                new SpawnTable.FamilyEntry("Blue", blueTier1, 5, new[] { "fusion_blue_t1" }),
                new SpawnTable.FamilyEntry("Purple", purpleTier1, 2, new[] { "fusion_redblue" }),
            });

            spawnDirector.BeginRun();
            yield return null;

            int redCount = CountCells(redTier1);
            Assert.LessOrEqual(redCount, 3, "No more than three red Tier1 cells should be active.");
        }

        private int CountCells(CellTypeDefinition type)
        {
            int count = 0;
            foreach (KeyValuePair<HexBoardService.AxialCoord, HexCellOccupant> pair in boardService.Occupants)
            {
                HexCellOccupant occupant = pair.Value;
                if (occupant != null && occupant.CellType == type)
                {
                    count++;
                }
            }

            return count;
        }

        private void LoadCells()
        {
#if UNITY_EDITOR
            redTier1 = AssetDatabase.LoadAssetAtPath<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Red_Claw_T1.asset");
            blueTier1 = AssetDatabase.LoadAssetAtPath<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Blue_Carapace_T1.asset");
            purpleTier1 = AssetDatabase.LoadAssetAtPath<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Purple_ArmoredClaw_T1.asset");
#endif
            Assert.NotNull(redTier1, "Red Tier1 cell is required for spawn tests.");
            Assert.NotNull(blueTier1, "Blue Tier1 cell is required for spawn tests.");
            Assert.NotNull(purpleTier1, "Purple Tier1 cell is required for spawn tests.");
        }
    }
}
