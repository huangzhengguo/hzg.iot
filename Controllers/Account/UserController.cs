using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Hzg.Services;
using Hzg.Iot.Data;
using Hzg.Const;
using Hzg.Data;
using Hzg.Tool;
using Hzg.Models;

namespace Hzg.Iot.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]/")]
public class UserController : ControllerBase
{
    private readonly AccountDbContext context;
    private readonly IUserService _userService;
    private readonly HzgIotContext _hzgIotContext;
    public UserController(AccountDbContext accountContext, IUserService userService, HzgIotContext hzgIotContext)
    {
        this.context = accountContext;
        this._userService = userService;
        this._hzgIotContext = hzgIotContext;
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get")]
    public async Task<string> get(string name)
    {
        var users = await context.Users.AsNoTracking().Where(m => m.Name == name).OrderBy(m => m.Name).ToListAsync();
        if (string.IsNullOrWhiteSpace(name)) {
            users = await context.Users.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        }

        var response = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "获取用户列表成功!",
            Data = users
        };

        return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 新建用户
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="groupId">分组 Id</param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    public async Task<string> Create([FromBody] User user, Guid? groupId)
    {
        var model = new User();

        model.Name = user.Name;
        model.Gender = user.Gender;

        var response = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_HasExisted,
            Message = "用户已存在!"
        };

        var u = await context.Users.SingleOrDefaultAsync(m => m.Name == user.Name);
        if (u != null)
        {
            return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
        }

        context.Users.Add(model);

        if (await context.SaveChangesAsync() == 1)
        {
            // 分组
            if (groupId != null)
            {
                var userGroup = new UserGroup();

                userGroup.UserId = model.Id;
                userGroup.GroupId = groupId!.Value;

                context.UserGroups.Add(userGroup);
                if (await context.SaveChangesAsync() != 1)
                {
                    response.Message = "加入分组失败!";
                    return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
                }
            }
        }

        response.Code = ErrorCode.ErrorCode_Success;
        response.Message = "创建成功!";

        return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("update")]
    public async Task<string> update([FromBody] User user)
    {
        var response = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_NotExist,
            Message = "用户不存在!"
        };

        var model = await context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
        if (model == null)
        {
            // 用户不存在
            return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
        }

        // 用户信息
        model.Name = user.Name;
        model.Gender = user.Gender;

        context.Users.Update(model);

        await context.SaveChangesAsync();

        response.Code = ErrorCode.ErrorCode_Success;
        response.Message = "修改成功!";

        return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    public async Task<string> delete(Guid? id)
    {
        var response = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_NotExist,
            Message = "用户不存在!"
        };

        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            // 用户不存在
            return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
        }

        context.Remove(user);

        await context.SaveChangesAsync();

        response.Code = ErrorCode.ErrorCode_Success;
        response.Message = "删除成功!";

        return JsonSerializer.Serialize(response, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 获取用户权限数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("user-menu-permission")]
    public async Task<string> GetUserMenuPermission()
    {
        // 需要所有用户数据和菜单权限数据做对比，放到前端做对比
        // 这里只获取菜单权限数据
        var userName = await _userService.GetCurrentUserName();

        var menus = await MenuTool.GetUserPermissionMenus(context, userName);
        var responseData = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "获取成功",
            Data = menus
        };

        return JsonSerializer.Serialize(responseData, JsonSerializerTool.DefaultOptions());
    }
}    