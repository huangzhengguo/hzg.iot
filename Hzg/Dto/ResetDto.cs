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

public class ResetDto
{

    public static long serialVersionUID = 1L;

    /// <summary>
    /// 企业ID
    /// </summary>
    /// <value></value>
    [Required]
    public String Corpid { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value></value>
    [Required]
    public String Email { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    /// <value></value>
    [Required]
    public String Verifycode { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    /// <value></value>
    [Required]
    public String New_password { get; set; }
}
