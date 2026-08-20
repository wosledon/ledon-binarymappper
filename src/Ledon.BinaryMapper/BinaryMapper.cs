using System;
using Ledon.BinaryMapper.Internal;
using BinWriter = Ledon.BinaryMapper.IO.BinaryWriter;
using BinReader = Ledon.BinaryMapper.IO.BinaryReader;

namespace Ledon.BinaryMapper;

/// <summary>
/// 提供将对象序列化为二进制数据，以及从二进制数据反序列化对象的能力。<br/>
/// Provides the ability to serialize objects into binary data and deserialize binary data back into objects.
/// </summary>
public static class BinaryMapper
{
    /// <summary>将对象序列化为二进制数据。<br/>Serializes an object into binary data.</summary>
    public static byte[] Serialize(object obj) => Serialize(obj, null);

    /// <summary>将对象序列化为二进制数据。<br/>Serializes an object into binary data using the specified settings.</summary>
    public static byte[] Serialize(object obj, BinaryMapperSettings? settings)
    {
        ArgumentNullException.ThrowIfNull(obj);
        settings ??= new BinaryMapperSettings();
        var writer = new BinWriter();
        BinaryMapperHelpers.WriteObject(writer, obj, obj.GetType(), settings);
        return writer.ToArray();
    }

    /// <summary>从二进制数据反序列化对象。<br/>Deserializes an object from binary data.</summary>
    public static T Deserialize<T>(byte[] data) => (T)Deserialize(data, typeof(T), null);

    /// <summary>从二进制数据反序列化对象。<br/>Deserializes an object from binary data using the specified settings.</summary>
    public static T Deserialize<T>(byte[] data, BinaryMapperSettings? settings) => (T)Deserialize(data, typeof(T), settings);

    /// <summary>从二进制数据反序列化对象。<br/>Deserializes an object from binary data.</summary>
    public static object Deserialize(byte[] data, Type type) => Deserialize(data, type, null);

    /// <summary>从二进制数据反序列化对象。<br/>Deserializes an object from binary data using the specified settings.</summary>
    public static object Deserialize(byte[] data, Type type, BinaryMapperSettings? settings)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(type);
        return Deserialize(data.AsSpan(), type, settings);
    }

    /// <summary>从只读跨度反序列化对象。<br/>Deserializes an object from a read-only span.</summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> data) => (T)Deserialize(data, typeof(T), null);

    /// <summary>从只读跨度反序列化对象。<br/>Deserializes an object from a read-only span using the specified settings.</summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> data, BinaryMapperSettings? settings) => (T)Deserialize(data, typeof(T), settings);

    /// <summary>从只读跨度反序列化对象。<br/>Deserializes an object from a read-only span.</summary>
    public static object Deserialize(ReadOnlySpan<byte> data, Type type) => Deserialize(data, type, null);

    /// <summary>从只读跨度反序列化对象。<br/>Deserializes an object from a read-only span using the specified settings.</summary>
    public static object Deserialize(ReadOnlySpan<byte> data, Type type, BinaryMapperSettings? settings)
    {
        ArgumentNullException.ThrowIfNull(type);
        settings ??= new BinaryMapperSettings();
        var position = 0;
        return BinaryMapperHelpers.ReadObject(data, ref position, type, settings);
    }
}
