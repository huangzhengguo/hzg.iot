using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Hzg.Services;
using Hzg.Iot.Data;
using Hzg.Iot.Models;
using Hzg.Const;
using Hzg.Tool;
using Hzg.Data;

namespace Hzg.Iot.Controllers;

/// <summary>
/// 菜单
/// </summary>
[Route("api/[controller]/")]
public class MenuController : BaseController
{
    private readonly AccountDbContext _accountDbContext;
    public MenuController(HzgIotContext context, IUserService userService, AccountDbContext accountDbContext) : base(context)
    {
        _accountDbContext = accountDbContext;
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="menu"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    public async Task<string> Create([FromBody] Menu menu)
    {
        if (menu == null)
        {
        }
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "创建成功!",
            Data = null
        };

        // 检测菜单是否已经存在
        var m = await context.Menus.AsNoTracking().Where(m => m.ParentMenuId == menu.ParentMenuId && m.Title == menu.Title).ToListAsync();
        if (m != null && m.Count > 0)
        {
            result.Code = ErrorCode.ErrorCode_HasExisted;
            result.Message = "菜单已存在!";

            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
        }

        context.Menus.Add(menu);
        var n = await context.SaveChangesAsync();
        if (n != 1)
        {
            result.Code = ErrorCode.ErrorCode_Failed;
            result.Message = "创建失败!";
        }

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="menu"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("update")]
    public async Task<string> Update([FromBody] Menu menu)
    {
        if (menu == null)
        {
        }
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "修改成功!",
            Data = null
        };

        // 检测菜单是否已经存在
        var data = await context.Menus.AsNoTracking().SingleOrDefaultAsync(m => m.Id == menu.Id);
        if (data != null)
        {
            context.Update(menu);

            await context.SaveChangesAsync();

            // 是否更新菜单对应的权限

            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
        }

        result.Code = ErrorCode.ErrorCode_Failed;
        result.Message = "修改失败!";

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    public async Task<string>Delete(Guid id)
    {
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Data = null,
            Message = "删除成功!"
        };

        var m = await context.Menus.SingleOrDefaultAsync(m => m.Id == id);
        if (m != null)
        {
            context.Menus.Remove(m);
        }

        var n = await context.SaveChangesAsync();
        if (n != 1)
        {
            // 删除失败
            result.Code = ErrorCode.ErrorCode_Failed;
            result.Message = "删除失败!";
        }

        // 需要删除菜单关联的权限数据
        var mps = await context.MenuPermissions.Where(p => p.SubMenuId == id).ToListAsync();
        foreach(var mp in mps)
        {
            context.MenuPermissions.Remove(mp);
        }

        await context.SaveChangesAsync();

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get")]
    public async Task<string> Get()
    {
        var data = await _accountDbContext.Menus.AsNoTracking().ToListAsync();

        var jsonData = MenuTool.GenerateTreeData(data, null, null);
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Data = jsonData,
            Message = "获取成功!"
        };

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 获取指定菜单
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("user-menu-permission")]
    public async Task<string>GetUserMenuPermission(string userName)
    {
        // 需要所有用户数据和菜单权限数据做对比，放到前端做对比
        // 这里只获取菜单权限数据
        var menuPermissions = await context.MenuPermissions.AsNoTracking().Where(m => m.UserName == userName).ToListAsync();

        // 根据权限数据获取 Menu 列表
        var menusToReturn = new List<Hzg.Models.Menu>();
        var menus = await _accountDbContext.Menus.AsNoTracking().ToListAsync();
        foreach(var p in menuPermissions)
        {
            foreach(var m in menus)
            {
                if (m.Id == p.SubMenuId && p.Usable == true)
                {
                    menusToReturn.Add(m);
                }
            }
        }

        var routers = MenuTool.GenerateVueRouterData(menusToReturn, null, null);
        var responseData = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "获取成功",
            Data = routers
        };

        return JsonSerializer.Serialize(responseData, JsonSerializerTool.DefaultOptions());
    }
}