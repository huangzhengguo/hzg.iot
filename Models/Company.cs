using System.ComponentModel.DataAnnotations;

namespace Hzg.Iot.Models;

/// <summary>
/// 公司
/// </summary>
public class Company : BaseModel
{
    /// <summary>
    /// 公司名称
    /// </summary>
    /// <value></value>
    [StringLength(256)]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <value></value>
    public string Des { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    /// <value></value>
    public string Remark { get; set; }
}