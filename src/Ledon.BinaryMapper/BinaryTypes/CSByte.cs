using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="sbyte"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="sbyte"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CSByte
{
    private sbyte _sbyte;

    /// <summary>
    /// 获取或设置底层 <see cref="sbyte"/> 值。<br/>
    /// Gets or sets the underlying <see cref="sbyte"/> value.
    /// </summary>
    public sbyte Value
    {
        readonly get => _sbyte;
        set => _sbyte = value;
    }

    /// <summary>
    /// 初始化 <see cref="CSByte"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CSByte"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="sbyte"/> 值。<br/>The <see cref="sbyte"/> value to wrap.</param>
    public CSByte(sbyte value) => _sbyte = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _sbyte.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CSByte other && _sbyte.Equals(other._sbyte);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _sbyte.GetHashCode();

    /// <summary>将 <see cref="CSByte"/> 隐式转换为 <see cref="sbyte"/>。<br/>Implicitly converts a <see cref="CSByte"/> to a <see cref="sbyte"/>.</summary>
    public static implicit operator sbyte(CSByte c) => c._sbyte;

    /// <summary>将 <see cref="sbyte"/> 隐式转换为 <see cref="CSByte"/>。<br/>Implicitly converts a <see cref="sbyte"/> to a <see cref="CSByte"/>.</summary>
    public static implicit operator CSByte(sbyte value) => new CSByte(value);
}