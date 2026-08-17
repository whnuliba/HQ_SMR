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
    /// 一级板CRC校验错误
    /// </summary>
    public class HYCRCCheckHandler : MessageHandler
    {
        public override string ReceiveKey { get; set; } = "0xF0";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            throw new NotImplementedException();
        }
    }
}
