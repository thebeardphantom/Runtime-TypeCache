// ReSharper disable All

using BeardPhantom.RuntimeTypeCache;
using System.Diagnostics.CodeAnalysis;

[SomeAttribute]
[TypeCacheTarget]
public abstract class TestType
{
    #region Fields

    [SomeAttribute]
    public static readonly string PublicStaticReadonlyField;

    [SomeAttribute]
    private static readonly string s_privateStaticReadonlyField;

    [SomeAttribute]
    public static string PublicStaticField;

    [SomeAttribute]
    private static string s_privateStaticField;

    [SomeAttribute]
    public readonly string PublicReadonlyField;

    [SomeAttribute]
    private readonly string _privateReadonlyField;

    [SomeAttribute]
    public string PublicField;

    [SomeAttribute]
    private string _privateField;

    #endregion

    #region Properties

    [SomeAttribute]
    public static int PublicStaticPropertyPublicPublic { get; set; }

    [SomeAttribute]
    public static int PublicStaticPropertyPublicPrivate { get; private set; }

    [SomeAttribute]
    public static int PublicStaticPropertyPrivatePublic { private get; set; }

    [SomeAttribute]
    private static int PrivateStaticProperty { get; set; }

    [SomeAttribute]
    public int PublicPropertyPublicPublic { get; set; }

    [SomeAttribute]
    public int PublicPropertyPublicPrivate { get; private set; }

    [SomeAttribute]
    public int PublicPropertyPrivatePublic { private get; set; }

    [SomeAttribute]
    private int PrivateProperty { get; set; }

    #endregion

    #region Methods

    [SomeAttribute]
    public static void PublicStaticMethod() { }

    [SomeAttribute]
    private static void PrivateStaticMethod() { }

    [SomeAttribute]
    public void PublicMethod() { }

    [SomeAttribute]
    private void PrivateMethod() { }

    [SomeAttribute]
    public abstract void PublicAbstractMethod();

    [SomeAttribute]
    protected abstract void PrivateAbstractMethod();

    [SomeAttribute]
    public virtual void PublicVirtualMethod() { }

    [SomeAttribute]
    protected virtual void PrivateVirtualMethod() { }

    #endregion
}