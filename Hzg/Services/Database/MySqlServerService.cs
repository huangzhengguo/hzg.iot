using Microsoft.EntityFrameworkCore;
using Hzg.Data;

namespace Hzg.Services;

/// <summary>
/// 数据库服务扩展
/// </summary>
public static class MySqlServerServiceExtesion
{
    private static MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 22));
    /// <summary>
    /// 添加 MySqlServer 服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddAccountDbSqlService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountDbContext>(options =>
        {
            options.UseMySql(configuration.GetConnectionString("AccountDbConnection"), serverVersion);
        });
    }
}