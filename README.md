# Runtime TypeCache
Extends Unity's UnityEditor.TypeCache class to support the editor and builds through a singular API.

**It's important to note that Runtime TypeCache does not _bypass_ reflection. It simply caches specific paths to ensure that the minimal amount of reflection calls are ever needed.**
## Installing 
You can install Runtime TypeCache via the "Install package from git URL..." option in Unity's Package Manager using the following URL:
```
https://github.com/thebeardphantom/Runtime-TypeCache.git?path=Packages/com.beardphantom.runtime-typecache
```

## What is it?
The Runtime TypeCache system is designed to give you the same access to information that you get through Unity's editor-only TypeCache API:
- `GetFieldsWithAttribute`
- `GetMethodsWithAttribute`
- `GetTypesWithAttribute`
- `GetTypesDerivedFrom`

In addition, Runtime TypeCache also provides `GetPropertiesWithAttribute`. Please note that accessing property information uses slower behavior _in the editor only_, as this data is not generated and cached until build-time.
## How to use
Using Runtime TypeCache is fairly straightforward:

- If you use the `TypeCacheTarget` attribute on a class that inherits from `Attribute` it will populate the TypeCache with reflection paths for any TypeCache function ending with **WithAttribute**.
- If you use the `TypeCacheTarget` attribute on a class that does not inherit from `Attribute` it will populate the TypeCache with reflection paths for `GetTypesDerivedFrom`.

To access reflection information in the editor or in a build you can use the singular `GlobalTypeCache` class.
## Under the Hood

In the editor, the methods in `GlobalTypeCache` forward (almost) everything to the built-in `UnityEditor.TypeCache`.

When making a build, a `SerializedTypeCacheAsset` is generated, which contains all the information necessary to populate a runtime version of the TypeCache with a minimal amount of reflection. During runtime, `GlobalTypeCache` will then forward all method calls to this generated version of the TypeCache.

## Debugging
When you make a build a `SerializedTypeCacheAsset` is generated in your Assets directory, temporarily added to the PlayerSettings Preloaded Assets, then deleted and cleaned up once the build process is completed. A copy of this asset, as well as a version in JSON format, is stored in your project's Temp folder (this folder is **deleted** when the Editor closes). You can use this to inspect the TypeCache generated for the last build to debug its contents. 
