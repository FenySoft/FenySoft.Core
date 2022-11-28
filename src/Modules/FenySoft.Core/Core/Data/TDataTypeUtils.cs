using System.Reflection;
using FenySoft.Core.Extensions;

using System.Collections.Concurrent;

namespace FenySoft.Core.Data
{
    public static class TDataTypeUtils
    {
        //dataType -> anonymous type
        private static readonly ConcurrentDictionary<TDataType, Type> cacheAnonymousTypes = new ConcurrentDictionary<TDataType, Type>();

        //Type -> true/false
        private static readonly ConcurrentDictionary<Type, bool> cacheIsAnonymousTypes = new ConcurrentDictionary<Type, bool>();

        public static IEnumerable<MemberInfo> GetPublicMembers(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            var members = type.GetPublicReadWritePropertiesAndFields();
            if (membersOrder == null)
                return members;

            return members.Where(x => membersOrder(type, x) >= 0).OrderBy(x => membersOrder(type, x));
        }

        public static bool IsAllPrimitive(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            if (TDataType.IsPrimitiveType(type))
                return true;

            if (type.IsArray || type.IsList() || type.IsDictionary() || type.IsKeyValuePair() || type.IsNullable() || type == typeof(Guid))
                return false;

            foreach (var member in GetPublicMembers(type, membersOrder))
            {
                if (!TDataType.IsPrimitiveType(member.GetPropertyOrFieldType()))
                    return false;
            }

            return true;
        }

        private static bool InternalIsAnonymousType(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            if (TDataType.IsPrimitiveType(type))
                return true;

            if (type.IsEnum || type == typeof(Guid))
                return false;

            if (type.IsNullable())
                return false;

            if (type.IsKeyValuePair())
                return false;

            if (type.IsArray)
                return InternalIsAnonymousType(type.GetElementType(), membersOrder);

            if (type.IsList())
                return InternalIsAnonymousType(type.GetGenericArguments()[0], membersOrder);

            if (type.IsDictionary())
                return InternalIsAnonymousType(type.GetGenericArguments()[0], membersOrder) && InternalIsAnonymousType(type.GetGenericArguments()[1], membersOrder);

            if (type.IsInheritInterface(typeof(ISlots)))
            {
                foreach (var slotType in GetPublicMembers(type, membersOrder))
                {
                    if (!InternalIsAnonymousType(slotType.GetPropertyOrFieldType(), membersOrder))
                        return false;
                }
            }

            if ((type.IsClass || type.IsStruct()) && !type.IsInheritInterface(typeof(ISlots)))
                return false;

            return true;
        }

        public static bool IsAnonymousType(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            if (membersOrder != null)
                return InternalIsAnonymousType(type, membersOrder);

            return cacheIsAnonymousTypes.GetOrAdd(type, (x) => InternalIsAnonymousType(x, null));
        }

        public static Type Anonymous(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            TDataType dataType = BuildDataType(type, membersOrder);

            return BuildType(dataType);
        }

        private static TDataType BuildDataType(Type type, Func<Type, MemberInfo, int> membersOrder, HashSet<Type> cycleCheck)
        {
            if (TDataType.IsPrimitiveType(type))
                return TDataType.FromPrimitiveType(type);

            if (type.IsEnum)
                return TDataType.FromPrimitiveType(type.GetEnumUnderlyingType());

            if (type == typeof(Guid))
                return TDataType.ByteArray;

            if (type.IsKeyValuePair())
            {
                return TDataType.Slots(
                    BuildDataType(type.GetGenericArguments()[0], membersOrder, cycleCheck),
                    BuildDataType(type.GetGenericArguments()[1], membersOrder, cycleCheck));
            }

            if (type.IsArray)
                return TDataType.Array(BuildDataType(type.GetElementType(), membersOrder, cycleCheck));

            if (type.IsList())
                return TDataType.List(BuildDataType(type.GetGenericArguments()[0], membersOrder, cycleCheck));

            if (type.IsDictionary())
            {
                return TDataType.Dictionary(
                    BuildDataType(type.GetGenericArguments()[0], membersOrder, cycleCheck),
                    BuildDataType(type.GetGenericArguments()[1], membersOrder, cycleCheck));
            }

            if (type.IsNullable())
                return TDataType.Slots(BuildDataType(type.GetGenericArguments()[0], membersOrder, cycleCheck));

            List<TDataType> slots = new List<TDataType>();
            foreach (var member in GetPublicMembers(type, membersOrder))
            {
                var memberType = member.GetPropertyOrFieldType();

                if (cycleCheck.Contains(memberType))
                    throw new NotSupportedException(String.Format("Type {0} has cycle declaration.", memberType));

                cycleCheck.Add(memberType);
                TDataType slot = BuildDataType(memberType, membersOrder, cycleCheck);
                cycleCheck.Remove(memberType);
                slots.Add(slot);
            }

            if (slots.Count == 0)
                throw new NotSupportedException(String.Format("{0} do not contains public read/writer properties and fields", type));

            return TDataType.Slots(slots.ToArray());
        }

        public static TDataType BuildDataType(Type type, Func<Type, MemberInfo, int> membersOrder = null)
        {
            if (TDataType.IsPrimitiveType(type) || type.IsEnum || type == typeof(Guid) || type.IsKeyValuePair() || type.IsArray || type.IsList() || type.IsDictionary() || type.IsNullable())
                return BuildDataType(type, membersOrder, new HashSet<Type>());

            List<TDataType> slots = new List<TDataType>();
            foreach (var member in GetPublicMembers(type, membersOrder))
                slots.Add(BuildDataType(member.GetPropertyOrFieldType(), membersOrder, new HashSet<Type>()));

            return TDataType.Slots(slots.ToArray());
        }

        private static Type InternalBuildType(TDataType dataType)
        {
            if (dataType.IsPrimitive)
                return dataType.PrimitiveType;

            if (dataType.IsArray)
                return InternalBuildType(dataType[0]).MakeArrayType();

            if (dataType.IsList)
                return typeof(List<>).MakeGenericType(InternalBuildType(dataType[0]));

            if (dataType.IsDictionary)
                return typeof(Dictionary<,>).MakeGenericType(InternalBuildType(dataType[0]), InternalBuildType(dataType[1]));

            if (dataType.IsSlots)
                return TSlotsBuilder.BuildType(dataType.Select(x => InternalBuildType(x)).ToArray());

            throw new NotSupportedException();
        }

        public static Type BuildType(TDataType dataType)
        {
            if (dataType.IsPrimitive)
                return dataType.PrimitiveType;

            return cacheAnonymousTypes.GetOrAdd(dataType, (x) => InternalBuildType(x));
        }
    }
}
