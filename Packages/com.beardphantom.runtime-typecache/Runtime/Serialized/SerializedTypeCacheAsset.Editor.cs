#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset
    {
        #region Methods

        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            SerializedTypeCache.Regenerate();
        }

        #endregion
    }
}
#endif