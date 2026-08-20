using System;

namespace Ledon.BinaryMapper.Attributes;

/// <summary>
/// Marks a field or property as having a fixed binary length.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FixedLengthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FixedLengthAttribute"/> class.
    /// </summary>
    /// <param name="length">The fixed length expected in the binary protocol.</param>
    public FixedLengthAttribute(int length)
    {
        Length = length;
    }

    /// <summary>
    /// Gets the fixed length.
    /// </summary>
    public int Length { get; }
}
