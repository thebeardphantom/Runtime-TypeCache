using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BeardPhantom.RuntimeTypeCache.Serialized;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.RuntimeTypeCache.Editor
{
    /// <summary>
    ///     Responsible for creating the <see cref="SerializedTypeCacheAsset" /> at build time.
    /// </summary>
    internal class SerializedTypeCacheBuilder : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <inheritdoc />
        int IOrderedCallback.callbackOrder { get; }

        /// <summary>
        ///     The name of which the debug copies of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string ADBName = "SerializedTypeCache";

        /// <summary>
        ///     The path at which the transitory SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string ADBPath = "Assets/" + ADBName + "SerializedTypeCache.asset";

        /// <summary>
        ///     The name of which the debug copies of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TempName = "SerializedTypeCacheCopy";

        /// <summary>
        ///     The path at which the debug copy of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TempPath = "Temp/" + TempName + ".asset";

        /// <summary>
        ///     The path at which the debug json copy of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TempPathJson = "Temp/" + TempName + ".json";

        /// <summary>
        ///     Cleans up after the <see cref="OnPreprocessBuild" /> function.
        /// </summary>
        private static void DeleteAndRemoveAsset()
        {
            AssetDatabase.DeleteAsset(ADBPath);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(obj => obj == null || obj is SerializedTypeCacheAsset);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        /// <inheritdoc />
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            try
            {
                // Generate type cache asset
                var asset = ScriptableObject.CreateInstance<SerializedTypeCacheAsset>();
                asset.SerializedTypeCache.Regenerate();

                // Serialize asset to temp folder for debug inspection
                asset.name = TempName;
                InternalEditorUtility.SaveToSerializedFileAndForget(
                    new Object[]
                    {
                        asset
                    },
                    TempPath,
                    true);

                // Serialize to json for debug inspection
                var json = JsonUtility.ToJson(asset, true);
                json = Regex.Replace(json, "(<|>|k__BackingField)", "");
                File.WriteAllText(TempPathJson, json);

                // Create adb asset so preloaded assets can actually use it
                asset.name = ADBName;
                AssetDatabase.CreateAsset(asset, ADBPath);

                var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

                preloadedAssets.Add(asset);
                preloadedAssets.RemoveAll(a => a == null);

                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
            catch (Exception)
            {
                DeleteAndRemoveAsset();
                throw;
            }
        }

        /// <inheritdoc />
        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            DeleteAndRemoveAsset();
        }
    }
}