using BeardPhantom.RuntimeTypeCache;
using System.Diagnostics.CodeAnalysis;

[SomeAttribute]
[TypeCacheTarget]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public class TestType
{
    #region Fields

    [SomeAttribute]
    private static string _someField3;

    [SomeAttribute]
    public string SomeField;

    [SomeAttribute]
    private string _someField2;

    #endregion

    #region Properties

    [SomeAttribute]
    public static int SomeProperty5 { get; set; }

    [SomeAttribute]
    public static int SomeProperty6 { get; private set; }

    [SomeAttribute]
    public static int SomeProperty7 { private get; set; }

    [SomeAttribute]
    private static int SomeProperty8 { get; set; }

    [SomeAttribute]
    public int SomeProperty { get; set; }

    [SomeAttribute]
    public int SomeProperty2 { get; private set; }

    [SomeAttribute]
    public int SomeProperty3 { private get; set; }

    [SomeAttribute]
    private int SomeProperty4 { get; set; }

    #endregion

    #region Methods

    [SomeAttribute]
    private static void SomeMethod3() { }

    [SomeAttribute]
    public void SomeMethod() { }

    [SomeAttribute]
    private void SomeMethod2() { }

    #endregion
}

public class DerivedType : TestType { }