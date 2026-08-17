using HPSocket;
using HPSocket.Udp;
using IDS.Common;
using IDS.Device.Communication.ServerEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IDS.Device.Communication
{
    public class UpdServiceListener: IDisposable
    {
        public event StartErrorEventHandler<string> OnMessage;
        public event StartErrorEventHandler<string> OnStart;
        public event StartErrorEventHandler<string> OnStop;
        public event ErrorEventHandler<string> OnError;
        public event StartErrorEventHandler<string> OnStartError;
        public event StartErrorEventHandler<string> OnStartSuccess;
        public event StartErrorEventHandler<string> OnCompleted;
        public event SendEventHandler<string> OnSend;
        public event ReceiveEventHandler<string> OnReceive;
        public event StartErrorEventHandler<string> OnClose;
        public event ShutdownEventHandler<string> OnShutdown;
        private string IP;
        private ushort Port;
        private UdpNode _UdpNode;
        public UpdServiceListener(string listenIp, ushort listenPort) {
            IP = listenIp;
            Port = listenPort;
            CreateServerUpdNode();
        }
        public UpdServiceListener CreateServerUpdNode() { 
           UdpNode node = new UdpNode();
            node.Port = Port;
            node.Address = IP ?? "0.0.0.0";
            node.OnReceive += (sender, remoteAddress, remotePort,pData) => {

               IdsResult<string> result = OnReceive?.Invoke(new IdsUdpNode(sender), remoteAddress, remotePort, pData) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };
            node.OnSend += (sender, remoteAddress, remotePort, pData) => {
                IdsResult<string> result = OnSend?.Invoke(new IdsUdpNode(sender), remoteAddress, remotePort, pData) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error; // 表示事件处理成功
            };
            node.OnError += (sender, socketOperation, errorCode, remoteAddress, remotePort, data) =>
            {
                IdsResult<string> result = OnError?.Invoke(new IdsUdpNode(sender), remoteAddress, remotePort, data) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error;
            };
            node.OnShutdown += (sender) =>
            {
                node.Dispose();
                IdsResult<string> result = OnShutdown?.Invoke(new IdsUdpNode(sender)) ?? IdsResult<string>.ok();
                if (result.Success) return HandleResult.Ok; // 表示事件处理成功
                return HandleResult.Error;
            };     
            _UdpNode = node;
            return this;
        }
        public bool Send(string remoteAddress, ushort remotePort, byte[] data, int length)
        {
            return _UdpNode.Send(remoteAddress, remotePort,data, length);
        }
        public bool Start() {

            if(_UdpNode.HasStarted)
                return true;
            bool success = false;
            if (_UdpNode != null && (success = _UdpNode.Start()))
            {
                OnStartSuccess?.Invoke(success);
            }
            else {
                OnStartSuccess?.Invoke(success);
            }
            return success;
        }
        public void Dispose()
        {
            _UdpNode.Dispose();
        }
    }
}
