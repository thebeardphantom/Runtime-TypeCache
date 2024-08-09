using BeardPhantom.RuntimeTypeCache;
using System.Reflection;
using UnityEngine;

public class CacheTest : MonoBehaviour
{
    #region Properties

    public float Property { private get; set; }

    #endregion

    #region Methods

    private static void TestTypeCache()
    {
        var typesWithAttribute = GlobalTypeCache.GetTypesWithAttribute<SomeAttributeAttribute>();
        foreach (var type in typesWithAttribute)
        {
            Debug.Log($"{type} has attribute {typeof(SomeAttributeAttribute)}");
        }

        var methodsWithAttribute = GlobalTypeCache.GetMethodsWithAttribute<SomeAttributeAttribute>();
        foreach (var method in methodsWithAttribute)
        {
            Debug.Log($"{method} has attribute {typeof(SomeAttributeAttribute)}");
        }

        var fieldsWithAttribute = GlobalTypeCache.GetFieldsWithAttribute<SomeAttributeAttribute>();
        foreach (var field in fieldsWithAttribute)
        {
            Debug.Log($"{field} has attribute {typeof(SomeAttributeAttribute)}");
        }

        var propertiesWithAttribute = GlobalTypeCache.GetPropertiesWithAttribute<SomeAttributeAttribute>();
        foreach (var property in propertiesWithAttribute)
        {
            Debug.Log($"{property} has attribute {typeof(SomeAttributeAttribute)}");
        }
        
        var typesDerivedFrom = GlobalTypeCache.GetTypesDerivedFrom<TestType>();
        foreach (var t in typesDerivedFrom)
        {
            Debug.Log($"{t} is derived from {typeof(TestType)}");
        }
    }

    private void Awake()
    {
        TestPropertyInfo();
        // TestTypeCache();
    }

    private void TestPropertyInfo()
    {
        Property++;
        var property = GetType().GetProperty(nameof(Property), BindingFlags.Instance | BindingFlags.Public);
        Debug.Log($"Property.Get public == {property.GetMethod.IsPublic}, Set public == {property.SetMethod.IsPublic}");
    }

    #endregion
}