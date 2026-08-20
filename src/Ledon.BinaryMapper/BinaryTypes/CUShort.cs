using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="ushort"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="ushort"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CUShort
{
    /// <summary>
    /// 获取或设置底层 <see cref="ushort"/> 值。<br/>
    /// Gets or sets the underlying <see cref="ushort"/> value.
    /// </summary>
    public ushort Value;

    /// <summary>
    /// 初始化 <see cref="CUShort"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CUShort"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="ushort"/> 值。<br/>The <see cref="ushort"/> value to wrap.</param>
    public CUShort(ushort value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CUShort"/> 隐式转换为 <see cref="ushort"/>。<br/>
    /// Implicitly converts a <see cref="CUShort"/> to a <see cref="ushort"/>.
    /// </summary>
    /// <param name="cushort">要转换的 <see cref="CUShort"/> 实例。<br/>The <see cref="CUShort"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="ushort"/> 值。<br/>The converted <see cref="ushort"/> value.</returns>
    public static implicit operator ushort(CUShort cushort) => cushort.Value;

    /// <summary>
    /// 将 <see cref="ushort"/> 隐式转换为 <see cref="CUShort"/>。<br/>
    /// Implicitly converts a <see cref="ushort"/> to a <see cref="CUShort"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="ushort"/> 值。<br/>The <see cref="ushort"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CUShort"/> 实例。<br/>The converted <see cref="CUShort"/> instance.</returns>
    public static implicit operator CUShort(ushort value) => new CUShort(value);
}
