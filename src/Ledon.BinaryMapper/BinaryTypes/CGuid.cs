using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射 <see cref="Guid"/> 的包装类型。<br/>
/// A wrapper type used to map <see cref="Guid"/> in binary protocols.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CGuid
{
    private Guid _guid;

    /// <summary>
    /// 获取或设置底层 <see cref="Guid"/> 值。<br/>
    /// Gets or sets the underlying <see cref="Guid"/> value.
    /// </summary>
    public Guid Value
    {
        readonly get => _guid;
        set => _guid = value;
    }

    /// <summary>
    /// 初始化 <see cref="CGuid"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CGuid"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="Guid"/> 值。<br/>The <see cref="Guid"/> value to wrap.</param>
    public CGuid(Guid value) => _guid = value;

    /// <summary>返回当前实例的字符串表示。<br/>Returns the string representation of the current instance.</summary>
    public override readonly string ToString() => _guid.ToString();

    /// <summary>判断当前实例是否等于指定对象。<br/>Determines whether the current instance equals the specified object.</summary>
    public override readonly bool Equals(object? obj) => obj is CGuid other && _guid.Equals(other._guid);

    /// <summary>获取当前实例的哈希码。<br/>Gets the hash code of the current instance.</summary>
    public override readonly int GetHashCode() => _guid.GetHashCode();

    /// <summary>将 <see cref="CGuid"/> 隐式转换为 <see cref="Guid"/>。<br/>Implicitly converts a <see cref="CGuid"/> to a <see cref="Guid"/>.</summary>
    public static implicit operator Guid(CGuid c) => c._guid;

    /// <summary>将 <see cref="Guid"/> 隐式转换为 <see cref="CGuid"/>。<br/>Implicitly converts a <see cref="Guid"/> to a <see cref="CGuid"/>.</summary>
    public static implicit operator CGuid(Guid value) => new CGuid(value);
}