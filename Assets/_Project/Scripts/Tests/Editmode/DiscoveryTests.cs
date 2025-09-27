using System;
using System.IO;
using Company.Game.Discovery;
using NUnit.Framework;
using UnityEngine;

namespace Company.Game.Tests.Editmode
{
    public sealed class DiscoveryTests
    {
        private string tempDirectory;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Application.temporaryCachePath, $"DiscoveryTests_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(tempDirectory) && Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void Persistence_ReloadsFromDisk()
        {
            DiscoveryLogService first = CreateService();
            first.ConfigureOverrideDirectory(tempDirectory);
            first.EnsureLoaded();

            bool added = first.RecordDiscovery("recipe_test");
            Assert.IsTrue(added, "First discovery should be new.");
            Assert.AreEqual(1, first.PersistentDiscoveries.Count, "Persistent count should be one after first record.");

            UnityEngine.Object.DestroyImmediate(first.gameObject);

            DiscoveryLogService second = CreateService();
            second.ConfigureOverrideDirectory(tempDirectory);
            second.EnsureLoaded();

            Assert.AreEqual(1, second.PersistentDiscoveries.Count, "Reloaded log should contain original entry.");
            Assert.IsFalse(second.RecordDiscovery("recipe_test"), "Duplicate discovery should return false.");
            Assert.AreEqual(1, second.PersistentDiscoveries.Count, "Persistent count must remain one after duplicate.");

            UnityEngine.Object.DestroyImmediate(second.gameObject);
        }

        private static DiscoveryLogService CreateService()
        {
            GameObject go = new GameObject("DiscoveryService");
            return go.AddComponent<DiscoveryLogService>();
        }
    }
}
