<div align="center">

[**中文版**](README.zh-CN.md) · **English**

# Ledon.BinaryMapper

A high-performance .NET binary serialization library with a Newtonsoft.Json-style API — map C# objects to and from binary protocol data with attributes.

![.NET 10](https://img.shields.io/badge/.NET-10.0-blue)
![Tests](https://img.shields.io/badge/tests-22%20passed-brightgreen)
![License](https://img.shields.io/badge/license-MIT-green)

</div>

## Features

- **Newtonsoft-style API**: `BinaryMapper.Serialize()` / `BinaryMapper.Deserialize<T>()`
- **Zero-copy deserialization**: `ReadOnlySpan<byte>` overloads
- **Full primitive support**: `int`, `short`, `long`, `float`, `double`, `byte`, `bool`, `char`, `Half`, `Guid`, enums, and more
- **String modes**: fixed-length, null-terminated, custom encoding
- **Collections**: arrays (`T[]`) and `IList<T>` (with `[FixedLength]`)
- **Nested objects**: recursive serialization of class/struct members
- **Bit fields**: pack multiple fields into a single byte with `[BitField(n)]`
- **Endianness**: per-member attributes or global settings
- **Performance**: type caching, compiled property accessors, ArrayPool buffers

## Quick Start

```csharp
using Ledon.BinaryMapper;
using Ledon.BinaryMapper.Attributes;

// Define your protocol packet
public class FixedStringPacket
{
    public int Id { get; set; }
    [FixedLength(4)] public string Name { get; set; } = "";
    public float Temperature { get; set; }
}

// Serialize
var packet = new FixedStringPacket { Id = 1, Name = "BOB", Temperature = 20.0f };
byte[] data = BinaryMapper.Serialize(packet);

// Deserialize
var restored = BinaryMapper.Deserialize<FixedStringPacket>(data);
Console.WriteLine(restored.Name); // "BOB"
```

## Installation

```
dotnet add package Ledon.BinaryMapper   <!-- pending publish -->
```

Or add a project reference to `src/Ledon.BinaryMapper/Ledon.BinaryMapper.csproj`.

## API

### Serialize

```csharp
byte[] Serialize(object obj)
byte[] Serialize(object obj, BinaryMapperSettings? settings)
```

### Deserialize

```csharp
T Deserialize<T>(byte[] data)
T Deserialize<T>(byte[] data, BinaryMapperSettings? settings)
object Deserialize(byte[] data, Type type)
object Deserialize(byte[] data, Type type, BinaryMapperSettings? settings)

// ReadOnlySpan<byte> overloads — zero-copy, high performance
T Deserialize<T>(ReadOnlySpan<byte> data)
object Deserialize(ReadOnlySpan<byte> data, Type type, BinaryMapperSettings? settings)
```

### Settings

```csharp
var settings = new BinaryMapperSettings
{
    Endianness = Endianness.BigEndian, // default
    Encoding = Encoding.UTF8,          // default
};

var data = BinaryMapper.Serialize(obj, settings);
```

## Attributes

| Attribute | Description |
|-----------|-------------|
| `[FixedLength(n)]` | Fixed size for strings/arrays/lists |
| `[NullTerminated]` | Null-terminated string |
| `[Encoding("ascii")]` | String encoding |
| `[BigEndian]` / `[LittleEndian]` | Member-level endianness |
| `[Ignore]` | Skip the field during mapping |
| `[BitField(n)]` | Map to `n` bits in a packed byte |

### Examples

**Strings**

```csharp
public class StringPacket
{
    [FixedLength(8)] public string Fixed { get; set; } = "";       // 8 bytes
    [NullTerminated] public string CString { get; set; } = "";     // \0 terminated
    [FixedLength(4)] [Encoding("ascii")] public string Code { get; set; } = "";
}
```

**Arrays & collections**

```csharp
public class ArrayPacket
{
    [FixedLength(3)] public int[] Values { get; set; } = [];
    [FixedLength(2)] public IList<float> Points { get; set; } = [];
}
```

**Bit fields** — consecutive `[BitField]` members are packed into the same byte:

```csharp
public class FlagsPacket
{
    [BitField(1)] public bool FlagA { get; set; } // bit 0
    [BitField(3)] public byte Value { get; set; } // bits 1-3
    [BitField(1)] public bool FlagB { get; set; } // bit 4
    public byte Tail { get; set; }                // next byte
}
```

**Nested objects**

```csharp
public class Inner { public int X { get; set; } }
public class Outer { public Inner Inner { get; set; } = new(); public int Y { get; set; } }
```

## Performance

> Benchmarked with BenchmarkDotNet on an 11-field packet, .NET 10, Release build.

| Operation | Time | Allocated |
|-----------|------|-----------|
| `Serialize` | ~97 ns | ~400 B |
| `Deserialize` (byte[]) | ~140 ns | ~360 B |
| `Deserialize` (ReadOnlySpan) | ~139 ns | ~360 B |

Optimizations include:
- Cached member metadata (`ConcurrentDictionary<Type, MappableMember[]>`)
- Pre-resolved attributes and endianness in `MappableMember`
- Expression-tree compiled property getters/setters (no reflection on hot path)
- Compiled constructor factories
- `ArrayPool<byte>` writer buffers
- `ReadOnlySpan<byte>` readers with no intermediate allocation

## Supported Types

| Category | Types |
|----------|-------|
| Integers | `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong` |
| Floating point | `Half`, `float`, `double` |
| Other primitives | `bool` (1 byte), `char` (2 bytes), `Guid` (16 bytes) |
| Strings | `string` with `[FixedLength]` / `[NullTerminated]` / `[Encoding]` |
| Enums | any enum, sized by its underlying type |
| Arrays | `T[]` with `[FixedLength(n)]` |
| Collections | types implementing `IList` / `IList<T>` with `[FixedLength(n)]` |
| Objects | nested class/struct members |

## Projects

| Project | Description |
|---------|-------------|
| `src/Ledon.BinaryMapper` | Core library |
| `tests/Ledon.BinaryMapper.Tests` | xUnit test suite (22 tests) |
| `tests/Ledon.BinaryMapper.SmokeTests` | Console smoke tests |
| `tests/Ledon.BinaryMapper.Benchmarks` | BenchmarkDotNet benchmarks |

## License

MIT