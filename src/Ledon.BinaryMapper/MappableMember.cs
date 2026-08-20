using System;
using System.IO;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper.Attributes;

namespace Ledon.BinaryMapper;

/// <summary>
/// 表示映射过程中的元数据。<br/>
/// Represents metadata for mapping.
/// </summary>
internal sealed class MappableMember
{
    private MappableMember(MemberInfo member, Type memberType)
    {
        Member = member;
        MemberType = memberType;
    }

    public MemberInfo Member { get; }

    public Type MemberType { get; }

    public bool IsIgnored => Member.GetCustomAttribute<IgnoreAttribute>() != null;

    public int? Offset => Member.GetCustomAttribute<OffsetAttribute>()?.OffsetValue;

    public int? FixedLength => Member.GetCustomAttribute<FixedLengthAttribute>()?.Length;

    public bool NullTerminated => Member.GetCustomAttribute<NullTerminatedAttribute>() != null;

    public Encoding? Encoding => ResolveEncoding(Member.GetCustomAttribute<EncodingAttribute>()?.Name);

    public Endianness Endianness => ResolveEndianness(Member);

    public void SetValue(object instance, object? value)
    {
        switch (Member)
        {
            case FieldInfo field:
                field.SetValue(instance, value);
                break;
            case PropertyInfo property when property.CanWrite:
                property.SetValue(instance, value);
                break;
        }
    }

    public object? GetValue(object instance)
    {
        switch (Member)
        {
            case FieldInfo field:
                return field.GetValue(instance);
            case PropertyInfo property when property.CanRead:
                return property.GetValue(instance);
            default:
                return null;
        }
    }

    public MappableMember WithType(Type memberType)
    {
        return new MappableMember(Member, memberType);
    }

    public static MappableMember? Create(MemberInfo member)
    {
        if (member is FieldInfo field && IsMappableField(field))
        {
            return new MappableMember(member, field.FieldType);
        }

        if (member is PropertyInfo property && IsMappableProperty(property))
        {
            return new MappableMember(member, property.PropertyType);
        }

        return null;
    }

    private static bool IsMappableField(FieldInfo field)
    {
        return field.IsPublic || field.GetCustomAttribute<IgnoreAttribute>() != null;
    }

    private static bool IsMappableProperty(PropertyInfo property)
    {
        return property.CanRead && property.GetMethod != null && property.GetMethod.IsPublic;
    }

    private static Encoding? ResolveEncoding(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        try
        {
            return Encoding.GetEncoding(name);
        }
        catch (ArgumentException)
        {
            throw new BinaryMapperException($"不支持的编码: '{name}'。");
        }
    }

    private static Endianness ResolveEndianness(MemberInfo member)
    {
        if (member.GetCustomAttribute<BigEndianAttribute>() != null)
        {
            return Endianness.BigEndian;
        }

        if (member.GetCustomAttribute<LittleEndianAttribute>() != null)
        {
            return Endianness.LittleEndian;
        }

        return Endianness.BigEndian;
    }
}
