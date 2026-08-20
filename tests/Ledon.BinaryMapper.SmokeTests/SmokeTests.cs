using System;
using Ledon.BinaryMapper;

namespace Ledon.BinaryMapper.SmokeTests;

public static class SmokeTests
{
    public static void Run()
    {
        RunByteArrayAPI();
        RunSpanAPI();

        Console.WriteLine("smoke tests passed");
    }

    private static void RunByteArrayAPI()
    {
        var payload = new byte[]
        {
            0x00, 0x00, 0x00, 0x01, // big-endian int32 Id = 1
            0x42, 0x4F, 0x42, 0x00, // fixed string "BOB"
            0x41, 0xA0, 0x00, 0x00  // big-endian float = 20.0
        };

        var packet = BinaryMapper.Deserialize<FixedStringPacket>(payload);

        if (packet.Id != 1)
            throw new Exception($"Expected Id=1, got {packet.Id}");

        if (packet.Name != "BOB")
            throw new Exception($"Expected Name=BOB, got '{packet.Name}'");

        if (Math.Abs(packet.Temperature - 20.0f) > 1e-3f)
            throw new Exception($"Expected Temperature=20.0, got {packet.Temperature}");

        var settings = new BinaryMapperSettings();
        var bytes = BinaryMapper.Serialize(packet, settings);
        var roundTrip = BinaryMapper.Deserialize<FixedStringPacket>(bytes, settings);

        if (roundTrip.Id != packet.Id)
            throw new Exception("Round-trip Id mismatch");

        if (roundTrip.Name != packet.Name)
            throw new Exception("Round-trip Name mismatch");

        if (Math.Abs(roundTrip.Temperature - packet.Temperature) > 1e-3f)
            throw new Exception("Round-trip Temperature mismatch");
    }

    private static void RunSpanAPI()
    {
        var payload = new byte[]
        {
            0x00, 0x00, 0x00, 0x01,
            0x42, 0x4F, 0x42, 0x00,
            0x41, 0xA0, 0x00, 0x00
        };

        // Test ReadOnlySpan<byte> deserialization
        var packet = BinaryMapper.Deserialize<FixedStringPacket>((ReadOnlySpan<byte>)payload);

        if (packet.Id != 1)
            throw new Exception($"Span: Expected Id=1, got {packet.Id}");

        if (packet.Name != "BOB")
            throw new Exception($"Span: Expected Name=BOB, got '{packet.Name}'");

        if (Math.Abs(packet.Temperature - 20.0f) > 1e-3f)
            throw new Exception($"Span: Expected Temperature=20.0, got {packet.Temperature}");

        // Test round-trip via Span deserialization
        var settings = new BinaryMapperSettings();
        var bytes = BinaryMapper.Serialize(packet, settings);
        var roundTrip = BinaryMapper.Deserialize<FixedStringPacket>((ReadOnlySpan<byte>)bytes, settings);

        if (roundTrip.Id != packet.Id)
            throw new Exception("Span round-trip Id mismatch");

        if (roundTrip.Name != packet.Name)
            throw new Exception("Span round-trip Name mismatch");

        if (Math.Abs(roundTrip.Temperature - packet.Temperature) > 1e-3f)
            throw new Exception("Span round-trip Temperature mismatch");
    }

    public static void Main()
    {
        Run();
    }
}
