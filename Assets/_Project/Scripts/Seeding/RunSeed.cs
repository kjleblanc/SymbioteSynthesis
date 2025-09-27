using UnityEngine;

namespace Company.Game.Seeding
{
    [CreateAssetMenu(menuName = "Game/Seeds/Run Seed", fileName = "RunSeed")]
    public sealed class RunSeed : ScriptableObject
    {
        [Tooltip("Deterministic seed for gameplay RNG.")]
        public int value = 41073;
    }
}
