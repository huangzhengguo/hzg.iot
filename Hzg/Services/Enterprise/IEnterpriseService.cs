using System.Net.Mail;
using Hzg.Models;

namespace Hzg.Services;

/// <summary>
/// 企业
/// </summary>
public interface IEnterpriseService
{
    /// <summary>
    /// 获取公司邮件信息
    /// </summary>
    /// <param name="corpid"></param>
    /// <returns></returns>
    Task<(MailProperties mailProperties, string errorMessage)> GetMailPropertie(String corpid);

    /// <summary>
    /// 获取发送邮件对象
    /// </summary>
    /// <param name="corpid"></param>
    /// <returns></returns>
    Task<SmtpClient> GetMailSender(String corpid);
}