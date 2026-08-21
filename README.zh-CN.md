<div align="center">

**中文版** · [**English**](README.md)

# Ledon.BinaryMapper

高性能 .NET 二进制序列化库，Newtonsoft.Json 风格 API —— 通过特性将 C# 对象映射为二进制协议数据，或从二进制数据还原对象。

![.NET 10](https://img.shields.io/badge/.NET-10.0-blue)
![Tests](https://img.shields.io/badge/tests-22%20passed-brightgreen)
![License](https://img.shields.io/badge/license-MIT-green)

</div>

## 特性

- **Newtonsoft 风格 API**：`BinaryMapper.Serialize()` / `BinaryMapper.Deserialize<T>()`
- **零拷贝反序列化**：`ReadOnlySpan<byte>` 重载
- **完整原始类型支持**：`int`、`short`、`long`、`float`、`double`、`byte`、`bool`、`char`、`Half`、`Guid`、枚举等
- **字符串模式**：定长、null 结尾、自定义编码
- **集合支持**：数组（`T[]`）与 `IList<T>`（配合 `[FixedLength]`）
- **嵌套对象**：递归序列化类/结构体成员
- **位域**：`[BitField(n)]` 将多个字段打包进单个字节
- **字节序**：成员级特性或全局设置
- **性能优化**：类型缓存、编译属性访问器、ArrayPool 缓冲

## 快速开始

```csharp
using Ledon.BinaryMapper;
using Ledon.BinaryMapper.Attributes;

// 定义协议包
public class FixedStringPacket
{
    public int Id { get; set; }
    [FixedLength(4)] public string Name { get; set; } = "";
    public float Temperature { get; set; }
}

// 序列化
var packet = new FixedStringPacket { Id = 1, Name = "BOB", Temperature = 20.0f };
byte[] data = BinaryMapper.Serialize(packet);

// 反序列化
var restored = BinaryMapper.Deserialize<FixedStringPacket>(data);
Console.WriteLine(restored.Name); // "BOB"
```

## 安装

```
dotnet add package Ledon.BinaryMapper   <!-- 待发布 -->
```

或直接引用 `src/Ledon.BinaryMapper/Ledon.BinaryMapper.csproj` 项目。

## API

### 序列化

```csharp
byte[] Serialize(object obj)
byte[] Serialize(object obj, BinaryMapperSettings? settings)
```

### 反序列化

```csharp
T Deserialize<T>(byte[] data)
T Deserialize<T>(byte[] data, BinaryMapperSettings? settings)
object Deserialize(byte[] data, Type type)
object Deserialize(byte[] data, Type type, BinaryMapperSettings? settings)

// ReadOnlySpan<byte> 重载 —— 零拷贝、高性能
T Deserialize<T>(ReadOnlySpan<byte> data)
object Deserialize(ReadOnlySpan<byte> data, Type type, BinaryMapperSettings? settings)
```

### 设置

```csharp
var settings = new BinaryMapperSettings
{
    Endianness = Endianness.BigEndian, // 默认
    Encoding = Encoding.UTF8,          // 默认
};

var data = BinaryMapper.Serialize(obj, settings);
```

## 特性

| 特性 | 说明 |
|------|------|
| `[FixedLength(n)]` | 字符串/数组/集合的固定长度 |
| `[NullTerminated]` | null 结尾字符串 |
| `[Encoding("ascii")]` | 字符串编码 |
| `[BigEndian]` / `[LittleEndian]` | 成员级字节序 |
| `[Ignore]` | 映射时跳过该字段 |
| `[BitField(n)]` | 打包到字节中的 n 位 |

### 示例

**字符串**

```csharp
public class StringPacket
{
    [FixedLength(8)] public string Fixed { get; set; } = "";       // 8 字节
    [NullTerminated] public string CString { get; set; } = "";     // \0 结尾
    [FixedLength(4)] [Encoding("ascii")] public string Code { get; set; } = "";
}
```

**数组与集合**

```csharp
public class ArrayPacket
{
    [FixedLength(3)] public int[] Values { get; set; } = [];
    [FixedLength(2)] public IList<float> Points { get; set; } = [];
}
```

**位域** —— 连续 `[BitField]` 成员自动打包到同一字节：

```csharp
public class FlagsPacket
{
    [BitField(1)] public bool FlagA { get; set; } // bit 0
    [BitField(3)] public byte Value { get; set; } // bits 1-3
    [BitField(1)] public bool FlagB { get; set; } // bit 4
    public byte Tail { get; set; }                // 下一个字节
}
```

**嵌套对象**

```csharp
public class Inner { public int X { get; set; } }
public class Outer { public Inner Inner { get; set; } = new(); public int Y { get; set; } }
```

## 性能

> 使用 BenchmarkDotNet 测量，11 字段数据包，.NET 10，Release 构建。

| 操作 | 耗时 | 分配内存 |
|------|------|---------|
| `Serialize` | ~97 ns | ~400 B |
| `Deserialize` (byte[]) | ~140 ns | ~360 B |
| `Deserialize` (ReadOnlySpan) | ~139 ns | ~360 B |

优化手段：
- 成员元数据缓存（`ConcurrentDictionary<Type, MappableMember[]>`）
- `MappableMember` 预解析特性和字节序
- 表达式树编译属性 getter/setter（热路径零反射）
- 构造函数工厂编译
- `ArrayPool<byte>` 写入缓冲
- `ReadOnlySpan<byte>` 读取零中间分配

## 支持的类型

| 分类 | 类型 |
|------|------|
| 整数 | `byte`、`sbyte`、`short`、`ushort`、`int`、`uint`、`long`、`ulong` |
| 浮点 | `Half`、`float`、`double` |
| 其他原始类型 | `bool`（1 字节）、`char`（2 字节）、`Guid`（16 字节） |
| 字符串 | `string` 配合 `[FixedLength]` / `[NullTerminated]` / `[Encoding]` |
| 枚举 | 任意枚举，按底层类型定长 |
| 数组 | `T[]` 配合 `[FixedLength(n)]` |
| 集合 | 实现 `IList` / `IList<T>` 的类型，配合 `[FixedLength(n)]` |
| 对象 | 嵌套类/结构体成员 |

## 项目结构

| 项目 | 说明 |
|------|------|
| `src/Ledon.BinaryMapper` | 核心库 |
| `tests/Ledon.BinaryMapper.Tests` | xUnit 测试套件（22 个测试） |
| `tests/Ledon.BinaryMapper.SmokeTests` | 控制台冒烟测试 |
| `tests/Ledon.BinaryMapper.Benchmarks` | BenchmarkDotNet 基准测试 |

## 许可证

MIT