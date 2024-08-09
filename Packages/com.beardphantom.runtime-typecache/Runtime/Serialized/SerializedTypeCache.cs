using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    [Serializable]
    internal partial class SerializedTypeCache
    {
        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedType>> TypesWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedMethod>> MethodsWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedProperty>> PropertiesWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedField>> FieldsWithAttribute { get; private set; } = new();

        [field: SerializeField] public List<TypesDerivedFromType> TypesDerivedFromType { get; private set; } = new();

        [field: SerializeField] public TypeStore TypeStore { get; private set; } = new();
    }
}