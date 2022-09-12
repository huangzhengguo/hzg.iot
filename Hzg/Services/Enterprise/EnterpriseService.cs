using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using Hzg.Models;

namespace Hzg.Services;

/// <summary>
/// 企业
/// </summary>
public class EnterpriseService : IEnterpriseService
{
    // private readonly IotDbContext _context;
    private readonly ILocalizerService _localizerService;
    private Dictionary<string, SmtpClient> mailSenders;
    private Dictionary<string, MailProperties> mailsTies;

    public EnterpriseService(ILocalizerService localizerService)
    {
        // this._context = context;
        this._localizerService = localizerService;

        this.mailSenders = new Dictionary<string, SmtpClient>();
        this.mailsTies = new Dictionary<string, MailProperties>();
    }

    /// <summary>
    /// 获取公司发件邮件信息
    /// </summary>
    /// <param name="corpid"></param>
    /// <returns></returns>
    public async Task<(MailProperties, string)> GetMailPropertie(String corpid)
    {
        if (corpid == null)
        {
            return (null, _localizerService.Localizer("corpidCannotEmpty"));
        }

        if (mailsTies == null || mailsTies.Count == 0)
        {
            await BuildMailSenders();
        }

        if (corpid != null && mailsTies.Keys.Contains(corpid) == true)
        {
            return (mailsTies[corpid], null);
        }

        return (null, _localizerService.Localizer("corpidDoesNotExist"));
    }

    /// <summary>
    /// 获取发送邮件对象
    /// </summary>
    /// <param name="corpid"></param>
    /// <returns></returns>
    public async Task<SmtpClient> GetMailSender(String corpid)
    {
        if (mailSenders == null || mailSenders.Count == 0)
        {
            await BuildMailSenders();
        }

        if (mailSenders.Keys.Contains(corpid) == true)
        {
            return mailSenders[corpid];
        }

        return null;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public async Task BuildMailSenders() {
        // 查询企业信息
        // List<Enterprise> properties = await _context.Enterprises.AsNoTracking().OrderBy(e => e.Sort).ToListAsync();
        
        // foreach(var p in properties)
        // {
        //     // 处理企业信息
        //     PutMailMap(p);
        // }
    }

    /// <summary>
    /// 构造所有公司的邮箱信息
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private Boolean PutMailMap(Enterprise property)
    {
        MailProperties properties1 = new MailProperties();

        properties1.CorpId = property.Id;
        properties1.CorpName = property.Name;
        properties1.UserName = property.MailUsername;
        properties1.Password = property.MailPassword;
        properties1.Encoding = property.MailEncoding;
        properties1.Host = property.MailHost;
        properties1.Port = Convert.ToInt32(property.MailPort);
        properties1.Protocol = property.MailProtocol;

        String encryption = property.MailPropertiesSslEnable.ToLower();

        Dictionary<object, object> properties2 = new Dictionary<object, object>();
        if (encryption == "true" || encryption == "ssl")
        {
            properties2["mail.smtp.ssl.enable"] = "true";
        }
        else if (encryption == "starttls")
        {
            properties2["mail.smtp.starttls.enable"] = "true";
        }

        properties1.Properties = properties2;

        // 01 注册验证码
        VerificationTemplate verificationTemplate = new VerificationTemplate();

        verificationTemplate.Subject = property.MailRegistrationSubject;
        verificationTemplate.Content = property.MailRegistrationContent;

        properties1.VerificationRegistrationTemplate = verificationTemplate;

        // 02 重置验证码
        VerificationTemplate verificationTemplate1 = new VerificationTemplate();

        verificationTemplate1.Subject = property.MailResetSubject;
        verificationTemplate1.Content = property.MailResetContent;

        properties1.VerificationResetTemplate = verificationTemplate1;

        // 数据转换
        if (properties1 != null)
        {
            var smtp = new SmtpClient();

            smtp.Host = properties1.Host;
            smtp.Port = properties1.Port;
            smtp.Credentials = new NetworkCredential(properties1.UserName, properties1.Password);

            mailSenders[properties1.CorpId.ToString()] = smtp;
            mailsTies[properties1.CorpId.ToString()] = properties1;
        }

        return true;
    }
}