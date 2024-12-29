#if UNITY_EDITOR
using System;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    public static class TypeCacheBuilderUtility
    {
        private static Predicate<Type> s_shouldCacheTypeInheritance = DefaultPredicate;

        private static Predicate<Type> s_shouldCacheAttributeTypePredicate = DefaultPredicate;

        /// <summary>
        /// If true predicates will not be invoked when building a <see cref="SerializedTypeCache" />.
        /// </summary>
        public static bool StrictMode { get; set; } = true;

        /// <summary>
        /// If true then test assemblies will be scanned when building a <see cref="SerializedTypeCache" />.
        /// </summary>
        public static bool IncludeTestAssemblies { get; set; }

        public static Predicate<Type> ShouldCacheTypeInheritance
        {
            get => s_shouldCacheTypeInheritance;
            set => s_shouldCacheTypeInheritance = value ?? DefaultPredicate;
        }

        public static Predicate<Type> ShouldCacheAttributeTypePredicate
        {
            get => s_shouldCacheAttributeTypePredicate;
            set => s_shouldCacheAttributeTypePredicate = value ?? DefaultPredicate;
        }

        public static bool DefaultPredicate(Type obj)
        {
            return false;
        }
    }
}
#endif