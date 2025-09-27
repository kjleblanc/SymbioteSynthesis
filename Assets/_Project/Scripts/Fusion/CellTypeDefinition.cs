using System.Collections.Generic;
using UnityEngine;

namespace Company.Game.Fusion
{
    [CreateAssetMenu(menuName = "Game/Cells/Cell Type", fileName = "CellType")]
    public sealed class CellTypeDefinition : ScriptableObject
    {
        [SerializeField]
        private string cellId = "cell_id";

        [SerializeField]
        private string displayName = "Cell";

        [SerializeField]
        private string family = "Default";

        [SerializeField]
        [Min(1)]
        private int tier = 1;

        [SerializeField]
        private CellTypeDefinition defaultUpgrade;

        [SerializeField]
        private List<CellTraitDefinition> traits = new();

        [SerializeField]
        [Min(1)]
        private int baseHealth = 4;

        [SerializeField]
        [Min(0)]
        private int attack = 1;

        [SerializeField]
        [Min(1)]
        private int speed = 10;

        public string CellId => cellId;
        public string DisplayName => displayName;
        public string Family => family;
        public int Tier => tier;
        public CellTypeDefinition DefaultUpgrade => defaultUpgrade;
        public IReadOnlyList<CellTraitDefinition> Traits => traits;
        public int BaseHealth => baseHealth;
        public int Attack => attack;
        public int Speed => speed;

        private void OnValidate()
        {
            if (tier < 1)
            {
                tier = 1;
            }

            if (baseHealth < 1)
            {
                baseHealth = 1;
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
