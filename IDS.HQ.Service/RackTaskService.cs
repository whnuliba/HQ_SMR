using Autofac;
using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Transactions;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackTaskService : DbLongBaseService<RackTask>, IRackTaskService
    {
        public object obj_lock = new object();
        public object obj_lock_out = new object();

        public IdsRedis RedisClient { get; set; }
        private string _checkPutwayKey = "HQ:HY:PUTWAY:CHECK:"; //料架号
        private string _checkOutboundKey = "HQ:HY:OUTBOUND:TASK:"; //料架号
        private string _checkOutboundAddrKey = "HQ:HY:OUTBOUND:TASK:ADDR:"; //料架号
        public  IDbContextFactory<RackDbContext> DbContextFactory { get; set; }
        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
        public IdsResult<RackTask> Putway(RackTask rackTask)
        {
            //做两个操作，1是确认当前是否已经完成绑定
            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo)) {
                return IdsResult<RackTask>.failure("上传的货架信息为空，或者货架号为空");
            }
            if (string.IsNullOrEmpty(rackTask.RackSide))
            {
                return IdsResult<RackTask>.failure($"上传的货架{rackTask.RackNo}信息面号为空");
            }
            if (!string.IsNullOrEmpty(rackTask.RackSide) && rackTask.RackSide.IndexOf("A,B") < 0) {

                return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}:面号{rackTask.RackSide} 不是A或B");
            }
            lock (obj_lock) {
                string token = RedisClient.GetDatabase().StringGet(_checkPutwayKey + rackTask.RackNo+":"+rackTask.RackSide);
                if (!string.IsNullOrWhiteSpace(token)) {
                    return IdsResult<RackTask>.failure($"01:当前该货架{rackTask.RackNo}有正在上架但未绑定的任务,任务token:{token}");
                }
                using (var ctx = DbContext()) {
                    long id = IdUtils.Id;
                    rackTask.Id = id;
                    //检查是否还有未完成的任务
                    var task = ctx.RackTask.Where(f => f.RackNo == rackTask.RackNo && f.RackSide == rackTask.RackSide && f.TaskState == (int)TaskStates.UP_WAIT).FirstOrDefault();
                    if (task != null) {
                        return IdsResult<RackTask>.failure($"02:当前该货架{rackTask.RackNo}有正在上架但未绑定的任务,任务token:{task.Id},但缓存已经完成，可能是人为调整数据库导致，请确认，并在系统上完成异常处理");
                    }
                    using (var ts = new TransactionScope())
                    {
                        try
                        {
                            rackTask.TaskState = (int)TaskStates.UP_WAIT;
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            RedisClient.GetDatabase().StringSet(_checkPutwayKey + rackTask.RackNo+":" + rackTask.RackSide, id + "");
                            ts.Complete();
                        }
                        catch (Exception ex) { 
                          return IdsResult<RackTask>.failure(ex.Message);
                        }
                  
                    }
                }

                //处理任务创建
                return IdsResult<RackTask>.ok(rackTask);
            }
        }

        public IdsResult<RackTask> Outbound(RackTask rackTask)
        {
            //处理出库需要检查

            //做两个操作，1是确认当前是否已经完成绑定
            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo))
            {
                return IdsResult<RackTask>.failure("下架信息为空，或者货架号为空");
            }
            if (string.IsNullOrEmpty(rackTask.Locations))
            {
                return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
            }
            lock (obj_lock_out)
            {
                rackTask.Id = IdUtils.Id;
                RedisValue[] addresses =  RedisClient.GetDatabase().HashKeys(_checkOutboundKey + rackTask.RackNo);
                List<int> addrCaches = new List<int>();
                foreach (var addr in addresses)
                {
                    if (addr.HasValue && addr.TryParse(out int _addr))
                    {
                        addrCaches.Add(_addr);
                    }
                }
                //解析储位号
                string[] stockAddress = rackTask.Locations.Split(",");
                if (stockAddress.Length == 0)
                {
                    return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
                }
                //判断储位号是否在当前的缓存中 同时判断储位号是否已经下发过出库做了
                HashEntry[] hashFields = new HashEntry[stockAddress.Length];
                for (int i = 0; i < stockAddress.Length; i++)
                {
                    var item = stockAddress[i];
                    if (!int.TryParse(item, out int _addr) || addrCaches.Contains(_addr))
                    {
                        return IdsResult<RackTask>.failure($"该储位有正在执行的任务，也有可能下发的储位号不是整数类型,货架:{rackTask.RackNo}:{item}");
                    }
                    hashFields[i] = new HashEntry(item, rackTask.Id);
                }


                using (var ctx = DbContext())
                {

                    using (var ts = new TransactionScope())
                    {
                        try
                        {
                            rackTask.TaskState = (int)TaskStates.DOWN_WAIT;
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            //存入到redis

                            RedisClient.GetDatabase().HashSet(_checkOutboundKey + rackTask.RackNo, hashFields);
                            ts.Complete();
                        }
                        catch (Exception ex)
                        {
                            return IdsResult<RackTask>.failure(ex.Message);
                        }

                    }
                }
                //处理任务创建
                return IdsResult<RackTask>.ok(rackTask);
            }
        }
        [Obsolete]
        public IdsResult<RackTask> Outbound1(RackTask rackTask)
        {
            //处理出库需要检查

            //做两个操作，1是确认当前是否已经完成绑定
            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo))
            {
                return IdsResult<RackTask>.failure("下架信息为空，或者货架号为空");
            }
            if (string.IsNullOrEmpty(rackTask.Locations)) {
                return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
            }
            lock (obj_lock_out)
            {

                //产生任务号
                List<long> taskIds = new List<long>();
                List<int> addrCaches = new List<int>();
                RedisValue[] tasks =  RedisClient.GetDatabase().SetMembers(_checkOutboundKey + rackTask.RackNo);
                if (tasks != null && tasks.Length>0) {
                    foreach (var task in tasks) {
                        if (task.HasValue && task.TryParse(out long _id)) {
                            taskIds.Add(_id);
                        }
                    }
                    taskIds.Sort();
                }
                if (taskIds.Count > 0) {
                    taskIds.ForEach(task =>
                    {
                        RedisValue[] addrs = RedisClient.GetDatabase().SetMembers(_checkOutboundAddrKey + rackTask.RackNo + ":" + task);
                        if (addrs != null && addrs.Length > 0)
                        {
                            foreach (var addr in addrs)
                            {
                                if (addr.HasValue && addr.TryParse(out int _addr))
                                {
                                    addrCaches.Add(_addr);
                                }
                            }
                        }
                    });
                }
                //解析储位号
                string[] stockAddress = rackTask.Locations.Split(",");
                if (stockAddress.Length == 0) {
                    return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
                }
                //判断储位号是否在当前的缓存中 同时判断储位号是否已经下发过出库做了
                RedisValue[] redisValues = new RedisValue[stockAddress.Length];
                for(int i = 0; i < stockAddress.Length; i++)
                {
                    var item = stockAddress[i];
                    if (!int.TryParse(item,out int _addr) || !addrCaches.Contains(_addr)) {
                        return IdsResult<RackTask>.failure($"该储位有正在执行的任务，也有可能下发的储位号不是整数类型,货架:{rackTask.RackNo}:{item}");
                    }
                    redisValues[i] = item;
                }
            

                using (var ctx = DbContext())
                {

                    using (var ts = new TransactionScope())
                    {
                        try
                        {
                            rackTask.TaskState = (int)TaskStates.DOWN_WAIT;
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            //存入到redis
                            RedisClient.GetDatabase().SetAdd(_checkOutboundAddrKey + rackTask.RackNo + ":" + rackTask.Id, redisValues);
                            RedisClient.GetDatabase().SetAdd(_checkOutboundKey + rackTask.RackNo, rackTask.Id);
                            ts.Complete();
                        }
                        catch (Exception ex)
                        {
                            return IdsResult<RackTask>.failure(ex.Message);
                        }

                    }
                }
                //处理任务创建
                return IdsResult<RackTask>.ok(rackTask);
            }
        }
    }
}
