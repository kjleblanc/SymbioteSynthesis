using System;
using System.Collections.Generic;
using UnityEngine;

namespace Company.Game.Board
{
    [DisallowMultipleComponent]
    public sealed class HexBoardService : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int radius = 3;

        private readonly Dictionary<AxialCoord, HexCellOccupant> _occupants = new();
        private bool _bootstrapped;

        public int Radius
        {
            get => radius;
            set => radius = Mathf.Max(1, value);
        }

        public IEnumerable<KeyValuePair<AxialCoord, HexCellOccupant>> Occupants => _occupants;

        private void Awake()
        {
            BootstrapOccupantsIfNeeded();
        }

        private void OnEnable()
        {
            BootstrapOccupantsIfNeeded();
        }

        public void ClearBoard()
        {
            foreach (KeyValuePair<AxialCoord, HexCellOccupant> pair in _occupants)
            {
                if (pair.Value != null)
                {
                    pair.Value.ClearBoardReference();
                }
            }

            _occupants.Clear();
        }

        public bool TryPlace(HexCellOccupant occupant, AxialCoord coord)
        {
            if (occupant == null)
            {
                throw new ArgumentNullException(nameof(occupant));
            }

            if (!IsWithinBounds(coord) || _occupants.ContainsKey(coord))
            {
                return false;
            }

            _occupants[coord] = occupant;
            occupant.SetBoardReference(this, coord);
            return true;
        }

        private void BootstrapOccupantsIfNeeded()
        {
            if (_bootstrapped)
            {
                return;
            }

            _bootstrapped = true;
            HexCellOccupant[] occupants = GetComponentsInChildren<HexCellOccupant>(true);
            for (int i = 0; i < occupants.Length; i++)
            {
                HexCellOccupant occupant = occupants[i];
                if (occupant == null)
                {
                    continue;
                }

                AxialCoord coord = occupant.GetInitialCoordinate();
                TryPlace(occupant, coord);
            }
        }

        public bool TryGetOccupant(AxialCoord coord, out HexCellOccupant occupant)
        {
            return _occupants.TryGetValue(coord, out occupant);
        }

        public HexMergeResult EvaluateMerge(AxialCoord from, AxialCoord to)
        {
            HexMergeResult result = new HexMergeResult(from, to);

            if (!IsWithinBounds(from) || !_occupants.TryGetValue(from, out HexCellOccupant source) || source == null)
            {
                return result;
            }

            result.Source = source;

            if (!IsWithinBounds(to) || !_occupants.TryGetValue(to, out HexCellOccupant target) || target == null)
            {
                return result;
            }

            result.Target = target;

            if (!AreNeighbors(from, to))
            {
                return result;
            }

            result.IsValidTarget = source.CanMergeWith(target);
            return result;
        }

        public HexMergeResult ExecuteMerge(AxialCoord from, AxialCoord to)
        {
            HexMergeResult result = EvaluateMerge(from, to);
            if (!result.IsValidTarget || result.Source == null || result.Target == null)
            {
                return result;
            }

            _occupants.Remove(from);
            result.Source.HandleMergedInto(result.Target);
            result.Target.HandleMergedFrom(result.Source);
            result.Success = true;
            return result;
        }

        public bool IsWithinBounds(AxialCoord coord)
        {
            int s = -coord.q - coord.r;
            return Mathf.Abs(coord.q) <= radius && Mathf.Abs(coord.r) <= radius && Mathf.Abs(s) <= radius;
        }

        public static bool AreNeighbors(AxialCoord a, AxialCoord b)
        {
            int dq = b.q - a.q;
            int dr = b.r - a.r;
            return (dq == 1 && dr == 0) ||
                   (dq == -1 && dr == 0) ||
                   (dq == 0 && dr == 1) ||
                   (dq == 0 && dr == -1) ||
                   (dq == 1 && dr == -1) ||
                   (dq == -1 && dr == 1);
        }

        public readonly struct AxialCoord : IEquatable<AxialCoord>
        {
            public readonly int q;
            public readonly int r;

            public AxialCoord(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public bool Equals(AxialCoord other)
            {
                return q == other.q && r == other.r;
            }

            public override bool Equals(object obj)
            {
                return obj is AxialCoord other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (q * 397) ^ r;
                }
            }

            public override string ToString()
            {
                return $"({q}, {r})";
            }
        }

        public struct HexMergeResult
        {
            public HexMergeResult(AxialCoord from, AxialCoord to)
            {
                From = from;
                To = to;
                Source = null;
                Target = null;
                IsValidTarget = false;
                Success = false;
            }

            public AxialCoord From { get; }
            public AxialCoord To { get; }
            public HexCellOccupant Source { get; internal set; }
            public HexCellOccupant Target { get; internal set; }
            public bool IsValidTarget { get; internal set; }
            public bool Success { get; internal set; }
        }
    }
}
