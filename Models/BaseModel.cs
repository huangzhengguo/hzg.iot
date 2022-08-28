using System.ComponentModel.DataAnnotations;

namespace Hzg.Iot.Models;

/// <summary>
/// 基类
/// </summary>
/// <typeparam name="KeyT"></typeparam>
public abstract class BaseModel<KeyT>
{
    /// <summary>
    /// 主键
    /// </summary>
    /// <value></value>
    public KeyT Id { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    /// <value></value>
    [StringLength(64)]
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateDateTime { get; set; }
}

// 默认
public abstract class BaseModel : BaseModel<Guid>
{}