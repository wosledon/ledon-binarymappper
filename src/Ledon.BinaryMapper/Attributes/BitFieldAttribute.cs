using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>指定字段或属性占用的位数（用于 bit-level 打包）。<br/>Specifies the number of bits a field or property occupies (for bit-level packing).</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BitFieldAttribute : Attribute
{
    /// <summary>获取占用的位数。<br/>Gets the number of bits occupied.</summary>
    public int Length { get; }

    /// <summary>初始化 <see cref="BitFieldAttribute"/> 的新实例。<br/>Initializes a new instance of the <see cref="BitFieldAttribute"/> class.</summary>
    /// <param name="length">占用的位数。<br/>The number of bits.</param>
    public BitFieldAttribute(int length) => Length = length;
}