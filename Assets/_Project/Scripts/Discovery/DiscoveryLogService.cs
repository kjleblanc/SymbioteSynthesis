using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Company.Game.Fusion;
using UnityEngine;

namespace Company.Game.Discovery
{
    [DisallowMultipleComponent]
    public sealed class DiscoveryLogService : MonoBehaviour
    {
        private const string DefaultFolderName = "SymbioteSynthesis/Meta";
        private const string DefaultFileName = "DiscoveryLog.json";

        [SerializeField]
        private bool loadOnAwake = true;

        [SerializeField]
        [Tooltip("Optional override for persistence relative path. Leave empty for default meta folder.")]
        private string relativeFolder = DefaultFolderName;

        [SerializeField]
        private string fileName = DefaultFileName;

        private readonly HashSet<string> _persistentDiscoveries = new(StringComparer.Ordinal);
        private readonly HashSet<string> _runDiscoveries = new(StringComparer.Ordinal);
        private string overrideDirectory;
        private bool _loaded;

        public event Action<IReadOnlyCollection<string>> DiscoveryChanged;

        public IReadOnlyCollection<string> PersistentDiscoveries => _persistentDiscoveries;
        public IReadOnlyCollection<string> RunDiscoveries => _runDiscoveries;

        private void Awake()
        {
            if (loadOnAwake)
            {
                LoadFromDisk();
            }
        }

        public void ConfigureOverrideDirectory(string directoryPath)
        {
            overrideDirectory = directoryPath;
            _loaded = false;
        }

        public bool RecordDiscovery(FusionRecipeDefinition recipe)
        {
            if (recipe == null)
            {
                return false;
            }

            string recipeId = string.IsNullOrEmpty(recipe.RecipeId) ? recipe.name : recipe.RecipeId;
            return RecordDiscovery(recipeId);
        }

        public bool RecordDiscovery(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId))
            {
                return false;
            }

            EnsureLoaded();

            bool addedPersistent = _persistentDiscoveries.Add(recipeId);
            bool addedRun = _runDiscoveries.Add(recipeId);

            if (addedPersistent)
            {
                SaveToDisk();
            }

            if (addedPersistent || addedRun)
            {
                DiscoveryChanged?.Invoke(_persistentDiscoveries);
            }

            return addedPersistent;
        }

        public void ResetRunDiscoveries()
        {
            if (_runDiscoveries.Count == 0)
            {
                return;
            }

            _runDiscoveries.Clear();
            DiscoveryChanged?.Invoke(_persistentDiscoveries);
        }

        public void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            LoadFromDisk();
        }

        private void LoadFromDisk()
        {
            string path = GetFilePath();
            _persistentDiscoveries.Clear();

            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path, Encoding.UTF8);
                    SerializableDiscoveryLog data = JsonUtility.FromJson<SerializableDiscoveryLog>(json);
                    if (data?.recipes != null)
                    {
                        for (int i = 0; i < data.recipes.Length; i++)
                        {
                            string entry = data.recipes[i];
                            if (!string.IsNullOrEmpty(entry))
                            {
                                _persistentDiscoveries.Add(entry);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Failed to load discovery log from {path}: {exception.Message}");
                }
            }

            _loaded = true;
            DiscoveryChanged?.Invoke(_persistentDiscoveries);
        }

        private void SaveToDisk()
        {
            try
            {
                string directory = GetDirectoryPath();
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                SerializableDiscoveryLog data = new SerializableDiscoveryLog
                {
                    recipes = _persistentDiscoveries.ToArray(),
                };

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(GetFilePath(), json, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to save discovery log: {exception.Message}");
            }
        }

        private string GetDirectoryPath()
        {
            if (!string.IsNullOrEmpty(overrideDirectory))
            {
                return overrideDirectory;
            }

            string basePath = Application.persistentDataPath;
            string relative = string.IsNullOrEmpty(relativeFolder) ? DefaultFolderName : relativeFolder;
            return Path.Combine(basePath, relative);
        }

        private string GetFilePath()
        {
            return Path.Combine(GetDirectoryPath(), string.IsNullOrEmpty(fileName) ? DefaultFileName : fileName);
        }

        [Serializable]
        private sealed class SerializableDiscoveryLog
        {
            public string[] recipes;
        }
    }
}
