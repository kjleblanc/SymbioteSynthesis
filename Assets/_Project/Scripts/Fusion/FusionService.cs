using System;
using System.Collections.Generic;
using Company.Game.Board;
using Company.Game.Discovery;
using UnityEngine;

namespace Company.Game.Fusion
{
    [DisallowMultipleComponent]
    public sealed class FusionService : MonoBehaviour
    {
        [SerializeField]
        private List<FusionRecipeDefinition> recipes = new();

        [SerializeField]
        private DiscoveryLogService discoveryLogService;

        private readonly Dictionary<string, FusionRecipeDefinition> _lookup = new(StringComparer.Ordinal);
        private bool _lookupDirty = true;

        private void OnValidate()
        {
            _lookupDirty = true;
        }

        public void ConfigureDiscoveryLog(DiscoveryLogService logService)
        {
            discoveryLogService = logService;
        }

        public void SetRecipes(IEnumerable<FusionRecipeDefinition> definitions)
        {
            recipes.Clear();
            if (definitions == null)
            {
                _lookupDirty = true;
                return;
            }

            foreach (FusionRecipeDefinition definition in definitions)
            {
                if (definition != null)
                {
                    recipes.Add(definition);
                }
            }

            _lookupDirty = true;
        }

        public FusionResolution ResolveFusion(HexCellOccupant source, HexCellOccupant target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            EnsureLookup();

            CellTypeDefinition sourceType = source.CellType;
            CellTypeDefinition targetType = target.CellType;

            if (sourceType == null || targetType == null)
            {
                return new FusionResolution(targetType, null, false);
            }

            FusionRecipeDefinition recipe = FindRecipe(sourceType, targetType);
            bool discovered = false;
            CellTypeDefinition output = targetType;

            if (recipe != null && recipe.Output != null)
            {
                output = recipe.Output;
                if (discoveryLogService != null)
                {
                    discovered = discoveryLogService.RecordDiscovery(recipe);
                }
            }
            else if (sourceType.Family == targetType.Family && sourceType.Tier == targetType.Tier)
            {
                output = targetType.DefaultUpgrade != null ? targetType.DefaultUpgrade : sourceType.DefaultUpgrade;
            }

            return new FusionResolution(output, recipe, discovered);
        }

        private void EnsureLookup()
        {
            if (!_lookupDirty)
            {
                return;
            }

            _lookup.Clear();
            for (int i = 0; i < recipes.Count; i++)
            {
                FusionRecipeDefinition recipe = recipes[i];
                if (recipe == null || recipe.InputA == null || recipe.InputB == null)
                {
                    continue;
                }

                string forwardKey = ComposeKey(recipe.InputA, recipe.InputB, recipe.Symmetric);
                _lookup[forwardKey] = recipe;

                if (!recipe.Symmetric)
                {
                    string reverseKey = ComposeKey(recipe.InputB, recipe.InputA, true);
                    if (!_lookup.ContainsKey(reverseKey))
                    {
                        _lookup[reverseKey] = recipe;
                    }
                }
            }

            _lookupDirty = false;
        }

        private FusionRecipeDefinition FindRecipe(CellTypeDefinition a, CellTypeDefinition b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            string key = ComposeKey(a, b, false);
            if (_lookup.TryGetValue(key, out FusionRecipeDefinition recipe))
            {
                return recipe;
            }

            return null;
        }

        private static string ComposeKey(CellTypeDefinition a, CellTypeDefinition b, bool ordered)
        {
            string idA = a != null ? a.CellId : string.Empty;
            string idB = b != null ? b.CellId : string.Empty;

            if (!ordered && string.CompareOrdinal(idA, idB) > 0)
            {
                (idA, idB) = (idB, idA);
            }

            return string.Concat(idA, ">", idB);
        }

        [Serializable]
        public readonly struct FusionResolution
        {
            public FusionResolution(CellTypeDefinition outputType, FusionRecipeDefinition recipe, bool wasNewDiscovery)
            {
                OutputType = outputType;
                Recipe = recipe;
                WasNewDiscovery = wasNewDiscovery;
            }

            public CellTypeDefinition OutputType { get; }
            public FusionRecipeDefinition Recipe { get; }
            public bool WasNewDiscovery { get; }
            public bool HasRecipe => Recipe != null;
        }
    }
}
