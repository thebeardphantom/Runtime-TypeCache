using System.Reflection;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal interface ISerializableMemberInfo
    {
    }

    /// <summary>
    ///     Represents an object that can be converted from a MemberInfo to a serializable form, and back again.
    /// </summary>
    internal interface ISerializableMemberInfo<T> : ISerializableMemberInfo where T : MemberInfo
    {
        void Serialize(T memberInfo, TypeStore typeStore);

        T Deserialize(TypeStore typeStore);
    }
}