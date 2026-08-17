using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 查询储位状态返回
    /// </summary>
    public class HYLocationStatusQueryHandler : MessageHandler
    {
        public override string ReceiveKey { get; set; } = "0x11";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            throw new NotImplementedException();
        }
    }
}
