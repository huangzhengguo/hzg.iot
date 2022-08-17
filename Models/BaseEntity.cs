namespace Hzg.Iot.Models;

/// <summary>
/// 基类
/// </summary>
/// <typeparam name="KeyT"></typeparam>
public abstract class BaseEntity<KeyT>
{
    /// <summary>
    /// 主键
    /// </summary>
    /// <value></value>
    public KeyT Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateDateTime { get; set; }
}

public abstract class BaseEntity : BaseEntity<Guid>
{}