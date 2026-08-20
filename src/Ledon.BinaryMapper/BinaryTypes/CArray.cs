using System;
using System.Runtime.InteropServices;

namespace Ledon.BinaryMapper.BinaryTypes;

/// <summary>
/// 二进制协议中用于映射固定长度数组的包装类型。<br/>
/// A wrapper type used to map fixed-length arrays in binary protocols.
/// </summary>
/// <typeparam name="T">元素类型，必须是值类型。<br/>The element type, must be a value type.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public struct CArray<T> where T : struct
{
    private T[] _value;

    /// <summary>
    /// 获取或设置底层数组。<br/>
    /// Gets or sets the underlying array.
    /// </summary>
    public T[] Value
    {
        readonly get => _value;
        set => _value = value;
    }

    /// <summary>
    /// 初始化指定长度的 <see cref="CArray{T}"/>。<br/>
    /// Initializes a <see cref="CArray{T}"/> with the specified length.
    /// </summary>
    /// <param name="length">数组长度。<br/>The array length.</param>
    public CArray(int length) => _value = new T[length];

    /// <summary>
    /// 包装现有数组。<br/>
    /// Wraps an existing array.
    /// </summary>
    /// <param name="value">要包装的数组。<br/>The array to wrap.</param>
    public CArray(T[] value) => _value = value;

    /// <summary>获取数组长度。<br/>Gets the array length.</summary>
    public readonly int Length => _value?.Length ?? 0;

    /// <summary>获取或设置指定索引处的元素。<br/>Gets or sets the element at the specified index.</summary>
    public readonly T this[int index]
    {
        get => _value[index];
        set => _value[index] = value;
    }

    /// <summary>将 <see cref="CArray{T}"/> 隐式转换为 <see cref="T"/>[]。</summary>
    public static implicit operator T[](CArray<T> arr) => arr._value;

    /// <summary>将 <see cref="T"/>[] 隐式转换为 <see cref="CArray{T}"/>。</summary>
    public static implicit operator CArray<T>(T[] value) => new CArray<T>(value);

    /// <summary>返回当前实例的字符串表示。</summary>
    public override readonly string ToString() => $"CArray<{typeof(T).Name}>[{Length}]";
}