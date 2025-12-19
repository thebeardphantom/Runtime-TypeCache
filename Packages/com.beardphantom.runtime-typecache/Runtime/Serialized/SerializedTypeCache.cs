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
            var typeCacheSource = new EditorTypeCacheSource();
            CacheAllAttributeUsages(targetTypesAttributes, typeCacheSource);

            Type[] targetTypesNonAttributes = targetTypesAll.Except(targetTypesAttributes).ToArray();
            CacheAllTypeInheritances(targetTypesNonAttributes, typeCacheSource);
        }

        private void CacheAllAttributeUsages(IEnumerable<Type> targetTypes, ITypeCacheSource typeCacheSource)
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
                CacheAttributeUsage(type, typeCacheSource);
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

                CacheAttributeUsage(type, typeCacheSource);
            }
        }

        private void CacheAttributeUsage(Type attributeType, ITypeCacheSource typeCacheSource)
        {
            var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
            SerializedType serializedAttributeType = new SerializedType().Build(attributeType, TypeStore);

            // Check for Types with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Class) || HasFlag(attributeUsage.ValidOn, AttributeTargets.Struct))
            {
                IEnumerable<Type> matches = typeCacheSource.GetTypesWithAttribute(attributeType).Where(t => !IsInEditorAssembly(t));
                if (matches.Any())
                {
                    List<SerializedType> matchesSerialized = matches
                        .Select(memberInfo => new SerializedType().Build(memberInfo, TypeStore))
                        .ToList();
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
                IEnumerable<MethodInfo> matches = typeCacheSource.GetMethodsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    List<SerializedMethod> matchesSerialized = matches
                        .Select(memberInfo => new SerializedMethod().Build(memberInfo, TypeStore))
                        .ToList();
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
                IEnumerable<PropertyInfo> matches = typeCacheSource.GetPropertiesWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    List<SerializedProperty> matchesSerialized = matches
                        .Select(memberInfo => new SerializedProperty().Build(memberInfo, TypeStore))
                        .ToList();
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
                IEnumerable<FieldInfo> matches = typeCacheSource.GetFieldsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    List<SerializedField> matchesSerialized = matches
                        .Select(memberInfo => new SerializedField().Build(memberInfo, TypeStore))
                        .ToList();
                    FieldsWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedField>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized,
                        });
                }
            }
        }

        private void CacheAllTypeInheritances(IEnumerable<Type> targetTypes, ITypeCacheSource typeCacheSource)
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
                CacheTypeInheritance(typeCachedType, typeCacheSource);
            }

            if (TypeCacheBuilderUtility.StrictMode)
            {
                return;
            }

            IEnumerable<Type> allTypes = typeCacheSource.GetTypesDerivedFrom<object>();
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

                CacheTypeInheritance(type, typeCacheSource);
            }
        }

        private void CacheTypeInheritance(Type parentType, ITypeCacheSource typeCacheSource)
        {
            Type[] derivedTypes = typeCacheSource.GetTypesDerivedFrom(parentType).Where(t => !IsInEditorAssembly(t)).ToArray();
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
                Type[] interfaces = derivedType.GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    if (!interfaceType.IsGenericType)
                    {
                        continue;
                    }

                    if (interfaceType.GetGenericTypeDefinition() != parentType)
                    {
                        continue;
                    }

                    CacheTypeInheritance(interfaceType, typeCacheSource);
                }
            }
        }
    }
#endif
}