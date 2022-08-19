using System.ComponentModel.DataAnnotations;

namespace Hzg.Iot.Models;

/// <summary>
/// 用户类
/// </summary>
public class User : BaseAccount
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <value></value>
    [StringLength(64)]
    public string Name { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value></value>
    [StringLength(64)]
    public string NickName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    /// <value></value>
    [StringLength(16)]
    public string Gender { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    /// <value></value>
    [StringLength(32)]
    public string Password { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value></value>
    [StringLength(128)]
    public string Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    /// <value></value>
    [StringLength(64)]
    public string Phone { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    /// <value></value>
    [StringLength(64)]
    public string Avatar { get; set; }

    // 导航属性
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}