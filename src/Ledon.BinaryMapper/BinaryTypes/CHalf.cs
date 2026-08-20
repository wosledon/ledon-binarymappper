using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="Half"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="Half"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CHalf
{
    /// <summary>
    /// 获取或设置底层 <see cref="Half"/> 值。<br/>
    /// Gets or sets the underlying <see cref="Half"/> value.
    /// </summary>
    public Half Value;

    /// <summary>
    /// 初始化 <see cref="CHalf"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CHalf"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="Half"/> 值。<br/>The <see cref="Half"/> value to wrap.</param>
    public CHalf(Half value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CHalf"/> 隐式转换为 <see cref="Half"/>。<br/>
    /// Implicitly converts a <see cref="CHalf"/> to a <see cref="Half"/>.
    /// </summary>
    public static implicit operator Half(CHalf ch) => ch.Value;

    /// <summary>
    /// 将 <see cref="Half"/> 隐式转换为 <see cref="CHalf"/>。<br/>
    /// Implicitly converts a <see cref="Half"/> to a <see cref="CHalf"/>.
    /// </summary>
    public static implicit operator CHalf(Half value) => new CHalf(value);
}