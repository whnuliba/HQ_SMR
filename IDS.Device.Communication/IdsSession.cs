using IDS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class IdsSession
    {
        public long SessionId { get; set; }
        public long SessionVersion { get; set; }
        public byte [] SessionKey { get; set; }
        public byte[] RequestData { get; set; }
        public byte[] ResponseData { get; set; }
        public IdsEndPoint RequestEndPoint { get; set; }
        public IdsEndPoint ResponseEndPoint { get; set; }
        public DateTime RequestTime { get; set; } = new DateTime();
        public DateTime ResponseTime { get; set; }
        public long Expires { get; set; } = 30; //过期时间30s
        public IServerConnection ServerConnection { get; set; }
        public string ClientId { get; set; }
        public string ServerId { get; set; }
        public IdsResult<object>? HandlerResult { get; set; }
        public int TimeOutMs { get; set; } = 3000;
        public TaskCompletionSource<IdsResult<object>> taskCompletionSource { get; private set; }
        public CancellationTokenSource cancellationTokenSource { get; private set; }
        public IdsSession(long sessionId,IServerConnection server,byte [] requestData) {
            SessionId = sessionId;
            RequestEndPoint = new IdsEndPoint(server.IP, server.Port);
            RequestData = requestData;
            ServerConnection = server;
            taskCompletionSource = new TaskCompletionSource<IdsResult<object>>();
            cancellationTokenSource = new CancellationTokenSource(TimeOutMs);
        }
        public IdsSession(byte [] sessionId, IServerConnection server, byte[] requestData)
        {
            SessionKey = sessionId;
            RequestEndPoint = new IdsEndPoint(server.IP, server.Port);
            RequestData = requestData;
            ServerConnection = server;
        }
        public static IdsSession CreateSession(long sessionId, IServerConnection server, byte[] requestData)
        {
            return new IdsSession(sessionId, server, requestData);
        }
        public static IdsSession CreateSession(byte [] sessionId, IServerConnection server, byte[] requestData)
        {
            return new IdsSession(sessionId, server, requestData);
        }
        public IdsSession() { }

    }
}
