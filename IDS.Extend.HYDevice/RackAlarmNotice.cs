using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    /// <summary>
    /// 轮询通知设备发送报警：只要系统记录报警存在，就不断向设备发送。
    /// 生命周期由 Host 管理，随应用启动/停止。
    /// </summary>
    public class RackAlarmNotice : BackgroundService
    {
        // 面的逻辑值（避免魔法数字）
        private const byte SideA = 0;
        private const byte SideB = 1;

        // 首次延迟 & 轮询间隔
        private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

        // 并发度：按设备/网络承受能力调整
        private const int MaxDegreeOfParallelism = 8;

        private readonly ILogger<RackAlarmNotice> _logger;
        private readonly IdsRedis _redis;

        public RackAlarmNotice(ILogger<RackAlarmNotice> logger, IdsRedis redis)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RackAlarmNotice 启动，{Delay} 秒后开始轮询，间隔 {Interval} 秒",
                StartupDelay.TotalSeconds, PollInterval.TotalSeconds);

            try
            {
                // 首次延迟：等应用初始化完成
                await Task.Delay(StartupDelay, stoppingToken);

                using var timer = new PeriodicTimer(PollInterval);
                do
                {
                    try
                    {
                        await SendNoticeAsync(stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // 正常关闭，跳出
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 单次轮询异常不影响后续轮询
                        _logger.LogError(ex, "RackAlarmNotice 本轮轮询发生异常");
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
                _logger.LogInformation("RackAlarmNotice 已停止");
            }
        }

        /// <summary>
        /// 遍历所有货架，并行发送报警通知。
        /// </summary>
        public async Task SendNoticeAsync(CancellationToken ct = default)
        {
            var rackNodeDic = SmartMaterialRackNode.Instance.GetAllRackNode();
            if (rackNodeDic == null || rackNodeDic.Count == 0)
            {
                return;
            }

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                CancellationToken = ct,
            };

            await Parallel.ForEachAsync(rackNodeDic.Values, options, async (node, token) =>
            {
                try
                {
                    await SendNoticeAsync(node, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    // 关闭中，忽略
                }
                catch (Exception ex)
                {
                    // 单个货架失败不影响其他货架
                    _logger.LogError(ex,
                        "向货架 {RackNo} ({Ip}:{Port}) 发送报警通知失败",
                        node.No, node.IP, node.Port);
                }
            });
        }

        /// <summary>
        /// 处理单个货架：A/B 两面依次通知。
        /// </summary>
        private async Task SendNoticeAsync(RackNode rackNode, CancellationToken ct)
        {
            var connection = ServerConnectionHolder.GetDefaultConnection();
            var endpoint = new IdsEndPoint(rackNode.IP, rackNode.Port);

            await SendNoticeWithSideAsync(connection, endpoint, rackNode, "A", SideA, ct);
            await SendNoticeWithSideAsync(connection, endpoint, rackNode, "B", SideB, ct);
        }

        /// <summary>
        /// 处理单个面的报警通知。
        /// 逻辑：
        ///   1) 有面报警 → 发面报警消息（mode=2），直接返回；
        ///   2) 否则收集所有储位报警地址 → 发储位报警消息（mode=1）。
        /// </summary>
        private async Task SendNoticeWithSideAsync(
            IServerConnection connection,          // 若底层连接有具体类型，替换为具体类型
            IdsEndPoint endpoint,
            RackNode rackNode,
            string side,
            byte shelfSide,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var db = _redis.GetDatabase();
            string key = $"{HYConstant.RackAlarmRecordKey}:{rackNode.No}_{side}";

            // ---------- 1. 面报警 ----------
            string? faceAlarm = await db.HashGetAsync(key, side);
            if (!string.IsNullOrEmpty(faceAlarm))
            {
                // 面报警不需要储位列表，传空列表
                var faceMessage = DeviceMessage.GetAlarm(new List<int>(), 0, 2, shelfSide);
                connection.Send(faceMessage, endpoint);
                return;
            }

            // ---------- 2. 储位报警 ----------
            var alarms = await db.HashGetAllAsync(key);
            if (alarms.Length == 0)
            {
                return;
            }

            var locs = new List<int>(alarms.Length);
            foreach (var alarm in alarms)
            {
                if (int.TryParse(alarm.Name.ToString(), out int addr))
                {
                    locs.Add(addr);
                }
            }

            if (locs.Count == 0)
            {
                return;
            }

            // 注意：原代码里 mode 变量算了没用，这里按原逻辑固定用 mode=1
            var message = DeviceMessage.GetAlarm(locs, 0, 1, shelfSide);
            connection.Send(message, endpoint);
        }
    }
}