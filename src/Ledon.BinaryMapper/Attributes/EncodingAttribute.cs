using System.Text;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// 指定字段或属性在二进制协议中使用的编码名称。<br/>
/// Specifies the encoding name to use for a field or property in the binary protocol.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EncodingAttribute : Attribute
{
    /// <summary>
    /// 初始化 <see cref="EncodingAttribute"/> 的新实例。<br/>
    /// Initializes a new instance of the <see cref="EncodingAttribute"/> class.
    /// </summary>
    /// <param name="name">编码名称，例如 "utf-8"、"ascii"、"unicode"。<br/>The encoding name, for example "utf-8", "ascii", or "unicode".</param>
    public EncodingAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 获取编码名称。<br/>
    /// Gets the encoding name.
    /// </summary>
    public string Name { get; }
}
