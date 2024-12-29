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
    /// Stores reflected data from a <see cref="SerializedTypeCache" /> instance.
    /// </summary>
    [UsedImplicitly]
    internal class DeserializedTypeCache
    {
        internal readonly Dictionary<Type, IEnumerable<Type>> TypesWithAttribute;

        internal readonly Dictionary<Type, IEnumerable<Type>> TypesDerivedFromType;

        internal readonly Dictionary<Type, IEnumerable<MethodInfo>> MethodsWithAttribute;

        internal readonly Dictionary<Type, IEnumerable<PropertyInfo>> PropertiesWithAttribute;

        internal readonly Dictionary<Type, IEnumerable<FieldInfo>> FieldsWithAttribute;

        public DeserializedTypeCache(SerializedTypeCache serializedTypeCache)
        {
            TypesDerivedFromType = GetTypesDerivedFromType(serializedTypeCache);
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

        private static Dictionary<Type, IEnumerable<TDeserialized>> MapFromSerialized<TSerialized, TDeserialized>(
            SerializedTypeCache serializedTypeCache,
            List<MemberInfoWithAttribute<TSerialized>> serializedForms)
            where TSerialized : ISerializableMemberInfo<TDeserialized> where TDeserialized : MemberInfo
        {
            var lookup = new Dictionary<Type, IEnumerable<TDeserialized>>();
            foreach (MemberInfoWithAttribute<TSerialized> serializedForm in serializedForms)
            {
                TDeserialized[] members = serializedForm
                    .Matches
                    .Select(m => m.Deserialize(serializedTypeCache.TypeStore))
                    .Where(m => m != null)
                    .ToArray();
                Type attributeType = serializedForm.AttributeType.Deserialize(serializedTypeCache.TypeStore);
                Assert.IsNotNull(attributeType, "attributeType != null");
                lookup.Add(attributeType, members);
            }

            return lookup;
        }

        private Dictionary<Type, IEnumerable<Type>> GetTypesDerivedFromType(SerializedTypeCache serializedTypeCache)
        {
            var lookup = new Dictionary<Type, IEnumerable<Type>>();
            foreach (TypesDerivedFromType serializedForm in serializedTypeCache.TypesDerivedFromType)
            {
                Type parentType = serializedForm.ParentType.Deserialize(serializedTypeCache.TypeStore);
                Type[] derivedTypes = serializedForm
                    .DerivedTypes
                    .Select(dt => dt.Deserialize(serializedTypeCache.TypeStore))
                    .ToArray();
                lookup.Add(parentType, derivedTypes);
            }

            return lookup;
        }
    }
}