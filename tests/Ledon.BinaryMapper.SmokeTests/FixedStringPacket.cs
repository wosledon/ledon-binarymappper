using Ledon.BinaryMapper.Attributes;
using Ledon.BinaryMapper.BinaryTypes;

namespace Ledon.BinaryMapper.SmokeTests;

public class FixedStringPacket
{
    public CInt Id { get; set; }

    [FixedLength(4)]
    public CString Name { get; set; }

    public CFloat Temperature { get; set; }
}
