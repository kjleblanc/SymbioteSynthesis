using System;
using Company.Game.Seeding;
using UnityEngine;

namespace Company.Game.RNG
{
    public enum RngCategory
    {
        Spawn = 0,
        Recipe = 1,
        Loot = 2,
    }

    [DisallowMultipleComponent]
    public sealed class RNGService : MonoBehaviour
    {
        private static readonly RngCategory[] Categories = (RngCategory[])Enum.GetValues(typeof(RngCategory));

        [SerializeField]
        private RunSeed runSeed;

        [SerializeField]
        private int fallbackSeed = 41073;

        private readonly System.Random[] _randoms = new System.Random[Categories.Length];
        private int _appliedSeed;
        private bool _initialized;

        private void OnEnable()
        {
            EnsureInitialized();
        }

        public void ConfigureSeedAsset(RunSeed asset)
        {
            runSeed = asset;
            _initialized = false;
        }

        public System.Random GetRandom(RngCategory category)
        {
            EnsureInitialized();
            int index = (int)category;
            System.Random instance = _randoms[index];
            if (instance == null)
            {
                instance = CreateRandom(_appliedSeed, category);
                _randoms[index] = instance;
            }

            return instance;
        }

        public void Reseed()
        {
            int seed = ResolveSeed();
            ApplySeed(seed);
        }

        public int CurrentSeed
        {
            get
            {
                EnsureInitialized();
                return _appliedSeed;
            }
        }

        private void EnsureInitialized()
        {
            int seed = ResolveSeed();
            if (!_initialized || seed != _appliedSeed)
            {
                ApplySeed(seed);
            }
        }

        private void ApplySeed(int seed)
        {
            _appliedSeed = seed;
            for (int i = 0; i < Categories.Length; i++)
            {
                RngCategory category = Categories[i];
                _randoms[i] = CreateRandom(seed, category);
            }

            _initialized = true;
        }

        private int ResolveSeed()
        {
            if (runSeed != null)
            {
                return runSeed.value;
            }

            return fallbackSeed;
        }

        private static System.Random CreateRandom(int seed, RngCategory category)
        {
            unchecked
            {
                int salt = ((int)category + 1) * 100003;
                return new System.Random(seed ^ salt);
            }
        }
    }
}
