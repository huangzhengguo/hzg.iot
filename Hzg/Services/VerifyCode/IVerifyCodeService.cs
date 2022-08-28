namespace Hzg.Services;

public interface IVerifyCodeService
{
    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    string GenerateVerifyCode(int minValue = 1000, int maxValue = 9999);

    /// <summary>
    /// 发送注册验证码
    /// </summary>
    /// <param name="email"></param>
    void SendRegisterVerifyCode(string email);
}