using IDS.Common;
using IDS.Device.Communication.ClientEvent;
using IDS.Device.Communication.ServerEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public interface IClientConnection : IDeviceConnection
    {
        public UpdClientListener ConnectWithUpd();
        string  ClientName { get=>string.IsNullOrWhiteSpace(IP)?$"{IP}":"LOCALHOST"+":"+Port;  }
        public UpdClientListener GetListener();
        public IClientConnection SetClientReceiveEvent(Func<IdsUpdClient, byte[], IdsResult<string>> rev);
        public IClientConnection SetClientSendEvent(Func<IdsUpdClient, byte[], IdsResult<string>> send);
        public IClientConnection SetClientCloseEvent(Func<IdsUpdClient, int, IdsResult<string>> error);
        public IClientConnection SetClientConnectEvent(Func<IdsUpdClient, IdsResult<string>> connect);
        public IClientConnection SetClientStartErrorEvent(Func<bool, IdsResult<string>> error);
        public IClientConnection SetClientStartSuccessEvent(Func<bool, IdsResult<string>> success);
        public IClientConnection SetClientHandShakeEvent(Func<IdsUpdClient, IdsResult<string>> connect);

    }
}
