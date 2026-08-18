using Autofac;
using HQ.SMR;
using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using IDS.SMR.Bootstrap;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.


        var configuration = builder.Configuration;
        AppConfig.InitGlobalConfiguration(configuration);
        builder.Services.AddPooledDbContextFactory<RackDbContext>((option) =>
        {
            var dbKey = configuration.GetSection("dbinfo:key").Value;
            var _dbConnectionString = configuration.GetConnectionString(dbKey);
            var db = configuration.GetSection("dbinfo:type").Value;
            var serverVersion = ServerVersion.AutoDetect(_dbConnectionString);
            option.UseMySql(_dbConnectionString, serverVersion);
            option.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((_, __) => false)));
            option.EnableDetailedErrors();
            option.EnableSensitiveDataLogging();

        }).AddMemoryCache();
        //builder.Services.UsePooledDbContextFactory(configuration);
        builder.Logging.AddLog4Net("log4net.config");
        //var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
        //XmlConfigurator.Configure(new FileInfo("log4net.config"));
        //builder.Services.AddHttpContextAccessor().AddControllers().AddControllersAsServices();
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(new GlobalExceptionHandler());
            //options.Filters.Add(typeof(ActionAttribute));
        }).AddControllersAsServices();
        builder.Services.AddSingleton<IdsRedis, RedisClient>();
        builder.Services.AddSingleton<IdsRedisLock>();
        //注册Autofac
        // 使用Autofac作为服务提供商
        var files = new string[] { "IDS.HQ.Service.dll", "IDS.Extend.HYDevice" };

        builder.Host.UseServiceProviderFactory(new IDSServiceProviderFactory())
               .ConfigureContainer<ContainerBuilder>(containerBuilder =>
               {
                   containerBuilder.UseScanModules(configuration, files);
               });
        //builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            // 在这里可以配置其他Newtonsoft.Json的设置
        });

        // 在添加 Autofac 容器之后注册
        builder.Services.AddHostedService<AppInitializationService>();

        // 如果你有多个 IHostedService，希望这个初始化服务先于其他后台任务执行，
        // 可以使用下面的方式调整顺序（需要安装 Microsoft.Extensions.Hosting.Abstractions）
        builder.Services.AddSingleton<IHostedService, AppInitializationService>(sp =>
            new AppInitializationService(sp, sp.GetRequiredService<ILogger<AppInitializationService>>()));

        #region  开启UDP监听
        ushort smrSocketPort = 9999;
        if (ushort.TryParse(configuration.GetSection("HQ_SMR:SocketConnection:Port").Value, out ushort port)) {
            smrSocketPort = port;
        }
        string ip = configuration.GetSection("HQ_SMR:SocketConnection:IP").Value ?? "0.0.0.0";

        IServerConnection serverConnection = new HYBootstrap().RegisterServiceAndStartup(new IdsEndPoint(ip, smrSocketPort));
        //注册全局连接器
       // ServerConnectionHolder.SetConnection(serverConnection);
        #endregion
        //初始化货架缓存
       // SmartMaterialRackNode.Instance.Initialize();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

#region  开启UDP监听

#endregion
