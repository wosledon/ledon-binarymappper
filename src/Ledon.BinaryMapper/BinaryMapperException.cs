using System;

namespace Ledon.BinaryMapper;

/// <summary>
/// 表示二进制映射过程中发生错误。<br/>
/// Represents an error that occurs during binary mapping.
/// </summary>
public class BinaryMapperException : Exception
{
    /// <summary>
    /// 初始化 <see cref="BinaryMapperException"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="BinaryMapperException"/> class.
    /// </summary>
    public BinaryMapperException()
    {
    }

    /// <summary>
    /// 初始化 <see cref="BinaryMapperException"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="BinaryMapperException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">错误消息。<br/>The error message.</param>
    public BinaryMapperException(string message) : base(message)
    {
    }

    /// <summary>
    /// 初始化 <see cref="BinaryMapperException"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="BinaryMapperException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">错误消息。<br/>The error message.</param>
    /// <param name="innerException">内部异常。<br/>The inner exception.</param>
    public BinaryMapperException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
