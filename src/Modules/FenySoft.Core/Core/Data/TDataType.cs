using System.Collections;
using System.Diagnostics;

namespace FenySoft.Core.Data
{
  /*  examples:

          TDataType type1 = TDataType.Boolean;
          TDataType type2 = TDataType.Int32;
          TDataType type3 = TDataType.String;

          TDataType type4 = TDataType.TSlots(
              TDataType.String,
              TDataType.DateTime,
              TDataType.Double,
              TDataType.Double,
              TDataType.Int64
              );

          TDataType type5 = TDataType.Array(TDataType.String);
          TDataType type6 = TDataType.List(TDataType.String);
          TDataType type7 = TDataType.Dictionary(TDataType.Int32, TDataType.String);

          TDataType type8 = TDataType.TSlots(
              TDataType.String,
              TDataType.DateTime,
              TDataType.Double,
              TDataType.Double,
              TDataType.TSlots(
                  TDataType.String,
                  TDataType.String),
                  TDataType.Array(TDataType.Boolean),
              TDataType.Int32,
              TDataType.Dictionary(TDataType.Int32, TDataType.String),
              TDataType.Boolean,
              TDataType.List(TDataType.Array(TDataType.DateTime)),
              TDataType.Dictionary(TDataType.String, TDataType.List(TDataType.DateTime))
              );
  */

  public sealed class TDataType : IEquatable<TDataType>, IEnumerable<TDataType>
  {
    #region Enums..

    private enum TCode : byte
    {
      Boolean = TypeCode.Boolean,   // 3  
      Char = TypeCode.Char,         // 4
      SByte = TypeCode.SByte,       // 5
      Byte = TypeCode.Byte,         // 6
      Int16 = TypeCode.Int16,       // 7
      UInt16 = TypeCode.UInt16,     // 8
      Int32 = TypeCode.Int32,       // 9
      UInt32 = TypeCode.UInt32,     // 10
      Int64 = TypeCode.Int64,       // 11
      UInt64 = TypeCode.UInt64,     // 12
      Single = TypeCode.Single,     // 13
      Double = TypeCode.Double,     // 14
      Decimal = TypeCode.Decimal,   // 15
      DateTime = TypeCode.DateTime, // 16
      TimeSpan = 17,
      String = TypeCode.String, // 18
      ByteArray = 19,

      Slots = 20,
      Array = 21,
      List = 22,
      Dictionary = 23
    }

    #endregion

    #region Static Methods..

    public static TDataType Deserialize(BinaryReader AReader)
    {
      TCode code = (TCode)AReader.ReadByte();

      if (code < TCode.Slots)
        return new TDataType(code, null);

      TDataType[] types = new TDataType[AReader.ReadByte()];

      for (int i = 0; i < types.Length; i++)
        types[i] = Deserialize(AReader);

      return new TDataType(code, types);
    }

    public static TDataType Slots(params TDataType[]? ASlots)
    {
      // ArgumentNullException, ha a ASlots null
      if (ASlots == null)
        throw new ArgumentNullException(nameof(ASlots));

      if (ASlots.Length == 0)
        throw new ArgumentException("slots.Length == 0");

      if (ASlots.Length > byte.MaxValue)
        throw new ArgumentException("slots.Length > 255");

      return new TDataType(TCode.Slots, ASlots);
    }

    public static TDataType Array(TDataType ADataType)
    {
      return new TDataType(TCode.Array, ADataType);
    }

    public static TDataType List(TDataType ADataType)
    {
      return new TDataType(TCode.List, ADataType);
    }

    public static TDataType Dictionary(TDataType AKey, TDataType AValue)
    {
      return new TDataType(TCode.Dictionary, AKey, AValue);
    }

    public static TDataType FromPrimitiveType(Type AType)
    {
      return FPrimitiveMap[AType];
    }

    public static bool IsPrimitiveType(Type AType)
    {
      return FPrimitiveMap.ContainsKey(AType);
    }

    public static bool operator ==(TDataType? AType1, TDataType AType2)
    {
      if (ReferenceEquals(AType1, AType2))
        return true;

      if (ReferenceEquals(AType1, null))
        return false;

      return AType1.Equals(AType2);
    }

    public static bool operator !=(TDataType AType1, TDataType AType2)
    {
      return !(AType1 == AType2);
    }

    #endregion

    #region Fields..

    private static readonly Type[] FPrimitiveTypes;
    private static readonly Dictionary<Type, TDataType> FPrimitiveMap;
    private int? FCachedHashCode;
    private string? FCachedToString;
    private byte[]? FCachedSerialize;
    private readonly TCode FCode;
    private readonly TDataType[]? FTypes;

    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBePrivate.Global
    public static readonly TDataType Boolean = new TDataType(TCode.Boolean, null);
    public static readonly TDataType Char = new TDataType(TCode.Char, null);
    public static readonly TDataType SByte = new TDataType(TCode.SByte, null);
    public static readonly TDataType Byte = new TDataType(TCode.Byte, null);
    public static readonly TDataType Int16 = new TDataType(TCode.Int16, null);
    public static readonly TDataType UInt16 = new TDataType(TCode.UInt16, null);
    public static readonly TDataType Int32 = new TDataType(TCode.Int32, null);
    public static readonly TDataType UInt32 = new TDataType(TCode.UInt32, null);
    public static readonly TDataType Int64 = new TDataType(TCode.Int64, null);
    public static readonly TDataType UInt64 = new TDataType(TCode.UInt64, null);
    public static readonly TDataType Single = new TDataType(TCode.Single, null);
    public static readonly TDataType Double = new TDataType(TCode.Double, null);
    public static readonly TDataType Decimal = new TDataType(TCode.Decimal, null);
    public static readonly TDataType DateTime = new TDataType(TCode.DateTime, null);
    public static readonly TDataType TimeSpan = new TDataType(TCode.TimeSpan, null);
    public static readonly TDataType String = new TDataType(TCode.String, null);

    public static readonly TDataType ByteArray = new TDataType(TCode.ByteArray, null);
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore InconsistentNaming

    #endregion

    #region Properties..

    public bool IsPrimitive => FCode < TCode.Slots;
    public bool IsSlots => FCode == TCode.Slots;
    public bool IsArray => FCode == TCode.Array;
    public bool IsList => FCode == TCode.List;
    public bool IsDictionary => FCode == TCode.Dictionary;

    public bool AreAllTypesPrimitive
    {
      get
      {
        if (IsPrimitive)
          throw new InvalidOperationException($"The type {this} is primitive");

        return FTypes != null && FTypes.All(AX => AX.IsPrimitive);
      }
    }

    public TDataType this[int AIndex]
    {
      get
      {
        if ((IsPrimitive) || (FTypes == null))
          throw new InvalidOperationException($"The type {this} is primitive");

        return FTypes[AIndex];
      }
    }

    public int TypesCount
    {
      get
      {
        if (IsPrimitive)
          throw new InvalidOperationException($"The type {this} is primitive");

        return FTypes?.Length ?? 0;
      }
    }

    public Type PrimitiveType
    {
      get
      {
        if (!IsPrimitive)
          throw new InvalidOperationException($"The type {this} is not primitive");

        return FPrimitiveTypes[(int)FCode];
      }
    }

    #endregion

    #region Constructors..

    static TDataType()
    {
      FPrimitiveTypes = new Type[(int)TCode.ByteArray + 1];
      FPrimitiveTypes[(int)TCode.Boolean] = typeof(Boolean);
      FPrimitiveTypes[(int)TCode.Char] = typeof(Char);
      FPrimitiveTypes[(int)TCode.SByte] = typeof(SByte);
      FPrimitiveTypes[(int)TCode.Byte] = typeof(Byte);
      FPrimitiveTypes[(int)TCode.Int16] = typeof(Int16);
      FPrimitiveTypes[(int)TCode.Int32] = typeof(Int32);
      FPrimitiveTypes[(int)TCode.UInt32] = typeof(UInt32);
      FPrimitiveTypes[(int)TCode.UInt16] = typeof(UInt16);
      FPrimitiveTypes[(int)TCode.Int64] = typeof(Int64);
      FPrimitiveTypes[(int)TCode.UInt64] = typeof(UInt64);
      FPrimitiveTypes[(int)TCode.Single] = typeof(Single);
      FPrimitiveTypes[(int)TCode.Double] = typeof(Double);
      FPrimitiveTypes[(int)TCode.Decimal] = typeof(Decimal);
      FPrimitiveTypes[(int)TCode.DateTime] = typeof(DateTime);
      FPrimitiveTypes[(int)TCode.TimeSpan] = typeof(TimeSpan);
      FPrimitiveTypes[(int)TCode.String] = typeof(String);
      FPrimitiveTypes[(int)TCode.ByteArray] = typeof(byte[]);

      FPrimitiveMap = new Dictionary<Type, TDataType>();
      FPrimitiveMap[typeof(Boolean)] = Boolean;
      FPrimitiveMap[typeof(Char)] = Char;
      FPrimitiveMap[typeof(SByte)] = SByte;
      FPrimitiveMap[typeof(Byte)] = Byte;
      FPrimitiveMap[typeof(Int16)] = Int16;
      FPrimitiveMap[typeof(Int32)] = Int32;
      FPrimitiveMap[typeof(UInt32)] = UInt32;
      FPrimitiveMap[typeof(UInt16)] = UInt16;
      FPrimitiveMap[typeof(Int64)] = Int64;
      FPrimitiveMap[typeof(UInt64)] = UInt64;
      FPrimitiveMap[typeof(Single)] = Single;
      FPrimitiveMap[typeof(Double)] = Double;
      FPrimitiveMap[typeof(Decimal)] = Decimal;
      FPrimitiveMap[typeof(DateTime)] = DateTime;
      FPrimitiveMap[typeof(TimeSpan)] = TimeSpan;
      FPrimitiveMap[typeof(String)] = String;
      FPrimitiveMap[typeof(byte[])] = ByteArray;
    }

    private TDataType(TCode ACode, params TDataType[]? ATypes)
    {
      FCode = ACode;
      FTypes = ATypes;
    }

    #endregion

    #region Methods..

    private int InternalGetHashCode()
    {
      if (IsPrimitive || FTypes == null)
        return (int)FCode;

      int hashcode = 37;
      hashcode = 17 * hashcode + (int)FCode;

      for (int i = 0; i < FTypes.Length; i++)
        hashcode = 17 * hashcode + FTypes[i]
            .InternalGetHashCode();

      return hashcode;
    }

    private string InternalToString()
    {
      if (IsPrimitive || FTypes == null)
        return FCode.ToString();

      var s = System.String.Join(", ", FTypes.Select(AX => AX.InternalToString()));

      if (IsSlots)
        return $"({s})";
      else
        return $"{FCode}<{s}>";
    }

    private void InternalSerialize(BinaryWriter AWriter)
    {
      AWriter.Write((byte)FCode);

      if (IsPrimitive || FTypes == null)
        return;

      AWriter.Write(checked((byte)FTypes.Length));

      for (int i = 0; i < FTypes.Length; i++)
        FTypes[i]
            .InternalSerialize(AWriter);
    }

    public bool Equals(TDataType? ADataType)
    {
      if (ReferenceEquals(this, ADataType))
        return true;

      if (ReferenceEquals(null, ADataType))
        return false;

      if (FCode != ADataType.FCode)
        return false;

      if (IsPrimitive)
        return true;

      TDataType[]? types1 = FTypes;
      TDataType[]? types2 = ADataType.FTypes;

      if ((types1 == null) || (types2 == null) || (types1.Length != types2.Length))
        return false;

      for (int i = 0; i < types1.Length; i++)
      {
        if (!types1[i]
                .Equals(types2[i]))
          return false;
      }

      return true;
    }

    public override bool Equals(object? AObject)
    {
      return AObject is TDataType obj && Equals(obj);
    }

    public override int GetHashCode()
    {
      // ReSharper disable NonReadonlyMemberInGetHashCode
      if ((FCachedHashCode == null) || !FCachedHashCode.HasValue)
        FCachedHashCode = InternalGetHashCode();

      return FCachedHashCode.Value;
      // ReSharper restore NonReadonlyMemberInGetHashCode
    }

    public override string? ToString()
    {
      if (FCachedToString == null)
        FCachedToString = InternalToString();

      return FCachedToString;
    }

    public IEnumerator<TDataType> GetEnumerator()
    {
      Debug.Assert(FTypes != null, nameof(FTypes) + " != null");

      for (int i = 0; i < FTypes.Length; i++)
        yield return FTypes[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Serialize(BinaryWriter AWriter)
    {
      if (FCachedSerialize == null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          InternalSerialize(new BinaryWriter(ms));
          FCachedSerialize = ms.ToArray();
        }
      }

      AWriter.Write(FCachedSerialize);
    }

    #endregion
  }
}