using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Hzg.Models;
using Hzg.Data;
using Hzg.Dto;
using Hzg.Tool;
using Hzg.Const;

namespace Hzg.Services;

/// <summary>
/// 用户相关服务
/// </summary>
public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AccountDbContext _accountDbContext;
    private readonly ILocalizerService _localizerService;

    public UserService(IHttpContextAccessor httpContextAccessor,
                       AccountDbContext accountDbContext,
                       ILocalizerService localizerService)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountDbContext = accountDbContext;
        _localizerService = localizerService;
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns></returns>
    public async Task<LoginUserInfo> GetLoginUserInfo()
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var currentUser = await _accountDbContext.Users.SingleOrDefaultAsync(u => u.Id.ToString() == userId);
        LoginUserInfo userInfo = null;
        if (currentUser != null)
        {
            userInfo = new LoginUserInfo();
            
            userInfo.UserName = currentUser.Name;
            userInfo.UserId = currentUser.Id;
            // userInfo.Groups = currentUser.Group;
            // userInfo.Roles = currentUser.Role;
        }

        return userInfo;
    }

    /// <summary>
    /// 获取当前登录用户的名称
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetCurrentUserName()
    {
        var userInfo = await GetLoginUserInfo();

        return userInfo.UserName;
    }

    /// <summary>
    /// 获取当前登录用户的名称
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GetCurrentUserId()
    {
        var userInfo = await GetLoginUserInfo();

        return userInfo.UserId;
    }

    /// <summary>
    /// 用户是否存在
    /// </summary>
    /// <param name="corpid">公司ID</param>
    /// <param name="email">邮箱</param>
    /// <returns></returns>
    public async Task<(bool, string)> IsUserExist(String corpid, String email)
    {
        if (corpid == null)
        {
            return (false, _localizerService.Localizer("corpidCannotEmpty"));
        }

        if (email == null)
        {
            return (false, _localizerService.Localizer("emailCannotEmpty"));
        }

        var user = await _accountDbContext.Users.SingleOrDefaultAsync(u => u.CorpId == corpid && u.Email == email);
        if (user != null)
        {
            return (true, _localizerService.Localizer("thisUserAlreadyExists"));
        }

        return (false, _localizerService.Localizer("thisUserNotExists"));
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetDto"></param>
    /// <returns></returns>
    public async Task<bool> ModifyPassword(ModifyDto modifyDto)
    {
        var loginUserInfo = await GetLoginUserInfo();
        if (loginUserInfo == null)
        {
            return false;
        }

        var user = await _accountDbContext.Users.SingleOrDefaultAsync(u => u.Id == loginUserInfo.UserId);
        if (user.Password != MD5Tool.Encrypt(modifyDto.old_password, user.Salt))
        {
            return false;
        }

        user.Password = MD5Tool.Encrypt(modifyDto.new_password, user.Salt);

        _accountDbContext.Users.Update(user);

        if (await _accountDbContext.SaveChangesAsync() != 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetDto"></param>
    /// <returns></returns>
    public async Task<bool> ResetPassword(ResetDto resetDto)
    {
        var verifyCodeKey = CommonConstant.EMAIL_RESETPSW_CODE_KEY + resetDto.Email;
        var verifyCode = RedisTool.GetStringValue(verifyCodeKey);
        if (verifyCode != null && resetDto.Verifycode == verifyCode)
        {
            // 删除 Redis 中的验证码
            RedisTool.DeleteStringValue(verifyCodeKey);

            var user = await _accountDbContext.Users.SingleOrDefaultAsync(u => u.Email == resetDto.Email && u.CorpId == resetDto.Corpid);
            
            user.Salt = new Random().Next(100000, 999999).ToString();
            user.Password = MD5Tool.Encrypt(resetDto.New_password, user.Salt);

            _accountDbContext.Users.Update(user);
            if (await _accountDbContext.SaveChangesAsync() != 1)
            {
                return false;
            }

            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="userEditDto"></param>
    /// <returns></returns>
    public async Task<bool> ModifyUser(Guid userid, UserEditDto userEditDto)
    {
        var user = await _accountDbContext.Users.SingleOrDefaultAsync(u => u.Id == userid);

        CommonTool.CopyProperties(userEditDto, user);
        // user.Avatar = userEditDto.remark1;
        // user.Remark1 = userEditDto.remark1;
        // user.Remark1 = userEditDto.remark1;
        // user.Remark2 = userEditDto.remark2;
        // user.Remark3 = userEditDto.remark3;

        // user.Remark1 = userEditDto.remark1;
        // user.Remark1 = userEditDto.remark1;
        // user.Remark1 = userEditDto.remark1;

        _accountDbContext.Users.Update(user);

        if (await _accountDbContext.SaveChangesAsync() != 1)
        {
            return false;
        }

        return true;
    }
}