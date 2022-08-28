using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Hzg.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // 监听 9000 端口
    options.ListenLocalhost(9000);
});

// 配置数据库连接
builder.Services.AddAccountDbSqlService(builder.Configuration);
builder.Services.AddIotSqlService(builder.Configuration);

// JWT 服务
builder.Services.AddJwt(builder.Configuration);
// 用户服务
builder.Services.AddTransient<IUserService, UserService>();
// 验证码
builder.Services.AddTransient<IVerifyCodeService, VerifyCodeService>();
// 邮件服务
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 HTTP 请求通道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
