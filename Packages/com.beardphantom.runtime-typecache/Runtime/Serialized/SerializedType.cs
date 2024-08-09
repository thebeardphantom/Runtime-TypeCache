using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form a reflectable Type.
    /// </summary>
    [Serializable]
    internal class SerializedType : ISerializableMemberInfo<Type>
    {
        [field: SerializeField] public int TypeStoreIndex { get; private set; }

        /// <inheritdoc />
        public void Serialize(Type memberInfo, TypeStore typeStore)
        {
            TypeStoreIndex = typeStore.AddOrGet(memberInfo);
        }

        /// <param name="typeStore"></param>
        /// <inheritdoc />
        public Type Deserialize(TypeStore typeStore)
        {
            var type = typeStore[TypeStoreIndex];
            Assert.IsNotNull(type, "type != null");
            return type;
        }
    }
}