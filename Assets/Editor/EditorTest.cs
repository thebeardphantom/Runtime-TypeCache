using UnityEditor;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorTest
    {
        #region Constructors

        // ReSharper disable once EmptyConstructor
        static EditorTest()
        {
            // TypeCacheBuilderUtility.StrictMode = false;
            // TypeCacheBuilderUtility.ShouldCacheAttributeTypePredicate = type => true;
            // TypeCacheBuilderUtility.ShouldCacheTypeInheritance = type => true;
        }

        #endregion
    }
}