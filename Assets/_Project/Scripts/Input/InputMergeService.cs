using System;
using Company.Game.Board;
using Company.Game.Gameplay;
using UnityEngine;

namespace Company.Game.Input
{
    [DisallowMultipleComponent]
    public sealed class InputMergeService : MonoBehaviour
    {
        [SerializeField]
        private HexBoardService boardService;

        [SerializeField]
        private TurnController turnController;

        [SerializeField]
        private Fusion.FusionService fusionService;

        public bool IsLastTargetValid { get; private set; }

        public event Action<HexBoardService.HexMergeResult> MergeSucceeded;
        public event Action<HexBoardService.HexMergeResult> MergeBlocked;

        public void Configure(HexBoardService board, TurnController turn, Fusion.FusionService fusion = null)
        {
            boardService = board;
            turnController = turn;
            fusionService = fusion;
            boardService?.ConfigureFusionService(fusionService);
        }

        private void Awake()
        {
            if (fusionService != null && boardService != null)
            {
                boardService.ConfigureFusionService(fusionService);
            }
        }

        public bool TryMerge(HexBoardService.AxialCoord from, HexBoardService.AxialCoord to)
        {
            if (boardService == null)
            {
                Debug.LogWarning("InputMergeService missing board reference.");
                IsLastTargetValid = false;
                return false;
            }

            HexBoardService.HexMergeResult evaluation = boardService.EvaluateMerge(from, to);
            IsLastTargetValid = evaluation.IsValidTarget;

            if (turnController != null && !turnController.CanMerge)
            {
                MergeBlocked?.Invoke(evaluation);
                return false;
            }

            if (!evaluation.IsValidTarget)
            {
                MergeBlocked?.Invoke(evaluation);
                return false;
            }

            HexBoardService.HexMergeResult result = boardService.ExecuteMerge(from, to);
            if (!result.Success)
            {
                MergeBlocked?.Invoke(result);
                return false;
            }

            turnController?.RegisterSuccessfulMerge();
            MergeSucceeded?.Invoke(result);
            return true;
        }
    }
}
