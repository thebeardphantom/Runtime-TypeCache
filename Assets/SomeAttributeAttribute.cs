using BeardPhantom.RuntimeTypeCache;
using System;

[TypeCacheTarget]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class SomeAttributeAttribute : Attribute { }