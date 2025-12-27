using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    /// Uses reflection in the least efficient way possible to guarantee results, if they exist. Should only be used for
    /// debugging purposes.
    /// </summary>
    public class ReflectionTypeCacheSource : ITypeCacheSource
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public
                                                  | System.Reflection.BindingFlags.Instance
                                                  | System.Reflection.BindingFlags.Static
                                                  | System.Reflection.BindingFlags.NonPublic;

        private static IEnumerable<Type> EnumerateAllTypes()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    yield return type;
                }
            }
        }

        public IEnumerable<Type> GetTypesDerivedFrom(Type parentType)
        {
            foreach (Type type in EnumerateAllTypes())
            {
                if (parentType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
        {
            foreach (Type type in EnumerateAllTypes())
            {
                if (type.GetCustomAttribute(attributeType) != null)
                {
                    yield return type;
                }
            }
        }

        public IEnumerable<MethodInfo> GetMethodsWithAttribute(Type attributeType)
        {
            foreach (Type type in EnumerateAllTypes())
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags);
                foreach (MethodInfo method in methods)
                {
                    if (method.GetCustomAttribute(attributeType) != null)
                    {
                        yield return method;
                    }
                }
            }
        }

        public IEnumerable<PropertyInfo> GetPropertiesWithAttribute(Type attributeType)
        {
            foreach (Type type in EnumerateAllTypes())
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags);
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute(attributeType) != null)
                    {
                        yield return property;
                    }
                }
            }
        }

        public IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            foreach (Type type in EnumerateAllTypes())
            {
                FieldInfo[] fields = type.GetFields(BindingFlags);
                foreach (FieldInfo field in fields)
                {
                    if (field.GetCustomAttribute(attributeType) != null)
                    {
                        yield return field;
                    }
                }
            }
        }
    }
}