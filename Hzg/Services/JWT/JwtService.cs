using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;

namespace Hzg.Services;

/// <summary>
/// JWT 功能类
/// </summary>
public class JwtService : IJwtService
{
    // Token 对象集合
    private static ISet<JwtAuthDto> _jwtTokens = new HashSet<JwtAuthDto>();
    // 获取 Http 上下文
    private readonly IHttpContextAccessor _httpContextAccessor;
    // 配置
    private readonly IConfiguration _configuration;
    public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 生成 JWT
    /// </summary>
    /// <param name="userDto">用户信息</param>
    /// <returns></returns>
    public string GetnerateJWTToken(UserDto userDto)
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim(ClaimTypes.Role, userDto.Roles),
            new Claim(ClaimTypes.NameIdentifier, userDto.UserId.ToString()),
            // 用户所在的分组
            new Claim("groups", userDto.Groups)
        };

        var issuer = _configuration[JwtOptionsConst.IssuerSettingPath];
        var audience = _configuration[JwtOptionsConst.AudienceSettingPath];
        var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration[JwtOptionsConst.ExpiresHourSettingPath]));

        var security = _configuration[JwtOptionsConst.SecurityKeySettingPath];
        SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(security));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(claims: claims, issuer: issuer, audience: audience, expires: expires, signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(jwtSecurityToken);
    }

    /// <summary>
    /// 从 token 中解析用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public UserDto DecodeJWTToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (tokenHandler.CanReadToken(token))
        {
            JwtPayload jwtPayload = tokenHandler.ReadJwtToken(token).Payload;
            
            var userName = jwtPayload.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var userGroups = jwtPayload.Claims.FirstOrDefault(c => c.Type == "groups").Value;
            var userRoles = jwtPayload.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

            UserDto user = new UserDto();

            user.UserName = userName;
            user.Groups = userGroups;
            user.Roles = userRoles;

            return user;
        }

        return null;
    }

    /// <summary>
    /// 新增 Token
    /// </summary>
    /// <param name="userDto">用户传输对象</param>
    /// <returns></returns>
    public JwtAuthDto Create(UserDto userDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["secretKey:sha256secret"]));

        DateTime authAt = DateTime.UtcNow;
        DateTime expires = authAt.AddMinutes(Convert.ToDouble(_configuration["JwtIssuerOptions:ExpireMinutes"]));

        // 声明
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim(ClaimTypes.Role, userDto.Roles),
            new Claim(ClaimTypes.Expiration, expires.ToString())
        };

        var identity = new ClaimsIdentity(claims);
        // 签发一个用户信息凭证
        _httpContextAccessor.HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        var jwtSecurityToken = new JwtSecurityToken
        (
            claims: claims,
            issuer: _configuration["JwtIssuerOptions:Issuer"],
            audience: _configuration["JwtIssuerOptions:Audience"],
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        var jwt = new JwtAuthDto
        {
            UserId = userDto.UserId,
            Token = tokenHandler.WriteToken(jwtSecurityToken),
            AuthTime = new DateTimeOffset(authAt).ToUnixTimeSeconds(),
            Expires = new DateTimeOffset(expires).ToUnixTimeSeconds(),
            Success = true,
            Code = 20000
        };

        _jwtTokens.Add(jwt);

        return jwt;
    }

    /// <summary>
    /// 刷新 Token
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="userDto">用户信息传输对象</param>
    /// <returns></returns>
    public Task<JwtAuthDto> RefreshTokenAsync(string token, UserDto userDto)
    {
        var jwt = GetToken(token);
        if (jwt == null)
        {
            return Task.FromResult(new JwtAuthDto
            {
                Token = "未获取到 Token",
                Success = false
            });
        }

        var newJwt = Create(userDto);

        // 停用旧的 Token
        InvalidateToken(token);

        return Task.FromResult(newJwt);
    }

    /// <summary>
    /// 当前 Token 是否有效
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsCurrentTokenValid()
    {
        return IsTokenValid(GetCurrentToken());
    }

    /// <summary>
    /// 判断 Token 是否有效
    /// </summary>
    /// <param name="token">指定 Token</param>
    /// <returns></returns>
    public Task<bool> IsTokenValid(string token)
    {
        // 只用判断当前 Token 集合中是否存在指定 Token
        var jwtToken = _jwtTokens.SingleOrDefault(j => j.Token == token);
        return Task.FromResult(jwtToken == null ? false : true);
    }

    /// <summary>
    /// 停用当前 Token
    /// </summary>
    /// <returns></returns>
    public Task InvalidateCurrentToken()
    {
        return InvalidateToken(GetCurrentToken());
    }

    /// <summary>
    /// 停用 Token
    /// </summary>
    /// <param name="token">token</param>
    /// <returns></returns>
    public Task InvalidateToken(string token)
    {
        // 删除 Token
        var jwtDto = _jwtTokens.SingleOrDefault(j => j.Token == token);
        if (jwtDto != null)
        {
            _jwtTokens.Remove(jwtDto);
        }

        return Task.FromResult(jwtDto);
    }

    /// <summary>
    /// 获取用户Id
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Guid? GetUserId(string token)
    {
        JwtAuthDto jwtAuthDto = _jwtTokens.SingleOrDefault(j => j.Token == token);
        if (jwtAuthDto == null)
        {
            return null;
        }

        return jwtAuthDto.UserId;
    }

    /// <summary>
    /// 获取当前 Token
    /// </summary>
    /// <returns></returns>
    private string GetCurrentToken()
    {
        // 获取认证头
        var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

        return authorizationHeader == StringValues.Empty ? string.Empty : authorizationHeader.Single().Split(" ").Last();
    }

    /// <summary>
    /// 获取 Token 对象
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    private JwtAuthDto GetToken(string token)
    {
        return _jwtTokens.SingleOrDefault(j => j.Token == token);
    }
}