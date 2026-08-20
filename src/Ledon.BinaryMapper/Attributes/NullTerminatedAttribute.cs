using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// 表示字段或属性采用空字符终止格式。<br/>
/// Indicates that a field or property uses a null-terminated format.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class NullTerminatedAttribute : Attribute
{
    /// <summary>
    /// 初始化 <see cref="NullTerminatedAttribute"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="NullTerminatedAttribute"/> class.
    /// </summary>
    public NullTerminatedAttribute()
    {
    }
}
