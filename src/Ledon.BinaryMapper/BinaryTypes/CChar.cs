using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="char"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="char"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CChar
{
    private char _char;

    /// <summary>
    /// 获取或设置底层 <see cref="char"/> 值。<br/>
    /// Gets or sets the underlying <see cref="char"/> value.
    /// </summary>
    public char Value
    {
        readonly get => _char;
        set => _char = value;
    }

    /// <summary>
    /// 初始化 <see cref="CChar"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CChar"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="char"/> 值。<br/>The <see cref="char"/> value to wrap.</param>
    public CChar(char value) => _char = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _char.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CChar other && _char.Equals(other._char);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _char.GetHashCode();

    /// <summary>将 <see cref="CChar"/> 隐式转换为 <see cref="char"/>。<br/>Implicitly converts a <see cref="CChar"/> to a <see cref="char"/>.</summary>
    public static implicit operator char(CChar c) => c._char;

    /// <summary>将 <see cref="char"/> 隐式转换为 <see cref="CChar"/>。<br/>Implicitly converts a <see cref="char"/> to a <see cref="CChar"/>.</summary>
    public static implicit operator CChar(char value) => new CChar(value);
}