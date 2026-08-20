using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="ulong"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="ulong"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CULong
{
    /// <summary>
    /// 获取或设置底层 <see cref="ulong"/> 值。<br/>
    /// Gets or sets the underlying <see cref="ulong"/> value.
    /// </summary>
    public ulong Value;

    /// <summary>
    /// 初始化 <see cref="CULong"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CULong"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="ulong"/> 值。<br/>The <see cref="ulong"/> value to wrap.</param>
    public CULong(ulong value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CULong"/> 隐式转换为 <see cref="ulong"/>。<br/>
    /// Implicitly converts a <see cref="CULong"/> to a <see cref="ulong"/>.
    /// </summary>
    /// <param name="culong">要转换的 <see cref="CULong"/> 实例。<br/>The <see cref="CULong"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="ulong"/> 值。<br/>The converted <see cref="ulong"/> value.</returns>
    public static implicit operator ulong(CULong culong) => culong.Value;

    /// <summary>
    /// 将 <see cref="ulong"/> 隐式转换为 <see cref="CULong"/>。<br/>
    /// Implicitly converts a <see cref="ulong"/> to a <see cref="CULong"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="ulong"/> 值。<br/>The <see cref="ulong"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CULong"/> 实例。<br/>The converted <see cref="CULong"/> instance.</returns>
    public static implicit operator CULong(ulong value) => new CULong(value);
}
