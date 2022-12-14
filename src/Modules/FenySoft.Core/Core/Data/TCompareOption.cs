using FenySoft.Core.Comparers;
using FenySoft.Core.Extensions;

using System.Reflection;

namespace FenySoft.Core.Data
{
    public class TCompareOption : IEquatable<TCompareOption>
    {
        public readonly TSortOrder SortOrder;
        public readonly TByteOrder ByteOrder;
        public readonly bool IgnoreCase;

        private TCompareOption(TSortOrder sortOrder, TByteOrder byteOrder, bool ignoreCase)
        {
            SortOrder = sortOrder;
            ByteOrder = byteOrder;
            IgnoreCase = ignoreCase;
        }

        public TCompareOption(TSortOrder sortOrder)
            : this(sortOrder, TByteOrder.Unspecified, false)
        {
        }

        public TCompareOption(TSortOrder sortOrder, TByteOrder byteOrder)
            : this(sortOrder, byteOrder, false)
        {
        }

        public TCompareOption(TByteOrder byteOrder)
            : this(TSortOrder.Ascending, byteOrder)
        {
        }

        public TCompareOption(TSortOrder sortOrder, bool ignoreCase)
            : this(sortOrder, TByteOrder.Unspecified, ignoreCase)
        {
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)SortOrder);
            writer.Write((byte)ByteOrder);
            writer.Write(IgnoreCase);
        }

        public static TCompareOption Deserialize(BinaryReader reader)
        {
            var sortOrder = (TSortOrder)reader.ReadByte();
            var byteOrder = (TByteOrder)reader.ReadByte();
            var ignoreCase = reader.ReadBoolean();

            return new TCompareOption(sortOrder, byteOrder, ignoreCase);
        }

        public bool Equals(TCompareOption other)
        {
            return SortOrder == other.SortOrder && ByteOrder == other.ByteOrder && IgnoreCase == other.IgnoreCase;
        }

        #region Utils

        public static TCompareOption GetDefaultCompareOption(Type type)
        {
            if (type == typeof(byte[]))
                return new TCompareOption(TSortOrder.Ascending, TByteOrder.BigEndian);

            if (type == typeof(String))
                return new TCompareOption(TSortOrder.Ascending, false);

            return new TCompareOption(TSortOrder.Ascending);
        }

        public static TCompareOption[] GetDefaultCompareOptions(Type type, Func<Type, MemberInfo, int> memberOrder = null)
        {
            if (TDataType.IsPrimitiveType(type))
                return new TCompareOption[] { GetDefaultCompareOption(type) };

            if (type == typeof(Guid))
                return new TCompareOption[] { GetDefaultCompareOption(type) };

            if (type.IsClass || type.IsStruct())
                return TDataTypeUtils.GetPublicMembers(type, memberOrder).Select(x => GetDefaultCompareOption(x.GetPropertyOrFieldType())).ToArray();

            throw new NotSupportedException(type.ToString());
        }

        public static void CheckCompareOption(Type type, TCompareOption option)
        {
            if (!TDataType.IsPrimitiveType(type) && type != typeof(Guid))
                throw new NotSupportedException(String.Format("The type '{0}' is not primitive.", type));

            if (type == typeof(string))
            {
                if (option.ByteOrder != TByteOrder.Unspecified)
                    throw new ArgumentException("String can't have ByteOrder option.");
            }
            else if (type == typeof(byte[]))
            {
                if (option.ByteOrder == TByteOrder.Unspecified)
                    throw new ArgumentException("byte[] must have ByteOrder option.");
            }
            else
            {
                if (option.ByteOrder != TByteOrder.Unspecified)
                    throw new ArgumentException(String.Format("{0} does not support ByteOrder option.", type));
            }
        }

        public static void CheckCompareOptions(Type type, TCompareOption[] compareOptions, Func<Type, MemberInfo, int> memberOrder = null)
        {
            if (type.IsClass || type.IsStruct())
            {
                int i = 0;
                foreach (var member in TDataTypeUtils.GetPublicMembers(type, memberOrder).Select(x => x.GetPropertyOrFieldType()).ToArray())
                    CheckCompareOption(member, compareOptions[i++]);
            }
            else
                CheckCompareOption(type, compareOptions[0]);
        }

        #endregion

        public static TCompareOption Ascending
        {
            get { return new TCompareOption(TSortOrder.Ascending); }
        }

        public static TCompareOption Descending
        {
            get { return new TCompareOption(TSortOrder.Descending); }
        }
    }
}
