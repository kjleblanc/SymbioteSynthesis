using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Company.Game.Board;
using Company.Game.Discovery;
using Company.Game.Fusion;
using Company.Game.Gameplay;
using Company.Game.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Company.Game.Tests.Playmode
{
    public sealed class FusionRecipeTests
    {
        private GameObject systems;
        private HexBoardService boardService;
        private FusionService fusionService;
        private DiscoveryLogService discoveryLogService;
        private InputMergeService inputService;
        private TurnController turnController;
        private List<GameObject> spawnedPieces;
        private string tempDirectory;

        private CellTypeDefinition redTier1;
        private CellTypeDefinition redTier2;
        private CellTypeDefinition blueTier1;
        private CellTypeDefinition purpleTier2;
        private FusionRecipeDefinition redRecipe;
        private FusionRecipeDefinition blueRecipe;
        private FusionRecipeDefinition crossRecipe;

        private HexBoardService.HexMergeResult lastResult;
        private Action<HexBoardService.HexMergeResult> captureHandler;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            systems = new GameObject("Systems");
            spawnedPieces = new List<GameObject>();

            boardService = systems.AddComponent<HexBoardService>();
            fusionService = systems.AddComponent<FusionService>();
            discoveryLogService = systems.AddComponent<DiscoveryLogService>();
            inputService = systems.AddComponent<InputMergeService>();
            turnController = systems.AddComponent<TurnController>();

            // Wire references that use serialized fields.
            fusionService.ConfigureDiscoveryLog(discoveryLogService);
            boardService.ConfigureFusionService(fusionService);
            inputService.Configure(boardService, turnController, fusionService);

            typeof(TurnController).GetField("discoveryLogService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(turnController, discoveryLogService);

            LoadAssets();
            fusionService.SetRecipes(new[] { redRecipe, blueRecipe, crossRecipe });

            tempDirectory = Path.Combine(Application.temporaryCachePath, $"FusionTests_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDirectory);
            discoveryLogService.ConfigureOverrideDirectory(tempDirectory);
            discoveryLogService.EnsureLoaded();

            captureHandler = result => lastResult = result;
            inputService.MergeSucceeded += captureHandler;

            turnController.BeginTurn();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (captureHandler != null)
            {
                inputService.MergeSucceeded -= captureHandler;
                captureHandler = null;
            }

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

            if (!string.IsNullOrEmpty(tempDirectory) && Directory.Exists(tempDirectory))
            {
                try
                {
                    Directory.Delete(tempDirectory, true);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator SameFamilyFusion_LogsOnce()
        {
            HexBoardService.AxialCoord from = SpawnOccupant(redTier1, 0, 0);
            HexBoardService.AxialCoord to = SpawnOccupant(redTier1, 1, 0);

            bool merged = inputService.TryMerge(from, to);
            Assert.IsTrue(merged, "Expected red tier1 merge to succeed.");

            Assert.NotNull(lastResult.Target, "Merge should populate target occupant.");
            Assert.AreEqual(redTier2, lastResult.Resolution.OutputType, "Red fusion should upgrade to tier 2.");
            Assert.AreEqual(1, discoveryLogService.PersistentDiscoveries.Count, "Discovery should log once.");

            // Merge again to confirm no duplicate entries.
            boardService.ClearBoard();
            HexBoardService.AxialCoord from2 = SpawnOccupant(redTier1, 0, 0);
            HexBoardService.AxialCoord to2 = SpawnOccupant(redTier1, 1, 0);
            inputService.TryMerge(from2, to2);
            Assert.AreEqual(1, discoveryLogService.PersistentDiscoveries.Count, "Duplicate recipe should not increment log.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator CrossFamilyFusion_YieldsPurple()
        {
            boardService.ClearBoard();
            HexBoardService.AxialCoord from = SpawnOccupant(redTier1, 0, 0);
            HexBoardService.AxialCoord to = SpawnOccupant(blueTier1, 1, 0);

            bool merged = inputService.TryMerge(from, to);
            Assert.IsTrue(merged, "Expected cross-family merge to succeed.");

            Assert.NotNull(lastResult.Resolution.Recipe, "Cross recipe should be registered.");
            Assert.AreEqual(crossRecipe, lastResult.Resolution.Recipe, "Cross recipe must match asset.");
            Assert.AreEqual(purpleTier2, lastResult.Resolution.OutputType, "Output should be the purple armored claw.");
            Assert.IsTrue(discoveryLogService.PersistentDiscoveries.Contains(crossRecipe.RecipeId), "Cross recipe should be persisted.");
            yield return null;
        }

        private HexBoardService.AxialCoord SpawnOccupant(CellTypeDefinition type, int q, int r)
        {
            GameObject go = new GameObject($"Occupant_{q}_{r}");
            spawnedPieces.Add(go);
            HexCellOccupant occupant = go.AddComponent<HexCellOccupant>();
            occupant.Initialize(type);
            HexBoardService.AxialCoord coord = new HexBoardService.AxialCoord(q, r);
            bool placed = boardService.TryPlace(occupant, coord);
            Assert.IsTrue(placed, $"Failed to place occupant at {coord}.");
            return coord;
        }

        private void LoadAssets()
        {
            redTier1 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Red_Claw_T1.asset");
            redTier2 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Red_Claw_T2.asset");
            blueTier1 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Blue_Carapace_T1.asset");
            purpleTier2 = LoadAsset<CellTypeDefinition>("Assets/_Project/Data/Cells/Cell_Purple_ArmoredClaw_T2.asset");
            redRecipe = LoadAsset<FusionRecipeDefinition>("Assets/_Project/Data/Fusions/Fusion_Red_T1.asset");
            blueRecipe = LoadAsset<FusionRecipeDefinition>("Assets/_Project/Data/Fusions/Fusion_Blue_T1.asset");
            crossRecipe = LoadAsset<FusionRecipeDefinition>("Assets/_Project/Data/Fusions/Fusion_RedBlue.asset");
        }

        private static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            Assert.IsNotNull(asset, $"Failed to load asset at {path}.");
            return asset;
#else
            throw new InvalidOperationException("Assets can only be loaded in editor test mode.");
#endif
        }
    }
}
