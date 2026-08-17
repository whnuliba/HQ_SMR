using Autofac;
using HQ.SMR;
using IDS.Common;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

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
builder.Services.AddControllers(options => {
    options.Filters.Add(new GlobalExceptionHandler());
    //options.Filters.Add(typeof(ActionAttribute));
}).AddControllersAsServices();
builder.Services.AddSingleton<IdsRedis, RedisClient>();
builder.Services.AddSingleton<IdsRedisLock>();
//注册Autofac
// 使用Autofac作为服务提供商
var files =  new string[] { "IDS.HQ.Service.dll", "IDS.Extend.HYDevice"};

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
