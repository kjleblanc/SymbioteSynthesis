using Company.Game.RNG;
using Company.Game.Seeding;
using NUnit.Framework;
using UnityEngine;

namespace Company.Game.Tests.Editmode
{
    public sealed class RNGServiceTests
    {
        [Test]
        public void SameSeed_ProducesStableSequencePerCategory()
        {
            RunSeed seed = ScriptableObject.CreateInstance<RunSeed>();
            seed.value = 12345;

            RNGService first = CreateService(seed);
            RNGService second = CreateService(seed);

            System.Random firstSpawn = first.GetRandom(RngCategory.Spawn);
            System.Random secondSpawn = second.GetRandom(RngCategory.Spawn);

            for (int i = 0; i < 10; i++)
            {
                int a = firstSpawn.Next(0, 1000);
                int b = secondSpawn.Next(0, 1000);
                Assert.AreEqual(a, b, $"Spawn stream mismatch at step {i}.");
            }

            UnityEngine.Object.DestroyImmediate(first.gameObject);
            UnityEngine.Object.DestroyImmediate(second.gameObject);
            UnityEngine.Object.DestroyImmediate(seed);
        }

        [Test]
        public void CategoryIsolation_PreservesSpawnSequence()
        {
            RunSeed seed = ScriptableObject.CreateInstance<RunSeed>();
            seed.value = 98765;

            RNGService control = CreateService(seed);
            RNGService underTest = CreateService(seed);

            System.Random controlSpawn = control.GetRandom(RngCategory.Spawn);
            System.Random spawn = underTest.GetRandom(RngCategory.Spawn);

            int initialControl = controlSpawn.Next(0, 1000);
            int initialTest = spawn.Next(0, 1000);
            Assert.AreEqual(initialControl, initialTest, "Initial spawn roll should match.");

            System.Random recipe = underTest.GetRandom(RngCategory.Recipe);
            for (int i = 0; i < 5; i++)
            {
                recipe.Next();
            }

            int nextControl = controlSpawn.Next(0, 1000);
            int nextTest = spawn.Next(0, 1000);
            Assert.AreEqual(nextControl, nextTest, "Spawn stream should be unaffected by recipe draws.");

            UnityEngine.Object.DestroyImmediate(control.gameObject);
            UnityEngine.Object.DestroyImmediate(underTest.gameObject);
            UnityEngine.Object.DestroyImmediate(seed);
        }

        private static RNGService CreateService(RunSeed seed)
        {
            GameObject go = new GameObject("RNGService");
            RNGService service = go.AddComponent<RNGService>();
            service.ConfigureSeedAsset(seed);
            service.Reseed();
            return service;
        }
    }
}
