using BeardPhantom.RuntimeTypeCache;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

public class CacheTests
{
    [Test]
    public void GetTypesDerivedFrom()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<TestType>().ToArray();
        Assert.AreEqual(1, types.Length);
    }

    [Test]
    public void GetTypesWithAttribute()
    {
        Type[] types = GlobalTypeCache.GetTypesWithAttribute<SomeAttributeAttribute>().ToArray();
        Assert.AreEqual(1, types.Length);
    }

    [Test]
    public void GetFieldsWithAttribute()
    {
        FieldInfo[] fields = GlobalTypeCache.GetFieldsWithAttribute<SomeAttributeAttribute>().ToArray();
        Assert.AreEqual(8, fields.Length);
    }

    [Test]
    public void GetPropertiesWithAttribute()
    {
        PropertyInfo[] properties = GlobalTypeCache.GetPropertiesWithAttribute<SomeAttributeAttribute>().ToArray();
        Assert.AreEqual(8, properties.Length);
    }

    [Test]
    public void GetMethodsWithAttribute()
    {
        MethodInfo[] methods = GlobalTypeCache.GetMethodsWithAttribute<SomeAttributeAttribute>().ToArray();
        Assert.AreEqual(8, methods.Length);
    }
}