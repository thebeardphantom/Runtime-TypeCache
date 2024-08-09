using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    /// <summary>
    ///     Represents in serializable form a reflectable MethodInfo.
    /// </summary>
    [Serializable]
    internal class SerializedMethod : ISerializableMemberInfo<MethodInfo>
    {
        [field: SerializeField] public SerializedType DeclaringType { get; private set; }

        [field: SerializeField] public string MethodName { get; private set; }

        [field: SerializeField] public BindingFlags BindingFlags { get; private set; }

        /// <inheritdoc />
        public void Serialize(MethodInfo memberInfo, TypeStore typeStore)
        {
            DeclaringType = new SerializedType().Build(memberInfo.DeclaringType, typeStore);

            MethodName = memberInfo.Name;

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
        public MethodInfo Deserialize(TypeStore typeStore)
        {
            var declaringType = DeclaringType.Deserialize(typeStore);
            Assert.IsNotNull(declaringType, "declaringType != null");
            var methodInfo = declaringType.GetMethod(MethodName, BindingFlags);
            return methodInfo;
        }
    }
}