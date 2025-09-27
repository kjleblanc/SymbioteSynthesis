using System;
using Company.Game.Combat;
using Company.Game.Discovery;
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

        [SerializeField]
        private CombatResolver combatResolver;

        [SerializeField]
        private DiscoveryLogService discoveryLogService;

        public event Action<int, int> MergeCountChanged;
        public event Action TurnExhausted;

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
            discoveryLogService?.EnsureLoaded();
            NotifyCountChanged();
        }

        public void BeginTurn()
        {
            mergesThisTurn = 0;
            discoveryLogService?.EnsureLoaded();
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

            if (!CanMerge)
            {
                HandleTurnExhausted();
            }
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

        private void HandleTurnExhausted()
        {
            TurnExhausted?.Invoke();
            combatResolver?.ResolveCurrentWave();
        }
    }
}
