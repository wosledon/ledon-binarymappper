using Xunit;
using Ledon.BinaryMapper.Internal;

namespace Ledon.BinaryMapper.Tests;

public class BinaryMapperTests
{
    #region Basic Primitives

    [Fact]
    public void Serialize_Deserialize_SimplePacket_RoundTrip()
    {
        var original = new SimplePacket
        {
            IntValue = 42,
            ShortValue = -123,
            LongValue = 999_999_999,
            FloatValue = 3.14f,
            DoubleValue = -2.718,
            BoolValue = true,
            ByteValue = 200,
            CharValue = 'A',
            UShortValue = 65000,
            UIntValue = 4_000_000_000,
            ULongValue = 18_000_000_000_000_000_000
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<SimplePacket>(data);

        Assert.Equal(original.IntValue, restored.IntValue);
        Assert.Equal(original.ShortValue, restored.ShortValue);
        Assert.Equal(original.LongValue, restored.LongValue);
        Assert.Equal(original.FloatValue, restored.FloatValue);
        Assert.Equal(original.DoubleValue, restored.DoubleValue);
        Assert.Equal(original.BoolValue, restored.BoolValue);
        Assert.Equal(original.ByteValue, restored.ByteValue);
        Assert.Equal(original.CharValue, restored.CharValue);
        Assert.Equal(original.UShortValue, restored.UShortValue);
        Assert.Equal(original.UIntValue, restored.UIntValue);
        Assert.Equal(original.ULongValue, restored.ULongValue);
    }

    [Fact]
    public void Deserialize_KnownBytes_ProducesExpectedValues()
    {
        // int=1 (BE), short=2 (BE), long=3 (BE)
        var data = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, // int 1
            0x00, 0x02,             // short 2
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, // long 3
            0x40, 0x00, 0x00, 0x00, // float 2.0
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // double 2.0
            0x01,                   // bool true
            0xFF,                   // byte 255
            0x00, 0x41,             // char 'A'
            0x00, 0x04,             // ushort 4
            0x00, 0x00, 0x00, 0x05, // uint 5
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06  // ulong 6
        };

        var p = BinaryMapper.Deserialize<SimplePacket>(data);

        Assert.Equal(1, p.IntValue);
        Assert.Equal(2, p.ShortValue);
        Assert.Equal(3, p.LongValue);
        Assert.Equal(2.0f, p.FloatValue);
        Assert.Equal(2.0, p.DoubleValue);
        Assert.True(p.BoolValue);
        Assert.Equal(255, p.ByteValue);
        Assert.Equal('A', p.CharValue);
        Assert.Equal(4, p.UShortValue);
        Assert.Equal(5u, p.UIntValue);
        Assert.Equal(6uL, p.ULongValue);
    }

    #endregion

    #region Half & Guid

    [Fact]
    public void Serialize_Deserialize_HalfGuid_RoundTrip()
    {
        var original = new HalfGuidPacket
        {
            HalfValue = (Half)3.14,
            GuidValue = Guid.NewGuid()
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<HalfGuidPacket>(data);

        Assert.Equal(original.HalfValue, restored.HalfValue);
        Assert.Equal(original.GuidValue, restored.GuidValue);
    }

    #endregion

    #region Strings

    [Fact]
    public void Serialize_Deserialize_FixedLengthString_RoundTrip()
    {
        var original = new StringPacket
        {
            FixedString = "BOB",
            NullTerminatedString = "hello",
            EncodedString = "ABCD"
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<StringPacket>(data);

        Assert.Equal("BOB", restored.FixedString);
        Assert.Equal("hello", restored.NullTerminatedString);
        Assert.Equal("ABCD", restored.EncodedString);
    }

    [Fact]
    public void Serialize_Deserialize_FixedLengthString_TrimsNullPadding()
    {
        var data = new byte[]
        {
            0x42, 0x4F, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, // fixed "BOB" + padding
            0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x00,             // null-term "hello"
            0x41, 0x42, 0x43, 0x44                          // fixed "ABCD"
        };

        var p = BinaryMapper.Deserialize<StringPacket>(data);

        Assert.Equal("BOB", p.FixedString);
        Assert.Equal("hello", p.NullTerminatedString);
        Assert.Equal("ABCD", p.EncodedString);
    }

    [Fact]
    public void Serialize_Deserialize_EmptyString()
    {
        var data = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // fixed ""
            0x00,                                              // null-term ""
            0x00, 0x00, 0x00, 0x00                            // fixed ""
        };

        var p = BinaryMapper.Deserialize<StringPacket>(data);

        Assert.Equal("", p.FixedString);
        Assert.Equal("", p.NullTerminatedString);
        Assert.Equal("", p.EncodedString);
    }

    #endregion

    #region Arrays

    [Fact]
    public void Serialize_Deserialize_IntArray_RoundTrip()
    {
        var original = new ArrayPacket
        {
            IntArray = [10, 20, 30],
            FloatArray = [1.5f, 2.5f]
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<ArrayPacket>(data);

        Assert.Equal([10, 20, 30], restored.IntArray);
        Assert.Equal([1.5f, 2.5f], restored.FloatArray);
    }

    [Fact]
    public void Deserialize_IntArray_KnownBytes()
    {
        var data = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, // [0] = 1
            0x00, 0x00, 0x00, 0x02, // [1] = 2
            0x00, 0x00, 0x00, 0x03, // [2] = 3
            0x3F, 0x80, 0x00, 0x00, // [0] = 1.0f
            0x40, 0x00, 0x00, 0x00  // [1] = 2.0f
        };

        var p = BinaryMapper.Deserialize<ArrayPacket>(data);

        Assert.Equal([1, 2, 3], p.IntArray);
        Assert.Equal([1.0f, 2.0f], p.FloatArray);
    }

    #endregion

    #region IList

    [Fact]
    public void Serialize_Deserialize_List_RoundTrip()
    {
        var original = new ListPacket
        {
            IntList = [100, 200, 300, 400],
            ShortList = [5, 10, 15]
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<ListPacket>(data);

        Assert.Equal([100, 200, 300, 400], restored.IntList);
        Assert.Equal([5, 10, 15], restored.ShortList);
    }

    #endregion

    #region Nested Objects

    [Fact]
    public void Serialize_Deserialize_NestedObject_RoundTrip()
    {
        var original = new NestedPacket
        {
            Inner = new NestedInner { X = 1, Y = 2 },
            Z = 3
        };

        var data = BinaryMapper.Serialize(original);
        var restored = BinaryMapper.Deserialize<NestedPacket>(data);

        Assert.Equal(1, restored.Inner.X);
        Assert.Equal(2, restored.Inner.Y);
        Assert.Equal(3, restored.Z);
    }

    #endregion

    #region Endianness

    [Fact]
    public void Serialize_Deserialize_LittleEndian_RoundTrip()
    {
        var settings = new BinaryMapperSettings { Endianness = Endianness.LittleEndian };

        var original = new { Value = 0x01020304 };
        var packet = new LittleEndianPacket { Value = 0x01020304 };

        var data = BinaryMapper.Serialize(packet, settings);
        // With LittleEndian the bytes should be reversed
        Assert.Equal([0x04, 0x03, 0x02, 0x01], data);

        var restored = BinaryMapper.Deserialize<LittleEndianPacket>(data, settings);
        Assert.Equal(0x01020304, restored.Value);
    }

    [Fact]
    public void Deserialize_LittleEndianAttribute_ReadsCorrectly()
    {
        // Bytes in little-endian: 0x04 0x03 0x02 0x01 = 0x01020304
        var data = new byte[] { 0x04, 0x03, 0x02, 0x01 };

        var p = BinaryMapper.Deserialize<LittleEndianPacket>(data);
        Assert.Equal(0x01020304, p.Value);
    }

    #endregion

    #region Ignore Attribute

    [Fact]
    public void IgnoreAttribute_SkipsField()
    {
        var data = new byte[] { 0x00, 0x00, 0x00, 0x01 };

        var p = BinaryMapper.Deserialize<IgnorePacket>(data);

        Assert.Equal(1, p.Id);
        Assert.Equal("", p.Temp);
    }

    #endregion

    #region Span API

    [Fact]
    public void Deserialize_ReadOnlySpan_Works()
    {
        var data = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, // int 1
            0x00, 0x02,             // short 2
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, // long 3
            0x40, 0x00, 0x00, 0x00, // float 2.0
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // double 2.0
            0x01,                   // bool true
            0xFF,                   // byte 255
            0x00, 0x41,             // char 'A'
            0x00, 0x04,             // ushort 4
            0x00, 0x00, 0x00, 0x05, // uint 5
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06  // ulong 6
        };

        var p = BinaryMapper.Deserialize<SimplePacket>(data.AsSpan());

        Assert.Equal(1, p.IntValue);
    }

    [Fact]
    public void Deserialize_Slice_ReadsCorrectly()
    {
        var data = new byte[]
        {
            0xFF, 0xFF, 0xFF, 0xFF, // garbage
            0x00, 0x00, 0x00, 0x01, // int 1
            0x00, 0x02,             // short 2
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, // long 3
            0x40, 0x00, 0x00, 0x00, // float 2.0
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // double 2.0
            0x01,                   // bool true
            0xFF,                   // byte 255
            0x00, 0x41,             // char 'A'
            0x00, 0x04,             // ushort 4
            0x00, 0x00, 0x00, 0x05, // uint 5
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, // ulong 6
            0xFF, 0xFF, 0xFF, 0xFF  // garbage
        };

        var slice = data.AsSpan(4, 44);
        var p = BinaryMapper.Deserialize<SimplePacket>(slice);

        Assert.Equal(1, p.IntValue);
    }

    #endregion

    #region Null Checks

    [Fact]
    public void Serialize_NullObject_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => BinaryMapper.Serialize(null!));
    }

    [Fact]
    public void Deserialize_NullData_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => BinaryMapper.Deserialize<SimplePacket>(null!));
    }

    [Fact]
    public void Deserialize_NullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => BinaryMapper.Deserialize(new byte[0], null!));
    }

    #endregion

    #region Cache

    [Fact]
    public void Cache_CachesMemberMetadata()
    {
        var members1 = BinaryMapperCache.GetMappableMembers(typeof(SimplePacket));
        var members2 = BinaryMapperCache.GetMappableMembers(typeof(SimplePacket));

        Assert.Same(members1, members2);
    }

    #endregion
}