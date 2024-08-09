using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.RuntimeTypeCache
{
    internal static class TypeCacheSourceUtility
    {
        /// <summary>
        ///     Retrieves types derived from parent type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesDerivedFrom<T>(this ITypeCacheSource typeCacheSource)
        {
            return typeCacheSource.GetTypesDerivedFrom(typeof(T));
        }

        /// <summary>
        ///     Retrieves types decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<Type> GetTypesWithAttribute<T>(this ITypeCacheSource typeCacheSource)
        {
            return typeCacheSource.GetTypesWithAttribute(typeof(T));
        }

        /// <summary>
        ///     Retrieves methods decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(this ITypeCacheSource typeCacheSource)
        {
            return typeCacheSource.GetMethodsWithAttribute(typeof(T));
        }

        /// <summary>
        ///     Retrieves properties decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this ITypeCacheSource typeCacheSource)
        {
            return typeCacheSource.GetPropertiesWithAttribute(typeof(T));
        }

        /// <summary>
        ///     Retrieves fields decorated with the attribute of type <typeparamref name="T" />.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<T>(this ITypeCacheSource typeCacheSource)
        {
            return typeCacheSource.GetFieldsWithAttribute(typeof(T));
        }
    }
}