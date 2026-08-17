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
    /// 返回执行结果OK
    /// </summary>
    public class HYResultHandler : MessageHandler
    {
        public override string ReceiveKey { get; set; } = "0xFF";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            throw new NotImplementedException();
        }
    }
}
