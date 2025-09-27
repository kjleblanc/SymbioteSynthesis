using System.Collections.Generic;
using Company.Game.Fusion;
using UnityEngine;

namespace Company.Game.Combat
{
    [CreateAssetMenu(menuName = "Game/Enemies/Enemy Definition", fileName = "EnemyDefinition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField]
        private string enemyId = "enemy";

        [SerializeField]
        private string displayName = "Enemy";

        [SerializeField]
        [Min(1)]
        private int health = 5;

        [SerializeField]
        [Min(0)]
        private int attack = 1;

        [SerializeField]
        [Min(1)]
        private int speed = 8;

        [SerializeField]
        private List<CellTraitDefinition> traits = new();

        public string EnemyId => enemyId;
        public string DisplayName => displayName;
        public int Health => health;
        public int Attack => attack;
        public int Speed => speed;
        public IReadOnlyList<CellTraitDefinition> Traits => traits;

        private void OnValidate()
        {
            if (health < 1)
            {
                health = 1;
            }

            if (attack < 0)
            {
                attack = 0;
            }

            if (speed < 1)
            {
                speed = 1;
            }
        }
    }
}
