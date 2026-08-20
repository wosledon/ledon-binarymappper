using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="byte"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="byte"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CByte
{
    private byte _byte;

    /// <summary>
    /// 获取或设置底层 <see cref="byte"/> 值。<br/>
    /// Gets or sets the underlying <see cref="byte"/> value.
    /// </summary>
    public byte Value
    {
        readonly get => _byte;
        set => _byte = value;
    }

    /// <summary>
    /// 初始化 <see cref="CByte"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CByte"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="byte"/> 值。<br/>The <see cref="byte"/> value to wrap.</param>
    public CByte(byte value) => _byte = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _byte.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CByte other && _byte.Equals(other._byte);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _byte.GetHashCode();

    /// <summary>将 <see cref="CByte"/> 隐式转换为 <see cref="byte"/>。<br/>Implicitly converts a <see cref="CByte"/> to a <see cref="byte"/>.</summary>
    public static implicit operator byte(CByte c) => c._byte;

    /// <summary>将 <see cref="byte"/> 隐式转换为 <see cref="CByte"/>。<br/>Implicitly converts a <see cref="byte"/> to a <see cref="CByte"/>.</summary>
    public static implicit operator CByte(byte value) => new CByte(value);
}