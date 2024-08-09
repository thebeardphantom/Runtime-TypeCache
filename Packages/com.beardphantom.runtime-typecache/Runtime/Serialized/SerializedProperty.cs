using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form a reflectable PropertyInfo.
    /// </summary>
    [Serializable]
    internal class SerializedProperty : ISerializableMemberInfo<PropertyInfo>
    {
        [field: SerializeField] public SerializedType DeclaringType { get; private set; }

        [field: SerializeField] public string PropertyName { get; private set; }

        [field: SerializeField] public BindingFlags BindingFlags { get; private set; }

        /// <inheritdoc />
        public void Serialize(PropertyInfo memberInfo, TypeStore typeStore)
        {
            DeclaringType = new SerializedType().Build(memberInfo.DeclaringType, typeStore);
            PropertyName = memberInfo.Name;

            if (IsPropertyPublic(memberInfo))
                BindingFlags |= BindingFlags.Public;
            else
                BindingFlags |= BindingFlags.NonPublic;

            if (IsPropertyStatic(memberInfo))
                BindingFlags |= BindingFlags.Static;
            else
                BindingFlags |= BindingFlags.Instance;
        }

        /// <inheritdoc />
        public PropertyInfo Deserialize(TypeStore typeStore)
        {
            var declaringType = DeclaringType.Deserialize(typeStore);
            Assert.IsNotNull(declaringType, "declaringType != null");
            var propertyInfo = declaringType.GetProperty(PropertyName, BindingFlags);
            return propertyInfo;
        }

        private bool IsPropertyPublic(PropertyInfo memberInfo)
        {
            var canRead = memberInfo.CanRead;
            var canWrite = memberInfo.CanWrite;
            if (canRead && canWrite) return memberInfo.GetMethod.IsPublic || memberInfo.SetMethod.IsPublic;

            return canRead ? memberInfo.GetMethod.IsPublic : memberInfo.SetMethod.IsPublic;
        }

        private bool IsPropertyStatic(PropertyInfo memberInfo)
        {
            var canRead = memberInfo.CanRead;
            var canWrite = memberInfo.CanWrite;
            if (canRead && canWrite) return memberInfo.GetMethod.IsStatic || memberInfo.SetMethod.IsStatic;

            return canRead ? memberInfo.GetMethod.IsStatic : memberInfo.SetMethod.IsStatic;
        }
    }
}