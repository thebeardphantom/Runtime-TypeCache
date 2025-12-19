using BeardPhantom.RuntimeTypeCache;

[TypeCacheTarget]
public interface ITestInterface { }

[TypeCacheTarget]
public interface ITestInterface<T> : ITestInterface { }