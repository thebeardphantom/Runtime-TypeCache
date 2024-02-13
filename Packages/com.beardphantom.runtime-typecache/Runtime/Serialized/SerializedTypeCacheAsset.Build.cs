#if !UNITY_EDITOR
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset
    {
        #region Properties

        internal static SerializedTypeCacheAsset Instance { get; private set; }

        #endregion

        #region Methods

        private void Awake()
        {
            Assert.IsNull(Instance, "Instance == null");
            Instance = this;
        }

        #endregion
    }
}
#endif