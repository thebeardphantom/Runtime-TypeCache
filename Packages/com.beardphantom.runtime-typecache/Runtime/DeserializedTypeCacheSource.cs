using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    /// ITypeCacheSource that uses a <see cref="DeserializedTypeCache" /> instance.
    /// </summary>
    [UsedImplicitly]
    internal class DeserializedTypeCacheSource : ITypeCacheSource
    {
        private readonly DeserializedTypeCache _deserializedTypeCache;

        public DeserializedTypeCacheSource(DeserializedTypeCache deserializedTypeCache)
        {
            _deserializedTypeCache = deserializedTypeCache;
        }

        private static IEnumerable<T> GetOrEmpty<T>(IReadOnlyDictionary<Type, IEnumerable<T>> dictionary, Type key)
        {
            return dictionary.TryGetValue(key, out IEnumerable<T> value) ? value : Enumerable.Empty<T>();
        }

        /// <inheritdoc />
        public IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            return GetOrEmpty(_deserializedTypeCache.TypesDerivedFromType, parentType);
        }

        /// <inheritdoc />
        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
        {
            return GetOrEmpty(_deserializedTypeCache.TypesWithAttribute, attributeType);
        }

        /// <inheritdoc />
        public IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType)
        {
            return GetOrEmpty(_deserializedTypeCache.MethodsWithAttribute, attributeType);
        }

        /// <inheritdoc />
        public IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type attributeType)
        {
            return GetOrEmpty(_deserializedTypeCache.PropertiesWithAttribute, attributeType);
        }

        /// <inheritdoc />
        public IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            return GetOrEmpty(_deserializedTypeCache.FieldsWithAttribute, attributeType);
        }
    }
}