using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="byte"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="byte"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CByte
{
    /// <summary>
    /// 获取或设置底层 <see cref="byte"/> 值。<br/>
    /// Gets or sets the underlying <see cref="byte"/> value.
    /// </summary>
    public byte Value;

    /// <summary>
    /// 初始化 <see cref="CByte"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CByte"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="byte"/> 值。<br/>The <see cref="byte"/> value to wrap.</param>
    public CByte(byte value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CByte"/> 隐式转换为 <see cref="byte"/>。<br/>
    /// Implicitly converts a <see cref="CByte"/> to a <see cref="byte"/>.
    /// </summary>
    /// <param name="cbyte">要转换的 <see cref="CByte"/> 实例。<br/>The <see cref="CByte"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="byte"/> 值。<br/>The converted <see cref="byte"/> value.</returns>
    public static implicit operator byte(CByte cbyte) => cbyte.Value;

    /// <summary>
    /// 将 <see cref="byte"/> 隐式转换为 <see cref="CByte"/>。<br/>
    /// Implicitly converts a <see cref="byte"/> to a <see cref="CByte"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="byte"/> 值。<br/>The <see cref="byte"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CByte"/> 实例。<br/>The converted <see cref="CByte"/> instance.</returns>
    public static implicit operator CByte(byte value) => new CByte(value);
}
