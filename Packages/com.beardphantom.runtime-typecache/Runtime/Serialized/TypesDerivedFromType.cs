using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form all types that derive from a specific parent type.
    /// </summary>
    [Serializable]
    internal class TypesDerivedFromType
    {
        [field: SerializeField] public SerializedType ParentType { get; set; }

        [field: SerializeField] public List<SerializedType> DerivedTypes { get; set; }
    }
}