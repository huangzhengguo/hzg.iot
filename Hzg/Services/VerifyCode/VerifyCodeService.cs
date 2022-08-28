using StackExchange.Redis;

namespace Hzg.Services;

public class VerifyCodeService : IVerifyCodeService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public VerifyCodeService(IConfiguration configuration, IEmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <param name="minValue">最大值</param>
    /// <param name="maxValue">最小值</param>
    /// <returns></returns>
    public string GenerateVerifyCode(int minValue = 1000, int maxValue = 9999)
    {
        return new Random().Next(minValue, maxValue).ToString();
    }

    /// <summary>
    /// 发送注册验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    public void SendRegisterVerifyCode(string email)
    {
        var verifyCode = GenerateVerifyCode();

        var body = _configuration["email:body"];

        body = String.Format(body, verifyCode);

        _emailService.SendEmail(email, body);

        // 存储到 Redis
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        IDatabase db = redis.GetDatabase();

        
    }

}