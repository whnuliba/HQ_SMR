using IDS.Common;
using IDS.Device.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.Handler
{
    public interface ISMRMessageHandler<E>
    {
        IdsResult<E> ReceiveHandler(IdsEndPoint endPoint,IServerConnection serverConnection,byte[] message);
    }
}
