using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Ledon.BinaryMapper;

namespace Ledon.BinaryMapper.IO;

/// <summary>
/// 提供将基础值类型写入二进制数据的工具。<br/>
/// Provides utilities to write primitive values into binary data.
/// </summary>
internal sealed class BinaryWriter
{
    private readonly MemoryStream _stream;

    public BinaryWriter()
    {
        _stream = new MemoryStream();
    }

    public byte[] ToArray()
    {
        return _stream.ToArray();
    }

    public void WriteByte(byte value)
    {
        _stream.WriteByte(value);
    }

    public void WriteSByte(sbyte value)
    {
        WriteByte((byte)value);
    }

    public void WriteBoolean(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
    }

    public void WriteChar(char value, Endianness endianness)
    {
        WriteInt16((short)value, endianness);
    }

    public void WriteInt16(short value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteUInt16(ushort value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteInt32(int value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteUInt32(uint value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteInt64(long value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteUInt64(ulong value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        BinaryPrimitives.WriteUInt64BigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteSingle(float value, Endianness endianness)
    {
        var intBits = BitConverter.SingleToInt32Bits(value);
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(buffer, intBits);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteDouble(double value, Endianness endianness)
    {
        var longBits = BitConverter.DoubleToInt64Bits(value);
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(buffer, longBits);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteHalf(Half value, Endianness endianness)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteHalfBigEndian(buffer, value);

        if (endianness != Endianness.BigEndian)
        {
            buffer.Reverse();
        }

        _stream.Write(buffer);
    }

    public void WriteGuid(Guid value)
    {
        Span<byte> buffer = stackalloc byte[16];
        value.TryWriteBytes(buffer, bigEndian: true, out _);
        _stream.Write(buffer);
    }

    public void WriteBytes(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    public void WriteNullTerminatedString(string value, Encoding encoding)
    {
        if (value == null)
        {
            _stream.WriteByte(0);
            return;
        }

        var bytes = encoding.GetBytes(value);
        _stream.Write(bytes, 0, bytes.Length);
        _stream.WriteByte(0);
    }

    public void WriteFixedLengthString(string value, Encoding encoding, int length)
    {
        if (length <= 0)
        {
            return;
        }

        Span<byte> buffer = length <= 1024 ? stackalloc byte[length] : new byte[length];

        if (value != null)
        {
            var maxBytes = encoding.GetMaxByteCount(length);
            Span<byte> temp = maxBytes <= 1024 ? stackalloc byte[maxBytes] : new byte[maxBytes];
            var written = encoding.GetBytes(value.AsSpan(), temp);
            var copyLength = Math.Min(written, length);
            temp.Slice(0, copyLength).CopyTo(buffer);
        }

        _stream.Write(buffer);
    }
}
