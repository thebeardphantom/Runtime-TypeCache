using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    ///     Defines methods for resolving TypeCache queries.
    /// </summary>
    internal interface ITypeCacheSource
    {
        /// <summary>
        ///     Retrieves types derived from <paramref name="parentType" />.
        /// </summary>
        IEnumerable<Type> GetTypesDerivedFrom(Type parentType);

        /// <summary>
        ///     Retrieves types decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        IEnumerable<Type> GetTypesWithAttribute(Type attributeType);

        /// <summary>
        ///     Retrieves methods decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType);

        /// <summary>
        ///     Retrieves properties decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type attributeType);

        /// <summary>
        ///     Retrieves fields decorated with the attribute of type <paramref name="attributeType" />.
        /// </summary>
        IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType);
    }
}