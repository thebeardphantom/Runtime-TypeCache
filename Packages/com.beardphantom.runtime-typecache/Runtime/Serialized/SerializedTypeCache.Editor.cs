#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal partial class SerializedTypeCache
    {
        private static HashSet<string> s_playerAssemblies;

        private static bool HasFlag(AttributeTargets flag, AttributeTargets value)
        {
            return (flag & value) != 0;
        }

        private static bool IsInEditorAssembly(Type type)
        {
            s_playerAssemblies ??= CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
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

            var targetTypesAll = TypeCache.GetTypesWithAttribute<TypeCacheTargetAttribute>().ToArray();
            var targetTypesAttributes = targetTypesAll.Where(typeof(Attribute).IsAssignableFrom).ToArray();
            var typeCacheSource = new EditorTypeCacheSource();
            CacheAllAttributeUsages(targetTypesAttributes, typeCacheSource);

            var targetTypesNonAttributes = targetTypesAll.Except(targetTypesAttributes).ToArray();
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
            foreach (var type in targetTypes) CacheAttributeUsage(type, typeCacheSource);

            if (TypeCacheBuilderUtility.StrictMode) return;

            var attributeTypes = TypeCache.GetTypesDerivedFrom<Attribute>();
            foreach (var type in attributeTypes)
            {
                if (IsInEditorAssembly(type)) continue;

                var passesPredicate = TypeCacheBuilderUtility.ShouldCacheAttributeTypePredicate(type);
                if (!passesPredicate) continue;

                CacheAttributeUsage(type, typeCacheSource);
            }
        }

        private void CacheAttributeUsage(Type attributeType, ITypeCacheSource typeCacheSource)
        {
            var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
            var serializedAttributeType = new SerializedType().Build(attributeType, TypeStore);

            // Check for Types with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Class) ||
                HasFlag(attributeUsage.ValidOn, AttributeTargets.Struct))
            {
                var matches = typeCacheSource.GetTypesWithAttribute(attributeType).Where(t => !IsInEditorAssembly(t));
                if (matches.Any())
                {
                    var matchesSerialized = matches
                        .Select(memberInfo => new SerializedType().Build(memberInfo, TypeStore))
                        .ToList();
                    TypesWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedType>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized
                        });
                }
            }

            // Check for Methods with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Method))
            {
                var matches = typeCacheSource.GetMethodsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    var matchesSerialized = matches
                        .Select(memberInfo => new SerializedMethod().Build(memberInfo, TypeStore))
                        .ToList();
                    MethodsWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedMethod>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized
                        });
                }
            }

            // Check for Fields with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Property))
            {
                var matches = typeCacheSource.GetPropertiesWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    var matchesSerialized = matches
                        .Select(memberInfo => new SerializedProperty().Build(memberInfo, TypeStore))
                        .ToList();
                    PropertiesWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedProperty>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized
                        });
                }
            }

            // Check for Fields with attribute
            if (HasFlag(attributeUsage.ValidOn, AttributeTargets.Field))
            {
                var matches = typeCacheSource.GetFieldsWithAttribute(attributeType)
                    .Where(memberInfo => !IsInEditorAssembly(memberInfo.DeclaringType));
                if (matches.Any())
                {
                    var matchesSerialized = matches
                        .Select(memberInfo => new SerializedField().Build(memberInfo, TypeStore))
                        .ToList();
                    FieldsWithAttribute.Add(
                        new MemberInfoWithAttribute<SerializedField>
                        {
                            AttributeType = serializedAttributeType,
                            Matches = matchesSerialized
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
            foreach (var typeCachedType in targetTypes) CacheTypeInheritance(typeCachedType, typeCacheSource);

            if (TypeCacheBuilderUtility.StrictMode) return;

            var allTypes = typeCacheSource.GetTypesDerivedFrom<object>();
            foreach (var type in allTypes)
            {
                if (IsInEditorAssembly(type)) continue;

                var passesPredicate = TypeCacheBuilderUtility.ShouldCacheTypeInheritance(type);
                if (!passesPredicate) continue;

                CacheTypeInheritance(type, typeCacheSource);
            }
        }

        private void CacheTypeInheritance(Type type, ITypeCacheSource typeCacheSource)
        {
            var derived = typeCacheSource.GetTypesDerivedFrom(type).Where(t => !IsInEditorAssembly(t));
            if (!derived.Any()) return;

            var derivedSerialized = derived.Select(t => new SerializedType().Build(t, TypeStore)).ToList();
            var parentType = new SerializedType();
            parentType.Serialize(type, TypeStore);
            TypesDerivedFromType.Add(
                new TypesDerivedFromType
                {
                    ParentType = parentType,
                    DerivedTypes = derivedSerialized
                });
        }
    }
}
#endif