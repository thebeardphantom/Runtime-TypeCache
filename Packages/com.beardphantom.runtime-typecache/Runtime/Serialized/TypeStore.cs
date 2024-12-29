using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    [Serializable]
    public class TypeStore : ISerializationCallbackReceiver
    {
        private readonly Dictionary<Type, int> _typeToIndex = new();

        private readonly List<Type> _deserializedTypes = new();

        [field: SerializeField]
        private List<string> SerializedTypeAqns { get; set; } = new();

        public Type this[int index] => _deserializedTypes[index];

        public int AddOrGet(Type type)
        {
            if (!_typeToIndex.TryGetValue(type, out int index))
            {
                index = _deserializedTypes.Count;
                _typeToIndex[type] = index;
                _deserializedTypes.Add(type);
            }

            return index;
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            IEnumerable<string> typeAqns = _deserializedTypes.Select(t => t.AssemblyQualifiedName);
            SerializedTypeAqns.Clear();
            SerializedTypeAqns.AddRange(typeAqns);
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            IEnumerable<Type> types = SerializedTypeAqns.Select(
                s =>
                {
                    var type = Type.GetType(s);
                    Assert.IsNotNull(type, "type != null");
                    return type;
                });
            _deserializedTypes.Clear();
            _deserializedTypes.AddRange(types);
        }
    }
}