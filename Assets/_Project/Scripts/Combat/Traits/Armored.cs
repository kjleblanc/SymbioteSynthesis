using Company.Game.Fusion;

namespace Company.Game.Combat.Traits
{
    public static class Armored
    {
        public const string TraitId = "trait_armored";
        public const int DamageReduction = 1;

        public static bool Matches(CellTraitDefinition trait)
        {
            return trait != null && trait.TraitId == TraitId;
        }
    }
}
