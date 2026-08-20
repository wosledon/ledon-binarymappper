using Ledon.BinaryMapper.Attributes;

namespace Ledon.BinaryMapper.SmokeTests;

public class FixedStringPacket
{
    public int Id { get; set; }

    [FixedLength(4)]
    public string Name { get; set; } = string.Empty;

    public float Temperature { get; set; }
}
