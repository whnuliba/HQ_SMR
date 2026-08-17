using IDS.Common;
using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
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

        public virtual IdsResult<object> SendNotice<E>(E data, IdsSession session)
        {
            var alarm = data as RackAlarmInfo;
            var message = DeviceMessage.GetAlarm(alarm.locations, alarm.AlarmMode, alarm.LocationMode, alarm.Side);
            RackNode rack = SmartMaterialRackNode.Instance.GetRackNode(session.ResponseEndPoint.Address);
            if (rack != null) {
                session?.ServerConnection.Send(message, new IdsEndPoint(rack.IP, rack.Port));
                return IdsResult<object>.ok();
            }

            return IdsResult<object>.failure();
        }
    }
}
