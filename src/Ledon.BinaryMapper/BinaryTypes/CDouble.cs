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
    private double _double;

    /// <summary>
    /// 获取或设置底层 <see cref="double"/> 值。<br/>
    /// Gets or sets the underlying <see cref="double"/> value.
    /// </summary>
    public double Value
    {
        readonly get => _double;
        set => _double = value;
    }

    /// <summary>
    /// 初始化 <see cref="CDouble"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CDouble"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="double"/> 值。<br/>The <see cref="double"/> value to wrap.</param>
    public CDouble(double value) => _double = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _double.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CDouble other && _double.Equals(other._double);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _double.GetHashCode();

    /// <summary>将 <see cref="CDouble"/> 隐式转换为 <see cref="double"/>。<br/>Implicitly converts a <see cref="CDouble"/> to a <see cref="double"/>.</summary>
    public static implicit operator double(CDouble c) => c._double;

    /// <summary>将 <see cref="double"/> 隐式转换为 <see cref="CDouble"/>。<br/>Implicitly converts a <see cref="double"/> to a <see cref="CDouble"/>.</summary>
    public static implicit operator CDouble(double value) => new CDouble(value);
}