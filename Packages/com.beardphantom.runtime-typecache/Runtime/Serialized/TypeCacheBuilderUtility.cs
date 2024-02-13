#if UNITY_EDITOR
using System;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    public static class TypeCacheBuilderUtility
    {
        #region Fields

        private static Predicate<Type> _shouldCacheTypeInheritance = DefaultPredicate;

        private static Predicate<Type> _shouldCacheAttributeTypePredicate = DefaultPredicate;

        #endregion

        #region Properties

        public static bool StrictMode { get; set; } = true;

        public static Predicate<Type> ShouldCacheTypeInheritance
        {
            get => _shouldCacheTypeInheritance;
            set => _shouldCacheTypeInheritance = value ?? DefaultPredicate;
        }

        public static Predicate<Type> ShouldCacheAttributeTypePredicate
        {
            get => _shouldCacheAttributeTypePredicate;
            set => _shouldCacheAttributeTypePredicate = value ?? DefaultPredicate;
        }

        #endregion

        #region Methods

        public static bool DefaultPredicate(Type obj)
        {
            return false;
        }

        #endregion
    }
}
#endif