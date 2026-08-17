using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice;
using IDS.Extend.HYDevice.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.SMR.Bootstrap
{
    public class HYBootstrap : AbstractBoostrap 
    {
        private static object _lock = new object();
        public override IBootstrap RegisterService(IdsEndPoint endPoints)
        {
            IServerConnection serverConnection = new HYServerConnection(endPoints);
            if(!CheckRegister(serverConnection))
                throw new Exception("Port duplication");
            serverConnection.SetReceiveEventHandler((sender, remoteAddress, remotePort, data) =>
            {
                IdsEndPoint endPointsEndPoint = new IdsEndPoint(remoteAddress, remotePort);
                ISMRMessageHandler<string> handler = new SMRMessageHandler<string>();
                return handler.ReceiveHandler(endPointsEndPoint, serverConnection, data);
            });
            lock (_lock) {
                _dictionary.AddOrUpdate(serverConnection.ServiceName, serverConnection, (key, oldValue) => serverConnection);
            }
            return this;
        }
        public override IBootstrap RegisterService(IdsEndPoint endPoints, Func<IdsEndPoint, byte[], IdsResult<string>> handler)
        {
            IServerConnection serverConnection = new HYServerConnection(endPoints);
            if (!CheckRegister(serverConnection))
                throw new Exception("Port duplication");
            serverConnection.SetReceiveEventHandler((sender, remoteAddress, remotePort, data) =>
            {
                IdsEndPoint endPointsEndPoint = new IdsEndPoint(remoteAddress, remotePort);
                return handler(endPointsEndPoint, data);
            });
            lock (_lock)
            {
                _dictionary.AddOrUpdate(serverConnection.ServiceName, serverConnection, (key, oldValue) => serverConnection);
            }
            return this;
        }
        public override IBootstrap RegisterService(List<IdsEndPoint> endPoints, Func<IdsEndPoint, byte[], IdsResult<string>> handler)
        {
            endPoints?.ForEach(endPoint =>
            {
                IServerConnection serverConnection = new HYServerConnection(endPoint);
                if (!CheckRegister(serverConnection))
                    throw new Exception("Port duplication");
                serverConnection.SetReceiveEventHandler((sender, remoteAddress, remotePort, data) =>
                {
                    IdsEndPoint endPointsEndPoint = new IdsEndPoint(remoteAddress, remotePort);
                    return handler(endPointsEndPoint, data);
                });
                lock (_lock)
                {
                    _dictionary.AddOrUpdate(serverConnection.ServiceName, serverConnection, (key, oldValue) => serverConnection);
                }
            });
            return this;
        }
        public override IServerConnection RegisterServiceAndStartup(IdsEndPoint endPoints)
        {
            IServerConnection serverConnection = new HYServerConnection(endPoints);
            if (!CheckRegister(serverConnection))
                throw new Exception("Port duplication");
            serverConnection.SetReceiveEventHandler((sender, remoteAddress, remotePort, data) =>
            {
                IdsEndPoint endPointsEndPoint = new IdsEndPoint(remoteAddress, remotePort);
                ISMRMessageHandler<string> handler = new SMRMessageHandler<string>();
                return handler.ReceiveHandler(endPointsEndPoint, serverConnection, data);
            });
            lock (_lock)
            {
                _dictionary.AddOrUpdate(serverConnection.ServiceName, serverConnection, (key, oldValue) => serverConnection);
            }
            serverConnection.Initialize();
            return serverConnection;
        }
    }
}
