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
public class ModifyDto
{

    public static long serialVersionUID = 1L;

    /**
     * 旧密码
     */
    [Required]
    public String old_password { get; set; }

    /**
     * 新密码
     */
    [Required]
    public String new_password { get; set; }
}
