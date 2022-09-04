using System.ComponentModel.DataAnnotations;

namespace Hzg.Iot.Dto;

/**
 * <p>
 * 用户表
 * </p>
 *
 * @author guoy
 * @since 2020-03-24
 */
public class SubscribeDto
{

    public static long serialVersionUID = 1L;

    /// <summary>
    /// 产品ID
    /// </summary>
    /// <value></value>
    [Required]
    public String ProductKey { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    /// <value></value>
    [Required]
    public String DeviceName { get; set; }

    /// <summary>
    /// 设备mac地址
    /// </summary>
    /// <value></value>
    [Required]
    public String Mac { get; set; }
}
