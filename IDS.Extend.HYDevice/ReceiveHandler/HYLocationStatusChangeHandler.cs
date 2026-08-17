using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.Handler;
using IDS.HQ.HYDevice.Protocol;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 储位状态变更,感应货架主动反馈,
    ///对于HEX 0x05 0CX 15 就是表示 用户上传需要上架的面号  或者需要下架的的位置号。然后用户在设备上操作相关的按钮。这里若出现报警同步需要反馈给货架
    /// </summary>
    public class HYLocationStatusChangeHandler : MessageHandler
    {
        //public  ILogger<HYLocationStatusChangeHandler> Logger = (ILogger<HYLocationStatusChangeHandler>)ContainerUtils.AutofacServiceProvider.GetService(typeof(Logger));
        public ILog Logger = LogManager.GetLogger(typeof(LogManager));

        public override string ReceiveKey { get; set; } = "0x0F";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session,DeviceCommand<E> command)
        {
            //获取ID
            if (data == null && data.Length < 11)
                return IdsResult<object>.failure();
            byte[] ids = new byte[10];
            Array.Copy(data, 1, ids, 0, 10);
            var message = DeviceMessage.GetMessage(ids, 13, byte.MaxValue, null);
            var connec = session.ServerConnection;
            //根据ID到货架信息表中查询 开启的服务端口
            RackNode rack= SmartMaterialRackNode.Instance.GetRackNode(session.ResponseEndPoint.Address);
            if (rack == null) {
                return   IdsResult<object>.failure("The shelf does not exist");
            }
            IdsEndPoint idsEnd = rack==null? session.ResponseEndPoint:new IdsEndPoint(rack.IP, rack.Port);
            connec?.Send(message,idsEnd);
            //报文解析
            string id = Encoding.ASCII.GetString(ids);
            var inductiveShelf = InductiveShelfInfoDto.Parse(data, rack.No, id);
            CheckOperation(inductiveShelf, rack, session);
            return IdsResult<object>.ok();
        }

        //检测上下货架状态
        public bool CheckOperation(InductiveShelfInfoDto locations,RackNode rackNode, IdsSession session) {
            if (rackNode == null)
            {
                return false;
            }
            List<InductiveShelfTask> inductiveShelves = new List<InductiveShelfTask>(); //后面根据数据库状态来判断
            List<InductiveShelfTask> down = inductiveShelves.Where(c=>c.Operation== (int)OperationState.DOWN && c.RackNo==rackNode.No).ToList();
            List<InductiveShelfTask> up = inductiveShelves.Where(c => c.Operation == (int)OperationState.UP && c.RackNo == rackNode.No).ToList();
            foreach (var item in locations.Locations)
            {

                byte side = item.Addr + 1 > rackNode.AQty ? (byte)1 : (byte)0; //0=>A 1=>B
                string sideStr = side == 0 ? "A" : "B";
                //判断地址的有效性
                bool isOutIndex = item.Addr + 1 > rackNode.AQty + rackNode.BQty;
                if (isOutIndex) {
                    continue;
                }
                //判断是否是非法拿起
                if (up.Count == 0 || down.Count==0) 
                {
                    var alarm = new RackAlarmInfo
                    {
                        Side = side,
                        location = item,
                        AlarmMode=0,
                        LocationMode=1, // 1是发单个 2 是发多个
                        locations = locations?.Locations?.Select(c=>c.Addr).ToList(),
                        ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法拿起或按下!"
                    };
                    Logger.Error(alarm.ErrorInfo);
                    SendNotice<RackAlarmInfo>(alarm, session);
                    continue;
                }
                // Status = 0表示被拿起
                InductiveShelfTask shelfTask_down = down.Where(f => f.RackNo == rackNode.No && f.Address == item.Addr).FirstOrDefault();
                if (item.Status == 0 && down.Count > 0 && down.Where(f =>f.RackNo == rackNode.No && f.Address == item.Addr).Count() == 0) // Status = 0表示被拿起
                {
                    var alarm = new RackAlarmInfo
                    {
                        Side = side,
                        location = item,
                        AlarmMode = 0,
                        LocationMode = 1,// 1是发单个 2 是发多个
                        locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                        ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法拿起!"
                    };
                    Logger.Error(alarm.ErrorInfo);
                    SendNotice<RackAlarmInfo>(alarm, session);
                    continue;
                }
                if (item.Status == 0 && shelfTask_down != null) {
                    down.Remove(shelfTask_down);
                    continue;
                }
                //检查按下上架，这个只判断面，不用判断具体的位置，需要通过位置计算面
                //获取到货架号
                var upA = up.Where(f=>f.Side== sideStr).ToList();
                if (upA.Count == 0) {
                    var alarm = new RackAlarmInfo
                    {
                        Side = side,
                        location = item,
                        AlarmMode = 0,
                        LocationMode = 0,// 1是发单个 2 是发多个
                        locations = locations?.Locations?.Select(c => c.Addr).ToList(),
                        ErrorInfo = $"货架:{rackNode.No};IP:{rackNode.IP};面 {sideStr};储位:{item.Addr} 非法按下!"
                    };
                    Logger.Error(alarm.ErrorInfo);
                    SendNotice<RackAlarmInfo>(alarm, session);
                    continue;
                }
            }
            return true;
        }
    }
}
