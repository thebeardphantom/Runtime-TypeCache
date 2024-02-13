using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset : ScriptableObject
    {
        #region Properties

        [field: SerializeField]
        public SerializedTypeCache SerializedTypeCache { get; set; } = new();

        #endregion
    }
}