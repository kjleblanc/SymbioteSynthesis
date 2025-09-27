using UnityEngine;

namespace Company.Game.Board
{
    [DisallowMultipleComponent]
    public sealed class HexCellOccupant : MonoBehaviour
    {
        [SerializeField]
        private int tier = 1;

        [SerializeField]
        private int initialQ;

        [SerializeField]
        private int initialR;

        public int Tier => tier;
        public HexBoardService.AxialCoord Coordinate { get; private set; }
        public HexBoardService Board { get; private set; }

        public bool CanMergeWith(HexCellOccupant other)
        {
            return other != null && other.tier == tier;
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
        }

        internal HexBoardService.AxialCoord GetInitialCoordinate()
        {
            return new HexBoardService.AxialCoord(initialQ, initialR);
        }

        internal void HandleMergedInto(HexCellOccupant target)
        {
            gameObject.SetActive(false);
        }

        internal void HandleMergedFrom(HexCellOccupant source)
        {
            tier = Mathf.Max(tier, source.tier + 1);
        }
    }
}
