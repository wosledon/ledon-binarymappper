using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="bool"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="bool"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CBool
{
    /// <summary>
    /// 获取或设置底层 <see cref="bool"/> 值。<br/>
    /// Gets or sets the underlying <see cref="bool"/> value.
    /// </summary>
    public bool Value;

    /// <summary>
    /// 初始化 <see cref="CBool"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CBool"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="bool"/> 值。<br/>The <see cref="bool"/> value to wrap.</param>
    public CBool(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CBool"/> 隐式转换为 <see cref="bool"/>。<br/>
    /// Implicitly converts a <see cref="CBool"/> to a <see cref="bool"/>.
    /// </summary>
    /// <param name="cbool">要转换的 <see cref="CBool"/> 实例。<br/>The <see cref="CBool"/> instance to convert.</param>
    /// <returns>转换后的 <see cref="bool"/> 值。<br/>The converted <see cref="bool"/> value.</returns>
    public static implicit operator bool(CBool cbool) => cbool.Value;

    /// <summary>
    /// 将 <see cref="bool"/> 隐式转换为 <see cref="CBool"/>。<br/>
    /// Implicitly converts a <see cref="bool"/> to a <see cref="CBool"/>.
    /// </summary>
    /// <param name="value">要转换的 <see cref="bool"/> 值。<br/>The <see cref="bool"/> value to convert.</param>
    /// <returns>转换后的 <see cref="CBool"/> 实例。<br/>The converted <see cref="CBool"/> instance.</returns>
    public static implicit operator CBool(bool value) => new CBool(value);
}
