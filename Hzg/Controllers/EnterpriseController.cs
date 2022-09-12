using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using Hzg.Models;
using Hzg.Tool;
using Hzg.Dto;
using Hzg.Services;

namespace Hzg.Controllers;

/// <summary>
/// 企业信息
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]/")]
public class EnterpriseController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    public EnterpriseController(IConfiguration configuration, IUserService userService)
    {
        this._configuration = configuration;
        this._userService = userService;
    }

    /// <summary>
    /// 获取所有企业信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get_all")]
    public async Task<string> GetAll()
    {
        var responseData = new ResponseData()
        {
            Code = Const.ErrorCode.ErrorCode_Success,
            Message = "成功"
        };

        var connect = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("IotDbConnection"));

        var data = await connect.QueryAsync<EnterpriseDto>("select * from enterprise");

        responseData.Data = data;

        return JsonSerializerTool.SerializeDefault(responseData);
    }

    /// <summary>
    /// 获取所有企业信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("get")]
    public async Task<string> GetDetail([FromQuery] string id)
    {
        var responseData = new ResponseData()
        {
            Code = Const.ErrorCode.ErrorCode_Success,
            Message = "success"
        };

        var connect = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("IotDbConnection"));

        var data = await connect.QueryAsync<EnterpriseDto>("select * from enterprise where id = @Id", new { Id = id });
        if (data.Count() > 0)
        {
            responseData.Data = data.First();
        }
        else
        {
            responseData.Code = Const.ErrorCode.ErrorCode_Failed;
            responseData.Message = "Failed";

            return JsonSerializerTool.SerializeDefault(responseData);
        }

        return JsonSerializerTool.SerializeDefault(responseData);
    }

    /// <summary>
    /// 新建
    /// </summary>
    /// <param name="enterprise"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    public async Task<string> Create([FromBody] EnterpriseDto enterpriseDto)
    {
        var responseData = new ResponseData()
        {
            Code = Const.ErrorCode.ErrorCode_Success,
            Message = LocalizerTool.Value("success")
        };

        var connect = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("IotDbConnection"));

        var count = await connect.ExecuteAsync(@"insert Enterprise(id, name, contacts_user, contacts_phone, sort, create_time, creator,
                                               mail_host, mail_port, mail_encoding, mail_protocol, mail_username, mail_password,
                                               mail_properties_ssl_enable, mail_registration_subject, mail_registration_content,
                                               mail_reset_subject, mail_reset_content)
                                  values(@Id, @Name, '', '', @0, '" + DateTime.Now + @"', '" + _userService.GetCurrentUserId() + 
                                  @"', @MailHost, @MainPort, @MailEncoding, @MailProtocol, @MailUsername, @MailPassword, 
                                  @MailPropertiesSslEnable, @MailRegistrationSubject, @MailRegistrationContent, MailResetSubject
                                  @MailResetContent)", enterpriseDto);
        if (count != 1)
        {
            responseData.Code = Const.ErrorCode.ErrorCode_Failed;
            responseData.Message = LocalizerTool.Value("failed");

            return JsonSerializerTool.SerializeDefault(responseData);
        }
        responseData.Data = count;

        return JsonSerializerTool.SerializeDefault(responseData);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="enterprise"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("update")]
    public async Task<string> update([FromBody] EnterpriseDto enterpriseDto)
    {
        var responseData = new ResponseData()
        {
            Code = Const.ErrorCode.ErrorCode_Success,
            Message = LocalizerTool.Value("success")
        };

        var connect = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("IotDbConnection"));

        var count = await connect.ExecuteAsync(@"update Enterprise set name = @Name, mail_host= @MailHost, mail_port=@MailPort
                                               mail_encoding = @MailEncoding, mail_protocol = @MailProtocol, mail_username = @MailUsername,
                                               mail_password = @MailPassword, mail_properties_ssl_enable = @MailPropertiesSslEnable,
                                               mail_registration_subject = @MailRegistrationSubject,
                                               mail_registration_content = @MailRegistrationContent,
                                               mail_reset_subject = @MailResetSubject,
                                               mail_reset_content = @MailResetContent where id = @Id", enterpriseDto);
        if (count != 1)
        {
            responseData.Code = Const.ErrorCode.ErrorCode_Failed;
            responseData.Message = LocalizerTool.Value("failed");

            return JsonSerializerTool.SerializeDefault(responseData);
        }

        responseData.Data = count;

        return JsonSerializerTool.SerializeDefault(responseData);
    }
}