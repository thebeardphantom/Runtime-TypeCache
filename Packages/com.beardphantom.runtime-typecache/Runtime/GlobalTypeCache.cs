#if UNITY_EDITOR
#define EDITOR
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
#if !EDITOR
using BeardPhantom.RuntimeTypeCache.Serialized;
#endif

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    /// The entry-point API for making queries.
    /// </summary>
    public static class GlobalTypeCache
    {
        #region Fields

        private static readonly ITypeCacheSource _source;

        #endregion

        #region Constructors

        static GlobalTypeCache()
        {
#if EDITOR
            _source = new EditorTypeCacheSource();
#else
            var serializedTypeCacheAsset = SerializedTypeCacheAsset.Instance;
            var deserializedTypeCache = new DeserializedTypeCache(serializedTypeCacheAsset.SerializedTypeCache);
            _source = new DeserializedTypeCacheSource(deserializedTypeCache);
#endif
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves types derived from parent type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesDerivedFrom<T>()
        {
            return _source.GetTypesDerivedFrom<T>();
        }

        /// <summary>
        /// Retrieves types derived from <paramref name="parentType" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            return _source.GetTypesDerivedFrom(parentType);
        }

        /// <summary>
        /// Retrieves types decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute<T>()
        {
            return _source.GetTypesWithAttribute<T>();
        }

        /// <summary>
        /// Retrieves types decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
        {
            return _source.GetTypesWithAttribute(attributeType);
        }

        /// <summary>
        /// Retrieves methods decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>()
        {
            return _source.GetMethodsWithAttribute<T>();
        }

        /// <summary>
        /// Retrieves methods decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType)
        {
            return _source.GetMethodsWithAttribute(attributeType);
        }

        /// <summary>
        /// Retrieves properties decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertysWithAttribute<T>()
        {
            return _source.GetPropertiesWithAttribute<T>();
        }

        /// <summary>
        /// Retrieves properties decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertysWithAttribute(Type attributeType)
        {
            return _source.GetPropertiesWithAttribute(attributeType);
        }

        /// <summary>
        /// Retrieves fields decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<T>()
        {
            return _source.GetFieldsWithAttribute<T>();
        }

        /// <summary>
        /// Retrieves fields decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            return _source.GetFieldsWithAttribute(attributeType);
        }

        #endregion
    }
}