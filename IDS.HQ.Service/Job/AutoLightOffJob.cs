using IDS.Base.Utils;
using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice;
using IDS.Extend.HYDevice.Dispatch;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.ReceiveHandler;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.HQ.Service.Adapter;
using IDS.Persistence;
using log4net;
using log4net.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;


namespace IDS.HQ.Service.Job
{
    //==============================================================
    // 文件名   : AutoLightOffJob.cs
    // 作者     : 100845520
    // 创建时间 : 2026/11/7 20:22:20
    // 版本     : V1.0.0
    // 描述     : 
    //==============================================================
    public class AutoLightOffJob : BaseJob<AutoLightOffJob>
    {
        public ILog Logger = LogManager.GetLogger(typeof(AutoLightOffJob));

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;
        public IdsRedisLock _idsRedisLock;
        public AutoLightOffJob(IDbContextFactory<RackDbContext> dbContext, IdsRedisLock idsRedisLock, ILogger<AutoLightOffJob> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
        {
            _dbContext = dbContext;
            _idsRedis = redis;
            _idsRedisLock = idsRedisLock;
        }
        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            using (var ctx = await _dbContext.CreateDbContextAsync())
            {
                //var query = ctx.Rack
                //   .Where(r => !ctx.RackTask.Any(t => t.RackNo == r.RackNo
                //                              && t.TaskType==(int)TaskTypes.IN
                //                              && t.TaskState == (int)TaskStates.UP_WAIT));

                var query =
                     (from rack in ctx.Rack
                      join task in ctx.RackTask on rack.RackNo equals task.RackNo into g
                      from task in g.Where(t => t.TaskType == (int)TaskTypes.IN
                                             && t.TaskState == (int)TaskStates.UP_WAIT)
                                    .DefaultIfEmpty()
                      where task == null
                      select rack.RackNo).AsNoTracking().ToList();

                string[] sides = { "A", "B" };
                foreach (var side in sides)
                {
                    await Parallel.ForEachAsync(
                        query,
                        new ParallelOptions
                        {
                            MaxDegreeOfParallelism = 8,   // 必须限流，否则可能打爆连接池
                            CancellationToken = stoppingToken
                        },
                        async (rack, token) =>
                        {
                            string value = BaseUtil.uuid();
                            string lockStr = "HQ:COMMON:PUTWAY_TASK_LOCK:" + rack;
                            try
                            {
                                if (_idsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(60)))
                                {
                                    try
                                    {
                                        //清除Redis上的任务
                                        string json = _idsRedis.GetDatabase().StringGet(string.Format( HYConstant.CurrentLightWithUpKey , rack ,side));
                                        if (!string.IsNullOrEmpty(json))
                                        {
                                            RackLocationTaskDto dto = JsonConvert.DeserializeObject<RackLocationTaskDto>(json);
                                            if (dto != null && dto.Locations != null && dto.Locations.Count > 0)
                                            {
                                                if (dto.CreateTime.AddMinutes(3) < DateTime.Now)
                                                {
                                                   // dto.CreateTime = DateTime.Now;
                                                    List<int> leds = dto.Locations.Select(f => LocationUtils.SmrToDevice(f??0)).ToList();
                                                    byte[] message = DeviceMessage.GetMultiLightOffMessage(leds);
                                                    SmartMaterialRackNode.Instance.NoticeRack(rack, message);
                                                    _idsRedis.GetDatabase().KeyDelete(string.Format(HYConstant.CurrentLightWithUpKey, rack, side));
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    finally
                                    {
                                        await _idsRedisLock.UnLock(lockStr, value);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                     });
                }
            }
        }
    }
}
