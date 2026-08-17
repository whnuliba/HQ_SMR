using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class ServiceWrapper
    {
        //注册连接对象
        private Dictionary<string, IDeviceConnection> _clientDictionary = new Dictionary<string, IDeviceConnection>();
    }
}
