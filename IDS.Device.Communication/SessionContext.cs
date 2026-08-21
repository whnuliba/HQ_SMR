using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public  class SessionContext: IDisposable
    {
        private static readonly Lazy<SessionContext> _instance = new Lazy<SessionContext>(() => new SessionContext());
        private static readonly ConcurrentDictionary<long,IdsSession> _session = new();
        private Timer _timer;
        public static SessionContext Instance => _instance.Value;

        private SessionContext() {
            //启动一个线程去清楚超时的Session
            _timer =new Timer(CleanSession, null, 2000, 2000);
        }
        private static void CleanSession(object state)
        {
            var now = DateTime.Now;
            foreach (var item in _session)
            {
                if (item.Value.RequestTime.AddSeconds(item.Value.Expires) < now)
                {
                    _session.TryRemove(item.Key, out _);
                }
            }
        }
        public IdsSession CreateSession(long sessionId,IServerConnection connection,byte[] reqData)
        {
            IdsSession session = IdsSession.CreateSession(sessionId,connection, reqData);
            _session.AddOrUpdate(sessionId, session,(k,ov)=> session);
            return session;
        }
        public bool RemoveSession(long sessionId,out IdsSession session) {
            return _session.TryRemove(sessionId, out session);
        }
        public bool RemoveSession(IdsSession session)
        {
            return _session.TryRemove(session.SessionId, out  _);
        }

        public IdsSession GetSession(long sessionId)
        {
            if (_session.TryGetValue(sessionId, out IdsSession session))
            {
                return session;
            }
            return null;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
