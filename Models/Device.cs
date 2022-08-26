using System.ComponentModel.DataAnnotations;
using Hzg.Models;

namespace Hzg.Iot.Models;

/// <summary>
/// 设备表
/// </summary>
public class Device : BaseEntity
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    /// <value></value>
    [Required]
    public Guid UserId { get; set; }
}