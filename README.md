# hzg.iot

在开发过程中，我们使用 https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows 来管理数据库连接，这样数据库密码就不会暴漏在代码仓库中。
对于新项目，我们需要先运行以下命令初始化：
dotnet user-secrets init
运行后，.csproj 项目文件里会生成一个唯一的 UserSecretsId，用来作为键。

设置数据库连接：
dotnet user-secrets set "connectionStrings:MySQLAccountConnection" "Server=127.0.0.1;Database=HzgIotAccountDb;Uid=root;Pwd=xxxxxxxxxx;"
这里设置好后，在我们调试过程中，应用程序就会读取这里的配置。

配置好数据库连接，就可以生成数据库了，对于本项目，只需要运行以下命令，即可完成数据库的创建
dotnet ef migrations database update -c AccountContext
也可以生成 SQL 语句，然后在数据库手动生成：
dotnet ef migrations script -o ./sql/InitAccountDb.sql -c AccountContext
生成完数据库，我们就可以开始开发调试了
