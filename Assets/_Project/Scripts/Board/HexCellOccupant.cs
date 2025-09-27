using System.Collections.Generic;
using Company.Game.Fusion;
using UnityEngine;

namespace Company.Game.Board
{
    [DisallowMultipleComponent]
    public sealed class HexCellOccupant : MonoBehaviour
    {
        [SerializeField]
        private int tier = 1;

        [SerializeField]
        private CellTypeDefinition cellType;

        [SerializeField]
        private int initialQ;

        [SerializeField]
        private int initialR;

        private void Awake()
        {
            SyncTierFromCellType();
        }

        private void OnValidate()
        {
            SyncTierFromCellType();
        }

        public int Tier => cellType != null ? cellType.Tier : tier;

        public CellTypeDefinition CellType => cellType;

        public IReadOnlyList<CellTraitDefinition> Traits => cellType?.Traits;
        public HexBoardService.AxialCoord Coordinate { get; private set; }
        public HexBoardService Board { get; private set; }

        public void Initialize(CellTypeDefinition type)
        {
            cellType = type;
            SyncTierFromCellType();
        }

        public bool CanMergeWith(HexCellOccupant other)
        {
            if (other == null)
            {
                return false;
            }

            if (cellType != null && other.cellType != null)
            {
                return cellType.Tier == other.cellType.Tier;
            }

            return other.tier == tier;
        }

        internal void SetBoardReference(HexBoardService board, HexBoardService.AxialCoord coord)
        {
            Board = board;
            Coordinate = coord;
            initialQ = coord.q;
            initialR = coord.r;
        }

        internal void ClearBoardReference()
        {
            Board = null;
            Coordinate = default;
        }

        internal HexBoardService.AxialCoord GetInitialCoordinate()
        {
            return new HexBoardService.AxialCoord(initialQ, initialR);
        }

        internal void HandleMergedInto(HexCellOccupant target)
        {
            gameObject.SetActive(false);
        }

        internal void ApplyFusionResult(CellTypeDefinition resultingType)
        {
            if (resultingType != null)
            {
                cellType = resultingType;
                tier = Mathf.Max(1, resultingType.Tier);
            }
            else
            {
                tier = Mathf.Max(tier, tier + 1);
            }
        }

        private void SyncTierFromCellType()
        {
            if (cellType != null)
            {
                tier = Mathf.Max(1, cellType.Tier);
            }
            else if (tier < 1)
            {
                tier = 1;
            }
        }
    }
}
