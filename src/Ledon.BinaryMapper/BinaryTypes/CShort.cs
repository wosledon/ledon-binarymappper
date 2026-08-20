using System;
using System.Runtime.InteropServices;
namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="short"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="short"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CShort
{
    /// <summary>
    /// 获取或设置底层 <see cref="short"/> 值。<br/>
    /// Gets or sets the underlying <see cref="short"/> value.
    /// </summary>
    public short Value;

    /// <summary>
    /// 初始化 <see cref="CShort"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CShort"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="short"/> 值。<br/>The <see cref="short"/> value to wrap.</param>
    public CShort(short value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CShort"/> 隐式转换为 <see cref="short"/>。<br/>
    /// Implicitly converts a <see cref="CShort"/> to a <see cref="short"/>.
    /// </summary>
    /// <param name="cshort">要转换的 <see cref="CShort"/> 实例。<br/>The <see cref="CShort"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="short"/> 值。<br/>The converted <see cref="short"/> value.</returns>
    public static implicit operator short(CShort cshort) => cshort.Value;

    /// <summary>
    /// 将 <see cref="short"/> 隐式转换为 <see cref="CShort"/>。<br/>
    /// Implicitly converts a <see cref="short"/> to a <see cref="CShort"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="short"/> 值。<br/>The <see cref="short"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CShort"/> 实例。<br/>The converted <see cref="CShort"/> instance.</returns>
    public static implicit operator CShort(short value) => new CShort(value);
}
