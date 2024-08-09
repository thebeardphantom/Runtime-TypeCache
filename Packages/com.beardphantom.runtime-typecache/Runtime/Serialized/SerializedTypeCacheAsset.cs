using System.Linq;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCacheAsset : ScriptableObject
    {
        [field: SerializeField] public SerializedTypeCache SerializedTypeCache { get; set; } = new();

        public static SerializedTypeCacheAsset GetInstance()
        {
            return Resources.FindObjectsOfTypeAll<SerializedTypeCacheAsset>().First();
        }
    }
}