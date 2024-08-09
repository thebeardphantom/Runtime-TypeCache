using System;
using System.Collections.Generic;
using System.Reflection;
#if !UNITY_EDITOR
using BeardPhantom.RuntimeTypeCache.Serialized;
#endif

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    ///     The entry-point API for making queries.
    /// </summary>
    public static class GlobalTypeCache
    {
        private static readonly ITypeCacheSource Source;

        static GlobalTypeCache()
        {
#if UNITY_EDITOR
            Source = new EditorTypeCacheSource();
#else
            var serializedTypeCacheAsset = SerializedTypeCacheAsset.GetInstance();
            var deserializedTypeCache = new DeserializedTypeCache(serializedTypeCacheAsset.SerializedTypeCache);
            Source = new DeserializedTypeCacheSource(deserializedTypeCache);
#endif
        }

        /// <summary>
        ///     Retrieves types derived from parent type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesDerivedFrom<T>()
        {
            return Source.GetTypesDerivedFrom<T>();
        }

        /// <summary>
        ///     Retrieves types derived from <paramref name="parentType" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            return Source.GetTypesDerivedFrom(parentType);
        }

        /// <summary>
        ///     Retrieves types decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute<T>()
        {
            return Source.GetTypesWithAttribute<T>();
        }

        /// <summary>
        ///     Retrieves types decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
        {
            return Source.GetTypesWithAttribute(attributeType);
        }

        /// <summary>
        ///     Retrieves methods decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>()
        {
            return Source.GetMethodsWithAttribute<T>();
        }

        /// <summary>
        ///     Retrieves methods decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType)
        {
            return Source.GetMethodsWithAttribute(attributeType);
        }

        /// <summary>
        ///     Retrieves properties decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>()
        {
            return Source.GetPropertiesWithAttribute<T>();
        }

        /// <summary>
        ///     Retrieves properties decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type attributeType)
        {
            return Source.GetPropertiesWithAttribute(attributeType);
        }

        /// <summary>
        ///     Retrieves fields decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<T>()
        {
            return Source.GetFieldsWithAttribute<T>();
        }

        /// <summary>
        ///     Retrieves fields decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            return Source.GetFieldsWithAttribute(attributeType);
        }
    }
}