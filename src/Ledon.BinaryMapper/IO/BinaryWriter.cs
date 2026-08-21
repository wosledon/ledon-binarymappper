using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using Ledon.BinaryMapper;

namespace Ledon.BinaryMapper.IO;

/// <summary>
/// 提供将基础值类型写入二进制数据的工具（基于 ArrayPool 缓冲）。<br/>
/// Provides utilities to write primitive values into binary data (ArrayPool-based buffer).
/// </summary>
internal sealed class BinaryWriter : IDisposable
{
    private const int DefaultCapacity = 256;

    private byte[] _buffer;
    private int _length;

    public BinaryWriter()
    {
        _buffer = ArrayPool<byte>.Shared.Rent(DefaultCapacity);
        _length = 0;
    }

    public byte[] ToArray()
    {
        var result = new byte[_length];
        _buffer.AsSpan(0, _length).CopyTo(result);
        return result;
    }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = [];
        _length = 0;
    }

    private void EnsureCapacity(int additional)
    {
        if (_length + additional <= _buffer.Length)
            return;

        var newSize = _buffer.Length * 2;
        while (newSize < _length + additional)
            newSize *= 2;

        var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
        _buffer.AsSpan(0, _length).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }

    public void WriteByte(byte value)
    {
        EnsureCapacity(1);
        _buffer[_length++] = value;
    }

    public void WriteSByte(sbyte value) => WriteByte((byte)value);

    public void WriteBoolean(bool value) => WriteByte(value ? (byte)1 : (byte)0);

    public void WriteChar(char value, Endianness endianness) => WriteUInt16(value, endianness);

    public void WriteInt16(short value, Endianness endianness)
    {
        EnsureCapacity(sizeof(short));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(short)).Reverse();
        _length += sizeof(short);
    }

    public void WriteUInt16(ushort value, Endianness endianness)
    {
        EnsureCapacity(sizeof(ushort));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(ushort)).Reverse();
        _length += sizeof(ushort);
    }

    public void WriteInt32(int value, Endianness endianness)
    {
        EnsureCapacity(sizeof(int));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(int)).Reverse();
        _length += sizeof(int);
    }

    public void WriteUInt32(uint value, Endianness endianness)
    {
        EnsureCapacity(sizeof(uint));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(uint)).Reverse();
        _length += sizeof(uint);
    }

    public void WriteInt64(long value, Endianness endianness)
    {
        EnsureCapacity(sizeof(long));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(long)).Reverse();
        _length += sizeof(long);
    }

    public void WriteUInt64(ulong value, Endianness endianness)
    {
        EnsureCapacity(sizeof(ulong));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(ulong)).Reverse();
        _length += sizeof(ulong);
    }

    public void WriteSingle(float value, Endianness endianness)
        => WriteInt32(BitConverter.SingleToInt32Bits(value), endianness);

    public void WriteDouble(double value, Endianness endianness)
        => WriteInt64(BitConverter.DoubleToInt64Bits(value), endianness);

    public void WriteHalf(Half value, Endianness endianness)
    {
        EnsureCapacity(sizeof(short));
        var span = _buffer.AsSpan(_length);
        BinaryPrimitives.WriteHalfBigEndian(span, value);
        if (endianness != Endianness.BigEndian)
            span.Slice(0, sizeof(short)).Reverse();
        _length += sizeof(short);
    }

    public void WriteGuid(Guid value)
    {
        EnsureCapacity(16);
        value.TryWriteBytes(_buffer.AsSpan(_length), bigEndian: true, out _);
        _length += 16;
    }

    public void WriteBytes(byte[] buffer, int offset, int count)
    {
        EnsureCapacity(count);
        buffer.AsSpan(offset, count).CopyTo(_buffer.AsSpan(_length));
        _length += count;
    }

    public void WriteNullTerminatedString(string value, Encoding encoding)
    {
        if (value == null)
        {
            WriteByte(0);
            return;
        }

        var bytes = encoding.GetBytes(value);
        WriteBytes(bytes, 0, bytes.Length);
        WriteByte(0);
    }

    public void WriteFixedLengthString(string value, Encoding encoding, int length)
    {
        if (length <= 0)
            return;

        EnsureCapacity(length);
        var buffer = _buffer.AsSpan(_length, length);
        buffer.Clear();

        if (value != null)
        {
            var maxBytes = encoding.GetMaxByteCount(length);
            Span<byte> temp = maxBytes <= 1024 ? stackalloc byte[maxBytes] : new byte[maxBytes];
            var written = encoding.GetBytes(value.AsSpan(), temp);
            var copyLength = Math.Min(written, length);
            temp.Slice(0, copyLength).CopyTo(buffer);
        }

        _length += length;
    }
}
