using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.Dispatch;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.Handler;
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
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        //public  ILogger<HYLocationStatusChangeHandler> Logger = (ILogger<HYLocationStatusChangeHandler>)ContainerUtils.AutofacServiceProvider.GetService(typeof(Logger));
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
            connec?.Send(message, idsEnd);
            //报文解析
            string id = Encoding.ASCII.GetString(ids);
            var inductiveShelf = InductiveShelfInfoDto.Parse(data, rack.No, id);
            var rackNode = new RackNode();
            ObjectExtensions.CopyProperties(rack, rackNode);
            var res =  CheckOperation(inductiveShelf, rackNode, session);
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

            IdsRedis RedisClient = ContainerUtils.AutofacServiceProvider.GetRequiredService<IdsRedis>();
            var taskIdStr = RedisClient.GetDatabase().StringGet(HYConstant.CheckPutwayKey + rackNode.No+":"+rackNode.RackSide);
            if (string.IsNullOrEmpty(taskIdStr))
            {
                return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务,非法按下");
            }
            RackLocationTaskDto locationTaskDto = JsonConvert.DeserializeObject<RackLocationTaskDto>(taskIdStr);
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext())
            {

                var uptasktask = ctx.Query<RackTask>(f => f.Id == locationTaskDto.TaskId && f.TaskState == (int)TaskStates.UP_WAIT).FirstOrDefault();
                if (uptasktask == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务{locationTaskDto.TaskId}，基于设备特性，每台料架智能有有个上架任务");
                }
                //判断当前货架是否出去载货状态
                var rackinfoload = ctx.Query<RackInfo>(f => f.RackNo == rackNode.No && f.Location == locationInfo.Addr && f.Loading == (int)LocationStates.FREE).FirstOrDefault();
                if (rackinfoload ==null ) {
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
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            //需要解除锁定的任务，在redis可以获取
            IdsRedis RedisClient = ContainerUtils.AutofacServiceProvider.GetRequiredService<IdsRedis>();

            var checkStateRes = CheckUpTaskState(rackNode, locationInfo);
            if (!checkStateRes.Success) return checkStateRes;
            var taskId_ = checkStateRes.Data as RackLocationTaskDto;
            if (taskId_==null) {
                string rackTaskStr  = RedisClient.GetDatabase().StringGet(HYConstant.CheckPutwayKey + rackNode.No);
                taskId_ = JsonConvert.DeserializeObject<RackLocationTaskDto>(rackTaskStr);
            }
            if (taskId_ == null) {
                return IdsResult<object>.failure($"设备{rackNode.No}没有等待上架的任务，基于设备特性，每台料架智能有一个上架任务");
            }
            string taskId = taskId_.TaskId;
            //判断储位是否可以上架
            int addr = locationInfo.Addr;
            if (!taskId_.Locations.Contains(addr)) {
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

                using (var ts = new TransactionScope()) {
                    rackinfoload.PPID = uptasktask.PPID;
                    rackinfoload.Loading = (int)LocationStates.LOADING;
                    ctx.RackInfo.Attach(rackinfoload);
                    //ctx.Entry(rackinfoload).State = EntityState.Modified;
                    ctx.Entry(rackinfoload).Property(p => p.LastModifyTime).IsModified = true;
                    ctx.Entry(rackinfoload).Property(p => p.Loading).IsModified = true;
                    int i = ctx.SaveChanges();
                    if (i == 0)
                    {
                        return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr} 状态变更失败，请检查");
                    }
                    uptasktask.TaskState = (int)TaskStates.UP_COMPLETE;
                    uptasktask.Location = locationInfo.Addr;
                    uptasktask.LastModifyTime = DateTime.Now;
                    uptasktask.TaskCmd= TaskCmds.Up_end.ToString();
                    ctx.RackTask.Attach(uptasktask);
                    ctx.Entry(uptasktask).Property(p => p.LastModifyTime).IsModified = true;
                    ctx.Entry(uptasktask).Property(p => p.TaskState).IsModified = true;
                    ctx.Entry(uptasktask).State = EntityState.Modified;
                    i = ctx.SaveChanges();
                    if (i == 0)
                    {
                        return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr} 任务状态变更失败，请检查");
                    }
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

                    //清除Redis上的任务
                    RedisClient.GetDatabase().KeyDelete(HYConstant.CheckPutwayKey + rackNode.No + ":" + rackNode.RackSide);
                    ts.Complete();
                }
            }
            return IdsResult<object>.ok();
        }

        public IdsResult<object> CheckAndExecDownTask(RackNode rackNode, LocationInfo locationInfo) {
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            //需要解除锁定的任务，在redis可以获取
            IdsRedis RedisClient = ContainerUtils.AutofacServiceProvider.GetRequiredService<IdsRedis>();

            //获取所有任务号
            //获取所有的value
            var entry = RedisClient.GetDatabase().HashGetAll(_checkOutboundKey + rackNode.No);
            if (entry == null || entry.Length == 0)
            {
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
                    {
                        taskDic.Add(addr.Value, new List<int?>() { _addr });
                    }
                    else {
                        taskDic[addr.Value].Add(_addr);
                    }

                    locDic.Add(_addr, addr.Value);
                }
            }
            if (!addrCaches.Contains(locationInfo.Addr)) { 
                
                 return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr}非法拿起，当前该储位不在出库队列，请检查");
            }

            var taskRedis = RedisClient.GetDatabase().HashGet(_checkOutboundKey + rackNode.No, locationInfo.Addr);

            if (!locDic.ContainsKey(locationInfo.Addr)) {
                return IdsResult<object>.failure($"货架{rackNode.No}:储位{locationInfo.Addr}非法拿起，当前该储位不在出库队列，请检查");
            }
            string taskId = locDic[locationInfo.Addr];

            //检查当前下架任务是否在库
            using (var ctx = dbContext.CreateDbContext()) {

                var uptasktask = ctx.Query<RackTask>(f => f.Id == taskId && f.TaskState == (int)TaskStates.DOWN_WAIT).FirstOrDefault();
                if (uptasktask == null)
                {
                    return IdsResult<object>.failure($"设备{rackNode.No}没有等待出库的任务{taskId}，Redis和数据库数据不一致");
                }
                using (var ts = new TransactionScope()) {

                    var now = DateTime.Now;
                    int i =  ctx.RackInfo
                        .Where(r => r.RackNo == rackNode.No && r.Location == locationInfo.Addr && r.Loading != (int)LocationStates.FREE)
                        .ExecuteUpdate(setters => setters
                            .SetProperty(r => r.LastModifyTime, now)
                            .SetProperty(r => r.Loading, (int)LocationStates.FREE)
                            .SetProperty(r => r.PPID, "")
                        );

                    if (i == 0) { 
                       return IdsResult<object>.failure($"设备{rackNode.No}出库的任务{taskId}，货架释放储位失败");
                    }

                    var rackIinfo = ctx.Query<RackInfo>(f=>f.RackNo==rackNode.No && f.Location== locationInfo.Addr).FirstOrDefault();
                    addrCaches.Remove(locationInfo.Addr);

                    //判断是都需要结束任务
                    if (taskDic.ContainsKey(uptasktask.Id) && taskDic[uptasktask.Id].Count == 1 && taskDic[uptasktask.Id].First() == locationInfo.Addr)
                    {
                        i = ctx.RackTask
                        .Where(r => r.Id == uptasktask.Id && r.TaskState== (int)TaskStates.DOWN_COMPLETE)
                        .ExecuteUpdate(setters => setters
                            .SetProperty(r => r.LastModifyTime, now)
                            .SetProperty(r => r.TaskState, (int)TaskStates.DOWN_COMPLETE)
                        );

                        if (i == 0)
                        {
                            return IdsResult<object>.failure($"设备{rackNode.No}出库的任务{taskId}，最后一个下架任务出货完成失败");
                        }

                    }
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
                    //清除redis缓存
                    RedisClient.GetDatabase().HashDelete(_checkOutboundKey + rackNode.No, locationInfo.Addr);
                    //清除刷新的内存
                    ts.Complete();
                }
            }
            return IdsResult<object>.ok();
        }
        //检查储位是否存在报警，若存在报警不做任务动作。直接推送一个报警日志，必须是所有报警都消除后才能取消。
        //消除范围从大到小
        public bool CheckIsExistsAlarm(string rackNo,string side,int? addr) {

            return SmartMaterialRackNode.Instance.IsExistsAlarm(rackNo, side, addr);
        }
        //检测上下货架状态
        public IdsResult<object> CheckOperation(InductiveShelfInfoDto locations, RackNode rackNode, IdsSession session)
        {
            if (rackNode == null)
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
                if (CheckIsExistsAlarm(rackNode.No, sideStr, locations?.Locations[0].Addr)) {
                    return IdsResult<object>.failure($"当前货架{rackNode.No}，面{sideStr},储位{locations?.Locations[0]?.Addr}正在发生报警，请先检查并消除报警");
                }
                var alarm = new RackAlarmInfo
                {
                    Side = side,
                    location = upCountList.First(),
                    AlarmMode = 0,
                    LocationMode = 1, // 0是发单个 1 是发多个 2 单面
                    locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                    ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{upCountList.First().Addr} 非法拿起或按下!"
                };
                Logger.Error(alarm.ErrorInfo);
                SendAlarmNotice<RackAlarmInfo>(alarm, session);
            }

            if (upCountList.Count == 1) {
                var item = upCountList.First();
                byte side = item.Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                string sideStr = side == 0 ? "A" : "B";
                rackNode.RackSide = sideStr;
                //处理上架
                if (item.Status == 1)
                {
                    var res = ExecutePutway(rackNode, upCountList.First());
                    if (!res.Success)
                    {

                        var alarm = new RackAlarmInfo
                        {
                            Side = side,
                            location = upCountList.First(),
                            AlarmMode = 0,
                            LocationMode = 1, // 1是发单个 2 是发多个
                            locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                            ErrorInfo = $"{res.Message};货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法按下!"
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
                //处理下架
                var res = CheckAndExecDownTask(rackNode, item);
                if (!res.Success) {

                    var alarm = new RackAlarmInfo
                    {
                        Side = side,
                        location = item,
                        AlarmMode = 0,//0是报警 1 是解除报警
                        LocationMode = 1,// 1是发单个 2 是发多个
                        locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                        ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法拿起!"
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
