using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form all MemberInfo derived types that are decorated with an attribute.
    /// </summary>
    [Serializable]
    internal class MemberInfoWithAttribute<T> where T : ISerializableMemberInfo
    {
        [field: SerializeField] public SerializedType AttributeType { get; set; }

        [field: SerializeField] public List<T> Matches { get; set; } = new();
    }
}