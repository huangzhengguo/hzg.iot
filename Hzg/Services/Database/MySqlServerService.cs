using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Hzg.Data;

namespace Hzg.Services;

/// <summary>
/// 数据库服务扩展
/// </summary>
public static class MySqlServerServiceExtesion
{
    private static MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 23));
    /// <summary>
    /// 添加 MySqlServer 服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddAccountDbSqlService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountContext>(options =>
        {
            options.UseMySql(configuration.GetConnectionString("MySQLAccountConnection"), serverVersion);
        });
    }

    /// <summary>
    /// 添加 MySqlServer 服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    static public void AddSqlService(IServiceCollection services, IConfiguration configuration)
    {
        // var serverVersion = new MySqlServerVersion(new Version(8, 0, 23));
        // services.AddDbContext<LedinproContext>(options =>
        // {
        //     options.UseMySql(configuration.GetConnectionString("MySQLConnection"), serverVersion);
        // });
    }
}