using System;
using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper.Attributes;
using Ledon.BinaryMapper.BinaryTypes;

namespace Ledon.BinaryMapper;

/// <summary>
/// 提供二进制序列化/反序列化的内部辅助方法。<br/>
/// Provides internal helper methods for binary serialization/deserialization.
/// </summary>
internal static class BinaryMapperHelpers
{
    #region Deserialization (Span-based, used by both byte[] and ReadOnlySpan<byte> paths)

    public static object ReadObject(ReadOnlySpan<byte> data, ref int position, Type type, BinaryMapperSettings settings)
    {
        var instance = Activator.CreateInstance(type)!;
        ReadObjectCore(data, ref position, instance, type, settings);
        return instance;
    }

    public static void ReadObjectCore(ReadOnlySpan<byte> data, ref int position, object instance, Type type, BinaryMapperSettings settings)
    {
        foreach (var member in BinaryMapperCache.GetMappableMembers(type))
        {
            if (member.IsIgnored)
                continue;

            var value = ReadMember(data, ref position, member, settings);
            member.SetValue(instance, value);
        }
    }

    public static object? ReadMember(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings settings)
    {
        if (member.MemberType == typeof(string))
            return ReadString(data, ref position, member, settings);

        if (TryReadBinaryType(data, ref position, member, out var binaryValue))
            return binaryValue;

        return ReadPrimitive(data, ref position, member, settings);
    }

    public static object ReadPrimitive(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings settings)
    {
        var underlyingType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (underlyingType == typeof(short))
            return ReadInt16(data, ref position, member.Endianness);
        if (underlyingType == typeof(ushort))
            return ReadUInt16(data, ref position, member.Endianness);
        if (underlyingType == typeof(int))
            return ReadInt32(data, ref position, member.Endianness);
        if (underlyingType == typeof(uint))
            return ReadUInt32(data, ref position, member.Endianness);
        if (underlyingType == typeof(long))
            return ReadInt64(data, ref position, member.Endianness);
        if (underlyingType == typeof(ulong))
            return ReadUInt64(data, ref position, member.Endianness);
        if (underlyingType == typeof(byte))
            return ReadByte(data, ref position);
        if (underlyingType == typeof(sbyte))
            return (sbyte)ReadByte(data, ref position);
        if (underlyingType == typeof(float))
            return ReadSingle(data, ref position, member.Endianness);
        if (underlyingType == typeof(double))
            return ReadDouble(data, ref position, member.Endianness);
        if (underlyingType == typeof(bool))
            return ReadBoolean(data, ref position);
        if (underlyingType == typeof(char))
            return (char)ReadUInt16(data, ref position, member.Endianness);

        if (underlyingType.IsEnum)
        {
            var enumUnderlying = Enum.GetUnderlyingType(underlyingType);
            var enumValue = ReadPrimitive(data, ref position, member.WithType(enumUnderlying), settings);
            return Enum.ToObject(underlyingType, enumValue);
        }

        if (underlyingType.IsValueType)
        {
            var nestedInstance = Activator.CreateInstance(underlyingType)!;
            ReadObjectCore(data, ref position, nestedInstance, underlyingType, settings);
            return nestedInstance;
        }

        throw new BinaryMapperException($"不支持映射到类型 '{member.MemberType.FullName}'。");
    }

    public static bool TryReadBinaryType(ReadOnlySpan<byte> data, ref int position, MappableMember member, out object? value)
    {
        value = null!;

        if (member.MemberType == typeof(CString))
        {
            value = new CString((string)ReadString(data, ref position, member, null!));
            return true;
        }

        if (member.MemberType == typeof(short))
        {
            value = ReadInt16(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(ushort))
        {
            value = ReadUInt16(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(int))
        {
            value = ReadInt32(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(uint))
        {
            value = ReadUInt32(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(long))
        {
            value = ReadInt64(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(ulong))
        {
            value = ReadUInt64(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(byte))
        {
            value = ReadByte(data, ref position);
            return true;
        }
        if (member.MemberType == typeof(sbyte))
        {
            value = (sbyte)ReadByte(data, ref position);
            return true;
        }
        if (member.MemberType == typeof(float))
        {
            value = ReadSingle(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(double))
        {
            value = ReadDouble(data, ref position, member.Endianness);
            return true;
        }
        if (member.MemberType == typeof(bool))
        {
            value = ReadBoolean(data, ref position);
            return true;
        }
        if (member.MemberType == typeof(char))
        {
            value = (char)ReadUInt16(data, ref position, member.Endianness);
            return true;
        }

        return false;
    }

    public static object ReadString(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings? settings)
    {
        var encoding = member.Encoding ?? settings?.Encoding ?? Encoding.UTF8;

        if (member.FixedLength.HasValue)
        {
            var buffer = ReadBytes(data, ref position, member.FixedLength.Value);
            buffer = TrimNullTerminator(buffer);
            return encoding.GetString(buffer);
        }

        if (member.NullTerminated)
            return ReadNullTerminatedString(data, ref position, encoding);

        var remaining = data.Length - position;
        var fallbackBuffer = ReadBytes(data, ref position, remaining);
        return encoding.GetString(fallbackBuffer);
    }

    #endregion

    #region Serialization (BinaryWriter based)

    public static void WriteObject(BinaryWriter writer, object instance, Type type, BinaryMapperSettings settings)
    {
        foreach (var member in BinaryMapperCache.GetMappableMembers(type))
        {
            if (member.IsIgnored)
                continue;

            WriteMember(writer, member, instance, settings);
        }
    }

    public static void WriteMember(BinaryWriter writer, MappableMember member, object instance, BinaryMapperSettings settings)
    {
        var value = member.GetValue(instance);
        if (value == null)
            return;

        if (member.MemberType == typeof(string))
        {
            WriteString(writer, (string)value, member, settings);
            return;
        }

        if (TryWriteBinaryType(writer, value, member, settings))
            return;

        if (TryWritePrimitive(writer, value, member, settings))
            return;

        if (value is not string && member.MemberType.IsClass)
        {
            WriteObject(writer, value, member.MemberType, settings);
            return;
        }

        throw new BinaryMapperException($"不支持序列化类型 '{member.MemberType.FullName}'。");
    }

    public static bool TryWriteBinaryType(BinaryWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        return value switch
        {
            CString cstring => WriteAndReturnTrue(writer, cstring, member, settings),
            short s => WriteAndReturnTrue(writer, s, member.Endianness),
            ushort us => WriteAndReturnTrue(writer, us, member.Endianness),
            int i => WriteAndReturnTrue(writer, i, member.Endianness),
            uint ui => WriteAndReturnTrue(writer, ui, member.Endianness),
            long l => WriteAndReturnTrue(writer, l, member.Endianness),
            ulong ul => WriteAndReturnTrue(writer, ul, member.Endianness),
            byte b => WriteAndReturnTrue(writer, b),
            sbyte sb => WriteAndReturnTrue(writer, sb),
            float f => WriteAndReturnTrue(writer, f, member.Endianness),
            double d => WriteAndReturnTrue(writer, d, member.Endianness),
            bool flag => WriteAndReturnTrue(writer, flag),
            char c => WriteAndReturnTrue(writer, c, member.Endianness),
            _ => false
        };
    }

    public static bool TryWritePrimitive(BinaryWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        var underlyingType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (underlyingType == typeof(short)) { writer.WriteInt16((short)value, member.Endianness); return true; }
        if (underlyingType == typeof(ushort)) { writer.WriteUInt16((ushort)value, member.Endianness); return true; }
        if (underlyingType == typeof(int)) { writer.WriteInt32((int)value, member.Endianness); return true; }
        if (underlyingType == typeof(uint)) { writer.WriteUInt32((uint)value, member.Endianness); return true; }
        if (underlyingType == typeof(long)) { writer.WriteInt64((long)value, member.Endianness); return true; }
        if (underlyingType == typeof(ulong)) { writer.WriteUInt64((ulong)value, member.Endianness); return true; }
        if (underlyingType == typeof(byte)) { writer.WriteByte((byte)value); return true; }
        if (underlyingType == typeof(sbyte)) { writer.WriteSByte((sbyte)value); return true; }
        if (underlyingType == typeof(float)) { writer.WriteSingle((float)value, member.Endianness); return true; }
        if (underlyingType == typeof(double)) { writer.WriteDouble((double)value, member.Endianness); return true; }
        if (underlyingType == typeof(bool)) { writer.WriteBoolean((bool)value); return true; }
        if (underlyingType == typeof(char)) { writer.WriteChar((char)value, member.Endianness); return true; }

        if (underlyingType.IsEnum)
        {
            var converted = Convert.ChangeType(value, Enum.GetUnderlyingType(underlyingType));
            TryWritePrimitive(writer, converted, member, settings);
            return true;
        }

        if (underlyingType.IsValueType)
        {
            WriteObject(writer, value, underlyingType, settings);
            return true;
        }

        return false;
    }

    public static void WriteString(BinaryWriter writer, string value, MappableMember member, BinaryMapperSettings settings)
    {
        var encoding = member.Encoding ?? settings.Encoding;

        if (member.FixedLength.HasValue)
        {
            writer.WriteFixedLengthString(value, encoding, member.FixedLength.Value);
            return;
        }

        if (member.NullTerminated)
        {
            writer.WriteNullTerminatedString(value, encoding);
            return;
        }

        var bytes = encoding.GetBytes(value);
        writer.WriteBytes(bytes, 0, bytes.Length);
    }

    #endregion

    #region Primitive Span Readers

    public static byte ReadByte(ReadOnlySpan<byte> data, ref int position)
    {
        if (position >= data.Length)
            throw new BinaryMapperException("二进制数据不足。");
        return data[position++];
    }

    public static bool ReadBoolean(ReadOnlySpan<byte> data, ref int position)
    {
        return ReadByte(data, ref position) != 0;
    }

    public static short ReadInt16(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(short));
        var value = BinaryPrimitives.ReadInt16BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static ushort ReadUInt16(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(ushort));
        var value = BinaryPrimitives.ReadUInt16BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static int ReadInt32(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(int));
        var value = BinaryPrimitives.ReadInt32BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static uint ReadUInt32(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(uint));
        var value = BinaryPrimitives.ReadUInt32BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static long ReadInt64(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(long));
        var value = BinaryPrimitives.ReadInt64BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static ulong ReadUInt64(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(ulong));
        var value = BinaryPrimitives.ReadUInt64BigEndian(span);
        return endianness == Endianness.BigEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    public static float ReadSingle(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(float));
        var intBits = BinaryPrimitives.ReadInt32BigEndian(span);
        if (endianness != Endianness.BigEndian)
            intBits = BinaryPrimitives.ReverseEndianness(intBits);
        return BitConverter.Int32BitsToSingle(intBits);
    }

    public static double ReadDouble(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(double));
        var longBits = BinaryPrimitives.ReadInt64BigEndian(span);
        if (endianness != Endianness.BigEndian)
            longBits = BinaryPrimitives.ReverseEndianness(longBits);
        return BitConverter.Int64BitsToDouble(longBits);
    }

    public static byte[] ReadBytes(ReadOnlySpan<byte> data, ref int position, int count)
    {
        if (position + count > data.Length)
            throw new BinaryMapperException($"二进制数据不足，需要 {count} 字节，当前剩余 {data.Length - position} 字节。");

        var buffer = new byte[count];
        data.Slice(position, count).CopyTo(buffer);
        position += count;
        return buffer;
    }

    public static string ReadNullTerminatedString(ReadOnlySpan<byte> data, ref int position, Encoding encoding)
    {
        var start = position;
        while (position < data.Length && data[position] != 0)
            position++;

        var length = position - start;
        var value = length > 0 ? encoding.GetString(data.Slice(start, length)) : string.Empty;

        if (position < data.Length)
            position++;

        return value;
    }

    #endregion

    #region Utilities

    private static ReadOnlySpan<byte> Slice(ReadOnlySpan<byte> data, ref int position, int count)
    {
        if (position + count > data.Length)
            throw new BinaryMapperException($"二进制数据不足，需要 {count} 字节，当前剩余 {data.Length - position} 字节。");

        var span = data.Slice(position, count);
        position += count;
        return span;
    }

    public static byte[] TrimNullTerminator(byte[] buffer)
    {
        var length = buffer.Length;
        while (length > 0 && buffer[length - 1] == 0)
            length--;

        if (length == buffer.Length)
            return buffer;

        var trimmed = new byte[length];
        Buffer.BlockCopy(buffer, 0, trimmed, 0, length);
        return trimmed;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, CString cstring, MappableMember member, BinaryMapperSettings settings)
    {
        WriteString(writer, cstring.Value, member, settings);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, short value, Endianness endianness)
    {
        writer.WriteInt16(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, ushort value, Endianness endianness)
    {
        writer.WriteUInt16(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, int value, Endianness endianness)
    {
        writer.WriteInt32(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, uint value, Endianness endianness)
    {
        writer.WriteUInt32(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, long value, Endianness endianness)
    {
        writer.WriteInt64(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, ulong value, Endianness endianness)
    {
        writer.WriteUInt64(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, byte value)
    {
        writer.WriteByte(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, sbyte value)
    {
        writer.WriteSByte(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, float value, Endianness endianness)
    {
        writer.WriteSingle(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, double value, Endianness endianness)
    {
        writer.WriteDouble(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, bool value)
    {
        writer.WriteBoolean(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinaryWriter writer, char value, Endianness endianness)
    {
        writer.WriteChar(value, endianness);
        return true;
    }

    #endregion
}