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
    private Half _half;

    /// <summary>
    /// 获取或设置底层 <see cref="Half"/> 值。<br/>
    /// Gets or sets the underlying <see cref="Half"/> value.
    /// </summary>
    public Half Value
    {
        readonly get => _half;
        set => _half = value;
    }

    /// <summary>
    /// 初始化 <see cref="CHalf"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CHalf"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="Half"/> 值。<br/>The <see cref="Half"/> value to wrap.</param>
    public CHalf(Half value) => _half = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _half.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CHalf other && _half.Equals(other._half);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _half.GetHashCode();

    /// <summary>将 <see cref="CHalf"/> 隐式转换为 <see cref="Half"/>。<br/>Implicitly converts a <see cref="CHalf"/> to a <see cref="Half"/>.</summary>
    public static implicit operator Half(CHalf c) => c._half;

    /// <summary>将 <see cref="Half"/> 隐式转换为 <see cref="CHalf"/>。<br/>Implicitly converts a <see cref="Half"/> to a <see cref="CHalf"/>.</summary>
    public static implicit operator CHalf(Half value) => new CHalf(value);
}