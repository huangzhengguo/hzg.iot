using System.Text;
using System.Net;
using System.Net.Mail;

namespace Hzg.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="body">邮件内容</param>
    public void SendEmail(string email, string body)
    {
        var fromEmail = _configuration["email:email"];
        var displayName = _configuration["email:displayName"];
        var password = _configuration["email:password"];
        var subject = _configuration["email:subject"];
        var host = _configuration["email:host"];
        var port = _configuration["email:port"];

        // 邮件消息信息
        var mailMessage = new MailMessage();

        mailMessage.From = new MailAddress(fromEmail, displayName, Encoding.UTF8);
        mailMessage.To.Add(new MailAddress(email));
        mailMessage.Subject = subject;
        mailMessage.Body = body;

        var smtp = new SmtpClient();

        smtp.Host = host;
        smtp.Port = Convert.ToInt32(port);
        smtp.Credentials = new NetworkCredential(fromEmail, password);

        smtp.Send(mailMessage);
    }
}