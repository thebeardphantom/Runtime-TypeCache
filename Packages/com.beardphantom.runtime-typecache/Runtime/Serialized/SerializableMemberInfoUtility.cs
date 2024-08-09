using System.Reflection;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal static class SerializableMemberInfoUtility
    {
        public static T1 Build<T1, T2>(this T1 serializableMemberInfo, T2 memberInfo, TypeStore typeStore)
            where T1 : ISerializableMemberInfo<T2> where T2 : MemberInfo
        {
            serializableMemberInfo.Serialize(memberInfo, typeStore);
            return serializableMemberInfo;
        }
    }
}