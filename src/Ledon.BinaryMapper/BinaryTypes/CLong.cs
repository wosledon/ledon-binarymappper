using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="long"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="long"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CLong
{
    /// <summary>
    /// 获取或设置底层 <see cref="long"/> 值。<br/>
    /// Gets or sets the underlying <see cref="long"/> value.
    /// </summary>
    public long Value;

    /// <summary>
    /// 初始化 <see cref="CLong"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CLong"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="long"/> 值。<br/>The <see cref="long"/> value to wrap.</param>
    public CLong(long value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CLong"/> 隐式转换为 <see cref="long"/>。<br/>
    /// Implicitly converts a <see cref="CLong"/> to a <see cref="long"/>.
    /// </summary>
    /// <param name="clong">要转换的 <see cref="CLong"/> 实例。<br/>The <see cref="CLong"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="long"/> 值。<br/>The converted <see cref="long"/> value.</returns>
    public static implicit operator long(CLong clong) => clong.Value;

    /// <summary>
    /// 将 <see cref="long"/> 隐式转换为 <see cref="CLong"/>。<br/>
    /// Implicitly converts a <see cref="long"/> to a <see cref="CLong"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="long"/> 值。<br/>The <see cref="long"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CLong"/> 实例。<br/>The converted <see cref="CLong"/> instance.</returns>
    public static implicit operator CLong(long value) => new CLong(value);
}
