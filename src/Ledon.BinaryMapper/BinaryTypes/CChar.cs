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
    /// <summary>
    /// 获取或设置底层 <see cref="char"/> 值。<br/>
    /// Gets or sets the underlying <see cref="char"/> value.
    /// </summary>
    public char Value;

    /// <summary>
    /// 初始化 <see cref="CChar"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CChar"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="char"/> 值。<br/>The <see cref="char"/> value to wrap.</param>
    public CChar(char value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CChar"/> 隐式转换为 <see cref="char"/>。<br/>
    /// Implicitly converts a <see cref="CChar"/> to a <see cref="char"/>.
    /// </summary>
    /// <param name="cchar">要转换的 <see cref="CChar"/> 实例。<br/>The <see cref="CChar"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="char"/> 值。<br/>The converted <see cref="char"/> value.</returns>
    public static implicit operator char(CChar cchar) => cchar.Value;

    /// <summary>
    /// 将 <see cref="char"/> 隐式转换为 <see cref="CChar"/>。<br/>
    /// Implicitly converts a <see cref="char"/> to a <see cref="CChar"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="char"/> 值。<br/>The <see cref="char"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CChar"/> 实例。<br/>The converted <see cref="CChar"/> instance.</returns>
    public static implicit operator CChar(char value) => new CChar(value);
}
