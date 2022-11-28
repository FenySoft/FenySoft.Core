using System.Collections;

namespace FenySoft.Core.Data;

[TestFixture]
public class TDataTypeTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void GetHashCodeBooleanTest()
  {
    TDataType dt = TDataType.Boolean;
    var hc = dt.GetHashCode();
    Assert.That(hc, Is.EqualTo((int)TypeCode.Boolean));
  }

  [Test]
  public void GetHashCodeArrayOfBooleanTest()
  {
    TDataType dt = TDataType.Array(TDataType.Boolean);
    var hc = dt.GetHashCode();
    Assert.That(hc, Is.EqualTo(11053));
  }
  
  // IsSlots = false test
  [Test]
  public void IsSlotsFalse_BoolenTest()
  {
    TDataType dt = TDataType.Boolean;
    Assert.That(dt.IsSlots, Is.False);
  }
  
  [Test]
  public void IsSlotsFalse_ArrayTest()
  {
    TDataType dt = TDataType.Array(TDataType.Boolean);
    Assert.That(dt.IsSlots, Is.False);
  }
  
  [Test]
  public void IsSlotsFalse_ListTest()
  {
    TDataType dt = TDataType.List(TDataType.Boolean);
    Assert.That(dt.IsSlots, Is.False);
  }
  
  [Test]
  public void IsSlotsFalse_DictionaryTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.String);
    Assert.That(dt.IsSlots, Is.False);
  }
  
  [Test]
  public void IsSlotsFalse_DictionarySlotsTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.Double, TDataType.String));
    Assert.That(dt.IsSlots, Is.False);
  }
  
  // IsSlots = true test
  [Test]
  public void IsSlotsTrueTest()
  {
    TDataType dt = TDataType.Slots(TDataType.Int32, TDataType.String);
    Assert.That(dt.IsSlots, Is.True);
  }
  
  // TSlots null test
  [Test]
  public void SlotsNullTest()
  {
    Assert.Catch<ArgumentNullException>(() => TDataType.Slots(null));
  }
  
  // TSlots empty test
  [Test]
  public void SlotsEmptyTest()
  {
    Assert.Catch<ArgumentException>(() => TDataType.Slots());
  }
  
  // TSlots (ASlots.Length > byte.MaxValue) test
  [Test]
  public void SlotsLengthTest()
  {
    Assert.Catch<ArgumentException>(() => TDataType.Slots(new TDataType[byte.MaxValue + 1]));
  }
  
  // IsArray = false test
  [Test]
  public void IsArrayFalseTest()
  {
    TDataType dt = TDataType.String;
    Assert.That(dt.IsArray, Is.False);
  }
  
  // IsArray = true test
  [Test]
  public void IsArrayTrueTest()
  {
    TDataType dt = TDataType.Array(TDataType.Int32);
    Assert.That(dt.IsArray, Is.True);
  }
  
  // IsList = false test
  [Test]
  public void IsListFalseTest()
  {
    TDataType dt = TDataType.String;
    Assert.That(dt.IsList, Is.False);
  }
  
  // IsList = true test
  [Test]
  public void IsListTrueTest()
  {
    TDataType dt = TDataType.List(TDataType.Int32);
    Assert.That(dt.IsList, Is.True);
  }
  
  // IsDictionary = false test
  [Test]
  public void IsDictionaryFalseTest()
  {
    TDataType dt = TDataType.String;
    Assert.That(dt.IsDictionary, Is.False);
  }
  
  // IsDictionary = true test
  [Test]
  public void IsDictionaryTrueTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.String);
    Assert.That(dt.IsDictionary, Is.True);
  }
  
  // AreAllTypesPrimitive = false test
  [Test]
  public void AreAllTypesPrimitiveFalseTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt.AreAllTypesPrimitive, Is.False);
  }
  
  // AreAllTypesPrimitive = true test
  [Test]
  public void AreAllTypesPrimitiveTrueTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Double);
    Assert.That(dt.AreAllTypesPrimitive, Is.True);
  }
  
  // AreAllTypesPrimitive InvalidOperationException test
  [Test]
  public void AreAllTypesPrimitiveInvalidOperationExceptionTest()
  {
    TDataType dt = TDataType.Int32;
    Assert.Catch<InvalidOperationException>(() =>
    {
      _ = dt.AreAllTypesPrimitive;
    });
  }
  
  // InternalToString test
  [Test]
  public void InternalToStringTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt.ToString(), Is.EqualTo("Dictionary<Int32, (Int32, String)>"));
  }
  
  // this[int index] test
  [Test]
  public void IndexerTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt[0], Is.EqualTo(TDataType.Int32));
    Assert.That(dt[1], Is.EqualTo(TDataType.Slots(TDataType.Int32, TDataType.String)));
  }
  
  // this[int index] InvalidOperationException test
  [Test]
  public void IndexerInvalidOperationExceptionTest()
  {
    TDataType dt = TDataType.Int32;
    Assert.Catch<InvalidOperationException>(() =>
    {
      _ = dt[0];
    });
  }
  
  // Equals test
  [Test]
  public void EqualsType1Type2Test()
  {
    TDataType dt1 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    TDataType dt2 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt1.Equals(dt2), Is.True);
  }
  
  // Equals null test
  [Test]
  public void EqualsNullTest()
  {
    TDataType dt1 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt1.Equals(null), Is.False);
  }
  
  // Equals IsPrimitive test
  [Test]
  public void EqualsIsPrimitiveTest()
  {
    TDataType dt1 = TDataType.Int32;
    TDataType dt2 = TDataType.Int32;
    Assert.That(dt1.Equals(dt2), Is.True);
  }
  
  // Equals Length test
  [Test]
  public void EqualsLengthTest()
  {
    TDataType dt1 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    TDataType dt2 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String, TDataType.Int32));
    Assert.That(dt1.Equals(dt2), Is.False);
  }
  
  // Equals types1[i].Equals(types2[i]) test
  [Test]
  public void EqualsTypesTest()
  {
    TDataType dt1 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    TDataType dt2 = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.Double));
    Assert.That(dt1.Equals(dt2), Is.False);
  }
  
  // TypesCount test
  [Test]
  public void TypesCountTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.That(dt.TypesCount, Is.EqualTo(2));
  }
  
  // TypesCount TSlots test
  [Test]
  public void TypesCountSlotsTest()
  {
    TDataType dt = TDataType.Slots(TDataType.Int32, TDataType.String);
    Assert.That(dt.TypesCount, Is.EqualTo(2));
  }
  
  // TypesCount InvalidOperationException test
  [Test]
  public void TypesCountInvalidOperationExceptionTest()
  {
    TDataType dt = TDataType.Int32;
    Assert.Catch<InvalidOperationException>(() =>
    {
      _ = dt.TypesCount;
    });
  }
  
  // PrimitiveType Boolean test
  [Test]
  public void PrimitiveTypeBooleanTest()
  {
    TDataType dt = TDataType.Boolean;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(bool)));
  }
  
  // PrimitiveType Char test
  [Test]
  public void PrimitiveTypeCharTest()
  {
    TDataType dt = TDataType.Char;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(char)));
  }
  
  // PrimitiveType SByte test
  [Test]
  public void PrimitiveTypeSByteTest()
  {
    TDataType dt = TDataType.SByte;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(sbyte)));
  }
  
  // PrimitiveType Byte test
  [Test]
  public void PrimitiveTypeByteTest()
  {
    TDataType dt = TDataType.Byte;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(byte)));
  }
  
  // PrimitiveType Int16 test
  [Test]
  public void PrimitiveTypeInt16Test()
  {
    TDataType dt = TDataType.Int16;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(short)));
  }
  
  // PrimitiveType UInt16 test
  [Test]
  public void PrimitiveTypeUInt16Test()
  {
    TDataType dt = TDataType.UInt16;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(ushort)));
  }

  // PrimitiveType Int32 test
  [Test]
  public void PrimitiveTypeTest()
  {
    TDataType dt = TDataType.Int32;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(int)));
  }
  
  // PrimitiveType UInt32 test
  [Test]
  public void PrimitiveTypeUInt32Test()
  {
    TDataType dt = TDataType.UInt32;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(uint)));
  }

  // PrimitiveType Int64 test
  [Test]
  public void PrimitiveTypeInt64Test()
  {
    TDataType dt = TDataType.Int64;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(long)));
  }
  
  // PrimitiveType UInt64 test
  [Test]
  public void PrimitiveTypeUInt64Test()
  {
    TDataType dt = TDataType.UInt64;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(ulong)));
  }

  // PrimitiveType Single test
  [Test]
  public void PrimitiveTypeSingleTest()
  {
    TDataType dt = TDataType.Single;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(float)));
  }

  // PrimitiveType Double test
  [Test]
  public void PrimitiveTypeDoubleTest()
  {
    TDataType dt = TDataType.Double;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(double)));
  }
  
  // PrimitiveType Decimal test
  [Test]
  public void PrimitiveTypeDecimalTest()
  {
    TDataType dt = TDataType.Decimal;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(decimal)));
  }
  
  // PrimitiveType DateTime test
  [Test]
  public void PrimitiveTypeDateTimeTest()
  {
    TDataType dt = TDataType.DateTime;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(DateTime)));
  }
  
  // PrimitiveType TimeSpan test
  [Test]
  public void PrimitiveTypeTimeSpanTest()
  {
    TDataType dt = TDataType.TimeSpan;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(TimeSpan)));
  }

  // PrimitiveType String test
  [Test]
  public void PrimitiveTypeStringTest()
  {
    TDataType dt = TDataType.String;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(String)));
  }
  
  // PrimitiveType ByteArray test
  [Test]
  public void PrimitiveTypeByteArrayTest()
  {
    TDataType dt = TDataType.ByteArray;
    Assert.That(dt.PrimitiveType, Is.EqualTo(typeof(byte[])));
  }
  
  // PrimitiveType InvalidOperationException test
  [Test]
  public void PrimitiveTypeInvalidOperationExceptionTest()
  {
    TDataType dt = TDataType.Dictionary(TDataType.Int32, TDataType.Slots(TDataType.Int32, TDataType.String));
    Assert.Catch<InvalidOperationException>(() =>
    {
      _ = dt.PrimitiveType;
    });
  }
  
  // Serialize - Deserialize Boolean test
  [Test]
  public void SerializeBooleanTest()
  {
    TDataType dt = TDataType.Boolean;
    BinaryWriter bw = new BinaryWriter(new MemoryStream());
    dt.Serialize(bw);
    bw.BaseStream.Position = 0;
    BinaryReader br = new BinaryReader(bw.BaseStream);
    TDataType dt2 = TDataType.Deserialize(br);
    Assert.That(dt2, Is.EqualTo(dt));
  }
  
  // Serialize - Deserialize TSlots test
  [Test]
  public void SerializeSlotsTest()
  {
    TDataType dt = TDataType.Slots(TDataType.Int32, TDataType.String);
    BinaryWriter bw = new BinaryWriter(new MemoryStream());
    dt.Serialize(bw);
    bw.BaseStream.Position = 0;
    BinaryReader br = new BinaryReader(bw.BaseStream);
    TDataType dt2 = TDataType.Deserialize(br);
    Assert.That(dt2, Is.EqualTo(dt));
  }
  
  // FromPrimitiveType Boolean test
  [Test]
  public void FromPrimitiveTypeBooleanTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(bool));
    Assert.That(dt, Is.EqualTo(TDataType.Boolean));
  }
  
  // FromPrimitiveType Char test
  [Test]
  public void FromPrimitiveTypeCharTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(char));
    Assert.That(dt, Is.EqualTo(TDataType.Char));
  }
  
  // FromPrimitiveType SByte test
  [Test]
  public void FromPrimitiveTypeSByteTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(sbyte));
    Assert.That(dt, Is.EqualTo(TDataType.SByte));
  }
  
  // FromPrimitiveType Byte test
  [Test]
  public void FromPrimitiveTypeByteTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(byte));
    Assert.That(dt, Is.EqualTo(TDataType.Byte));
  }
  
  // FromPrimitiveType Int16 test
  [Test]
  public void FromPrimitiveTypeInt16Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(short));
    Assert.That(dt, Is.EqualTo(TDataType.Int16));
  }
  
  // FromPrimitiveType UInt16 test
  [Test]
  public void FromPrimitiveTypeUInt16Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(ushort));
    Assert.That(dt, Is.EqualTo(TDataType.UInt16));
  }
  
  // FromPrimitiveType Int32 test
  [Test]
  public void FromPrimitiveTypeInt32Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(int));
    Assert.That(dt, Is.EqualTo(TDataType.Int32));
  }
  
  // FromPrimitiveType UInt32 test
  [Test]
  public void FromPrimitiveTypeUInt32Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(uint));
    Assert.That(dt, Is.EqualTo(TDataType.UInt32));
  }
  
  // FromPrimitiveType Int64 test
  [Test]
  public void FromPrimitiveTypeInt64Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(long));
    Assert.That(dt, Is.EqualTo(TDataType.Int64));
  }
  
  // FromPrimitiveType UInt64 test
  [Test]
  public void FromPrimitiveTypeUInt64Test()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(ulong));
    Assert.That(dt, Is.EqualTo(TDataType.UInt64));
  }
  
  // FromPrimitiveType Single test
  [Test]
  public void FromPrimitiveTypeSingleTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(float));
    Assert.That(dt, Is.EqualTo(TDataType.Single));
  }
  
  // FromPrimitiveType Double test
  [Test]
  public void FromPrimitiveTypeDoubleTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(double));
    Assert.That(dt, Is.EqualTo(TDataType.Double));
  }
  
  // FromPrimitiveType Decimal test
  [Test]
  public void FromPrimitiveTypeDecimalTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(decimal));
    Assert.That(dt, Is.EqualTo(TDataType.Decimal));
  }
  
  // FromPrimitiveType DateTime test
  [Test]
  public void FromPrimitiveTypeDateTimeTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(DateTime));
    Assert.That(dt, Is.EqualTo(TDataType.DateTime));
  }
  
  // FromPrimitiveType TimeSpan test
  [Test]
  public void FromPrimitiveTypeTimeSpanTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(TimeSpan));
    Assert.That(dt, Is.EqualTo(TDataType.TimeSpan));
  }

  // FromPrimitiveType String test
  [Test]
  public void FromPrimitiveTypeStringTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(string));
    Assert.That(dt, Is.EqualTo(TDataType.String));
  }
  
  // FromPrimitiveType ByteArray test
  [Test]
  public void FromPrimitiveTypeByteArrayTest()
  {
    TDataType dt = TDataType.FromPrimitiveType(typeof(byte[]));
    Assert.That(dt, Is.EqualTo(TDataType.ByteArray));
  }

  // IsPrimitiveType true test
  [Test]
  public void IsPrimitiveTypeBooleanTest()
  {
    Assert.That(TDataType.IsPrimitiveType(typeof(bool)), Is.True);
  }
  
  // IsPrimitiveType false test
  [Test]
  public void IsPrimitiveTypeStringTest()
  {
    Assert.That(TDataType.IsPrimitiveType(typeof(List<int>)), Is.False);
  }

  // operator == test
  [Test]
  public void OperatorEqualTest()
  {
    Assert.That(TDataType.Boolean == TDataType.Boolean, Is.True);
  }
  
  // operator == null test
  [Test]
  public void OperatorEqualNullTest()
  {
    TDataType? dt = null;
    Assert.That(dt == TDataType.Boolean, Is.False);
  }
  
  // operator != test
  [Test]
  public void OperatorNotEqualTest()
  {
    Assert.That(TDataType.Boolean != TDataType.String, Is.True);
  }
  
  // Equals test
  [Test]
  public void EqualsTest()
  {
    var dt = TDataType.Boolean;
    object obj = TDataType.Boolean;
    Assert.That(dt.Equals(obj), Is.True);
  }
  
  // IEnumerator<TDataType> test
  [Test]
  public void GetEnumeratorTest()
  {
    var dt = TDataType.Slots(TDataType.Int32, TDataType.Int32, TDataType.String);
    IEnumerator<TDataType> enumerator = dt.GetEnumerator();
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.False);
  }
  
  // IEnumerator test
  [Test]
  public void GetEnumeratorTest2()
  {
    var dt = TDataType.Slots(TDataType.Int32, TDataType.Int32, TDataType.String);
    IEnumerator enumerator = ((IEnumerable)dt).GetEnumerator();
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.True);
    Assert.That(enumerator.MoveNext(), Is.False);
  }
}