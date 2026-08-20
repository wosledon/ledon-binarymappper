using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="uint"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="uint"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CUInt
{
    /// <summary>
    /// 获取或设置底层 <see cref="uint"/> 值。<br/>
    /// Gets or sets the underlying <see cref="uint"/> value.
    /// </summary>
    public uint Value;

    /// <summary>
    /// 初始化 <see cref="CUInt"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CUInt"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="uint"/> 值。<br/>The <see cref="uint"/> value to wrap.</param>
    public CUInt(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CUInt"/> 隐式转换为 <see cref="uint"/>。<br/>
    /// Implicitly converts a <see cref="CUInt"/> to a <see cref="uint"/>.
    /// </summary>
    /// <param name="cuint">要转换的 <see cref="CUInt"/> 实例。<br/>The <see cref="CUInt"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="uint"/> 值。<br/>The converted <see cref="uint"/> value.</returns>
    public static implicit operator uint(CUInt cuint) => cuint.Value;

    /// <summary>
    /// 将 <see cref="uint"/> 隐式转换为 <see cref="CUInt"/>。<br/>
    /// Implicitly converts a <see cref="uint"/> to a <see cref="CUInt"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="uint"/> 值。<br/>The <see cref="uint"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CUInt"/> 实例。<br/>The converted <see cref="CUInt"/> instance.</returns>
    public static implicit operator CUInt(uint value) => new CUInt(value);
}
