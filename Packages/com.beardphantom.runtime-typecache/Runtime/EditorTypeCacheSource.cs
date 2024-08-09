#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    ///     ITypeCacheSource that uses the standard <see cref="UnityEditor.TypeCache" /> API.
    /// </summary>
    internal class EditorTypeCacheSource : ITypeCacheSource
    {
        /// <inheritdoc />
        public IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            return TypeCache.GetTypesDerivedFrom(parentType);
        }

        /// <inheritdoc />
        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
        {
            return TypeCache.GetTypesWithAttribute(attributeType);
        }

        /// <inheritdoc />
        public IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType)
        {
            return TypeCache.GetMethodsWithAttribute(attributeType);
        }

        /// <inheritdoc />
        public IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type attributeType)
        {
            const BindingFlags AllProperties = BindingFlags.Public
                                               | BindingFlags.Instance
                                               | BindingFlags.NonPublic
                                               | BindingFlags.Static;

            foreach (var t in this.GetTypesDerivedFrom<object>())
            foreach (var property in t.GetProperties(AllProperties))
            {
                if (property.GetCustomAttribute(attributeType) == null) continue;

                yield return property;
            }
        }

        /// <inheritdoc />
        public IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            return TypeCache.GetFieldsWithAttribute(attributeType);
        }
    }
}
#endif