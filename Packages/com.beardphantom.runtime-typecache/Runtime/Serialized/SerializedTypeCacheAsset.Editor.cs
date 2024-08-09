#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset
    {
        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            SerializedTypeCache.Regenerate();
        }
    }
}
#endif