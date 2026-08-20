using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="int"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="int"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CInt
{
    private int _int;

    /// <summary>
    /// 获取或设置底层 <see cref="int"/> 值。<br/>
    /// Gets or sets the underlying <see cref="int"/> value.
    /// </summary>
    public int Value
    {
        readonly get => _int;
        set => _int = value;
    }

    /// <summary>
    /// 初始化 <see cref="CInt"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CInt"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="int"/> 值。<br/>The <see cref="int"/> value to wrap.</param>
    public CInt(int value) => _int = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _int.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CInt other && _int.Equals(other._int);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _int.GetHashCode();

    /// <summary>将 <see cref="CInt"/> 隐式转换为 <see cref="int"/>。<br/>Implicitly converts a <see cref="CInt"/> to a <see cref="int"/>.</summary>
    public static implicit operator int(CInt c) => c._int;

    /// <summary>将 <see cref="int"/> 隐式转换为 <see cref="CInt"/>。<br/>Implicitly converts a <see cref="int"/> to a <see cref="CInt"/>.</summary>
    public static implicit operator CInt(int value) => new CInt(value);
}