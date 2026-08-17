using IDS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public interface IReceiveHandler
    {
        public IServerConnection Connection { get; }
        public string ReceiveKey { get; set; }
        IdsResult<object> Handle<E>(byte[] data,IdsSession session, DeviceCommand<E> command);
        IdsResult<object> SendNotice<E>(E e, IdsSession session);
    }
}
