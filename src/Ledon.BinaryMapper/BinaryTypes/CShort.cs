using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="short"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="short"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CShort
{
    private short _short;

    /// <summary>
    /// 获取或设置底层 <see cref="short"/> 值。<br/>
    /// Gets or sets the underlying <see cref="short"/> value.
    /// </summary>
    public short Value
    {
        readonly get => _short;
        set => _short = value;
    }

    /// <summary>
    /// 初始化 <see cref="CShort"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CShort"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="short"/> 值。<br/>The <see cref="short"/> value to wrap.</param>
    public CShort(short value) => _short = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _short.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CShort other && _short.Equals(other._short);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _short.GetHashCode();

    /// <summary>将 <see cref="CShort"/> 隐式转换为 <see cref="short"/>。<br/>Implicitly converts a <see cref="CShort"/> to a <see cref="short"/>.</summary>
    public static implicit operator short(CShort c) => c._short;

    /// <summary>将 <see cref="short"/> 隐式转换为 <see cref="CShort"/>。<br/>Implicitly converts a <see cref="short"/> to a <see cref="CShort"/>.</summary>
    public static implicit operator CShort(short value) => new CShort(value);
}