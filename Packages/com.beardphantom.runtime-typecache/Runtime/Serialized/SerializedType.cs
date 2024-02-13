using System;
using UnityEngine;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    /// Represents in serializable form a reflectable Type.
    /// </summary>
    [Serializable]
    internal class SerializedType : ISerializableMemberInfo<Type>
    {
        #region Properties

        [field: SerializeField]
        public int TypeStoreIndex { get; private set; }

        #endregion

        #region Methods

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
            return type;
        }

        #endregion
    }
}