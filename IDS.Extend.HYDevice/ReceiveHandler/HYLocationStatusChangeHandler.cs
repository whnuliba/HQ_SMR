using IDS.Common;
using IDS.Common.Utils;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.Dispatch;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.Handler;
using IDS.Extend.HYDevice.Utils;
using IDS.Extension;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB;
using LinqToDB.Common;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static LinqToDB.Common.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 储位状态变更,感应货架主动反馈,
    ///对于HEX 0x05 0CX 15 就是表示 用户上传需要上架的面号  或者需要下架的的位置号。然后用户在设备上操作相关的按钮。这里若出现报警同步需要反馈给货架
    /// </summary>
    public class HYLocationStatusChangeHandler : MessageHandler
    {
        //private string _checkPutwayKey = "HQ:HY:PUTWAY:CHECK:"; //料架号
        private string _checkOutboundKey = "HQ:HY:OUTBOUND:TASK:"; //下架任务号
                                                                   // private string _checkOutboundAddrKey = "HQ:HY:OUTBOUND:TASK:ADDR:"; //下架储位号
                                                                   //public  ILogger<HYLocationStatusChangeHandler> Logger = (ILogger<HYLocationStatusChangeHandler>)ContainerUtils.GetService(typeof(Logger));
        public ILog Logger = LogManager.GetLogger(typeof(HYLocationStatusChangeHandler));

        public override string ReceiveKey { get; set; } = "0x0F";
        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            //获取ID
            if (data == null && data.Length < 11)
                return IdsResult<object>.failure();
            byte[] ids = new byte[10];
            Array.Copy(data, 1, ids, 0, 10);
            var message = DeviceMessage.GetMessage(ids, 13, byte.MaxValue, null);
            var connec = session.ServerConnection;
            //根据ID到货架信息表中查询 开启的服务端口
            RackNode rack = SmartMaterialRackNode.Instance.GetRackNode(session.ResponseEndPoint.Address);
            if (rack == null)
            {
                return IdsResult<object>.failure("The shelf does not exist");
            }
            IdsEndPoint idsEnd = rack == null ? session.ResponseEndPoint : new IdsEndPoint(rack.IP, rack.Port);
            // connec?.Send(message, idsEnd);
            //报文解析
            string id = Encoding.ASCII.GetString(ids);
            var inductiveShelf = InductiveShelfInfoDto.Parse(data, rack.No, id);
            var rackNode = new RackNode();
            ObjectExtensions.CopyProperties(rack, rackNode);
            //过滤短期内的相同型号，1秒内的信号
            //SmartMaterialRackNode
            byte side = inductiveShelf.Locations.First().Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
            string sideStr = side == 0 ? "A" : "B";
            string key = $"{rack.No}_{sideStr}_{inductiveShelf.Locations.First().Addr}_{inductiveShelf.Locations.First().Status}";
            if (SmartMaterialRackNode.Instance.HasLastTime(key, 1000))
            { //若1秒内同一个事件触发两次丢弃
                return IdsResult<object>.ok();
            }
            var res = CheckOperation(inductiveShelf, rackNode, session);
            #region 已经在调用方法中发报警了，该处不需要再发送报警
            //if (!res.Success) {
            //    //发送报警信息
            //    //计算面号
            //    int side = inductiveShelf.Locations?[0].Addr??0;
            //    byte[] alarm = DeviceMessage.GetAlarmLight((byte)side, (byte)ALARM.BUZZER,true);
            //    connec?.Send(alarm, idsEnd);
            //}
            #endregion
            return IdsResult<object>.ok();
        }
        //处理上架部分，上架的的PPI只能更具redis来做串行化执行
        public IdsResult<object> CheckUpTaskState(RackNode rackNode, LocationInfo locationInfo)
        {

            IdsRedis RedisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            var taskIdStr = RedisClient.GetDatabase().StringGet(HYConstant.CheckPutwayKey + rackNode.No + ":" + rackNode.RackSide);
            if (string.IsNullOrEmpty(taskIdStr))
            {
                return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务,非法按下");
            }
            RackLocationTaskDto locationTaskDto = JsonConvert.DeserializeObject<RackLocationTaskDto>(taskIdStr);
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext())
            {

                var uptasktask = ctx.Query<RackTask>(f => f.Id == locationTaskDto.TaskId && f.TaskState == (int)TaskStates.UP_WAIT).FirstOrDefault();
                if (uptasktask == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务{locationTaskDto.TaskId}，基于设备特性，每台料架智能有有个上架任务");
                }
                //判断当前货架是否出去载货状态
                var rackinfoload = ctx.Query<RackInfo>(f => f.RackNo == rackNode.No && f.Location == locationInfo.Addr && f.Loading == (int)LocationStates.FREE).FirstOrDefault();
                if (rackinfoload == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}系统记录当前位置处于非空闲状态,可能是载货，PPID:{rackinfoload.PPID}，基于设备特性，每台料架智能有一个上架任务");
                }
                var uptask = ctx.Query<RackTask>(f => f.RackNo == rackNode.No && f.TaskState == (int)TaskStates.UP_WAIT).ToList();
                if (uptask.Count < 1)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务，基于设备特性，每台料架智能有一个上架任务");
                }

                if (uptask.Count > 1)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}当前存在多个等待上架的任务，基于设备特性，每台料架智能有一个上架任务");
                }
            }
            return IdsResult<object>.ok(locationTaskDto);
        }
        public IdsResult<object> ExecutePutway(RackNode rackNode, LocationInfo locationInfo)
        {
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            //需要解除锁定的任务，在redis可以获取
            IdsRedis RedisClient = ContainerUtils.GetRequiredService<IdsRedis>();

            var checkStateRes = CheckUpTaskState(rackNode, locationInfo);
            if (!checkStateRes.Success) return checkStateRes;
            var taskId_ = checkStateRes.Data as RackLocationTaskDto;
            if (taskId_ == null)
            {
                string rackTaskStr = RedisClient.GetDatabase().StringGet(HYConstant.CheckPutwayKey + rackNode.No);
                taskId_ = JsonConvert.DeserializeObject<RackLocationTaskDto>(rackTaskStr);
            }
            if (taskId_ == null)
            {
                return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务，基于设备特性，每台料架智能有一个上架任务");
            }
            string taskId = taskId_.TaskId;
            //判断储位是否可以上架
            int addr = locationInfo.Addr;
            if (!taskId_.Locations.Contains(addr))
            {
                return IdsResult<object>.failure($"设备{rackNode.No}等待上架的任务{taskId}，储位号{addr}，当前任务不允许该储位号上架，可能是下架任务报文丢失人工多次触发，请检查");
            }
            using (var ctx = dbContext.CreateDbContext())
            {
                var uptasktask = ctx.Query<RackTask>(f => f.Id == taskId && f.TaskState == (int)TaskStates.UP_WAIT).FirstOrDefault();
                if (uptasktask == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务{taskId}，基于设备特性，每台料架智能有一个上架任务");
                }
                //更新货位状态及PPID
                var rackinfoload = ctx.Query<RackInfo>(f => f.RackNo == rackNode.No && f.Location == locationInfo.Addr && f.Loading == (int)LocationStates.FREE).FirstOrDefault();

                using (var ts = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        rackinfoload.PPID = uptasktask.PPID;
                        rackinfoload.Loading = (int)LocationStates.LOADING;
                        rackinfoload.updateInit();
                        ctx.RackInfo.Attach(rackinfoload);
                        //ctx.Entry(rackinfoload).State = EntityState.Modified;
                        ctx.Entry(rackinfoload).Property(p => p.LastModifyTime).IsModified = true;
                        ctx.Entry(rackinfoload).Property(p => p.Loading).IsModified = true;
                        ctx.Entry(rackinfoload).Property(p => p.PPID).IsModified = true;
                        int i = ctx.SaveChanges();
                        if (i == 0)
                        {
                            ts.Rollback();
                            return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr} 状态变更失败，请检查");
                        }
                        uptasktask.TaskState = (int)TaskStates.UP_COMPLETE;
                        uptasktask.Location = locationInfo.Addr;
                        uptasktask.LastModifyTime = DateTime.Now;
                        uptasktask.TaskCmd = TaskCmds.Up_end.ToString();
                        //ctx.RackTask.Attach(uptasktask);
                        ctx.Delete<RackTask>(f => f.Id == uptasktask.Id);

                        var taskHis = new RackTaskHis();
                        ObjectExtensions.CopyProperties(uptasktask, taskHis);
                        taskHis.Id = IdUtils.Id + "";
                        taskHis.SourceId = uptasktask.Id;
                        ctx.Insert(taskHis);


                        #region 处理发送给WMS的逻辑
                        var sendWms = new List<LocationInfoChangeData>
                    {
                        new LocationInfoChangeData{
                         OperateType = uptasktask.OperateType,
                         PpId = uptasktask.PPID,
                         LedId = locationInfo.Addr+"",
                         IsLegal = 1+"",
                         RackId = uptasktask.RackNo,
                         Status = 1+"", //状态：1-上架，0-下架
                         TimeStamp = DateTime.UtcNow.ToString(),
                        }
                    };
                        TaskReturnWmsDispatchHandler.Instance.SendLocationInfoChange<LocationInfoChangeData>(sendWms, uptasktask.Id);
                        #endregion
                        //关闭亮灯
                        var locsStr = RedisClient.GetDatabase().StringGet(HYConstant.CheckPutwayKey + rackNode.No + ":" + rackNode.RackSide);
                        if (!string.IsNullOrWhiteSpace(locsStr))
                        {
                            var locs = JsonConvert.DeserializeObject<RackLocationTaskDto>(locsStr);
                            //调用多灯灭的接口
                            var leds = locs.Locations?.Where(f => f != null).Select(f => LocationUtils.SmrToDevice(f ?? 0)).ToList();
                            byte[] message = DeviceMessage.GetMultiLightOffMessage(leds);
                            SmartMaterialRackNode.Instance.NoticeRack(rackNode.No, message);
                        }
                        //清除Redis上的任务
                        RedisClient.GetDatabase().KeyDelete(HYConstant.CheckPutwayKey + rackNode.No + ":" + rackNode.RackSide);
                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        ts.Rollback();
                        throw ex;
                    }
                }
            }
            return IdsResult<object>.ok();
        }
        /// <summary>
        /// 非法下架，同样需要移除料盘
        /// </summary>
        /// <param name="rackNode"></param>
        /// <param name="locationInfo"></param>
        public void IllegalDown(RackNode rackNode, LocationInfo locationInfo) {
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            IdsRedis RedisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            //不在出库队列，需要同货架料盘做移除处理
            using (var ctx = dbContext.CreateDbContext())
            {

                using (var transaction = ctx.Database.BeginTransaction())
                {
                    //移除操作
                    try
                    {
                        byte side = locationInfo.Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                        string sideStr = side == 0 ? "A" : "B";
                        // ---- 1. 用 FOR UPDATE 锁定储位行，同时拿到 PPID ----
                        var rackinfoload = ctx.RackInfo
                            .FromSqlInterpolated($@"SELECT * FROM rackinfo 
                                        WHERE RackNo = {rackNode.No} 
                                        AND Location = {locationInfo.Addr} 
                                        AND Loading = {(int)LocationStates.LOADING} 
                                        FOR UPDATE")
                            .FirstOrDefault();

                        if (rackinfoload == null)
                        {
                            throw new BussinessException($"货架储位没有物料{rackNode.No}:{JsonConvert.SerializeObject(locationInfo)}");
                        }

                        var now = DateTime.Now;

                        // ---- 2. 修改实体并 SaveChanges（参与事务，可回滚）----
                        rackinfoload.LastModifyTime = now;
                        rackinfoload.Loading = (int)LocationStates.FREE;
                        rackinfoload.PPID = "";
                        ctx.SaveChanges();
                        // ---- 3. 归档物料 ----
                        var matl = ctx.Query<MaterialInfo>(f => f.PPID == rackinfoload.PPID).FirstOrDefault();
                        if (matl != null)
                        {
                            var matlHis = new MaterialInfoHis();
                            ObjectExtensions.CopyProperties(matl, matlHis);

                            ctx.Delete<MaterialInfo>(f => f.Id == matl.Id);
                            ctx.Insert(matlHis);
                            ctx.SaveChanges();

                            // 如果需要校验受影响行数，可用 ChangeTracker 或重新查询确认
                        }
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        transaction.Rollback();
                    }

                }
            }
        }

        public IdsResult<object> CheckAndExecDownTask(RackNode rackNode, LocationInfo locationInfo)
        {
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            IdsRedis RedisClient = ContainerUtils.GetRequiredService<IdsRedis>();

            // ---- Redis 读取与校验（事务外）----
            var entry = RedisClient.GetDatabase().HashGetAll(_checkOutboundKey + rackNode.No);
            if (entry == null || entry.Length == 0)
            {
                IllegalDown(rackNode, locationInfo);
                return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr}非法拿起，当前该储位不在出库队列，请检查");
            }

            List<string> ids = new List<string>();
            Dictionary<string?, List<int?>> taskDic = new Dictionary<string?, List<int?>>();
            Dictionary<int, string> locDic = new Dictionary<int, string>();
            List<int> addrCaches = new List<int>();

            foreach (var addr in entry)
            {
                if (addr.Name.HasValue && addr.Name.TryParse(out int _addr) && addr.Value.HasValue && addr.Value.TryParse(out long _id))
                {
                    addrCaches.Add(_addr);
                    ids.Add(addr.Value);
                    if (!taskDic.ContainsKey(addr.Value))
                        taskDic.Add(addr.Value, new List<int?>() { _addr });
                    else
                        taskDic[addr.Value].Add(_addr);

                    locDic.Add(_addr, addr.Value);
                }
            }

            if (!addrCaches.Contains(locationInfo.Addr)) {
                IllegalDown(rackNode, locationInfo);
                return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr}非法拿起，当前该储位不在出库队列，请检查");

            }

            if (!locDic.ContainsKey(locationInfo.Addr)) {
                IllegalDown(rackNode, locationInfo);
                return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr}非法拿起，当前该储位不在出库队列，请检查");
            }
            string taskId = locDic[locationInfo.Addr];

            // 用于事务提交后再执行的非数据库操作
            RackTask uptasktask = null;
            RackInfo rackIinfo = null;
            bool taskCompleted = false;

            using (var ctx = dbContext.CreateDbContext())
            {
                // ---- 事务外查询任务是否存在（只读，无需锁）----
                uptasktask = ctx.Query<RackTask>(f => f.Id == taskId && f.TaskState == (int)TaskStates.DOWN_WAIT).FirstOrDefault();
                if (uptasktask == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待出库的任务{taskId}，Redis和数据库数据不一致");
                }

                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        // ---- 1. 用 FOR UPDATE 锁定储位行，同时拿到 PPID ----
                        var rackinfoload = ctx.RackInfo
                            .FromSqlInterpolated($@"SELECT * FROM rackinfo 
                        WHERE RackNo = {rackNode.No} 
                        AND Location = {locationInfo.Addr} 
                        AND Loading = {(int)LocationStates.LOADING} 
                        FOR UPDATE")
                            .FirstOrDefault();

                        if (rackinfoload == null)
                        {
                            transaction.Rollback();
                            return IdsResult<object>.failure($"设备{rackNode.No}出库的任务{taskId}，未找到可释放的储位记录");
                        }

                        var now = DateTime.Now;

                        // ---- 2. 修改实体并 SaveChanges（参与事务，可回滚）----
                        rackinfoload.LastModifyTime = now;
                        rackinfoload.Loading = (int)LocationStates.FREE;
                        rackinfoload.PPID = "";
                        ctx.SaveChanges();

                        // ---- 3. 归档物料 ----
                        var matl = ctx.Query<MaterialInfo>(f => f.PPID == rackinfoload.PPID).FirstOrDefault();
                        if (matl != null)
                        {
                            var matlHis = new MaterialInfoHis();
                            ObjectExtensions.CopyProperties(matl, matlHis);

                            ctx.Delete<MaterialInfo>(f => f.Id == matl.Id);
                            ctx.Insert(matlHis);
                            ctx.SaveChanges();

                            // 如果需要校验受影响行数，可用 ChangeTracker 或重新查询确认
                        }

                        // ---- 4. 判断是否需要结束任务 ----
                        if (taskDic.ContainsKey(uptasktask.Id) && taskDic[uptasktask.Id].Count == 1
                            && taskDic[uptasktask.Id].First() == locationInfo.Addr)
                        {
                            var taskEntity = ctx.RackTask.FirstOrDefault(r => r.Id == uptasktask.Id && r.TaskState == (int)TaskStates.DOWN_WAIT);
                            if (taskEntity == null)
                            {
                                transaction.Rollback();
                                return IdsResult<object>.failure($"设备{rackNode.No}出库的任务{taskId}，最后一个下架任务出货完成失败");
                            }

                            taskEntity.LastModifyTime = now;
                            taskEntity.TaskState = (int)TaskStates.DOWN_COMPLETE;
                            ctx.SaveChanges();
                            taskCompleted = true;
                        }
                        rackIinfo = ctx.Query<RackInfo>(f => f.RackNo == rackNode.No && f.Location == locationInfo.Addr).FirstOrDefault();
                        //清除刷新的内存
                        //清除redis缓存
                        RedisClient.GetDatabase().HashDelete(_checkOutboundKey + rackNode.No, locationInfo.Addr);
                        #region 处理发送给WMS的下架逻辑
                        var sendWms = new List<LocationInfoChangeData>
                    {
                        new LocationInfoChangeData{
                         OperateType = uptasktask.OperateType,
                         PpId = rackIinfo.PPID,
                         LedId = locationInfo.Addr+"",
                         IsLegal = 1+"",
                         RackId = uptasktask.RackNo,
                         Status = 0+"", //状态：1-上架，0-下架
                         TimeStamp = DateTime.UtcNow.ToString(),
                        }
                    };
                        TaskReturnWmsDispatchHandler.Instance.SendLocationInfoChange<LocationInfoChangeData>(sendWms, uptasktask.Id);
                        #endregion
                        //发送灭灯
                        var led = LocationUtils.SmrToDevice(locationInfo.Addr);
                        byte[] message = DeviceMessage.GetSingleLightOffMessage(led);
                        SmartMaterialRackNode.Instance.NoticeRack(rackNode.No, message);
                        // ---- 5. 事务内只保留数据库操作，先提交 ----
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }

                // ---- 6. 事务提交后再执行非数据库操作 ----
                // 重新查询一次最新状态（事务外，用于后续发送 WMS 消息）
                // 
            }



            return IdsResult<object>.ok();
        }
        //检查储位是否存在报警，若存在报警不做任务动作。直接推送一个报警日志，必须是所有报警都消除后才能取消。
        //消除范围从大到小
        public bool CheckIsExistsAlarm(string rackNo, string side, int? addr)
        {

            return SmartMaterialRackNode.Instance.IsExistsAlarm(rackNo, side, addr);
        }
        public bool CheckLocationIsLegal(RackNode rackNode, InductiveShelfInfoDto locations)
        {
            if (locations?.Locations.Count > 1)
            {
                string msg = $"当前货架{rackNode.No}，储位{JsonConvert.SerializeObject(locations)}是非法的,存在多个并行触发储位，请检查";
                var log = new RackRunningLog
                {
                    RackNo = rackNode.No,
                    RackSide = "",
                    Location = locations?.Locations.First().Addr + "",
                    Message = msg,
                    LogLevel = Utils.LogLevel.Error.ToString(),
                    Class = nameof(HYLocationStatusChangeHandler)
                };
                IdsMessageHandler<object>.ErrorMessage(msg);
                RackRunningLogUtils<RackDbContext>.CreateLog(log);
                return false;
            }
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            int loc = locations.Locations.First().Addr;
            using (var ctx = dbContext.CreateDbContext())
            {
                bool isLegal = true;
                var count = ctx.Count<RackInfo>(f => f.RackNo == rackNode.No && f.Location == locations.Locations.First().Addr);
                if (count == 0)
                    isLegal = false;
                if (!isLegal)
                {
                    string msg = $"当前货架{rackNode.No}，储位{loc}是非法的，系统中不存在该储位或货架，请检查";
                    var log = new RackRunningLog
                    {
                        RackNo = rackNode.No,
                        RackSide = "",
                        Location = loc + "",
                        Message = msg,
                        LogLevel = Utils.LogLevel.Error.ToString(),
                        Class = nameof(HYLocationStatusChangeHandler)
                    };
                    IdsMessageHandler<object>.ErrorMessage(msg);
                    RackRunningLogUtils<RackDbContext>.CreateLog(ctx, log);
                }
                return isLegal;
            }
        }
        //检测上下货架状态
        public IdsResult<object> CheckOperation(InductiveShelfInfoDto locations, RackNode rackNode, IdsSession session)
        {
            if (rackNode == null)
            {
                return IdsResult<object>.failure();
            }
            //检查货架及储位是否存在
            if (!CheckLocationIsLegal(rackNode, locations))
            {
                return IdsResult<object>.failure();
            }
            //处理上架部分，上架的的PPID只能根据redis来做串行化执行
            var upCountList = locations.Locations.Where(f => f.Status == 1).ToList();
            if (upCountList.Count > 1)
            {

                byte side = upCountList.First().Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                string sideStr = side == 0 ? "A" : "B";
                //非法按下，同时间智能处理一个上架任务
                rackNode.RackSide = sideStr;
                //检查当前储位或面是否存在报警
                if (CheckIsExistsAlarm(rackNode.No, sideStr, locations?.Locations[0].Addr))
                {
                    string msg = $"当前货架{rackNode.No}，面{sideStr},储位{locations?.Locations[0]?.Addr}正在发生多储位并行上架报警，请先检查并消除报警";
                    var log = new RackRunningLog
                    {
                        RackNo = rackNode.No,
                        RackSide = sideStr,
                        Location = locations?.Locations[0].Addr + "",
                        Message = msg,
                        LogLevel = Utils.LogLevel.Error.ToString(),
                        Class = nameof(HYLocationStatusChangeHandler)
                    };
                    RackRunningLogUtils<RackDbContext>.CreateLog(log);
                    return IdsResult<object>.failure(msg);
                }
                var alarm = new RackAlarmInfo
                {
                    Side = side,
                    location = upCountList.First(),
                    AlarmMode = 0,
                    LocationMode = 0, // 0是发单个 1 是发多个 2 单面
                    locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                    ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{upCountList.First().Addr} 非法拿起或按下,存在多个存储同时上架!"
                };
                Logger.Error(alarm.ErrorInfo);
                SendAlarmNotice<RackAlarmInfo>(alarm, session);
            }

            if (upCountList.Count == 1)
            {
                var item = upCountList.First();
                byte side = item.Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                string sideStr = side == 0 ? "A" : "B";
                rackNode.RackSide = sideStr;
                //处理上架
                if (item.Status == 1)
                {

                    //检查当前储位或面是否存在报警
                    if (CheckIsExistsAlarm(rackNode.No, sideStr, locations?.Locations[0].Addr))
                    {
                        string msg = $"当前货架{rackNode.No}，面{sideStr},储位{locations?.Locations[0]?.Addr}正在发生上架报警，请先检查并消除报警";
                        var log = new RackRunningLog
                        {
                            Id = IdUtils.Id + "",
                            RackNo = rackNode.No,
                            RackSide = sideStr,
                            Location = locations?.Locations[0].Addr + "",
                            Message = msg,
                            LogLevel = Utils.LogLevel.Error.ToString(),
                            Class = nameof(HYLocationStatusChangeHandler)
                        };
                        RackRunningLogUtils<RackDbContext>.CreateLog(log);
                        return IdsResult<object>.failure(msg);
                    }
                    var res = ExecutePutway(rackNode, upCountList.First());
                    if (!res.Success)
                    {

                        var alarm = new RackAlarmInfo
                        {
                            Side = side,
                            location = upCountList.First(),
                            AlarmMode = 0,
                            LocationMode = 0, // 1是发单个 2 是发多个
                            locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                            ErrorInfo = $"{res.Message};货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法按下 ，原因{res.Message}!"
                        };
                        Logger.Error(alarm.ErrorInfo);
                        SendAlarmNotice<RackAlarmInfo>(alarm, session);
                    }
                }
            }

            var downCountList = locations.Locations.Where(f => f.Status == 0).ToList();

            foreach (var item in downCountList)
            {
                byte side = item.Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                string sideStr = side == 0 ? "A" : "B";
                rackNode.RackSide = sideStr;

                //检查当前储位或面是否存在报警
                if (CheckIsExistsAlarm(rackNode.No, sideStr, item.Addr))
                {
                    string msg = $"当前货架{rackNode.No}，面{sideStr},储位{locations?.Locations[0]?.Addr}正在发生下架报警，请先检查并消除报警";
                    var log = new RackRunningLog
                    {
                        RackNo = rackNode.No,
                        RackSide = sideStr,
                        Location = item.Addr + "",
                        Message = msg,
                        LogLevel = Utils.LogLevel.Error.ToString(),
                        Class = nameof(HYLocationStatusChangeHandler)
                    };
                    RackRunningLogUtils<RackDbContext>.CreateLog(log);
                    //return IdsResult<object>.failure(msg);
                    continue;
                }

                //处理下架
                var res = CheckAndExecDownTask(rackNode, item);
                if (!res.Success)
                {

                    var alarm = new RackAlarmInfo
                    {
                        Side = side,
                        location = item,
                        AlarmMode = 0,//0是报警 1 是解除报警
                        LocationMode = 0,// 0是发单个 1是发单个 2 是发多个
                        locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                        ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法拿起 ，原因{res.Message}!"
                    };
                    Logger.Error(alarm.ErrorInfo);
                    SendAlarmNotice<RackAlarmInfo>(alarm, session);
                    continue;
                }
                continue;
            }
            return IdsResult<object>.ok();
        }
    }
}
