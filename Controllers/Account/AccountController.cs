using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hzg.Data;
using Hzg.Services;
using Hzg.Models;
using Hzg.Const;
using Hzg.Tool;

namespace Hzg.Iot.Controllers;

/// <summary>
/// 账号
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]/")]
public class AccountController : ControllerBase
{
    private readonly AccountContext _accountContext;
    private IConfiguration _configuration;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IUserService _userService;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="inledcoContext">数据库上下文</param>
    /// <param name="configuration">配置</param>
    /// <param name="jwtService">JWT服务</param>
    public AccountController(AccountContext accountContext,
                             IConfiguration configuration,
                             IJwtService jwtService,
                             ILogger<LoginViewModel> logger,
                             IUserService userService)
    {
        _accountContext = accountContext;
        _configuration = configuration;
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("[action]")]
    public async Task<string> Login([FromBody] LoginViewModel model)
    {
        var result = new ResponseData() {
            Code = ErrorCode.ErrorCode_Success,
            Message = ErrorCodeMessage.Message(ErrorCode.ErrorCode_Success)
        };

        var user = await _accountContext.Users.SingleOrDefaultAsync(u => u.Name == model.UserName && u.Password == model.Password);
        if (user != null)
        {
            var userDto = new UserDto();

            userDto.UserId = user.Id;
            userDto.UserName = user.Name;

            var jwtToken = _jwtService.GetnerateJWTToken(userDto);

            // var menusToReturn = await MenuUtility.GetUserPermissionMenus(_ledinproContext, user.Name);

            result.Data = new
            {
                token = jwtToken,
                // menuData = menusToReturn
            };

            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
        }

        result.Code = ErrorCode.ErrorCode_Failed;
        result.Message = "账号或密码错误!";

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public IActionResult LogOut()
    {
        return new OkObjectResult(new { code = 20000 });
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("user/info")]
    public async Task<IActionResult> UserInfo(string token)
    {
        // 获取用户信息
        Guid? userId = _jwtService.GetUserId(token);
        var user = await _accountContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            // return new OkObjectResult(new { code = 20000, data = new { roles = user.Role.Split(",") , name = user.Name, group = user.Group, avatar = "", introduction = "introduction" } });
        }

        return new BadRequestResult();
    }

    /// <summary>
    /// 获取指定分组的用户,如果分组名为空,则获取所有
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getgroupusers")]
    public async Task<string> GetGroupUsers(string group)
    {
        var users = new List<User>();
        if (group != null)
        {
            // users = await _accountContext.Users.AsNoTracking().Where(u => u.Group == group).ToListAsync();
        }
        else
        {
            users = await _accountContext.Users.AsNoTracking().ToListAsync();
        }

        var response = new { code = ErrorCode.ErrorCode_Success, data = users };

        return JsonSerializer.Serialize(response, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    /// <summary>
    /// 获取所有分组
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getallgroups")]
    public async Task<string> GetAllGroups()
    {
        var allGroups = await _accountContext.Groups.AsNoTracking().OrderBy(g => g.Name).ToListAsync();

        var response = new { code = ErrorCode.ErrorCode_Success, data = allGroups };

        return JsonSerializer.Serialize(response);
    }

    /// <summary>
    /// 获取用户目录树数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get-user-treedata")]
    public async Task<string> GetUserTreeData()
    {
        // 格式 [{ label: '', children: [ { label: '', children: [] } ] }]
        var userTreeData = new List<UserTreeNode>();
        var groups = _accountContext.Groups.AsNoTracking().OrderBy(g => g.Name).ToList();
        foreach(var m in groups)
        {
            var nodeModel = new UserTreeNode();

            nodeModel.NodeKey = m.Id.ToString();
            nodeModel.Id = m.Id;
            nodeModel.ParentMenuId = null;
            nodeModel.IsLeaf = false;
            nodeModel.Disabled = true;

            // 获取分组成员
            var childrenUserTreeData = new List<UserTreeNode>();
            var groupsUsers = await _accountContext.Users.AsNoTracking().Where(u => u.Name == m.Name).OrderBy(u => u.Name).ToListAsync();
            foreach(var gu in groupsUsers)
            {
                var userNodeModel = new UserTreeNode();
                
                userNodeModel.NodeKey = m.Id.ToString() + gu.Id.ToString();
                userNodeModel.Id = gu.Id;
                userNodeModel.ParentMenuId = m.Id;
                userNodeModel.LabelValue = gu.Name;
                // userNodeModel.Label = gu.Name + " (" + gu.Role + ")";
                userNodeModel.IsLeaf = true;
                userNodeModel.Disabled = false;

                childrenUserTreeData.Add(userNodeModel);
            }

            nodeModel.Children = childrenUserTreeData.ToArray();
            nodeModel.Label = m.Name + " (" + nodeModel.Children.Length.ToString() + ")";
            nodeModel.LabelValue = m.Name;

            userTreeData.Add(nodeModel);
        }

        var responseData = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = "获取成功",
            Data = userTreeData
        };

        return JsonSerializer.Serialize(responseData, JsonSerializerTool.DefaultOptions());
    }

    public class UserTreeNode
    {
        public string NodeKey { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentMenuId { get; set; }
        public string Label { get; set; }
        public string LabelValue { get; set; }
        public UserTreeNode[] Children { get; set; }
        public bool IsLeaf { get; set; }
        public bool Disabled { get; set; }
    }

    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class LoginUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 前端显示的用户信息
    /// </summary>
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }            
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
    }
}