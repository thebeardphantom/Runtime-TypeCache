using System;

namespace BeardPhantom.RuntimeTypeCache
{
    /// <summary>
    ///     <para>
    ///         If used on classes inheriting from Attribute, will cache results for
    ///         <see cref="GlobalTypeCache.GetTypesWithAttribute" />,
    ///         <see cref="GlobalTypeCache.GetMethodsWithAttribute" />,
    ///         <see cref="GlobalTypeCache.GetPropertiesWithAttribute" />,
    ///         and <see cref="GlobalTypeCache.GetFieldsWithAttribute" /> that use said Attribute.
    ///     </para>
    ///     <para>
    ///         If used on classes NOT inheriting from Attribute, will cache results for
    ///     </para>
    ///     <see cref="GlobalTypeCache.GetTypesDerivedFrom" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class TypeCacheTargetAttribute : Attribute
    {
    }
}