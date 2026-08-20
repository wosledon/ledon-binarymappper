using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="sbyte"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="sbyte"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CSByte
{
    /// <summary>
    /// 获取或设置底层 <see cref="sbyte"/> 值。<br/>
    /// Gets or sets the underlying <see cref="sbyte"/> value.
    /// </summary>
    public sbyte Value;

    /// <summary>
    /// 初始化 <see cref="CSByte"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CSByte"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="sbyte"/> 值。<br/>The <see cref="sbyte"/> value to wrap.</param>
    public CSByte(sbyte value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CSByte"/> 隐式转换为 <see cref="sbyte"/>。<br/>
    /// Implicitly converts a <see cref="CSByte"/> to a <see cref="sbyte"/>.
    /// </summary>
    /// <param name="csbyte">要转换的 <see cref="CSByte"/> 实例。<br/>The <see cref="CSByte"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="sbyte"/> 值。<br/>The converted <see cref="sbyte"/> value.</returns>
    public static implicit operator sbyte(CSByte csbyte) => csbyte.Value;

    /// <summary>
    /// 将 <see cref="sbyte"/> 隐式转换为 <see cref="CSByte"/>。<br/>
    /// Implicitly converts a <see cref="sbyte"/> to a <see cref="CSByte"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="sbyte"/> 值。<br/>The <see cref="sbyte"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CSByte"/> 实例。<br/>The converted <see cref="CSByte"/> instance.</returns>
    public static implicit operator CSByte(sbyte value) => new CSByte(value);
}
