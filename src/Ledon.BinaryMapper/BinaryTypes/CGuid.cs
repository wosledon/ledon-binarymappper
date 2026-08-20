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
    /// <summary>
    /// 获取或设置底层 <see cref="Guid"/> 值。<br/>
    /// Gets or sets the underlying <see cref="Guid"/> value.
    /// </summary>
    public Guid Value;

    /// <summary>
    /// 初始化 <see cref="CGuid"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="CGuid"/> struct.
    /// </summary>
    /// <param name="value">要包装的 <see cref="Guid"/> 值。<br/>The <see cref="Guid"/> value to wrap.</param>
    public CGuid(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// 将 <see cref="CGuid"/> 隐式转换为 <see cref="Guid"/>。<br/>
    /// Implicitly converts a <see cref="CGuid"/> to a <see cref="Guid"/>.
    /// </summary>
    public static implicit operator Guid(CGuid cg) => cg.Value;

    /// <summary>
    /// 将 <see cref="Guid"/> 隐式转换为 <see cref="CGuid"/>。<br/>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="CGuid"/>.
    /// </summary>
    public static implicit operator CGuid(Guid value) => new CGuid(value);
}