using System.ComponentModel.DataAnnotations;

namespace Hzg.Iot.Models;

/// <summary>
/// 设备表
/// </summary>
public class Device : BaseModel
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    /// <value></value>
    [Required]
    public Guid UserId { get; set; }
}