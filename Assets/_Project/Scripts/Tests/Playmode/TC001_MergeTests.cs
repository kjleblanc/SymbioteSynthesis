using System.Collections;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Gameplay;
using Company.Game.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Company.Game.Tests.Playmode
{
    public sealed class TC001_MergeTests
    {
        private GameObject systems;
        private HexBoardService boardService;
        private TurnController turnController;
        private InputMergeService inputService;
        private List<GameObject> spawnedPieces;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            spawnedPieces = new List<GameObject>();
            systems = new GameObject("Systems");
            boardService = systems.AddComponent<HexBoardService>();
            turnController = systems.AddComponent<TurnController>();
            inputService = systems.AddComponent<InputMergeService>();
            inputService.Configure(boardService, turnController);
            turnController.BeginTurn();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (spawnedPieces != null)
            {
                for (int i = 0; i < spawnedPieces.Count; i++)
                {
                    GameObject piece = spawnedPieces[i];
                    if (piece != null)
                    {
                        Object.DestroyImmediate(piece);
                    }
                }

                spawnedPieces.Clear();
            }

            if (systems != null)
            {
                Object.DestroyImmediate(systems);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator DragMerge_IncrementsCount()
        {
            HexBoardService.AxialCoord from = SpawnOccupant(0, 0);
            HexBoardService.AxialCoord to = SpawnOccupant(1, 0);

            bool success = inputService.TryMerge(from, to);

            Assert.IsTrue(success, "Expected merge to succeed.");
            Assert.AreEqual(1, turnController.MergesThisTurn, "Merge counter should increment.");
            Assert.IsTrue(inputService.IsLastTargetValid, "Valid merge should report true.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator FourthMerge_IsBlocked()
        {
            for (int i = 0; i < 3; i++)
            {
                boardService.ClearBoard();
                HexBoardService.AxialCoord from = SpawnOccupant(0, 0);
                HexBoardService.AxialCoord to = SpawnOccupant(1, 0);
                bool success = inputService.TryMerge(from, to);
                Assert.IsTrue(success, $"Merge {i + 1} should succeed.");
            }

            boardService.ClearBoard();
            HexBoardService.AxialCoord blockedFrom = SpawnOccupant(0, 0);
            HexBoardService.AxialCoord blockedTo = SpawnOccupant(1, 0);
            bool blockedSuccess = inputService.TryMerge(blockedFrom, blockedTo);

            Assert.IsFalse(blockedSuccess, "Fourth merge should be blocked by the turn controller.");
            Assert.AreEqual(3, turnController.MergesThisTurn, "Merge counter should remain capped at three.");
            Assert.IsTrue(inputService.IsLastTargetValid, "Target should remain valid even when blocked by turn limit.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator InvalidTarget_DoesNotIncrement()
        {
            boardService.ClearBoard();
            HexBoardService.AxialCoord from = SpawnOccupant(0, 0);
            HexBoardService.AxialCoord invalidTo = SpawnOccupant(2, 0);

            bool success = inputService.TryMerge(from, invalidTo);

            Assert.IsFalse(success, "Non-adjacent merge should fail.");
            Assert.AreEqual(0, turnController.MergesThisTurn, "Invalid merge must not change the counter.");
            Assert.IsFalse(inputService.IsLastTargetValid, "Invalid target should be reported for UI feedback.");
            yield return null;
        }

        private HexBoardService.AxialCoord SpawnOccupant(int q, int r)
        {
            GameObject go = new GameObject($"Piece_{q}_{r}");
            spawnedPieces.Add(go);
            HexCellOccupant occupant = go.AddComponent<HexCellOccupant>();
            HexBoardService.AxialCoord coord = new HexBoardService.AxialCoord(q, r);
            bool placed = boardService.TryPlace(occupant, coord);
            Assert.IsTrue(placed, $"Failed to place occupant at {coord}.");
            return coord;
        }
    }
}
