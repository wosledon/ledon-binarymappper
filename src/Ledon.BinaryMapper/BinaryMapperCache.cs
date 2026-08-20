using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Ledon.BinaryMapper;

/// <summary>
/// 提供类型映射成员的缓存。<br/>
/// Provides a cache for type mapping members.
/// </summary>
internal static class BinaryMapperCache
{
    private static readonly ConcurrentDictionary<Type, MappableMember[]> s_cache = new();

    /// <summary>
    /// 获取指定类型的可映射成员列表（已缓存）。<br/>
    /// Gets the cached mappable members for the specified type.
    /// </summary>
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
}