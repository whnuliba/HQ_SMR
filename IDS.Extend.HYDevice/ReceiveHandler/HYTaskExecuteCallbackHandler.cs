using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.DTO;
using IDS.Extend.HYDevice.Handler;
using IDS.HQ.HYDevice.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 用于处理HY设备任务执行回调的接收处理器，CmdType = 0
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class HYTaskExecuteCallbackHandler : MessageHandler
    {
        public override string ReceiveKey   { get; set;} = "0x00";
        public override IdsResult<object> Handle<E>(byte[] data,IdsSession session, DeviceCommand<E> command)
        {
            DeviceInfoDto deviceInfo = new DeviceInfoDto();
            deviceInfo.port = session.ResponseEndPoint.Port + "";
            deviceInfo.Type = "0x00";
            deviceInfo.Id = session.SessionId+""; // 被动执行任务返回一定会有ID
            deviceInfo.RackNo = session.ClientId;
            string str = $"客户端IP{session.RequestEndPoint.Address};客户端端口{session.RequestEndPoint.Port};内容是ID{session.SessionId}";
            Console.WriteLine(str);
            return IdsResult<object>.ok("已经收到反馈信息了");
        }
    }
}
