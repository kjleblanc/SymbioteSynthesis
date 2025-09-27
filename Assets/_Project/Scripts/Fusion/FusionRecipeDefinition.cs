using UnityEngine;

namespace Company.Game.Fusion
{
    [CreateAssetMenu(menuName = "Game/Fusions/Fusion Recipe", fileName = "FusionRecipe")]
    public sealed class FusionRecipeDefinition : ScriptableObject
    {
        [SerializeField]
        private string recipeId = "fusion_recipe";

        [SerializeField]
        private CellTypeDefinition inputA;

        [SerializeField]
        private CellTypeDefinition inputB;

        [SerializeField]
        private bool symmetric = true;

        [SerializeField]
        private CellTypeDefinition output;

        public string RecipeId => recipeId;
        public CellTypeDefinition InputA => inputA;
        public CellTypeDefinition InputB => inputB;
        public bool Symmetric => symmetric;
        public CellTypeDefinition Output => output;

        public bool Matches(CellTypeDefinition a, CellTypeDefinition b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (a == inputA && b == inputB)
            {
                return true;
            }

            return symmetric && a == inputB && b == inputA;
        }
    }
}
