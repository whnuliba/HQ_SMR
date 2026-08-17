using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 查询初始化返回的接收处理器
    /// </summary>
    public class HYInitInfoQueryHandler : MessageHandler
    {
        public override string ReceiveKey { get; set; } = "0x05";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            RackNode rack = SmartMaterialRackNode.Instance.GetRackNode(session.ResponseEndPoint.Address);
            var messageDto = DeviceInitInfoDto.Parse(data, rack.No, session.SessionId.ToString());
            //完成数据解析移除Session
            SessionContext.Instance.RemoveSession(session);
            //TODO 继续处理后续获取到初始状态的结构信息
            return IdsResult<object>.ok();
        }
    }
}
