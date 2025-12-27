// #undef UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
#endif

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    [Serializable]
    internal partial class SerializedTypeCache
    {
        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedType>> TypesWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedMethod>> MethodsWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedProperty>> PropertiesWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<MemberInfoWithAttribute<SerializedField>> FieldsWithAttribute { get; private set; } = new();

        [field: SerializeField]
        public List<TypesDerivedFromType> TypesDerivedFromType { get; private set; } = new();

        [field: SerializeField]
        public TypeStore TypeStore { get; private set; } = new();
    }

#if UNITY_EDITOR
    internal partial class SerializedTypeCache
    {
        private static HashSet<string> s_playerAssemblies;

        private static bool HasFlag(AttributeTargets flag, AttributeTargets value)
        {
            return (flag & value) != 0;
        }

        private static bool IsInEditorAssembly(Type type)
        {
            AssembliesType playerAssembliesType = TypeCacheBuilderUtility.IncludeTestAssemblies
                ? AssembliesType.Player
                : AssembliesType.PlayerWithoutTestAssemblies;
            s_playerAssemblies ??= CompilationPipeline.GetAssemblies(playerAssembliesType)
                .Select(a => a.name)
                .Concat(
                    CompilationPipeline
                        .GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources.UnityEngine)
                        .Select(Path.GetFileNameWithoutExtension)
                        .Where(n => !n.StartsWith("UnityEditor")))
                .Concat(
                    CompilationPipeline
                        .GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources.SystemAssembly)
                        .Select(Path.GetFileNameWithoutExtension))
                .Concat(
                    CompilationPipeline
                        .GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources.UserAssembly)
                        .Select(Path.GetFileNameWithoutExtension))
                .Distinct()
                .ToHashSet();

            return !s_playerAssemblies.Contains(type.Assembly.GetName().Name)
                   || type.FullName.StartsWith("UnityEditorInternal");
        }

        internal void Regenerate()
        {
            TypesWithAttribute.Clear();
            MethodsWithAttribute.Clear();
            FieldsWithAttribute.Clear();
            TypesDerivedFromType.Clear();

            Type[] targetTypesAll = TypeCache.GetTypesWithAttribute<TypeCacheTargetAttribute>().ToArray();
            Type[] targetTypesAttributes = targetTypesAll.Where(typeof(Attribute).IsAssignableFrom).ToArray();

            var context = new CacheContext(new EditorTypeCacheSource());
            CacheAllAttributeUsages(targetTypesAttributes, context);

            Type[] targetTypesNonAttributes = targetTypesAll.Except(targetTypesAttributes).ToArray();
            CacheAllTypeInheritances(targetTypesNonAttributes, context);
        }

        private void CacheAllAttributeUsages(IEnumerable<Type> targetTypes, CacheContext context)
        {
            /*
             * Only cache attributes that are decorated with the TypeCachedAttribute attribute, OR if:
             * 1. StrictMode is disabled.
             * 2. They aren't in editor assemblies.
             * AND
             * 3. They pass the predicate filter.
             */
            foreach (Type type in targetTypes)
            {
                CacheAttributeUsage(type, context);
            }

            if (TypeCacheBuilderUtility.StrictMode)
            {
                return;
            }

            TypeCache.TypeCollection attributeTypes = TypeCache.GetTypesDerivedFrom<Attribute>();
            foreach (Type type in attributeTypes)
            {
                if (IsInEditorAssembly(type))
                {
                    continue;
                }

                bool passesPredicate = TypeCacheBuilderUtility.ShouldCacheAttributeTypePredicate(type);
                if (!passesPredicate)
                {
                    continue;
                }

                CacheAttributeUsage(type, context);
            }
        }

        private void CacheAttributeUsage(Type attributeType, CacheContext context)
        {
            var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
            SerializedType serializedAttributeType = new SerializedType().Build(attributeType, TypeStore);

            // Check for Types with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Class) || HasFlag(attributeUsage.ValidOn, AttributeTargets.Struct))
            {
                Type[] matches = context
                    .TypeCacheSource
                    .GetTypesWithAttribute(attributeType)
                    .Where(t => !IsInEditorAssembly(t))
                    .ToArray();
                if (matches.Length > 0)
                {
                    SerializedType[] matchesSerialized = matches
                        .Select(memberInfo => new SerializedType().Build(memberInfo, TypeStore))
                        .ToArray();
                    TypesWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedType>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized,
                        });
                }
            }

            // Check for Methods with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Method))
            {
                MethodInfo[] matches = context.TypeCacheSource.GetMethodsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType))
                    .ToArray();
                if (matches.Length > 0)
                {
                    SerializedMethod[] matchesSerialized = matches
                        .Select(memberInfo => new SerializedMethod().Build(memberInfo, TypeStore))
                        .ToArray();
                    MethodsWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedMethod>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized,
                        });
                }
            }

            // Check for Fields with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Property))
            {
                PropertyInfo[] matches = context.TypeCacheSource.GetPropertiesWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType))
                    .ToArray();
                if (matches.Length > 0)
                {
                    SerializedProperty[] matchesSerialized = matches
                        .Select(memberInfo => new SerializedProperty().Build(memberInfo, TypeStore))
                        .ToArray();
                    PropertiesWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedProperty>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized,
                        });
                }
            }

            // Check for Fields with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Field))
            {
                FieldInfo[] matches = context.TypeCacheSource.GetFieldsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType))
                    .ToArray();
                if (matches.Length > 0)
                {
                    SerializedField[] matchesSerialized = matches
                        .Select(memberInfo => new SerializedField().Build(memberInfo, TypeStore))
                        .ToArray();
                    FieldsWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedField>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized,
                        });
                }
            }
        }

        private void CacheAllTypeInheritances(IEnumerable<Type> targetTypes, CacheContext context)
        {
            /*
             * Only cache types that are decorated with the TypeCachedType attribute, OR if:
             * 1. StrictMode is disabled.
             * 2. They aren't in editor assemblies.
             * AND
             * 3. They pass the predicate filter.
             */
            foreach (Type typeCachedType in targetTypes)
            {
                CacheTypeInheritance(typeCachedType, context);
            }

            if (TypeCacheBuilderUtility.StrictMode)
            {
                return;
            }

            IEnumerable<Type> allTypes = context.TypeCacheSource.GetTypesDerivedFrom<object>();
            foreach (Type type in allTypes)
            {
                if (IsInEditorAssembly(type))
                {
                    continue;
                }

                bool passesPredicate = TypeCacheBuilderUtility.ShouldCacheTypeInheritance(type);
                if (!passesPredicate)
                {
                    continue;
                }

                CacheTypeInheritance(type, context);
            }
        }

        private void CacheTypeInheritance(Type parentType, CacheContext context)
        {
            if (!context.TryVisitType(parentType))
            {
                return;
            }

            Type[] derivedTypes = context
                .TypeCacheSource
                .GetTypesDerivedFrom(parentType)
                .Where(t => !IsInEditorAssembly(t))
                .ToArray();
            if (derivedTypes.Length == 0)
            {
                return;
            }

            List<SerializedType> serializedDerivedTypes = derivedTypes.Select(t => new SerializedType().Build(t, TypeStore)).ToList();
            var serializedParentType = new SerializedType();
            serializedParentType.Serialize(parentType, TypeStore);
            TypesDerivedFromType.Add(
                new TypesDerivedFromType
                {
                    ParentType = serializedParentType,
                    DerivedTypes = serializedDerivedTypes,
                });

            if (!parentType.IsGenericTypeDefinition)
            {
                return;
            }

            foreach (Type derivedType in derivedTypes)
            {
                IEnumerable<Type> collectedTypes = parentType.IsInterface
                    ? CollectInterfacesInTypeHierarchy(derivedType)
                    : CollectTypeHierarchy(derivedType);
                Type[] interfaces = collectedTypes
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == parentType)
                    .Distinct()
                    .ToArray();
                foreach (Type interfaceType in interfaces)
                {
                    CacheTypeInheritance(interfaceType, context);
                }
            }
        }

        private IEnumerable<Type> CollectInterfacesInTypeHierarchy(Type type)
        {
            Type currentType = type;
            while (currentType != null)
            {
                Type[] interfaces = currentType.GetInterfaces();
                foreach (Type @interface in interfaces)
                {
                    yield return @interface;
                }

                currentType = currentType.BaseType;
            }
        }

        private IEnumerable<Type> CollectTypeHierarchy(Type type)
        {
            Type currentType = type.BaseType;
            while (currentType != null && currentType != typeof(object))
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }
    }
#endif
}