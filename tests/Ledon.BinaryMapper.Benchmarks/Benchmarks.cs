using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Ledon.BinaryMapper.Benchmarks;

[MemoryDiagnoser]
public class SerializationBenchmarks
{
    private SimplePacket _packet = null!;
    private byte[] _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _packet = new SimplePacket
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

        _data = BinaryMapper.Serialize(_packet);
    }

    [Benchmark]
    public byte[] Serialize() => BinaryMapper.Serialize(_packet);

    [Benchmark]
    public SimplePacket Deserialize() => BinaryMapper.Deserialize<SimplePacket>(_data);

    [Benchmark]
    public SimplePacket DeserializeFromSpan() => BinaryMapper.Deserialize<SimplePacket>((ReadOnlySpan<byte>)_data);
}

[MemoryDiagnoser]
public class ArrayBenchmarks
{
    private ArrayPacket _packet = null!;
    private byte[] _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _packet = new ArrayPacket
        {
            IntArray = [1, 2, 3],
            FloatArray = [1.0f, 2.0f]
        };

        _data = BinaryMapper.Serialize(_packet);
    }

    [Benchmark]
    public byte[] SerializeArray() => BinaryMapper.Serialize(_packet);

    [Benchmark]
    public ArrayPacket DeserializeArray() => BinaryMapper.Deserialize<ArrayPacket>(_data);
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<SerializationBenchmarks>(args: args);
        BenchmarkRunner.Run<ArrayBenchmarks>(args: args);
    }
}

// Benchmark models
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

public class ArrayPacket
{
    [Ledon.BinaryMapper.Attributes.FixedLength(3)]
    public int[] IntArray { get; set; } = [];

    [Ledon.BinaryMapper.Attributes.FixedLength(2)]
    public float[] FloatArray { get; set; } = [];
}