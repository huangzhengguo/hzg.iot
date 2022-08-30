using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hzg.Iot.Data;

namespace Hzg.Iot.Controllers;

/// <summary>
/// API 请求基类
/// </summary>
[Authorize]
[ApiController]
public class BaseController : ControllerBase
{
    protected readonly HzgIotContext context;
    public BaseController(HzgIotContext hzgIotContext)
    {
        this.context = hzgIotContext;
    }
}