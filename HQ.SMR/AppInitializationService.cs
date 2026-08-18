using IDS.Extend.HYDevice;
using IDS.HQ.Module;
using Microsoft.EntityFrameworkCore;

namespace HQ.SMR
{
    public class AppInitializationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppInitializationService> _logger;

        public AppInitializationService(IServiceProvider serviceProvider, ILogger<AppInitializationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("应用容器完全启动，开始执行自定义初始化...");

            // 创建一个作用域来解析 Scoped 服务（如 DbContext、Autofac 注册的服务）
            using (var scope = _serviceProvider.CreateScope())
            {

               // var dbContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();

                // 执行你的初始化逻辑（异步）
                await InitializeDatabaseAsync();
            }

            _logger.LogInformation("自定义初始化完成。");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 可以留空，或者实现优雅停止的逻辑
            return Task.CompletedTask;
        }

        private async Task InitializeDatabaseAsync()
        {
            SmartMaterialRackNode.Instance.Initialize();
        }
    }
}
