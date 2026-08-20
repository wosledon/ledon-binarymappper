using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="ulong"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="ulong"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CULong
{
    private ulong _ulong;

    /// <summary>
    /// 获取或设置底层 <see cref="ulong"/> 值。<br/>
    /// Gets or sets the underlying <see cref="ulong"/> value.
    /// </summary>
    public ulong Value
    {
        readonly get => _ulong;
        set => _ulong = value;
    }

    /// <summary>
    /// 初始化 <see cref="CULong"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CULong"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="ulong"/> 值。<br/>The <see cref="ulong"/> value to wrap.</param>
    public CULong(ulong value) => _ulong = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _ulong.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CULong other && _ulong.Equals(other._ulong);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _ulong.GetHashCode();

    /// <summary>将 <see cref="CULong"/> 隐式转换为 <see cref="ulong"/>。<br/>Implicitly converts a <see cref="CULong"/> to a <see cref="ulong"/>.</summary>
    public static implicit operator ulong(CULong c) => c._ulong;

    /// <summary>将 <see cref="ulong"/> 隐式转换为 <see cref="CULong"/>。<br/>Implicitly converts a <see cref="ulong"/> to a <see cref="CULong"/>.</summary>
    public static implicit operator CULong(ulong value) => new CULong(value);
}