using Microsoft.AspNetCore.Mvc;
using Hzg.Const;
using Hzg.Iot.Data;

namespace Hzg.Iot.Controllers;

/// <summary>
/// 验证码控制器
/// </summary>
public class VerifyCodeController : BaseController
{
    public VerifyCodeController(HzgIotContext context) : base(context) {}
    // public async Task<string> SendVerifyCode(VerifyCodeType type, string email)
    // {
    //     var smtp = new SmtpClient();
    // }
}