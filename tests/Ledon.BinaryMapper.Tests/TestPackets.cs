using Ledon.BinaryMapper.Attributes;

namespace Ledon.BinaryMapper.Tests;

public class SimplePacket
{
    public int IntValue { get; set; }
    public short ShortValue { get; set; }
    public long LongValue { get; set; }
    public float FloatValue { get; set; }
    public double DoubleValue { get; set; }
    public bool BoolValue { get; set; }
    public byte ByteValue { get; set; }
    public char CharValue { get; set; }
    public ushort UShortValue { get; set; }
    public uint UIntValue { get; set; }
    public ulong ULongValue { get; set; }
}

public class StringPacket
{
    [FixedLength(8)]
    public string FixedString { get; set; } = string.Empty;

    [NullTerminated]
    public string NullTerminatedString { get; set; } = string.Empty;

    [FixedLength(4)]
    [Encoding("ascii")]
    public string EncodedString { get; set; } = string.Empty;
}

public class ArrayPacket
{
    [FixedLength(3)]
    public int[] IntArray { get; set; } = [];

    [FixedLength(2)]
    public float[] FloatArray { get; set; } = [];
}

public class ListPacket
{
    [FixedLength(4)]
    public List<int> IntList { get; set; } = [];

    [FixedLength(3)]
    public IList<short> ShortList { get; set; } = [];
}

public class NestedInner
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class NestedPacket
{
    public NestedInner Inner { get; set; } = new();
    public int Z { get; set; }
}

public class LittleEndianPacket
{
    [LittleEndian]
    public int Value { get; set; }
}

public class HalfGuidPacket
{
    public Half HalfValue { get; set; }
    public Guid GuidValue { get; set; }
}

public class IgnorePacket
{
    public int Id { get; set; }

    [Ignore]
    public string Temp { get; set; } = string.Empty;
}

public class BitFieldPacket
{
    [BitField(1)]
    public bool FlagA { get; set; }

    [BitField(3)]
    public byte Value { get; set; }

    [BitField(1)]
    public bool FlagB { get; set; }

    public byte Tail { get; set; }
}