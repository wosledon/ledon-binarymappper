using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="string"/> 的包装类型；具体长度、编码和终止方式由映射器决定。<br/>
/// A wrapper type used to map <see cref="string"/> in binary protocols; length, encoding, and termination are defined by the mapper.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CString
{
    /// <summary>
    /// 获取或设置底层 <see cref="string"/> 值。<br/>
    /// Gets or sets the underlying <see cref="string"/> value.
    /// </summary>
    public string Value;

    /// <summary>
    /// 初始化 <see cref="CString"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CString"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="string"/> 值。<br/>The <see cref="string"/> value to wrap.</param>
    public CString(string value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CString"/> 隐式转换为 <see cref="string"/>。<br/>
    /// Implicitly converts a <see cref="CString"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="cstring">要转换的 <see cref="CString"/> 实例。<br/>The <see cref="CString"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="string"/> 值。<br/>The converted <see cref="string"/> value.</returns>
    public static implicit operator string(CString cstring) => cstring.Value;

    /// <summary>
    /// 将 <see cref="string"/> 隐式转换为 <see cref="CString"/>。<br/>
    /// Implicitly converts a <see cref="string"/> to a <see cref="CString"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="string"/> 值。<br/>The <see cref="string"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CString"/> 实例。<br/>The converted <see cref="CString"/> instance.</returns>
    public static implicit operator CString(string value) => new CString(value);
}
