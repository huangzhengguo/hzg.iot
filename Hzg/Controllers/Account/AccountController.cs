using System.Text.Json;
using System.Linq;
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
    private readonly AccountDbContext _accountContext;
    private IConfiguration _configuration;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IVerifyCodeService _verifyCodeService;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="inledcoContext">数据库上下文</param>
    /// <param name="configuration">配置</param>
    /// <param name="jwtService">JWT服务</param>
    public AccountController(AccountDbContext accountContext,
                             IConfiguration configuration,
                             IJwtService jwtService,
                             ILogger<LoginViewModel> logger,
                             IUserService userService,
                             IEmailService emailService,
                             IVerifyCodeService verifyCodeService)
    {
        _accountContext = accountContext;
        _configuration = configuration;
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
        _emailService = emailService;
        _verifyCodeService = verifyCodeService;
    }

    /// <summary>
    /// 获取注册验证码
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("send-register-verify-code")]
    public async Task<string> SendRegisterVerifyCode(string email)
    {
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Success,
            Message = ErrorCodeMessage.Message(ErrorCode.ErrorCode_Success)
        };

        // 检测邮箱是否已存在
        var user = await _accountContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            result.Message = "用户已存在!";
            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());;
        }

        // 1. 生成验证码
        // 2. 保存验证码到 redis
        // 3. 发送验证码到邮箱
        // _verifyCodeService.SendRegisterVerifyCode(email);

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
    }

    /// <summary>
    /// 注册账号流程
    /// 1. 获取验证码
    /// 2. 输入注册信息及收到的验证码
    /// 3. 注册账号
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<string> Register([FromBody] RegisterModel model)
    {
        var result = new ResponseData()
        {
            Code = ErrorCode.ErrorCode_Failed,
            Message = ErrorCodeMessage.Message(ErrorCode.ErrorCode_Failed)
        };

        // 检测用户是否已经注册
        var exitUser = await _accountContext.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
        if (exitUser != null)
        {
            result.Message = "用户已存在!";

            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
        }

        // 获取本地验证码
        var localVerifyCode = RedisTool.GetStringValue(model.Email);
        if (localVerifyCode == null || localVerifyCode.Equals(model.VerifyCode) == false)
        {
            // 验证码不一致
            result.Message = "验证码过期或者不正确!";

            return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
        }

        var user = new User();

        user.Name = model.Email;
        user.Email = model.Email;
        user.Password = model.Password;
    
        _accountContext.Users.Add(user);

        var count = await _accountContext.SaveChangesAsync();
        if (count != 1)
        {

        }

        result.Code = ErrorCode.ErrorCode_Success;
        result.Message = "注册成功!";

        return JsonSerializer.Serialize(result, JsonSerializerTool.DefaultOptions());
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

        var user = await _accountContext.Users.SingleOrDefaultAsync(u => u.Name == model.UserName);
        if (user == null)
        {
            result.Code = ErrorCode.ErrorCode_Failed;
            result.Message = "User not exist!";

            return JsonSerializerTool.SerializeDefault(result);
        }
        var password = MD5Tool.Encrypt(model.Password, user.Salt);
        if (password != user.Password)
        {
            result.Code = ErrorCode.ErrorCode_Failed;
            result.Message = "Password not correct!";

            return JsonSerializerTool.SerializeDefault(result);
        }

        if (user != null)
        {
            var userDto = new UserDto();

            // userDto.UserId = user.Id;
            userDto.UserName = user.Name;

            var jwtToken = _jwtService.GetnerateJWTToken(userDto);


            var menusToReturn = await MenuTool.GetUserPermissionMenus(_accountContext, user.Name);

            result.Data = new
            {
                token = jwtToken,
                menuData = menusToReturn
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
    [Route("groupusers")]
    public async Task<string> GetGroupUsers(string group)
    {
        var users = new List<User>();
        if (group != null)
        {
            var groupModel = await _accountContext.Groups.SingleOrDefaultAsync(g => g.Name == group);
            if (groupModel != null)
            {
                users = groupModel.UserGroups.Select((ug, i) => ug.User).ToList();
            }
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
                userNodeModel.IsLeaf = true;
                userNodeModel.Disabled = false;

                                // userNodeModel.Label = gu.Name + " (" + gu.Role + ")";
                string roles = "";
                foreach(var r in gu.UserRoles)
                {
                    roles = roles + r.Role.Name + ",";
                }
                
                userNodeModel.Label = gu.Name + " (" + roles.TrimEnd(',') + ")";

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