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
    private float _float;

    /// <summary>
    /// 获取或设置底层 <see cref="float"/> 值。<br/>
    /// Gets or sets the underlying <see cref="float"/> value.
    /// </summary>
    public float Value
    {
        readonly get => _float;
        set => _float = value;
    }

    /// <summary>
    /// 初始化 <see cref="CFloat"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CFloat"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="float"/> 值。<br/>The <see cref="float"/> value to wrap.</param>
    public CFloat(float value) => _float = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _float.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CFloat other && _float.Equals(other._float);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _float.GetHashCode();

    /// <summary>将 <see cref="CFloat"/> 隐式转换为 <see cref="float"/>。<br/>Implicitly converts a <see cref="CFloat"/> to a <see cref="float"/>.</summary>
    public static implicit operator float(CFloat c) => c._float;

    /// <summary>将 <see cref="float"/> 隐式转换为 <see cref="CFloat"/>。<br/>Implicitly converts a <see cref="float"/> to a <see cref="CFloat"/>.</summary>
    public static implicit operator CFloat(float value) => new CFloat(value);
}