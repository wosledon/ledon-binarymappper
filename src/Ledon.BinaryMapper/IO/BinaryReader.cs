using System;
using System.Buffers.Binary;
using System.IO;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper;

namespace Ledon.BinaryMapper.IO;

/// <summary>
/// 提供从二进制数据读取基础值类型的工具。<br/>
/// Provides utilities to read primitive values from binary data.
/// </summary>
internal sealed class BinaryReader
{
    private readonly byte[] _data;
    private int _position;

    public BinaryReader(byte[] data)
    {
        _data = data;
        _position = 0;
    }

    public int Position => _position;

    public int Remaining => _data.Length - _position;

    public void Seek(int offset)
    {
        if (offset < 0 || offset > _data.Length)
        {
            throw new BinaryMapperException($"无效的偏移量: {offset}。");
        }

        _position = offset;
    }

    public byte ReadByte()
    {
        EnsureCapacity(sizeof(byte));
        return _data[_position++];
    }

    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    public bool ReadBoolean()
    {
        return ReadByte() != 0;
    }

    public char ReadChar(Endianness endianness)
    {
        return (char)ReadUInt16(endianness);
    }

    public short ReadInt16(Endianness endianness)
    {
        EnsureCapacity(sizeof(short));
        var value = BinaryPrimitives.ReadInt16BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(short);
        return value;
    }

    public ushort ReadUInt16(Endianness endianness)
    {
        EnsureCapacity(sizeof(ushort));
        var value = BinaryPrimitives.ReadUInt16BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(ushort);
        return value;
    }

    public int ReadInt32(Endianness endianness)
    {
        EnsureCapacity(sizeof(int));
        var value = BinaryPrimitives.ReadInt32BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(int);
        return value;
    }

    public uint ReadUInt32(Endianness endianness)
    {
        EnsureCapacity(sizeof(uint));
        var value = BinaryPrimitives.ReadUInt32BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(uint);
        return value;
    }

    public long ReadInt64(Endianness endianness)
    {
        EnsureCapacity(sizeof(long));
        var value = BinaryPrimitives.ReadInt64BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(long);
        return value;
    }

    public ulong ReadUInt64(Endianness endianness)
    {
        EnsureCapacity(sizeof(ulong));
        var value = BinaryPrimitives.ReadUInt64BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        _position += sizeof(ulong);
        return value;
    }

    public float ReadSingle(Endianness endianness)
    {
        EnsureCapacity(sizeof(float));
        var intBits = BinaryPrimitives.ReadInt32BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            intBits = BinaryPrimitives.ReverseEndianness(intBits);
        }

        _position += sizeof(float);
        return BitConverter.Int32BitsToSingle(intBits);
    }

    public double ReadDouble(Endianness endianness)
    {
        EnsureCapacity(sizeof(double));
        var longBits = BinaryPrimitives.ReadInt64BigEndian(_data.AsSpan(_position));

        if (endianness != Endianness.BigEndian)
        {
            longBits = BinaryPrimitives.ReverseEndianness(longBits);
        }

        _position += sizeof(double);
        return BitConverter.Int64BitsToDouble(longBits);
    }

    public byte[] ReadBytes(int count)
    {
        EnsureCapacity(count);

        var buffer = new byte[count];
        _data.AsSpan(_position, count).CopyTo(buffer);
        _position += count;
        return buffer;
    }

    public string ReadNullTerminatedString(Encoding encoding)
    {
        var start = _position;

        while (_position < _data.Length && _data[_position] != 0)
        {
            _position++;
        }

        var length = _position - start;

        if (length == 0)
        {
            return string.Empty;
        }

        var value = encoding.GetString(_data.AsSpan(start, length));

        if (_position < _data.Length)
        {
            _position++;
        }

        return value;
    }

    private void EnsureCapacity(int required)
    {
        if (_position + required > _data.Length)
        {
            throw new BinaryMapperException($"二进制数据不足，需要 {required} 字节，当前剩余 {Remaining} 字节。");
        }
    }
}
