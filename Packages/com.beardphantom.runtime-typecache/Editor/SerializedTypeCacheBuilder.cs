using BeardPhantom.RuntimeTypeCache.Serialized;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.RuntimeTypeCache.Editor
{
    /// <summary>
    /// Responsable for creating the <see cref="SerializedTypeCacheAsset" /> at build time.
    /// </summary>
    internal class SerializedTypeCacheBuilder : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        #region Fields

        /// <summary>
        /// The name of which the debug copies of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string ADB_NAME = "SerializedTypeCache";

        /// <summary>
        /// The path at which the transitory SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string ADB_PATH = "Assets/" + ADB_NAME + "SerializedTypeCache.asset";

        /// <summary>
        /// The name of which the debug copies of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TEMP_NAME = "SerializedTypeCacheCopy";

        /// <summary>
        /// The path at which the debug copy of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TEMP_PATH = "Temp/" + TEMP_NAME + ".asset";

        /// <summary>
        /// The path at which the debug json copy of the SerializedTypeCacheAsset will be created.
        /// </summary>
        private const string TEMP_PATH_JSON = "Temp/" + TEMP_NAME + ".json";

        #endregion

        #region Properties

        /// <inheritdoc />
        int IOrderedCallback.callbackOrder { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans up after the <see cref="OnPreprocessBuild" /> function.
        /// </summary>
        private static void DeleteAndRemoveAsset()
        {
            AssetDatabase.DeleteAsset(ADB_PATH);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(ShouldRemovePreloadedAsset);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        private static bool ShouldRemovePreloadedAsset(Object p)
        {
            return p == null || p is SerializedTypeCacheAsset;
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
                asset.name = TEMP_NAME;
                InternalEditorUtility.SaveToSerializedFileAndForget(
                    new Object[]
                    {
                        asset
                    },
                    TEMP_PATH,
                    true);

                // Serialize to json for debug inspection
                var json = JsonUtility.ToJson(asset, true);
                json = Regex.Replace(json, "(<|>|k__BackingField)", "");
                File.WriteAllText(TEMP_PATH_JSON, json);

                // Create adb asset so preloaded assets can actually use it
                asset.name = ADB_NAME;
                AssetDatabase.CreateAsset(asset, ADB_PATH);

                var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

                preloadedAssets.Add(asset);
                preloadedAssets.RemoveAll(ShouldRemovePreloadedAsset);

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

        #endregion
    }
}