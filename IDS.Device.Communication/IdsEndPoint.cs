using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class IdsEndPoint
    {
       public string? Address { get; set; }
       public ushort Port { get; set; }
       public string? EndpointName { get; set; }
        public IdsEndPoint(string? address, ushort port, string? endpointName)
        {
            Address = address;
            Port = port;
            EndpointName = endpointName;
        }
        public IdsEndPoint(string? address, ushort port)
        {
            Address = address;
            Port = port;
        }
        public IdsEndPoint()
        {
        }
    }
}
