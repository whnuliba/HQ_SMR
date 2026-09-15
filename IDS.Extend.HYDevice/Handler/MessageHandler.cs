using IDS.Common;
using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.Handler
{
    public class MessageHandler : IReceiveHandler
    {
        public virtual IServerConnection Connection => throw new NotImplementedException();

        public virtual string ReceiveKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            throw new NotImplementedException();
        }

        public virtual IdsResult<object> SendAlarmNotice<E>(E data, IdsSession session, Action<IdsSession>? action = null)
        {
            //return IdsResult<object>.ok();
            return SmartMaterialRackNode.Instance.SendAlarmNotice<E>(data, session, action);
        }
    }
}
