using IDS.Device.Communication;
using IDS.SMR.Bootstrap;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    public class HYServerConnection: ServerConnection
    {
        public HYServerConnection(string listenIp, ushort listenPort) : base(listenIp, listenPort)
        {
        }
        public HYServerConnection(IdsEndPoint hYEndPoint) : base(hYEndPoint.Address, hYEndPoint.Port) { }
        public override bool Send(byte[] data, IdsEndPoint endpoint)
        {
            //在这里需要注入Session
            bool send =false;
            if (data != null && data.Length > 13 && (send = base.Send(data, endpoint))) {
                //按照HY协议的要求，且报文必须包含ID，发送数据时需要将Session注入到SessionContext中
                if (data != null && data.Length > 13) { }
                byte[] result = new byte[8];
                Array.Copy(data, 3, result, 0, 8);
                long value = BitConverter.ToInt64(result, 0);
                SessionContext.Instance.CreadeSession(value, this, data);
                //后面看情况 是否需要等待监听接收发生，作为服务端理论不需要。考虑到UDP随时都有可能丢包，阻塞等待会造成极大的效率影响
            }
            return send;
        }
    }
}
