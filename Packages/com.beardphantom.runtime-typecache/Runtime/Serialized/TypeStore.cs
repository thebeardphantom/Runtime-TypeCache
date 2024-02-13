using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    [Serializable]
    public class TypeStore : ISerializationCallbackReceiver
    {
        #region Fields

        private readonly Dictionary<Type, int> _typeToIndex = new();

        private readonly List<Type> _deserializedTypes = new();

        #endregion

        #region Properties

        [field: SerializeField]
        private List<string> SerializedTypeAqns { get; set; } = new();

        #endregion

        #region Methods

        public Type this[int index] => _deserializedTypes[index];

        public int AddOrGet(Type type)
        {
            if (!_typeToIndex.TryGetValue(type, out var index))
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
            var typeAqns = _deserializedTypes.Select(t => t.AssemblyQualifiedName);
            SerializedTypeAqns.Clear();
            SerializedTypeAqns.AddRange(typeAqns);
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var types = SerializedTypeAqns.Select(Type.GetType);
            _deserializedTypes.Clear();
            _deserializedTypes.AddRange(types);
        }

        #endregion
    }
}