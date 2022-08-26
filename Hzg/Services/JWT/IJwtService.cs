using System.Threading.Tasks;

namespace Hzg.Services;

/// <summary>
/// Jwt 功能接口
/// </summary>
public interface IJwtService
{
    // 生成 JWT token
    string GetnerateJWTToken(UserDto userDto);

    // 解析 JWT token
    UserDto DecodeJWTToken(string token);

    /// <summary>
    /// 新增 Token
    /// </summary>
    /// <param name="userDto">用户信息传输对象</param>
    JwtAuthDto Create(UserDto userDto);

    /// <summary>
    /// 刷新 Token
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="userDto">用户信息传输对象</param>
    /// <returns></returns>
    Task<JwtAuthDto> RefreshTokenAsync(string token, UserDto userDto);

    /// <summary>
    /// 当前 Token 是否有效
    /// </summary>
    /// <returns></returns>
    Task<bool> IsCurrentTokenValid();

    /// <summary>
    /// 判断 Token 是否有效
    /// </summary>
    /// <param name="token">指定 Token</param>
    /// <returns></returns>
    Task<bool> IsTokenValid(string token);

    /// <summary>
    /// 停用当前 Token
    /// </summary>
    /// <returns></returns>
    Task InvalidateCurrentToken();

    /// <summary>
    /// 停用 Token
    /// </summary>
    /// <param name="token">token</param>
    /// <returns></returns>
    Task InvalidateToken(string token);

    /// <summary>
    /// 获取用户Id
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Guid? GetUserId(string token);
}