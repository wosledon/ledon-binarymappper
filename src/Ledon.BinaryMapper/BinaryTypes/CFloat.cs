using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="float"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="float"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CFloat
{
    /// <summary>
    /// 获取或设置底层 <see cref="float"/> 值。<br/>
    /// Gets or sets the underlying <see cref="float"/> value.
    /// </summary>
    public float Value;

    /// <summary>
    /// 初始化 <see cref="CFloat"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CFloat"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="float"/> 值。<br/>The <see cref="float"/> value to wrap.</param>
    public CFloat(float value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CFloat"/> 隐式转换为 <see cref="float"/>。<br/>
    /// Implicitly converts a <see cref="CFloat"/> to a <see cref="float"/>.
    /// </summary>
    /// <param name="cfloat">要转换的 <see cref="CFloat"/> 实例。<br/>The <see cref="CFloat"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="float"/> 值。<br/>The converted <see cref="float"/> value.</returns>
    public static implicit operator float(CFloat cfloat) => cfloat.Value;

    /// <summary>
    /// 将 <see cref="float"/> 隐式转换为 <see cref="CFloat"/>。<br/>
    /// Implicitly converts a <see cref="float"/> to a <see cref="CFloat"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="float"/> 值。<br/>The <see cref="float"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CFloat"/> 实例。<br/>The converted <see cref="CFloat"/> instance.</returns>
    public static implicit operator CFloat(float value) => new CFloat(value);
}
