using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Ledon.BinaryMapper;
using Ledon.BinaryMapper.Attributes;

namespace Ledon.BinaryMapper.Internal;

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
        _isIgnored = member.GetCustomAttribute<IgnoreAttribute>() != null;
        _fixedLength = member.GetCustomAttribute<FixedLengthAttribute>()?.Length;
        _nullTerminated = member.GetCustomAttribute<NullTerminatedAttribute>() != null;
        _encoding = ResolveEncoding(member.GetCustomAttribute<EncodingAttribute>()?.Name);
        _endianness = ResolveEndianness(member);
        TryCompileAccessors(member, memberType);
    }

    public MemberInfo Member { get; }

    public Type MemberType { get; }

    private readonly bool _isIgnored;
    private readonly int? _fixedLength;
    private readonly bool _nullTerminated;
    private readonly Encoding? _encoding;
    private readonly Endianness _endianness;
    private Func<object, object?>? _getter;
    private Action<object, object?>? _setter;

    public bool IsIgnored => _isIgnored;
    public int? FixedLength => _fixedLength;
    public bool NullTerminated => _nullTerminated;
    public Encoding? Encoding => _encoding;
    public Endianness Endianness => _endianness;

    public void SetValue(object instance, object? value)
    {
        if (_setter != null)
        {
            _setter(instance, value);
            return;
        }
        // Fallback for WithType members where types don't match
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
        if (_getter != null)
            return _getter(instance);
        // Fallback for WithType members where types don't match
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
        var m = new MappableMember(Member, memberType);
        // For element types that differ from the declared member type,
        // compiled getter/setter would produce invalid assignments,
        // so skip compilation — reflection fallback handles them.
        m._getter = null!;
        m._setter = null!;
        return m;
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

    private void TryCompileAccessors(MemberInfo member, Type memberType)
    {
        // Only compile when the memberType matches the declared field/property type.
        // WithType may create members where types differ, which would cause
        // Expression.Assign to throw during compilation.
        Type declaredType = member switch
        {
            FieldInfo f => f.FieldType,
            PropertyInfo p => p.PropertyType,
            _ => memberType
        };
        if (declaredType == memberType)
        {
            _getter = CompileGetter(member);
            _setter = CompileSetter(member, memberType);
        }
    }

    private static Func<object, object?> CompileGetter(MemberInfo member)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var cast = Expression.Convert(instance, member.DeclaringType!);
        Expression access = member switch
        {
            FieldInfo f => Expression.Field(cast, f),
            PropertyInfo p => Expression.Property(cast, p),
            _ => throw new BinaryMapperException($"不支持的成员类型: {member.MemberType}。")
        };
        return Expression.Lambda<Func<object, object?>>(
            Expression.Convert(access, typeof(object)), instance).Compile();
    }

    private static Action<object, object?> CompileSetter(MemberInfo member, Type memberType)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var value = Expression.Parameter(typeof(object), "value");
        var castInstance = Expression.Convert(instance, member.DeclaringType!);
        var castValue = Expression.Convert(value, memberType);
        Expression access = member switch
        {
            FieldInfo f => Expression.Field(castInstance, f),
            PropertyInfo p => Expression.Property(castInstance, p),
            _ => throw new BinaryMapperException($"不支持的成员类型: {member.MemberType}。")
        };
        return Expression.Lambda<Action<object, object?>>(
            Expression.Assign(access, castValue), instance, value).Compile();
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
