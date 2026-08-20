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
    /// <summary>
    /// 获取或设置底层 <see cref="int"/> 值。<br/>
    /// Gets or sets the underlying <see cref="int"/> value.
    /// </summary>
    public int Value;

    /// <summary>
    /// 初始化 <see cref="CInt"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CInt"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="int"/> 值。<br/>The <see cref="int"/> value to wrap.</param>
    public CInt(int value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CInt"/> 隐式转换为 <see cref="int"/>。<br/>
    /// Implicitly converts a <see cref="CInt"/> to a <see cref="int"/>.
    /// </summary>
    /// <param name="cint">要转换的 <see cref="CInt"/> 实例。<br/>The <see cref="CInt"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="int"/> 值。<br/>The converted <see cref="int"/> value.</returns>
    public static implicit operator int(CInt cint) => cint.Value;

    /// <summary>
    /// 将 <see cref="int"/> 隐式转换为 <see cref="CInt"/>。<br/>
    /// Implicitly converts a <see cref="int"/> to a <see cref="CInt"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="int"/> 值。<br/>The <see cref="int"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CInt"/> 实例。<br/>The converted <see cref="CInt"/> instance.</returns>
    public static implicit operator CInt(int value) => new CInt(value);
}
