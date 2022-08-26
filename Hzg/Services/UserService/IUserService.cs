using System;
using System.Threading.Tasks;
using Hzg.Models;

namespace Hzg.Services;

/// <summary>
/// 用户功能
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取当前登录用户的信息
    /// </summary>
    /// <returns></returns>
    public Task<LoginUserInfo> GetLoginUserInfo();
    
    /// <summary>
    /// 获取当前用户名
    /// </summary>
    /// <returns></returns>
    public Task<string> GetCurrentUserName();
}