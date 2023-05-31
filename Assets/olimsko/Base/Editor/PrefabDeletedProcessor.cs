using System.IO;
using UnityEditor;
using UnityEngine;

namespace olimsko
{
    public class PrefabDeletedProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string deletedAsset in deletedAssets)
            {
                if (deletedAsset.EndsWith(".prefab"))
                {
                    UIConfiguration uIConfig = ConfigurationProvider.LoadOrDefault<UIConfiguration>();

                    for (int i = uIConfig.ListPrefabs.Count - 1; i >= 0; i--)
                    {
                        if (uIConfig.ListPrefabs[i].Prefab == null)
                        {
                            uIConfig.ListPrefabs.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
