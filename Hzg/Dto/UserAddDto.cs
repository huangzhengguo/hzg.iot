using System.ComponentModel.DataAnnotations;

namespace Ledinpro.Iot.Dto;

/**
 * <p>
 * 用户表
 * </p>
 *
 * @author guoy
 * @since 2020-03-24
 */
public class UserAddDto
{

    public static long serialVersionUID = 1L;

    /// <summary>
    /// 企业ID
    /// </summary>
    /// <value></value>
    [Required]
    public String Corpid { get; set; }

    /// <summary>
    /// 用户邮箱
    /// </summary>
    /// <value></value>
    [Required]
    public String Email { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    /// <value></value>
    [Required]
    public String Password { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    /// <value></value>
    [Required]
    public String Verifycode { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    /// <value></value>
    public String Nickname { get; set; }



}
