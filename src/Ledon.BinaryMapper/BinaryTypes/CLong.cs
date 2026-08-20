using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="long"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="long"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CLong
{
    private long _long;

    /// <summary>
    /// 获取或设置底层 <see cref="long"/> 值。<br/>
    /// Gets or sets the underlying <see cref="long"/> value.
    /// </summary>
    public long Value
    {
        readonly get => _long;
        set => _long = value;
    }

    /// <summary>
    /// 初始化 <see cref="CLong"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CLong"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="long"/> 值。<br/>The <see cref="long"/> value to wrap.</param>
    public CLong(long value) => _long = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _long.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CLong other && _long.Equals(other._long);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _long.GetHashCode();

    /// <summary>将 <see cref="CLong"/> 隐式转换为 <see cref="long"/>。<br/>Implicitly converts a <see cref="CLong"/> to a <see cref="long"/>.</summary>
    public static implicit operator long(CLong c) => c._long;

    /// <summary>将 <see cref="long"/> 隐式转换为 <see cref="CLong"/>。<br/>Implicitly converts a <see cref="long"/> to a <see cref="CLong"/>.</summary>
    public static implicit operator CLong(long value) => new CLong(value);
}