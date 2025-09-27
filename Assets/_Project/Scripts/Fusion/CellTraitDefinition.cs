using System.Collections.Generic;
using UnityEngine;

namespace Company.Game.Fusion
{
    [CreateAssetMenu(menuName = "Game/Traits/Cell Trait", fileName = "CellTrait")]
    public sealed class CellTraitDefinition : ScriptableObject
    {
        [SerializeField]
        private string traitId = "trait";

        [SerializeField]
        private string displayName = "Trait";

        [SerializeField]
        private List<string> keywords = new();

        [SerializeField]
        [Tooltip("If true, multiple copies of this trait can stack their effects.")]
        private bool stackable;

        [SerializeField]
        [TextArea]
        private string stackingRuleDescription = string.Empty;

        [SerializeField]
        private TraitHooks hooks = new();

        public string TraitId => traitId;
        public string DisplayName => displayName;
        public IReadOnlyList<string> Keywords => keywords;
        public bool Stackable => stackable;
        public string StackingRuleDescription => stackingRuleDescription;
        public TraitHooks Hooks => hooks;

        [System.Serializable]
        public sealed class TraitHooks
        {
        }
    }
}
