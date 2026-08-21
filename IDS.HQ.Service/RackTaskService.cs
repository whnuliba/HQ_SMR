using Autofac;
using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Transactions;
using ZstdSharp.Unsafe;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackTaskService : DbBaseService<RackTask>, IRackTaskService
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
            if (!string.IsNullOrEmpty(rackTask.RackSide) && "A,B".IndexOf(rackTask.RackSide) < 0) {

                return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}:面号{rackTask.RackSide} 不是A或B");
            }
            lock (obj_lock) {
                string token = RedisClient.GetDatabase().StringGet(_checkPutwayKey + rackTask.RackNo+":"+rackTask.RackSide);
                if (!string.IsNullOrWhiteSpace(token)) {
                    return IdsResult<RackTask>.failure($"01:当前该货架{rackTask.RackNo}有正在上架但未绑定的任务,任务token:{token}");
                }
                using (var ctx = DbContext()) {
                    long id = IdUtils.Id;
                    rackTask.Id = id+"";
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
                            rackTask.TaskType = (int)TaskTypes.IN;
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            //处理亮灯问题。
                            //检查当前位置信息是空的货架
                            var allowLight = from light in ctx.RackInfo
                                             where light.RackNo == rackTask.RackNo
                                             && light.RackSide == rackTask.RackSide
                                             && light.Loading == (int)LocationStates.FREE
                                             select light.Location;
                            //先亮绿灯吧？后续按照需求规格来设置颜色
                            Dictionary<int, byte> dic = allowLight.ToDictionary(k => k??0, v => (byte)Light.G);
                            SmartMaterialRackNode.Instance.NoticeRackMultiLightOn(rackTask.RackNo, dic);
                            RedisClient.GetDatabase().StringSet(_checkPutwayKey + rackTask.RackNo + ":" + rackTask.RackSide, id + "");
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
                rackTask.Id = IdUtils.Id+"";
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
                List<int> light = new List<int>();
                //判断储位号是否在当前的缓存中 同时判断储位号是否已经下发过出库做了
                HashEntry[] hashFields = new HashEntry[stockAddress.Length];
                for (int i = 0; i < stockAddress.Length; i++)
                {
                    var item = stockAddress[i];
                    if (!int.TryParse(item, out int _addr) || addrCaches.Contains(_addr))
                    {
                        return IdsResult<RackTask>.failure($"该储位有正在执行的任务，也有可能下发的储位号不是整数类型,货架:{rackTask.RackNo}:{item}");
                    }
                    light.Add(_addr);
                    hashFields[i] = new HashEntry(item, rackTask.Id);
                }


                using (var ctx = DbContext())
                {

                    using (var ts = new TransactionScope())
                    {
                        try
                        {
                            rackTask.TaskState = (int)TaskStates.DOWN_WAIT;
                            rackTask.TaskType = (int)TaskTypes.OUT;
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            //存入到redis

                            RedisClient.GetDatabase().HashSet(_checkOutboundKey + rackTask.RackNo, hashFields);
                            Dictionary<int, byte>? dic = light?.ToDictionary(k => k, v => (byte)Light.R);
                            //发送亮灯信息
                            SmartMaterialRackNode.Instance.NoticeRackMultiLightOn(rackTask.RackNo, dic);
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
        public IdsResult<RackTask> CancelTask(RackTask rackTask)
        {
            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo))
            {
                return IdsResult<RackTask>.failure("下架信息为空，或者货架号为空");
            }
            if (rackTask.TaskType != (int)TaskTypes.IN && rackTask.TaskType != (int)TaskTypes.OUT) {
                return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}没有指定需要取消的上下架类型");
            }
            if (rackTask.TaskType == (int)TaskTypes.IN) {
                return CancelPutwayTask(rackTask);
            }
            if (rackTask.TaskType == (int)TaskTypes.OUT)
            {
                return CancelOutboundTask(rackTask);
            }
            return IdsResult<RackTask>.ok();
        }

        private IdsResult<RackTask> CancelOutboundTask(RackTask rackTask) {
            if (string.IsNullOrEmpty(rackTask.Locations))
            {
                //判断是否有任务ID

                using (var ctx = DbContext())
                {
                    var task = ctx.RackTask.Where(f => f.Id == rackTask.Id).FirstOrDefault();
                    if(task!=null)
                        rackTask.Locations = task.Locations; ;
                 }
                if(string.IsNullOrEmpty(rackTask.Locations))
                    return IdsResult<RackTask>.failure($"没有下发需要取消的储位号,货架:{rackTask.RackNo}");
            }
            //解析储位号
            string[] stockAddress = rackTask.Locations.Split(",");
            if (stockAddress.Length == 0)
            {
             return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
            }

            //TODO判断若没有指定取消储位的方法,及时缓存的所有任务进行取消
            //该功能待业务确定，到任务级别还是位置级别
            //判断储位号是否在当前的缓存中 同时判断储位号是否已经下发过出库做了
            List<int> ligthAddrs = new List<int>();
            RedisValue[] hashFields = new RedisValue[stockAddress.Length];
            for (int i = 0; i < stockAddress.Length; i++)
            {
                var item = stockAddress[i];
                if (!int.TryParse(item, out int _addr))
                {
                    return IdsResult<RackTask>.failure($"下发的储位号不是整数类型,货架:{rackTask.RackNo}:{item}");
                }
                ligthAddrs.Add(_addr);
                hashFields[i] = new RedisValue(item);
            }
            using (var ctx = DbContext()) {
                using (var ts = new TransactionScope()) {
                    var task = (from rt in ctx.RackTask
                                where rt.RackNo == rackTask.RackNo
                                && rt.RackSide == rackTask.RackSide
                                && rt.TaskState == (int)TaskStates.DOWN_WAIT
                                select rt).FirstOrDefault();
                    if (task != null)
                    {
                        var cancelTask = new RackCancelTask();
                        ObjectExtensions.CopyProperties(task, cancelTask);
                        cancelTask.Id = IdUtils.Id + "";
                        cancelTask.SourceId = task.Id;
                        cancelTask.updateInit();
                        ctx.Insert(cancelTask);
                        ctx.Remove(task);
                        ctx.SaveChanges();
                        SmartMaterialRackNode.Instance.NoticeRackMultiLightOff(rackTask.RackNo, ligthAddrs);
                        ts.Complete();
                    }
                }

                RedisClient.GetDatabase().HashDelete(_checkOutboundKey + rackTask.RackNo, hashFields);
            }
           return IdsResult<RackTask>.ok();
        }

        private IdsResult<RackTask> CancelPutwayTask(RackTask rackTask) {

            if (string.IsNullOrEmpty(rackTask.RackSide))
            {
                return IdsResult<RackTask>.failure($"上传的货架{rackTask.RackNo}信息面号为空");
            }
            if (!string.IsNullOrEmpty(rackTask.RackSide) && "A,B".IndexOf(rackTask.RackSide) < 0)
            {
                return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}:面号{rackTask.RackSide} 不是A或B");
            }
            using (var ctx = DbContext()) {
                using (var ts = new TransactionScope()) {
                    var task = (from rt in ctx.RackTask
                                where rt.RackNo == rackTask.RackNo
                                && rt.RackSide == rackTask.RackSide
                                && rt.TaskState == (int)TaskStates.UP_WAIT
                                select rt).FirstOrDefault();
                    if (task != null)
                    {
                        var cancelTask = new RackCancelTask();
                        ObjectExtensions.CopyProperties(task, cancelTask);
                        cancelTask.Id = IdUtils.Id + "";
                        cancelTask.SourceId = task.Id;
                        cancelTask.updateInit();
                        ctx.Insert(cancelTask);
                        ctx.Remove(task);
                        ctx.SaveChanges();

                        //检查当前位置信息是空的货架
                        List<int?> allowLight = (from light in ctx.RackInfo
                                         where light.RackNo == rackTask.RackNo
                                         && light.RackSide == rackTask.RackSide
                                         && light.Loading == (int)LocationStates.FREE
                                         select light.Location).ToList();
                        List<int> onlight = new List<int>();
                        allowLight.ForEach(item =>
                        {
                            if (item != null)
                                onlight.Add(item??0);
                        });
                        //先亮绿灯吧？后续按照需求规格来设置颜色
                        if (allowLight != null && allowLight.Count > 0) {
                            SmartMaterialRackNode.Instance.NoticeRackMultiLightOff(rackTask.RackNo, onlight);
                        }
                        ts.Complete();
                    }
                }
              
                string token = RedisClient.GetDatabase().StringGet(_checkPutwayKey + rackTask.RackNo + ":" + rackTask.RackSide);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    RedisClient.GetDatabase().KeyDelete(_checkPutwayKey + rackTask.RackNo + ":" + rackTask.RackSide);
                }

                //已经点亮的灯需要熄灭

            }
           return IdsResult<RackTask>.ok();
        }
        public override Page<RackTask> List(Page<RackTask> page, Expression<Func<RackTask, bool>> predicate)
        {

            var upload = page.requestData ?? new RackTask();
            if (!string.IsNullOrWhiteSpace(upload.RackNo))  //托盘编码批量
            {
                var trayNum = upload.RackNo.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.RackNo);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.RackNo));
            }
            if (!string.IsNullOrWhiteSpace(upload.PPID))  //托盘编码批量
            {
                var trayNum = upload.PPID.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.PPID);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.PPID));
            }
            if (upload.TaskType != null) {
                if (predicate == null)
                    predicate = f => f.TaskType == upload.TaskType;
                else
                    predicate = predicate.And(f => f.TaskType == upload.TaskType);
            }
            if (upload.TaskState != null) {

                if (predicate == null)
                    predicate = f => f.TaskState == upload.TaskState;
                else
                    predicate = predicate.And(f => f.TaskState == upload.TaskState);
            }
            return base.List(page, predicate);

        }

        public IdsResult<RackTask> ForceCompleteTask(RackTask rackTask)
        {
            //前置完成任务必须输入任务ID和RACK_NO

            //任务强制完成不对系统做任务处理。只是对系统内部数据变更

            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo))
            {
                return IdsResult<RackTask>.failure("下架信息为空，或者货架号为空");
            }
            if (rackTask.TaskType != (int)TaskTypes.IN && rackTask.TaskType != (int)TaskTypes.OUT)
            {
                return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}没有指定需要取消的上下架类型");
            }
            if (rackTask.TaskType == (int)TaskTypes.IN)
            {
                return ForceCompleteInTask(rackTask);
            }
            if (rackTask.TaskType == (int)TaskTypes.OUT)
            {
                return ForceCompleteOutboundTask(rackTask);
            }
            return IdsResult<RackTask>.ok();
        }

        private IdsResult<RackTask> ForceCompleteOutboundTask(RackTask rackTask) {

            //当前所有待出库的储位号都在这里
            Dictionary<string, List<int>> locDic = new Dictionary<string, List<int>>();
            //完成后需要删除的储位ID
            List<int?> completeDoc = new List<int?>();
            if (string.IsNullOrEmpty(rackTask.Locations))
            {
                //判断是否有任务ID
                using (var ctx = DbContext())
                {
                    RackTask task= ctx.RackTask.Where(f => f.Id == rackTask.Id).FirstOrDefault();
                    if (task != null)
                        rackTask.Locations = task.Locations; ;
                }
                if (string.IsNullOrEmpty(rackTask.Locations))
                    return IdsResult<RackTask>.failure($"没有下发需要取消的储位号,货架:{rackTask.RackNo}");
                //获取redis的所有KV进行比对
               var entries =  RedisClient.GetDatabase().HashGetAll(_checkOutboundKey + rackTask.RackNo);
                if (entries!=null && entries.Length > 0) {
                    foreach (var item in entries)
                    {
                        if (!item.Name.TryParse(out int addr))
                            continue;
                        if (locDic.ContainsKey(item.Value))
                        {
                           
                            locDic[item.Value].Add(addr);
                        }
                        else {
                            locDic.Add(item.Value, new List<int>() { addr });
                        }
                        completeDoc.Add(addr);
                    }
                }
            }
            //解析储位号
            string[] stockAddress = rackTask.Locations.Split(",");
            if (stockAddress.Length == 0)
            {
                return IdsResult<RackTask>.failure($"没有下发需要下架的储位号,货架:{rackTask.RackNo}");
            }
            //TODO判断若没有指定取消储位的方法,及时缓存的所有任务进行取消
            //该功能待业务确定，到任务级别还是位置级别
            //判断储位号是否在当前的缓存中 同时判断储位号是否已经下发过出库做了
            List<int> ligthAddrs = new List<int>();
            RedisValue[] hashFields = new RedisValue[stockAddress.Length];
            for (int i = 0; i < completeDoc.Count; i++)
            {
                hashFields[i] = new RedisValue(completeDoc[i]+"");
            }
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    var task = (from rt in ctx.RackTask
                                where rt.RackNo == rackTask.RackNo
                                && rt.RackSide == rackTask.RackSide
                                && rt.TaskState == (int)TaskStates.DOWN_WAIT
                                select rt).FirstOrDefault();
                    if (task != null)
                    {

                        //完成任务
                        task.updateInit();
                        task.updateInit();
                        task.TaskState = (int)TaskStates.DOWN_COMPLETE;
                        ctx.Entry(task).Property(p => p.LastModifyTime).IsModified = true;
                        ctx.Entry(task).Property(p => p.LastModifyUser).IsModified = true;
                        ctx.Entry(task).Property(p => p.TaskState).IsModified = true;
                        ctx.SaveChanges();
                        int i = ctx.RackInfo
                            .Where(r => r.RackNo == task.RackNo && completeDoc.Contains(r.Location))
                            .ExecuteUpdate(setters => setters
                                .SetProperty(r => r.LastModifyTime, DateTime.Now)
                                .SetProperty(r => r.Loading, (int)LocationStates.FREE)
                                   .SetProperty(r => r.PPID, string.Empty)
                            );
                        RedisClient.GetDatabase().HashDelete(_checkOutboundKey + rackTask.RackNo, hashFields);
                        ts.Complete();
                    }
                }
            }
            return IdsResult<RackTask>.ok();

        }
        private IdsResult<RackTask> ForceCompleteInTask(RackTask rackTask) {
            //完成入库任务.
            using (var ctx = DbContext()) {
                if (string.IsNullOrEmpty(rackTask.RackNo) || string.IsNullOrEmpty(rackTask.RackSide) || string.IsNullOrEmpty(rackTask.Id)) {
                    return IdsResult<RackTask>.failure("强制完成任务必须执行货架位置");
                }
                if (rackTask.Location == null) {
                    return IdsResult<RackTask>.failure("强制完成任务必须执行货架位置");
                }
               var task = ctx.RackTask.Where(f=>f.RackNo==rackTask.RackNo&&f.RackSide==rackTask.RackSide && rackTask.TaskState==(int)TaskStates.UP_WAIT).FirstOrDefault();
                if (task == null && !string.IsNullOrEmpty(rackTask.Id)) {
                    task = ctx.RackTask.Where(f => f.Id == rackTask.Id && f.RackSide == rackTask.RackSide && rackTask.TaskState == (int)TaskStates.UP_WAIT).FirstOrDefault();
                }
                if (task == null) {
                    return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}-{rackTask.Id}等待入库的任务不存在，或任务已经完成" );
                }
                //检查当前位置是否空闲
                bool isAlowComplete = false;
                var rackinfo = ctx.RackInfo.Where(f => f.RackNo == rackTask.RackNo && f.Location == rackTask.Location).FirstOrDefault();
                if (rackinfo == null)
                {
                    return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}-{rackTask.Id}-位置{rackTask.Location}在数据库中查无记录");
                }
                if (rackinfo != null && rackinfo.Loading==(int)LocationStates.LOADING  && task.PPID.Equals(rackinfo.PPID)) {
                    isAlowComplete = true;
                }
                if (rackinfo != null && rackinfo.Loading == (int)LocationStates.LOADING && !task.PPID.Equals(rackinfo.PPID)) {
                    return IdsResult<RackTask>.failure($"货架{rackTask.RackNo}-{rackTask.Id}-位置{rackTask.Location}是载货状态及PPID不一致，不可强制");
                }
                using (var ts = new TransactionScope()) {
                    //修改货架指定的任务号
                    task.updateInit();
                    task.TaskState = (int)TaskStates.UP_COMPLETE;
                     ctx.Entry(task).Property(p => p.LastModifyTime).IsModified = true;
                     ctx.Entry(task).Property(p => p.LastModifyUser).IsModified = true;
                     ctx.Entry(task).Property(p => p.TaskState).IsModified = true;
                     ctx.SaveChanges();
                    if (isAlowComplete && rackinfo.Loading == (int)LocationStates.FREE) {
                        rackinfo.Loading = (int)LocationStates.FREE;
                        rackinfo.PPID = task.PPID;
                        ctx.Entry(rackinfo).Property(p => p.LastModifyTime).IsModified = true;
                        ctx.Entry(rackinfo).Property(p => p.LastModifyUser).IsModified = true;
                        ctx.Entry(rackinfo).Property(p => p.Loading).IsModified = true;
                        ctx.Entry(rackinfo).Property(p => p.PPID).IsModified = true;
                        ctx.SaveChanges();
                    }
                    RedisClient.GetDatabase().KeyDelete(_checkPutwayKey + rackTask.RackNo + ":" + rackTask.RackSide);
                    ts.Complete();
                }

                return IdsResult<RackTask>.ok();
            }

        }
    }
}
