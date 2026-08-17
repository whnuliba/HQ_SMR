using HPSocket;
using HPSocket.Base;
using HPSocket.Sdk;
using HPSocket.Udp;
using IDS.Common;
using IDS.Device.Communication.ClientEvent;
using IDS.Device.Communication.ServerEvent;
using log4net.Core;
using System.Net;
namespace IDS.Device.Communication
{
    public class UpdClientListener:IDisposable
    {
        public ushort Port;
        public string Ip;
        private IUdpClient _UdpClient;
        public event UdpClientReceiveEventHandler<string> OnReceive;
        public event UdpClientReceiveEventHandler<string> OnSend;
        public event UdpClientCloseEventHandler<string> OnClose;
        public event StartErrorEventHandler<string> OnStartError;
        public event StartErrorEventHandler<string> OnStartSuccess;
        public event ClientConnectEventHandler<string> OnConnect;
        public event ClientHandShakeEventHandler<string> OnHandShake;
        public UpdClientListener(string cIp, ushort cPort) {
            Port = cPort;
            Ip = cIp;
            CreateClientNode();
        }

        public UpdClientListener CreateClientNode() { 
            IUdpClient udpClient = new UdpClient();
            udpClient.OnReceive += (sender, data) =>
            {
                IdsResult<string> result = OnReceive?.Invoke(new IdsUpdClient(sender), data) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };
            udpClient.OnSend += (sender, data) => {
                IdsResult<string> result = OnSend?.Invoke(new IdsUpdClient(sender), data) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };
            udpClient.OnHandShake += (sender) =>
            {
                IdsResult<string> result = OnHandShake?.Invoke(new IdsUpdClient(sender)) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功,连接已完全就绪，可以安全地开始通信了
                return HandleResult.Error; // 表示事件处理成功,
            };
            udpClient.OnClose += (sender, socketOperation, errorCode) =>
            {
                IdsResult<string> result = OnClose?.Invoke(new IdsUpdClient(sender, socketOperation), errorCode) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };
            udpClient.OnConnect += (sender) =>
            {
                IdsResult<string> result = OnConnect?.Invoke(new IdsUpdClient(sender)) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };

            _UdpClient = udpClient;
            return this;
        }
        public bool Send(byte[] data, int length) {
            if (!_UdpClient.IsConnected)
                Connect();//每次发送时检查，若端口需要重新连接
            return _UdpClient.Send(data, length);
        }
        public UpdClientListener Connect()
        {

            bool success = false;
            _UdpClient.Address = Ip;
            _UdpClient.Port = Port;
            if (_UdpClient != null && (success = _UdpClient.Connect()))
            {
                OnStartSuccess?.Invoke(success);
            }
            else
            {
                OnStartSuccess?.Invoke(success);
            }
            return this;
        }
        public bool Stop() {
            bool success = false;
            if (_UdpClient != null && (success = _UdpClient.Stop()))
            {
                success = true;
            }
            return success;
        }

        public void Dispose()
        {
            _UdpClient.Dispose();
        }
    }
}
