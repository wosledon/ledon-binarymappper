using System.Text;

namespace Ledon.BinaryMapper;

/// <summary>
/// 表示二进制映射器的配置。<br/>
/// Represents settings for the binary mapper.
/// </summary>
public class BinaryMapperSettings
{
    /// <summary>
    /// 获取或设置全局默认字节序。<br/>
    /// Gets or sets the global default endianness.
    /// </summary>
    public Endianness Endianness { get; set; } = Endianness.BigEndian;

    /// <summary>
    /// 获取或设置全局默认编码。<br/>
    /// Gets or sets the global default encoding.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;
}
