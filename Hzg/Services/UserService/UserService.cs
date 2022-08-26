using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Hzg.Models;
using Hzg.Data;

namespace Hzg.Services;

public class UserService : IUserService
{
    // private readonly UserManager<AccountUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AccountContext _context;

    public UserService(IHttpContextAccessor httpContextAccessor, AccountContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns></returns>
    public async Task<LoginUserInfo> GetLoginUserInfo()
    {
        var user = _httpContextAccessor.HttpContext.User;
        var name = user.FindFirst(ClaimTypes.Name)?.Value;

        var currentUser = await _context.Users.SingleOrDefaultAsync(u => u.Name == name);
        var userInfo = new LoginUserInfo();
        if (currentUser != null)
        {
            userInfo.UserName = currentUser.Name;
            userInfo.UserId = currentUser.Id;
            // userInfo.Groups = currentUser.Group;
            // userInfo.Roles = currentUser.Role;
        }
        else
        {
            userInfo.UserName = "用户";
        }

        return userInfo;
    }

    public async Task<string> GetCurrentUserName()
    {
        var userInfo = await GetLoginUserInfo();

        return userInfo.UserName;
    }
}