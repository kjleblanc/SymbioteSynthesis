using System;
using UnityEngine;

namespace Company.Game.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class TurnController : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int mergesPerTurn = 3;

        private int mergesThisTurn;

        public event Action<int, int> MergeCountChanged;

        public int MergesPerTurn
        {
            get => mergesPerTurn;
            set => mergesPerTurn = Mathf.Max(1, value);
        }

        public int MergesThisTurn => mergesThisTurn;

        public bool CanMerge => mergesThisTurn < mergesPerTurn;

        public int MergesRemaining => Mathf.Max(0, mergesPerTurn - mergesThisTurn);

        private void OnEnable()
        {
            mergesThisTurn = Mathf.Clamp(mergesThisTurn, 0, mergesPerTurn);
            NotifyCountChanged();
        }

        public void BeginTurn()
        {
            mergesThisTurn = 0;
            NotifyCountChanged();
        }

        public void RegisterSuccessfulMerge()
        {
            if (!CanMerge)
            {
                return;
            }

            mergesThisTurn++;
            NotifyCountChanged();
        }

        public void ForceSetMergeCount(int value)
        {
            mergesThisTurn = Mathf.Clamp(value, 0, mergesPerTurn);
            NotifyCountChanged();
        }

        private void NotifyCountChanged()
        {
            MergeCountChanged?.Invoke(mergesThisTurn, mergesPerTurn);
        }
    }
}
