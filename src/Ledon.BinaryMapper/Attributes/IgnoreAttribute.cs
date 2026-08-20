using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// 表示该字段或属性在二进制映射过程中应被忽略。<br/>
/// Indicates that the field or property should be ignored during binary mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
}
