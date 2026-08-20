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
    private bool _bool;

    /// <summary>
    /// 获取或设置底层 <see cref="bool"/> 值。<br/>
    /// Gets or sets the underlying <see cref="bool"/> value.
    /// </summary>
    public bool Value
    {
        readonly get => _bool;
        set => _bool = value;
    }

    /// <summary>
    /// 初始化 <see cref="CBool"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CBool"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="bool"/> 值。<br/>The <see cref="bool"/> value to wrap.</param>
    public CBool(bool value) => _bool = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _bool.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CBool other && _bool.Equals(other._bool);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _bool.GetHashCode();

    /// <summary>将 <see cref="CBool"/> 隐式转换为 <see cref="bool"/>。<br/>Implicitly converts a <see cref="CBool"/> to a <see cref="bool"/>.</summary>
    public static implicit operator bool(CBool c) => c._bool;

    /// <summary>将 <see cref="bool"/> 隐式转换为 <see cref="CBool"/>。<br/>Implicitly converts a <see cref="bool"/> to a <see cref="CBool"/>.</summary>
    public static implicit operator CBool(bool value) => new CBool(value);
}