using IDS.Common;
using IDS.Device.Communication.ServerEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class ServerConnection : IServerConnection
    {

        private UpdServiceListener _listener;

        public bool IsConnected => throw new NotImplementedException();

        public bool Closed => throw new NotImplementedException();

        public string IP { get; set; }
        public ushort Port { get; set; }

        public ServerConnection(string? ip,ushort port)
        {
            IP = ip;
            Port = port;
            _listener = new UpdServiceListener(IP, Port);
        }

        public ServerConnection(string? ip, ushort port, Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> rev)
        {
            IP = ip;
            Port = port;
            _listener = new UpdServiceListener(IP, Port);
        }
        public UpdServiceListener GetListener() => _listener;
        public IServerConnection SetReceiveEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> rev)
        {
            _listener.OnReceive += new ReceiveEventHandler<string>(rev);
            return this;
        }
        public IServerConnection SetStartErrorEventHandler(Func<bool, IdsResult<string>> startError)
        {
            _listener.OnStartError += new StartErrorEventHandler<string>(startError);
            return this;
        }
        public IServerConnection SetShutdownEventHandler(Func<IdsUdpNode, IdsResult<string>> shutdown)
        {
            _listener.OnShutdown += new ShutdownEventHandler<string>(shutdown);
            return this;
        }
        public IServerConnection SetSendEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> send)
        {
            _listener.OnSend += new SendEventHandler<string>(send);
            return this;
        }
        public IServerConnection SetErrorEventHandler(Func<IdsUdpNode, string, ushort, byte[], IdsResult<string>> error)
        {
            _listener.OnError += new ErrorEventHandler<string>(error);
            return this;
        }
        public IServerConnection SetStartEventHandler(Func<bool, IdsResult<string>> start)
        {
            _listener.OnStart += new StartErrorEventHandler<string>(start);
            return this;
        }
        public IServerConnection SetStopEventHandler(Func<bool, IdsResult<string>> stop)
        {
            _listener.OnStop += new StartErrorEventHandler<string>(stop);
            return this;
        }
        public IServerConnection SetCloseEventHandler(Func<bool, IdsResult<string>> close)
        {
            _listener.OnClose += new StartErrorEventHandler<string>(close);
            return this;
        }
        public IServerConnection SetCompletedEventHandler(Func<bool, IdsResult<string>> completed)
        {
            _listener.OnCompleted += new StartErrorEventHandler<string>(completed);
            return this;
        }
        public IServerConnection SetStartSuccessEventHandler(Func<bool, IdsResult<string>> startSuccess)
        {
            _listener.OnStartSuccess += new StartErrorEventHandler<string>(startSuccess);
            return this;
        }

        public bool Initialize() {
           return  _listener.Start();
        }

        public virtual void Connect(string ip, ushort port, string protocol = "TCP")
        {
            throw new NotImplementedException();
        }
        public virtual void ConnectWithUpd(string ip, ushort port)
        {
            throw new NotImplementedException();
        }

        public virtual void Connect()
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
        public virtual bool Send(byte[] data, IdsEndPoint endpoint, Action<IdsSession> action = null) {
           return _listener.Send(endpoint.Address, endpoint.Port, data, data.Length);
        }

        public virtual Task SendAsync(byte[] data, IdsEndPoint endpoint)
        {
            return Task.Run(()=> _listener.Send(endpoint.Address, endpoint.Port, data, data.Length));
        }

    }
}
