using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="uint"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="uint"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CUInt
{
    private uint _uint;

    /// <summary>
    /// 获取或设置底层 <see cref="uint"/> 值。<br/>
    /// Gets or sets the underlying <see cref="uint"/> value.
    /// </summary>
    public uint Value
    {
        readonly get => _uint;
        set => _uint = value;
    }

    /// <summary>
    /// 初始化 <see cref="CUInt"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CUInt"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="uint"/> 值。<br/>The <see cref="uint"/> value to wrap.</param>
    public CUInt(uint value) => _uint = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _uint.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CUInt other && _uint.Equals(other._uint);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _uint.GetHashCode();

    /// <summary>将 <see cref="CUInt"/> 隐式转换为 <see cref="uint"/>。<br/>Implicitly converts a <see cref="CUInt"/> to a <see cref="uint"/>.</summary>
    public static implicit operator uint(CUInt c) => c._uint;

    /// <summary>将 <see cref="uint"/> 隐式转换为 <see cref="CUInt"/>。<br/>Implicitly converts a <see cref="uint"/> to a <see cref="CUInt"/>.</summary>
    public static implicit operator CUInt(uint value) => new CUInt(value);
}