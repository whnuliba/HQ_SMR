using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.Handler;
using IDS.SMR.Bootstrap;
using log4net;
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
        public ILog Logger = LogManager.GetLogger(typeof(HYServerConnection));

        public HYServerConnection(string listenIp, ushort listenPort) : base(listenIp, listenPort)
        {
        }
        public HYServerConnection(IdsEndPoint hYEndPoint) : base(hYEndPoint.Address, hYEndPoint.Port) { }
        public override bool Send(byte[] data, IdsEndPoint endpoint,Action<IdsSession> action = null)
        {
            //在这里需要注入Session
            bool send =false;
            if (data != null && data.Length > 13 && (send = base.Send(data, endpoint))) {
                //按照HY协议的要求，且报文必须包含ID，发送数据时需要将Session注入到SessionContext中
                if (data != null && data.Length > 13) { }
                byte[] result = new byte[8];
                Array.Copy(data, 3, result, 0, 8);
                long value = BitConverter.ToInt64(result, 0);
                var session = SessionContext.Instance.CreateSession(value, this, data);
                //注册等待事件
                session.cancellationTokenSource.Token.Register(() =>
                {
                    Logger.Error($"Message sent successfully, but no response received from server within {session.TimeOutMs}ms.");
                    session.taskCompletionSource.TrySetCanceled(session.cancellationTokenSource.Token);
                });

                try
                {
                    session.taskCompletionSource.Task.Wait(session.TimeOutMs-1000);
                    var result1 = session.taskCompletionSource.Task.Result;
                    session.HandlerResult = result1;
                }
                catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
                {
                    Logger.Warn("任务已取消");
                }
                if (action != null)
                {
                    action.Invoke(session);
                }
                //后面看情况 是否需要等待监听接收发生，作为服务端理论不需要。考虑到UDP随时都有可能丢包，阻塞等待会造成极大的效率影响
            }
            return send;
        }
    }
}
