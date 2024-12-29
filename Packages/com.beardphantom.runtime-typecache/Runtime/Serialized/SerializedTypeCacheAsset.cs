// #undef UNITY_EDITOR

using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset : ScriptableObject
    {
        [field: SerializeField]
        public SerializedTypeCache SerializedTypeCache { get; set; } = new();

        public static SerializedTypeCacheAsset GetInstance()
        {
            return Resources.FindObjectsOfTypeAll<SerializedTypeCacheAsset>().First();
        }
    }

#if UNITY_EDITOR
    internal partial class SerializedTypeCacheAsset
    {
        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            SerializedTypeCache.Regenerate();
        }
    }
#else
    internal partial class SerializedTypeCacheAsset
    {
        #region Fields

        private static SerializedTypeCacheAsset s_instance;

        #endregion

        #region Properties

        internal static SerializedTypeCacheAsset Instance
        {
            get
            {
                Assert.IsNotNull(s_instance, "_instance != null");
                return s_instance;
            }
            private set => s_instance = value;
        }

        #endregion

        #region Methods

        private void Awake()
        {
            Assert.IsNull(s_instance, "_instance == null");
            Instance = this;
        }

        #endregion
    }
#endif
}