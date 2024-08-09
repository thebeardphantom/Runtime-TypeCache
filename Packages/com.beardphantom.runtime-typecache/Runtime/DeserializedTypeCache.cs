using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeardPhantom.RuntimeTypeCache.Serialized;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    ///     Stores reflected data from a <see cref="SerializedTypeCache" /> instance.
    /// </summary>
    [UsedImplicitly]
    internal class DeserializedTypeCache
    {
        public DeserializedTypeCache(SerializedTypeCache serializedTypeCache)
        {
            TypesWithAttribute = MapFromSerialized<SerializedType, Type>(
                serializedTypeCache,
                serializedTypeCache.TypesWithAttribute);
            MethodsWithAttribute = MapFromSerialized<SerializedMethod, MethodInfo>(
                serializedTypeCache,
                serializedTypeCache.MethodsWithAttribute);
            PropertiesWithAttribute = MapFromSerialized<SerializedProperty, PropertyInfo>(
                serializedTypeCache,
                serializedTypeCache.PropertiesWithAttribute);
            FieldsWithAttribute = MapFromSerialized<SerializedField, FieldInfo>(
                serializedTypeCache,
                serializedTypeCache.FieldsWithAttribute);
        }

        private static Dictionary<Type, T2[]> MapFromSerialized<T1, T2>(
            SerializedTypeCache serializedTypeCache,
            List<MemberInfoWithAttribute<T1>> serializedForms)
            where T1 : ISerializableMemberInfo<T2> where T2 : MemberInfo
        {
            var lookup = new Dictionary<Type, T2[]>();
            foreach (var serializedForm in serializedForms)
            {
                var members = serializedForm.Matches.Select(m => m.Deserialize(serializedTypeCache.TypeStore))
                    .Where(m => m != null)
                    .ToArray();
                var attributeType = serializedForm.AttributeType.Deserialize(serializedTypeCache.TypeStore);
                Assert.IsNotNull(attributeType, "attributeType != null");
                lookup.Add(attributeType, members);
            }

            return lookup;
        }

        internal readonly Dictionary<Type, Type[]> TypesWithAttribute;

        internal readonly Dictionary<Type, MethodInfo[]> MethodsWithAttribute;

        internal readonly Dictionary<Type, PropertyInfo[]> PropertiesWithAttribute;

        internal readonly Dictionary<Type, FieldInfo[]> FieldsWithAttribute;
    }
}