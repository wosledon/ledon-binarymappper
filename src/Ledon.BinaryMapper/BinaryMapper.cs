using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper.Attributes;
using Ledon.BinaryMapper.BinaryTypes;

namespace Ledon.BinaryMapper;

/// <summary>
/// 提供将对象序列化为二进制数据，以及从二进制数据反序列化对象的能力。<br/>
/// Provides the ability to serialize objects into binary data and deserialize binary data back into objects.
/// </summary>
public static class BinaryMapper
{
    /// <summary>
    /// 将对象序列化为二进制数据。<br/>
    /// Serializes an object into binary data.
    /// </summary>
    /// <param name="obj">要序列化的对象。<br/>The object to serialize.</param>
    /// <returns>序列化后的二进制数据。<br/>The serialized binary data.</returns>
    public static byte[] Serialize(object obj)
    {
        return Serialize(obj, null);
    }

    /// <summary>
    /// 将对象序列化为二进制数据。<br/>
    /// Serializes an object into binary data using the specified settings.
    /// </summary>
    /// <param name="obj">要序列化的对象。<br/>The object to serialize.</param>
    /// <param name="settings">序列化设置。<br/>The serialization settings.</param>
    /// <returns>序列化后的二进制数据。<br/>The serialized binary data.</returns>
    public static byte[] Serialize(object obj, BinaryMapperSettings? settings)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        settings ??= new BinaryMapperSettings();
        var writer = new BinaryWriter();
        WriteObject(writer, obj, obj.GetType(), settings);
        return writer.ToArray();
    }

    /// <summary>
    /// 从二进制数据反序列化指定类型的对象。<br/>
    /// Deserializes an object of the specified type from binary data.
    /// </summary>
    /// <typeparam name="T">要反序列化的目标类型。<br/>The target type to deserialize.</typeparam>
    /// <param name="data">包含二进制数据的缓冲区。<br/>The buffer containing binary data.</param>
    /// <returns>反序列化得到的对象。<br/>The deserialized object.</returns>
    public static T Deserialize<T>(byte[] data)
    {
        return (T)Deserialize(data, typeof(T), null);
    }

    /// <summary>
    /// 从二进制数据反序列化指定类型的对象。<br/>
    /// Deserializes an object of the specified type from binary data using the specified settings.
    /// </summary>
    /// <typeparam name="T">要反序列化的目标类型。<br/>The target type to deserialize.</typeparam>
    /// <param name="data">包含二进制数据的缓冲区。<br/>The buffer containing binary data.</param>
    /// <param name="settings">反序列化设置。<br/>The deserialization settings.</param>
    /// <returns>反序列化得到的对象。<br/>The deserialized object.</returns>
    public static T Deserialize<T>(byte[] data, BinaryMapperSettings? settings)
    {
        return (T)Deserialize(data, typeof(T), settings);
    }

    /// <summary>
    /// 从二进制数据反序列化指定类型的对象。<br/>
    /// Deserializes an object of the specified type from binary data.
    /// </summary>
    /// <param name="data">包含二进制数据的缓冲区。<br/>The buffer containing binary data.</param>
    /// <param name="type">要反序列化的目标类型。<br/>The target type to deserialize.</param>
    /// <returns>反序列化得到的对象。<br/>The deserialized object.</returns>
    public static object Deserialize(byte[] data, Type type)
    {
        return Deserialize(data, type, null);
    }

    /// <summary>
    /// 从二进制数据反序列化指定类型的对象。<br/>
    /// Deserializes an object of the specified type from binary data using the specified settings.
    /// </summary>
    /// <param name="data">包含二进制数据的缓冲区。<br/>The buffer containing binary data.</param>
    /// <param name="type">要反序列化的目标类型。<br/>The target type to deserialize.</param>
    /// <param name="settings">反序列化设置。<br/>The deserialization settings.</param>
    /// <returns>反序列化得到的对象。<br/>The deserialized object.</returns>
    public static object Deserialize(byte[] data, Type type, BinaryMapperSettings? settings)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        settings ??= new BinaryMapperSettings();
        var reader = new BinaryReader(data);
        return ReadObject(reader, type, settings);
    }

    private static object ReadObject(BinaryReader reader, Type type, BinaryMapperSettings settings)
    {
        var instance = Activator.CreateInstance(type)!;
        ReadObjectCore(reader, instance, type, settings);
        return instance;
    }

    private static void ReadObjectCore(BinaryReader reader, object instance, Type type, BinaryMapperSettings settings)
    {
        foreach (var member in GetMappableMembers(type))
        {
            if (member.IsIgnored)
            {
                continue;
            }

            if (member.Offset.HasValue)
            {
                reader.Seek(member.Offset.Value);
            }

            var value = ReadMember(reader, member, settings);
            member.SetValue(instance, value);
        }
    }

    private static void WriteObject(BinaryWriter writer, object instance, Type type, BinaryMapperSettings settings)
    {
        foreach (var member in GetMappableMembers(type))
        {
            if (member.IsIgnored)
            {
                continue;
            }

            WriteMember(writer, member, instance, settings);
        }
    }

    private static void WriteMember(BinaryWriter writer, MappableMember member, object instance, BinaryMapperSettings settings)
    {
        var value = member.GetValue(instance);

        if (value == null)
        {
            return;
        }

        if (member.MemberType == typeof(string))
        {
            WriteString(writer, (string)value, member, settings);
            return;
        }

        if (TryWriteBinaryType(writer, value, member, settings))
        {
            return;
        }

        if (TryWritePrimitive(writer, value, member, settings))
        {
            return;
        }

        if (value is not string && member.MemberType.IsClass)
        {
            WriteObject(writer, value, member.MemberType, settings);
            return;
        }

        throw new BinaryMapperException($"不支持序列化类型 '{member.MemberType.FullName}'。");
    }

    private static object? ReadMember(BinaryReader reader, MappableMember member, BinaryMapperSettings settings)
    {
        if (member.MemberType == typeof(string))
        {
            return ReadString(reader, member, settings);
        }

        if (TryReadBinaryType(reader, member, out var binaryValue))
        {
            return binaryValue;
        }

        return ReadPrimitive(reader, member, settings);
    }

    private static bool TryWriteBinaryType(BinaryWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        if (value is string text)
        {
            WriteString(writer, text, member, settings);
            return true;
        }

        switch (value)
        {
            case CString cstring:
                WriteString(writer, cstring.Value, member, settings);
                return true;
            case short s:
                writer.WriteInt16(s, member.Endianness);
                return true;
            case ushort us:
                writer.WriteUInt16(us, member.Endianness);
                return true;
            case int i:
                writer.WriteInt32(i, member.Endianness);
                return true;
            case uint ui:
                writer.WriteUInt32(ui, member.Endianness);
                return true;
            case long l:
                writer.WriteInt64(l, member.Endianness);
                return true;
            case ulong ul:
                writer.WriteUInt64(ul, member.Endianness);
                return true;
            case byte b:
                writer.WriteByte(b);
                return true;
            case sbyte sb:
                writer.WriteSByte(sb);
                return true;
            case float f:
                writer.WriteSingle(f, member.Endianness);
                return true;
            case double d:
                writer.WriteDouble(d, member.Endianness);
                return true;
            case bool flag:
                writer.WriteBoolean(flag);
                return true;
            case char c:
                writer.WriteChar(c, member.Endianness);
                return true;
            default:
                return false;
        }
    }

    private static bool TryWritePrimitive(BinaryWriter writer, object value, MappableMember member, BinaryMapperSettings settings)
    {
        var underlyingType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (underlyingType == typeof(short))
        {
            writer.WriteInt16((short)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(ushort))
        {
            writer.WriteUInt16((ushort)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(int))
        {
            writer.WriteInt32((int)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(uint))
        {
            writer.WriteUInt32((uint)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(long))
        {
            writer.WriteInt64((long)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(ulong))
        {
            writer.WriteUInt64((ulong)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(byte))
        {
            writer.WriteByte((byte)value);
            return true;
        }

        if (underlyingType == typeof(sbyte))
        {
            writer.WriteSByte((sbyte)value);
            return true;
        }

        if (underlyingType == typeof(float))
        {
            writer.WriteSingle((float)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(double))
        {
            writer.WriteDouble((double)value, member.Endianness);
            return true;
        }

        if (underlyingType == typeof(bool))
        {
            writer.WriteBoolean((bool)value);
            return true;
        }

        if (underlyingType == typeof(char))
        {
            writer.WriteChar((char)value, member.Endianness);
            return true;
        }

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

    private static object ReadPrimitive(BinaryReader reader, MappableMember member, BinaryMapperSettings settings)
    {
        var underlyingType = Nullable.GetUnderlyingType(member.MemberType) ?? member.MemberType;

        if (underlyingType == typeof(short))
        {
            return reader.ReadInt16(member.Endianness);
        }

        if (underlyingType == typeof(ushort))
        {
            return reader.ReadUInt16(member.Endianness);
        }

        if (underlyingType == typeof(int))
        {
            return reader.ReadInt32(member.Endianness);
        }

        if (underlyingType == typeof(uint))
        {
            return reader.ReadUInt32(member.Endianness);
        }

        if (underlyingType == typeof(long))
        {
            return reader.ReadInt64(member.Endianness);
        }

        if (underlyingType == typeof(ulong))
        {
            return reader.ReadUInt64(member.Endianness);
        }

        if (underlyingType == typeof(byte))
        {
            return reader.ReadByte();
        }

        if (underlyingType == typeof(sbyte))
        {
            return reader.ReadSByte();
        }

        if (underlyingType == typeof(float))
        {
            return reader.ReadSingle(member.Endianness);
        }

        if (underlyingType == typeof(double))
        {
            return reader.ReadDouble(member.Endianness);
        }

        if (underlyingType == typeof(bool))
        {
            return reader.ReadBoolean();
        }

        if (underlyingType == typeof(char))
        {
            return reader.ReadChar(member.Endianness);
        }

        if (underlyingType.IsEnum)
        {
            var enumUnderlying = Enum.GetUnderlyingType(underlyingType);
            var enumValue = ReadPrimitive(reader, member.WithType(enumUnderlying), settings);
            return Enum.ToObject(underlyingType, enumValue);
        }

        if (underlyingType.IsValueType)
        {
            var nestedInstance = Activator.CreateInstance(underlyingType)!;
            ReadObjectCore(reader, nestedInstance, underlyingType, settings);
            return nestedInstance;
        }

        throw new BinaryMapperException($"不支持映射到类型 '{member.MemberType.FullName}'。");
    }

    private static bool TryReadBinaryType(BinaryReader reader, MappableMember member, out object? value)
    {
        value = null!;

        if (member.MemberType == typeof(CString))
        {
            value = new CString((string)ReadString(reader, member, null!));
            return true;
        }

        if (member.MemberType == typeof(short))
        {
            value = reader.ReadInt16(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(ushort))
        {
            value = reader.ReadUInt16(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(int))
        {
            value = reader.ReadInt32(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(uint))
        {
            value = reader.ReadUInt32(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(long))
        {
            value = reader.ReadInt64(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(ulong))
        {
            value = reader.ReadUInt64(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(byte))
        {
            value = reader.ReadByte();
            return true;
        }

        if (member.MemberType == typeof(sbyte))
        {
            value = reader.ReadSByte();
            return true;
        }

        if (member.MemberType == typeof(float))
        {
            value = reader.ReadSingle(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(double))
        {
            value = reader.ReadDouble(member.Endianness);
            return true;
        }

        if (member.MemberType == typeof(bool))
        {
            value = reader.ReadBoolean();
            return true;
        }

        if (member.MemberType == typeof(char))
        {
            value = reader.ReadChar(member.Endianness);
            return true;
        }

        return false;
    }

    private static void WriteString(BinaryWriter writer, string value, MappableMember member, BinaryMapperSettings settings)
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

        if (value != null)
        {
            var bytes = encoding.GetBytes(value);
            writer.WriteBytes(bytes, 0, bytes.Length);
        }
    }

    private static object ReadString(BinaryReader reader, MappableMember member, BinaryMapperSettings? settings)
    {
        var encoding = member.Encoding ?? settings?.Encoding ?? Encoding.UTF8;

        if (member.FixedLength.HasValue)
        {
            var buffer = reader.ReadBytes(member.FixedLength.Value);
            buffer = TrimNullTerminator(buffer);
            return encoding.GetString(buffer);
        }

        if (member.NullTerminated)
        {
            return reader.ReadNullTerminatedString(encoding);
        }

        var remaining = reader.Remaining;
        var fallbackBuffer = reader.ReadBytes(remaining);
        return encoding.GetString(fallbackBuffer);
    }

    private static byte[] TrimNullTerminator(byte[] buffer)
    {
        var length = buffer.Length;

        while (length > 0 && buffer[length - 1] == 0)
        {
            length--;
        }

        if (length == buffer.Length)
        {
            return buffer;
        }

        var trimmed = new byte[length];
        Buffer.BlockCopy(buffer, 0, trimmed, 0, length);
        return trimmed;
    }

    private static MappableMember[] GetMappableMembers(Type type)
    {
        return type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => MappableMember.Create(m))
            .OfType<MappableMember>()
            .OrderBy(m => m.Member.MetadataToken)
            .ToArray();
    }
}
