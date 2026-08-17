using IDS.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public abstract class AbstractBoostrap : IBootstrap 

    {
        protected static ConcurrentDictionary<ushort, IServerConnection> _dictionary = new ConcurrentDictionary<ushort, IServerConnection>();
        protected static ConcurrentDictionary<ushort, bool> _bootstrapStates = new ConcurrentDictionary<ushort, bool>();

        public bool CheckRegister(ushort severName, IServerConnection connection)
        {
            if (!_dictionary.ContainsKey(severName))
            {
                return true;
            }
            if (_dictionary.Values.Where(v => v.Equals(connection)).Count()==0){
                return true;
            }
            return false;
        }
        public bool CheckRegister(ushort severName)
        {
            if (!_dictionary.ContainsKey(severName))
            {
                return true;
            }
            return false;
        }
        public bool CheckRegister(IServerConnection connection)
        {
            if (connection!=null && !_dictionary.ContainsKey(connection.ServiceName))
            {
                return true;
            }
            return false;
        }

        public virtual IBootstrap RegisterService(List<IdsEndPoint> endPoints, Func<IdsEndPoint, byte[], IdsResult<string>> handler)
        {
            throw new NotImplementedException();
        }

        public virtual IBootstrap RegisterService(IdsEndPoint endPoints, Func<IdsEndPoint, byte[], IdsResult<string>> handler)
        {
            throw new NotImplementedException();
        }

        public virtual IBootstrap RegisterService(IdsEndPoint endPoints)
        {
            throw new NotImplementedException();
        }
        public virtual IServerConnection GetService(ushort severName)
        {
            if (severName>0 && _dictionary.TryGetValue(severName, out IServerConnection server)) { 
               return server;
            }
            return  null;
        }

        public virtual void StartAll()
        {
            if(_dictionary.Count==0)
                throw new Exception("No available services to start");
            foreach (var s in _dictionary) {
                bool started = s.Value.Initialize();
                _bootstrapStates.AddOrUpdate(s.Key, started, (key, oldValue) => started);
            }
        }

        public virtual IServerConnection RegisterServiceAndStartup(IdsEndPoint endPoints)
        {
            throw new NotImplementedException();
        }
    }
}
