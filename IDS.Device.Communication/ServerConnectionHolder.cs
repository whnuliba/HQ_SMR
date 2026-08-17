using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class ServerConnectionHolder
    {
        private static  IServerConnection _asyncLocal;
        public static void SetConnection(IServerConnection connection)
        {
            _asyncLocal = connection;
        }
        public static IServerConnection GetDefaultConnection()
        {
            return _asyncLocal ;
        }
    }
}
