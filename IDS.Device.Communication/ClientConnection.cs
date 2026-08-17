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
    public class ClientConnection : IClientConnection
    {

        private UpdClientListener _listener;

        public bool IsConnected => throw new NotImplementedException();

        public bool Closed => throw new NotImplementedException();

        public string IP { get; set; }
        public ushort Port { get; set; }

        public ClientConnection(string? ip,ushort port)
        {
            IP = ip;
            Port = port;
            _listener = new UpdClientListener(IP, Port);
        }

        public ClientConnection(string? ip, ushort port, Func<IdsUpdClient, ushort, byte[], IdsResult<string>> rev)
        {
            IP = ip;
            Port = port;
            _listener = new UpdClientListener(IP, Port);
        }
        public UpdClientListener GetListener() => _listener;

        public IClientConnection SetClientReceiveEvent(Func<IdsUpdClient, byte[], IdsResult<string>> rev)
        {
            _listener.OnReceive += new UdpClientReceiveEventHandler<string>(rev);
            return this;
        }
        public IClientConnection SetClientSendEvent(Func<IdsUpdClient, byte[], IdsResult<string>> send)
        {
            _listener.OnSend += new UdpClientReceiveEventHandler<string>(send);
            return this;
        }
        public IClientConnection SetClientCloseEvent(Func<IdsUpdClient, int, IdsResult<string>> error)
        {
            _listener.OnClose += new UdpClientCloseEventHandler<string>(error);
            return this;
        }
        public IClientConnection SetClientConnectEvent(Func<IdsUpdClient, IdsResult<string>> connect)
        {
            _listener.OnConnect += new ClientConnectEventHandler<string>(connect);
            return this;
        }
        public IClientConnection SetClientStartErrorEvent(Func<bool, IdsResult<string>> error)
        {
            _listener.OnStartError += new StartErrorEventHandler<string>(error);
            return this;
        }
        public IClientConnection SetClientStartSuccessEvent(Func<bool, IdsResult<string>> success)
        {
            _listener.OnStartSuccess += new StartErrorEventHandler<string>(success);
            return this;
        }

        public virtual void Connect(string ip, ushort port, string protocol = "TCP")
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public virtual bool Close(bool forceClose = true)
        {
            throw new NotImplementedException();
        }

        public virtual bool Read(out byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadAsync(out byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual bool Write(byte[] data)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteAsync(byte[] data)
        {
            throw new NotImplementedException();
        }
        public virtual bool Send(byte[] data, IdsEndPoint endpoint) {
           return _listener.Send(data, data.Length);
        }

        public Task SendAsync(byte[] data, IdsEndPoint endpoint)
        {
            return Task.Run(()=> _listener.Send(data, data.Length));
        }
        public virtual UpdClientListener ConnectWithUpd()
        {
            return _listener.Connect();
        }

        public IClientConnection SetClientHandShakeEvent(Func<IdsUpdClient, IdsResult<string>> connect)
        {
            _listener.OnHandShake += new ClientHandShakeEventHandler<string>(connect);
            return this;
        }
    }
}
