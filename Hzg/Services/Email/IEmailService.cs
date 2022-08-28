namespace Hzg.Services;

/// <summary>
/// 邮件服务
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="body">邮件内容</param>
    void SendEmail(string email, string body);
    
    /// <summary>
    /// 异步发送邮件
    /// </summary>
    /// <param name="email"></param>
    /// <param name="body"></param>
    public void SendEmailAsync(string email, string body, Action<bool> callback);
}