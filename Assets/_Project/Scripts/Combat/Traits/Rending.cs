using Company.Game.Fusion;

namespace Company.Game.Combat.Traits
{
    public static class Rending
    {
        public const string TraitId = "trait_rending";
        public const int ArmorPierce = 1;

        public static bool Matches(CellTraitDefinition trait)
        {
            return trait != null && trait.TraitId == TraitId;
        }
    }
}
