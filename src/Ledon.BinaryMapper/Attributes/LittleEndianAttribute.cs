using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// 表示字段或属性按小端字节序读取或写入。<br/>
/// Indicates that a field or property should be read or written in little-endian byte order.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LittleEndianAttribute : Attribute
{
}
