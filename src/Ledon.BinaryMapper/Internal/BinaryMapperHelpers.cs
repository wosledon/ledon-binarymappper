using System;
using System.Buffers.Binary;
using System.Collections;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper;
using BinWriter = Ledon.BinaryMapper.IO.BinaryWriter;
using BinReader = Ledon.BinaryMapper.IO.BinaryReader;

namespace Ledon.BinaryMapper.Internal;

/// <summary>
/// 提供二进制序列化/反序列化的内部辅助方法。<br/>
/// Provides internal helper methods for binary serialization/deserialization.
/// </summary>
internal static class BinaryMapperHelpers
{
    #region Deserialization (Span-based, used by both byte[] and ReadOnlySpan<byte> paths)

    public static object ReadObject(ReadOnlySpan<byte> data, ref int position, Type type, BinaryMapperSettings settings)
    {
        var instance = BinaryMapperCache.CreateInstance(type);
        ReadObjectCore(data, ref position, instance, type, settings);
        return instance;
    }

    public static void ReadObjectCore(ReadOnlySpan<byte> data, ref int position, object instance, Type type, BinaryMapperSettings settings)
    {
        var members = BinaryMapperCache.GetMappableMembers(type);
        int i = 0;
        while (i < members.Length)
        {
            var member = members[i];
            if (member.IsIgnored) { i++; continue; }

            if (member.BitLength.HasValue)
            {
                // Read the containing byte, extract bits for consecutive bit fields
                var byteVal = ReadByte(data, ref position);
                var byteStartBit = 0;

                while (i < members.Length && members[i].BitLength.HasValue && byteStartBit < 8)
                {
                    var m = members[i];
                    if (!m.IsIgnored)
                    {
                        var bits = m.BitLength!.Value;
                        var remaining = 8 - byteStartBit;
                        var take = Math.Min(bits, remaining);
                        var mask = (1 << take) - 1;
                        var rawValue = (byte)((byteVal >> byteStartBit) & mask);
                        var targetType = Nullable.GetUnderlyingType(m.MemberType) ?? m.MemberType;
                        // Handle bool specially: non-zero = true
                        var converted = targetType == typeof(bool)
                            ? (object)(rawValue != 0)
                            : Convert.ChangeType(rawValue, targetType);
                        m.SetValue(instance, converted);
                        byteStartBit += take;
                    }
                    i++;
                }
            }
            else
            {
                var value = ReadMember(data, ref position, member, settings);
                member.SetValue(instance, value);
                i++;
            }
        }
    }

    public static object? ReadMember(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings settings)
    {
        if (member.MemberType == typeof(string))
            return ReadString(data, ref position, member, settings);

        if (member.MemberType.IsArray)
        {
            if (!member.FixedLength.HasValue)
                throw new BinaryMapperException($"数组 '{member.MemberType.Name}' 必须指定 [FixedLength]。");

            var elementType = member.MemberType.GetElementType()!;
            var elementMember = BinaryMapperCache.GetElementMember(member, elementType);
            var length = member.FixedLength.Value;
            var array = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
                array.SetValue(ReadMember(data, ref position, elementMember, settings), i);

            return array;
        }

        if (member.MemberType != typeof(string) && (typeof(IList).IsAssignableFrom(member.MemberType) ||
            (member.MemberType.IsGenericType && member.MemberType.GetGenericTypeDefinition() == typeof(IList<>))))
        {
            Type? listType = null;
            Type elementType;

            if (member.MemberType.IsGenericType)
            {
                if (member.MemberType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    // IList<T> interface — instantiate as List<T>
                    elementType = member.MemberType.GetGenericArguments()[0];
                    listType = typeof(List<>).MakeGenericType(elementType);
                }
                else
                {
                    // Concrete generic type implementing IList, e.g. List<T>, Collection<T>
                    elementType = member.MemberType.GetGenericArguments()[0];
                    listType = member.MemberType;
                }
            }
            else if (member.MemberType == typeof(IList))
            {
                // Non-generic IList interface — instantiate as List<object>
                elementType = typeof(object);
                listType = typeof(List<object>);
            }
            else
            {
                // Non-generic concrete IList, e.g. ArrayList — get element type from ICollection<T>
                elementType = member.MemberType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .FirstOrDefault(typeof(object));
                listType = member.MemberType;
            }

            if (!member.FixedLength.HasValue)
                throw new BinaryMapperException($"IList '{member.MemberType.Name}' 必须指定 [FixedLength]。");

            var elementMember = BinaryMapperCache.GetElementMember(member, elementType);
            var list = (IList)BinaryMapperCache.CreateInstance(listType);
            var length = member.FixedLength.Value;

            for (int i = 0; i < length; i++)
                list.Add(ReadMember(data, ref position, elementMember, settings));

            return list;
        }

        if (TryReadBinaryType(data, ref position, member, settings, out var binaryValue))
            return binaryValue;

        if (member.MemberType.IsClass && member.MemberType != typeof(string))
            return ReadObject(data, ref position, member.MemberType, settings);

        return ReadPrimitive(data, ref position, member, settings);
    }

    public static object ReadPrimitive(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings settings)
    {
        var underlyingType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (underlyingType.IsEnum)
        {
            var enumUnderlying = Enum.GetUnderlyingType(underlyingType);
            var enumValue = ReadMember(data, ref position, BinaryMapperCache.GetElementMember(member, enumUnderlying), settings);
            return Enum.ToObject(underlyingType, enumValue!);
        }

        if (underlyingType.IsValueType)
        {
            var nestedInstance = BinaryMapperCache.CreateInstance(underlyingType);
            ReadObjectCore(data, ref position, nestedInstance, underlyingType, settings);
            return nestedInstance;
        }

        throw new BinaryMapperException($"不支持映射到类型 '{member.MemberType.FullName}'。");
    }

    public static bool TryReadBinaryType(ReadOnlySpan<byte> data, ref int position, MappableMember member, BinaryMapperSettings settings, out object? value)
    {
        value = null!;

        var checkType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (checkType == typeof(short))
        {
            value = ReadInt16(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(ushort))
        {
            value = ReadUInt16(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(int))
        {
            value = ReadInt32(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(uint))
        {
            value = ReadUInt32(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(long))
        {
            value = ReadInt64(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(ulong))
        {
            value = ReadUInt64(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(byte))
        {
            value = ReadByte(data, ref position);
            return true;
        }
        if (checkType == typeof(sbyte))
        {
            value = (sbyte)ReadByte(data, ref position);
            return true;
        }
        if (checkType == typeof(Half))
        {
            value = ReadHalf(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(float))
        {
            value = ReadSingle(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(double))
        {
            value = ReadDouble(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(bool))
        {
            value = ReadBoolean(data, ref position);
            return true;
        }
        if (checkType == typeof(char))
        {
            value = (char)ReadUInt16(data, ref position, member.Endianness);
            return true;
        }
        if (checkType == typeof(Guid))
        {
            value = ReadGuid(data, ref position);
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

    public static void WriteObject(BinWriter writer, object instance, Type type, BinaryMapperSettings settings)
    {
        var members = BinaryMapperCache.GetMappableMembers(type);
        int i = 0;
        while (i < members.Length)
        {
            var member = members[i];
            if (member.IsIgnored) { i++; continue; }

            if (member.BitLength.HasValue)
            {
                byte byteVal = 0;
                var bitPos = 0;

                while (i < members.Length && members[i].BitLength.HasValue && bitPos < 8)
                {
                    var m = members[i];
                    if (!m.IsIgnored)
                    {
                        var bits = m.BitLength!.Value;
                        var remaining = 8 - bitPos;
                        var take = Math.Min(bits, remaining);
                        var rawValue = Convert.ToByte(m.GetValue(instance)!);
                        byteVal |= (byte)((rawValue & ((1 << take) - 1)) << bitPos);
                        bitPos += take;
                    }
                    i++;
                }

                writer.WriteByte(byteVal);
            }
            else
            {
                WriteMember(writer, member, instance, settings);
                i++;
            }
        }
    }

    public static void WriteMember(BinWriter writer, MappableMember member, object instance, BinaryMapperSettings settings)
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

        if (value is Array array)
        {
            var elementType = value.GetType().GetElementType()!;
            var elementMember = BinaryMapperCache.GetElementMember(member, elementType);
            for (int i = 0; i < array.Length; i++)
                WriteElement(writer, array.GetValue(i)!, elementMember, settings);
            return;
        }

        if (value is IList list)
        {
            var elementType = value.GetType().GetGenericArguments()[0];
            var elementMember = BinaryMapperCache.GetElementMember(member, elementType);
            for (int i = 0; i < list.Count; i++)
                WriteElement(writer, list[i]!, elementMember, settings);
            return;
        }

        if (value is not string && member.MemberType.IsClass)
        {
            WriteObject(writer, value, member.MemberType, settings);
            return;
        }

        throw new BinaryMapperException($"不支持序列化类型 '{member.MemberType.FullName}'。");
    }

    public static bool TryWriteBinaryType(BinWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        return value switch
        {
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

    /// <summary>
    /// 写入单个元素值（用于数组/IList/CArray 的元素级分发）。<br/>
    /// Writes a single element value (element-level dispatch for arrays/IList/CArray).
    /// </summary>
    private static void WriteElement(BinWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        if (member.MemberType == typeof(string))
        {
            WriteString(writer, (string)value, member, settings);
            return;
        }

        if (TryWriteBinaryType(writer, value, member, settings))
            return;

        if (TryWritePrimitive(writer, value, member, settings))
            return;

        if (!member.MemberType.IsValueType)
        {
            WriteObject(writer, value, member.MemberType, settings);
            return;
        }

        throw new BinaryMapperException($"不支持序列化类型 '{member.MemberType.FullName}'。");
    }

    public static bool TryWritePrimitive(BinWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
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
        if (underlyingType == typeof(Half)) { writer.WriteHalf((Half)value, member.Endianness); return true; }
        if (underlyingType == typeof(Guid)) { writer.WriteGuid((Guid)value); return true; }

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

    public static void WriteString(BinWriter writer, string value, MappableMember member, BinaryMapperSettings settings)
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

    public static Half ReadHalf(ReadOnlySpan<byte> data, ref int position, Endianness endianness)
    {
        var span = Slice(data, ref position, sizeof(short));
        Span<byte> tmp = stackalloc byte[sizeof(short)];
        span.CopyTo(tmp);
        if (endianness != Endianness.BigEndian)
            tmp.Reverse();
        return BinaryPrimitives.ReadHalfBigEndian(tmp);
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

    public static Guid ReadGuid(ReadOnlySpan<byte> data, ref int position)
    {
        var buffer = ReadBytes(data, ref position, 16);
        return new Guid(buffer, bigEndian: true);
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

    private static bool WriteAndReturnTrue(BinWriter writer, short value, Endianness endianness)
    {
        writer.WriteInt16(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, ushort value, Endianness endianness)
    {
        writer.WriteUInt16(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, int value, Endianness endianness)
    {
        writer.WriteInt32(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, uint value, Endianness endianness)
    {
        writer.WriteUInt32(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, long value, Endianness endianness)
    {
        writer.WriteInt64(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, ulong value, Endianness endianness)
    {
        writer.WriteUInt64(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, byte value)
    {
        writer.WriteByte(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, sbyte value)
    {
        writer.WriteSByte(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, float value, Endianness endianness)
    {
        writer.WriteSingle(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, double value, Endianness endianness)
    {
        writer.WriteDouble(value, endianness);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, bool value)
    {
        writer.WriteBoolean(value);
        return true;
    }

    private static bool WriteAndReturnTrue(BinWriter writer, char value, Endianness endianness)
    {
        writer.WriteChar(value, endianness);
        return true;
    }

    #endregion
}
