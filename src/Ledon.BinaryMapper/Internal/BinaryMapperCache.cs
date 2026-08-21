using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ledon.BinaryMapper.Internal;

/// <summary>
/// 提供类型映射成员的缓存。<br/>
/// Provides a cache for type mapping members.
/// </summary>
internal static class BinaryMapperCache
{
    private static readonly ConcurrentDictionary<Type, MappableMember[]> s_cache = new();
    private static readonly ConcurrentDictionary<Type, Func<object>> s_factories = new();
    private static readonly ConcurrentDictionary<(MemberInfo, Type), MappableMember> s_elementMembers = new();

    /// <summary>获取指定类型的可映射成员列表（已缓存）。<br/>Gets the cached mappable members for the specified type.</summary>
    public static MappableMember[] GetMappableMembers(Type type)
    {
        return s_cache.GetOrAdd(type, static t =>
        {
            return t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(m => MappableMember.Create(m))
                .OfType<MappableMember>()
                .OrderBy(m => m.Member.MetadataToken)
                .ToArray();
        });
    }

    /// <summary>创建指定类型的实例（工厂委托已缓存）。<br/>Creates an instance of the specified type (factory delegate cached).</summary>
    public static object CreateInstance(Type type)
    {
        return s_factories.GetOrAdd(type, static t =>
        {
            var ctor = t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            if (ctor == null)
                return () => Activator.CreateInstance(t)!;

            var newExpr = Expression.New(ctor);
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(newExpr, typeof(object)));
            return lambda.Compile();
        })();
    }

    /// <summary>获取元素的映射成员（已缓存）。<br/>Gets the cached element mapping member.</summary>
    public static MappableMember GetElementMember(MappableMember parent, Type elementType)
    {
        return s_elementMembers.GetOrAdd((parent.Member, elementType), key => parent.WithType(key.Item2));
    }
}
