using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// 指定字段或属性在二进制结构中的偏移量。<br/>
/// Specifies the offset of a field or property within the binary structure.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class OffsetAttribute : Attribute
{
    /// <summary>
    /// 初始化 <see cref="OffsetAttribute"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="OffsetAttribute"/> class.
    /// </summary>
    /// <param name="offset">相对于结构起始位置的偏移量。<br/>The offset relative to the start of the structure.</param>
    public OffsetAttribute(int offset)
    {
        OffsetValue = offset;
    }

    /// <summary>
    /// 获取偏移量。<br/>
    /// Gets the offset value.
    /// </summary>
    public int OffsetValue { get; }
}
