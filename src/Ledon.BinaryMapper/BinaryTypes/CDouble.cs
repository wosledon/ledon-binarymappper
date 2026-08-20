using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="double"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="double"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CDouble
{
    /// <summary>
    /// 获取或设置底层 <see cref="double"/> 值。<br/>
    /// Gets or sets the underlying <see cref="double"/> value.
    /// </summary>
    public double Value;

    /// <summary>
    /// 初始化 <see cref="CDouble"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CDouble"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="double"/> 值。<br/>The <see cref="double"/> value to wrap.</param>
    public CDouble(double value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CDouble"/> 隐式转换为 <see cref="double"/>。<br/>
    /// Implicitly converts a <see cref="CDouble"/> to a <see cref="double"/>.
    /// </summary>
    /// <param name="cdouble">要转换的 <see cref="CDouble"/> 实例。<br/>The <see cref="CDouble"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="double"/> 值。<br/>The converted <see cref="double"/> value.</returns>
    public static implicit operator double(CDouble cdouble) => cdouble.Value;

    /// <summary>
    /// 将 <see cref="double"/> 隐式转换为 <see cref="CDouble"/>。<br/>
    /// Implicitly converts a <see cref="double"/> to a <see cref="CDouble"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="double"/> 值。<br/>The <see cref="double"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CDouble"/> 实例。<br/>The converted <see cref="CDouble"/> instance.</returns>
    public static implicit operator CDouble(double value) => new CDouble(value);
}
