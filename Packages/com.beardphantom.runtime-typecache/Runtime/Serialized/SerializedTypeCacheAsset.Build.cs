#if !UNITY_EDITOR
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset
    {
        #region Fields

        private static SerializedTypeCacheAsset _instance;

        #endregion

        #region Properties

        internal static SerializedTypeCacheAsset Instance
        {
            get
            {
                Assert.IsNotNull(_instance, "_instance != null");
                return _instance;
            }
            private set => _instance = value;
        }

        #endregion

        #region Methods

        private void Awake()
        {
            Assert.IsNull(_instance, "_instance == null");
            Instance = this;
        }

        #endregion
    }
}
#endif