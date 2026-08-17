using Autofac;
using IDS.Security.Service;
using IDS.Ioc;
using IDS.Bpms.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IDS.Common;
using IDS.Persistence;
using IDS.Security.Api.Controller;
using Autofac.Core;
using Newtonsoft.Json.Serialization;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
//builder.Services.AddDbContextFactory<AuthDbContext>().AddMemoryCache();
AppConfig.InitGlobalConfiguration(configuration);
builder.Services.AddPooledDbContextFactory<AuthDbContext>((option) =>
{
    var dbKey = configuration.GetSection("dbinfo:key").Value;
    var _dbConnectionString = configuration.GetConnectionString(dbKey);
    var db = configuration.GetSection("dbinfo:type").Value;
    option.UseSqlServer(_dbConnectionString);
    option.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((_, __) => false)));
    option.EnableDetailedErrors();
    option.EnableSensitiveDataLogging();

}).AddMemoryCache();
builder.Logging.AddLog4Net("log4net.config");
//var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
//XmlConfigurator.Configure(new FileInfo("log4net.config"));
//builder.Services.AddHttpContextAccessor().AddControllers().AddControllersAsServices();
builder.Services.AddControllers(options => {
    options.Filters.Add(new GlobalExceptionHandler());
    options.Filters.Add(typeof(RequestFilter));
}).AddControllersAsServices();
builder.Services.AddSingleton<IdsRedis, RedisClient>();
builder.Services.AddSingleton<IdsRedisLock>();
//注册Autofac
// 使用Autofac作为服务提供商
var files = new string[] { "IDS.Security.Adapter.dll", "IDS.Security.Service.dll" };

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

#region 注册Jwt认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer", o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,//是否验证签名,不验证的话可以篡改数据，不安全
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppConfig.GetConfigInfo("JwtTokenOptions:SecurityKey"))),//解密的密钥
        ValidateIssuer = true,//是否验证发行人，就是验证载荷中的Iss是否对应ValidIssuer参数
        ValidIssuer = AppConfig.GetConfigInfo("JwtTokenOptions:Issuer"),//发行人,
        ValidateAudience = true,//是否验证订阅人，就是验证载荷中的Aud是否对应ValidAudience参数
        ValidAudience = AppConfig.GetConfigInfo("JwtTokenOptions:Audience")//订阅人       
    };
});
builder.Services.AddAuthorization();
#endregion


//builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<LoginWsService>();
builder.Services.AddWebSockets(c => { 
    c.KeepAliveInterval= TimeSpan.FromSeconds(60);
    c.ReceiveBufferSize= 1024*2;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseRouting();
//启用sigalR后原生支持的Websocket将失效，
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=security1}/{action=Index}/{id?}"
//    );
//    endpoints.MapHub<ChatHub>("/security1");
//});


app.UseWebSockets();
//app.MapGet("/security/{id?}", async (HttpContext context, string id) => //async (LoginWsService service)=>
//{

//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        var service = (LoginWsService)ContainerUtils.AutofacServiceProvider.GetService(typeof(LoginWsService));
//        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//        await service.HandleWebSocketConnection(webSocket, id);
//    }
//});



app.UseAuthorization();

app.MapControllers();

app.Run();
