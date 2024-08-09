using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form a reflectable FieldInfo.
    /// </summary>
    [Serializable]
    internal class SerializedField : ISerializableMemberInfo<FieldInfo>
    {
        [field: SerializeField] public SerializedType DeclaringType { get; private set; }

        [field: SerializeField] public string FieldName { get; private set; }

        [field: SerializeField] public BindingFlags BindingFlags { get; private set; }

        /// <inheritdoc />
        public void Serialize(FieldInfo memberInfo, TypeStore typeStore)
        {
            DeclaringType = new SerializedType().Build(memberInfo.DeclaringType, typeStore);
            FieldName = memberInfo.Name;

            if (memberInfo.IsPublic)
                BindingFlags |= BindingFlags.Public;
            else
                BindingFlags |= BindingFlags.NonPublic;

            if (memberInfo.IsStatic)
                BindingFlags |= BindingFlags.Static;
            else
                BindingFlags |= BindingFlags.Instance;
        }

        /// <inheritdoc />
        public FieldInfo Deserialize(TypeStore typeStore)
        {
            var declaringType = DeclaringType.Deserialize(typeStore);
            Assert.IsNotNull(declaringType, "declaringType != null");
            var fieldInfo = declaringType.GetField(FieldName, BindingFlags);
            return fieldInfo;
        }
    }
}