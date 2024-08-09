#if UNITY_EDITOR
using System;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    public static class TypeCacheBuilderUtility
    {
        public static bool DefaultPredicate(Type obj)
        {
            return false;
        }

        private static Predicate<Type> s_shouldCacheTypeInheritance = DefaultPredicate;

        private static Predicate<Type> s_shouldCacheAttributeTypePredicate = DefaultPredicate;

        public static bool StrictMode { get; set; } = true;

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
    }
}
#endif