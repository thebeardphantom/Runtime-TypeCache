using BeardPhantom.RuntimeTypeCache;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
    public void GetTypesDerivedFromInterface()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<ITestInterface>().ToArray();
        Assert.AreEqual(5, types.Length);
    }

    [Test]
    public void GetTypesDerivedFromGenericInterface()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<ITestInterface<int>>().ToArray();
        Assert.AreEqual(1, types.Length);
    }

    [Test]
    public void GetTypesDerivedFromGenericInterfaceCurious()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<ITestInterface<TestTypeFromInterfaceCurious>>().ToArray();
        Assert.AreEqual(1, types.Length);
    }
    
    [Test]
    public void GetTypesDerivedFromGenericInterfaceRecursive1()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<ITestInterface<ITestInterface<int>>>().ToArray();
        Assert.AreEqual(1, types.Length);
    }
    
    [Test]
    public void GetTypesDerivedFromGenericInterfaceRecursive2()
    {
        Type[] types = GlobalTypeCache.GetTypesDerivedFrom<ITestInterface<ITestInterface>>().ToArray();
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