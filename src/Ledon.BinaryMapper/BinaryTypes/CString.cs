using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="string"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="string"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CString
{
    private string _string;

    /// <summary>
    /// 获取或设置底层 <see cref="string"/> 值。<br/>
    /// Gets or sets the underlying <see cref="string"/> value.
    /// </summary>
    public string Value
    {
        readonly get => _string;
        set => _string = value;
    }

    /// <summary>
    /// 初始化 <see cref="CString"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CString"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="string"/> 值。<br/>The <see cref="string"/> value to wrap.</param>
    public CString(string value) => _string = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _string.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CString other && _string.Equals(other._string);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _string.GetHashCode();

    /// <summary>将 <see cref="CString"/> 隐式转换为 <see cref="string"/>。<br/>Implicitly converts a <see cref="CString"/> to a <see cref="string"/>.</summary>
    public static implicit operator string(CString c) => c._string;

    /// <summary>将 <see cref="string"/> 隐式转换为 <see cref="CString"/>。<br/>Implicitly converts a <see cref="string"/> to a <see cref="CString"/>.</summary>
    public static implicit operator CString(string value) => new CString(value);
}