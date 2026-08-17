using IDS.Common;
using IDS.Device.Communication.ServerEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public interface IServerConnection: IDeviceConnection
    {
        void ConnectWithUpd(string ip, ushort port);
        ushort  ServiceName { get=>Port;}
        public UpdServiceListener GetListener();
        public bool Initialize();
        public IServerConnection SetReceiveEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> rev);
        public IServerConnection SetStartErrorEventHandler(Func<bool, IdsResult<string>> startError);
        public IServerConnection SetShutdownEventHandler(Func<IdsUdpNode, IdsResult<string>> shutdown);
        public IServerConnection SetSendEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> send);
        public IServerConnection SetErrorEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> error);
        public IServerConnection SetStartEventHandler(Func<bool, IdsResult<string>> start);
        public IServerConnection SetStopEventHandler(Func<bool, IdsResult<string>> stop);
        public IServerConnection SetCloseEventHandler(Func<bool, IdsResult<string>> close);
        public IServerConnection SetCompletedEventHandler(Func<bool, IdsResult<string>> completed);
        public IServerConnection SetStartSuccessEventHandler(Func<bool, IdsResult<string>> startSuccess);
    }
}
