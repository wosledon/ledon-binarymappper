using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="ushort"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="ushort"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CUShort
{
    private ushort _ushort;

    /// <summary>
    /// 获取或设置底层 <see cref="ushort"/> 值。<br/>
    /// Gets or sets the underlying <see cref="ushort"/> value.
    /// </summary>
    public ushort Value
    {
        readonly get => _ushort;
        set => _ushort = value;
    }

    /// <summary>
    /// 初始化 <see cref="CUShort"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CUShort"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="ushort"/> 值。<br/>The <see cref="ushort"/> value to wrap.</param>
    public CUShort(ushort value) => _ushort = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _ushort.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CUShort other && _ushort.Equals(other._ushort);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _ushort.GetHashCode();

    /// <summary>将 <see cref="CUShort"/> 隐式转换为 <see cref="ushort"/>。<br/>Implicitly converts a <see cref="CUShort"/> to a <see cref="ushort"/>.</summary>
    public static implicit operator ushort(CUShort c) => c._ushort;

    /// <summary>将 <see cref="ushort"/> 隐式转换为 <see cref="CUShort"/>。<br/>Implicitly converts a <see cref="ushort"/> to a <see cref="CUShort"/>.</summary>
    public static implicit operator CUShort(ushort value) => new CUShort(value);
}