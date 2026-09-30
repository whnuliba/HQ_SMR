using IDS.Extend.HYDevice;
using IDS.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Job
{
    public abstract class BaseJob<T> : BackgroundService
    {
        public ILogger<T> _logger;
        private TimeSpan StartupDelay = TimeSpan.FromSeconds(1);
        private TimeSpan PollInterval = TimeSpan.FromSeconds(5);
        public BaseJob(ILogger<T> logger,TimeSpan startupDelay, TimeSpan pollInterval)
        {
            _logger = logger;
            StartupDelay = startupDelay;
            PollInterval = pollInterval;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await OnTimer(stoppingToken);
        }
        private async Task OnTimer(CancellationToken stoppingToken)
        {
            _logger?.LogInformation("{BaseJob} 启动，{Delay} 秒后开始轮询，间隔 {Interval} 秒",
               this.GetType().FullName,StartupDelay.TotalSeconds, PollInterval.TotalSeconds);

            try
            {
                // 首次延迟：等应用初始化完成
                await Task.Delay(StartupDelay, stoppingToken);

                using var timer = new PeriodicTimer(PollInterval);
                do
                {
                    try
                    {
                        await ActionAsync(stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // 正常关闭，跳出
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 单次轮询异常不影响后续轮询
                        _logger.LogError(ex, $"{this.GetType().FullName} 本轮轮询发生异常");
                    }
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // 应用关闭，正常退出
            }
            finally
            {
                _logger.LogInformation($"{this.GetType().FullName} 已停止");
            }
        }
        protected abstract Task ActionAsync(CancellationToken stoppingToken);
    }
}
