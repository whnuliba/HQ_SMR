using HPSocket;
using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.Persistence;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using static LinqToDB.Common.Configuration;


namespace IDS.HQ.Service.Job
{
    //==============================================================
    // 文件名   : LocationCheckJob.cs
    // 作者     : 100845520
    // 创建时间 : 2026/11/9 19:19:40
    // 版本     : V1.0.0
    // 描述     : 
    //==============================================================
    public class LocationCheckJob : BaseJob<AutoLightOffJob>
    {
        public ILog Logger = LogManager.GetLogger(typeof(AutoLightOffJob));

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;
        public IdsRedisLock _idsRedisLock;
        public LocationCheckJob(IDbContextFactory<RackDbContext> dbContext, IdsRedisLock idsRedisLock, ILogger<AutoLightOffJob> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(60))
        {
            _dbContext = dbContext;
            _idsRedis = redis;
            _idsRedisLock = idsRedisLock;
        }

        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            await using (var ctx = await _dbContext.CreateDbContextAsync()) {
                var rack = ctx.Rack.Where(f=>f.Enable==1).AsNoTracking().ToList();
                if (rack == null || rack.Count == 0) return;
                await Parallel.ForEachAsync(
                       rack,
                       new ParallelOptions
                       {
                           MaxDegreeOfParallelism = 8,   // 必须限流，否则可能打爆连接池
                           CancellationToken = stoppingToken
                       },
                       async (rack, token) => {
                           var start = new Stopwatch();
                           try
                           {
                               // 获取货架位置状态
                   
                               start.Start();
                               Logger.Info($"{rack.RackNo} 开始盘点!");
                               byte[] message = DeviceMessage.GetAddrStatusMessage();
                               SmartMaterialRackNode.Instance.NoticeRack(rack.RackNo, message, async session =>
                               {
                                   var result = session.HandlerResult;
                                   if (result == null || !result.Success)
                                       return;

                                   await using (var ctx1 = await _dbContext.CreateDbContextAsync())
                                   {
                                       var obj = (result.Data as List<LocationInfo>)?.ToDictionary(f => f.Addr, f => f.Status);
                                       var dbStatus = ctx1.RackInfo.AsNoTracking().ToList().Select(f => {
                                           return new LocationInfo { Addr = LocationUtils.SmrToDevice(f.Location ?? 0), Status = (byte)((f.Loading == (int)LocationStates.FREE ? 0 : 1)) };
                                       }).ToDictionary(f => f.Addr, f => f.Status);
                                       foreach (var item in obj)
                                       {
                                           if (!dbStatus.ContainsKey(item.Key))
                                           {
                                               Logger.Error($"设备储位号{item.Key + 1}和系统不一致");
                                               await IdsMessageHandler<object>.ErrorMessage($"设备储位号{item.Key + 1}和系统不一致");
                                               continue;
                                           }
                                           if (dbStatus[item.Key] != item.Value)
                                           {
                                               Logger.Error($"设备储位号{item.Key + 1}和系统储位号{item.Key + 1}库存不一致");
                                               await IdsMessageHandler<object>.ErrorMessage($"设备储位号{item.Key + 1}和系统储位号{item.Key + 1}库存不一致");
                                           }
                                       }

                                   }

                               });
                               
                           }
                           catch (Exception ex)
                           {
                               start.Stop();
                               Logger.Error($"{rack.RackNo} 盘点完成!：{ex.Message}");

                           }
                           finally {
                               start.Stop();
                               Logger.Info($"{rack.RackNo} 盘点完成!");
                           }
         
                       });
            }
        }
    }
}
