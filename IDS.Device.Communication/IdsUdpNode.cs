using HPSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class IdsUdpNode
    {
        public IUdpNode? UdpNode { get; set; }
        public IdsUdpNode(IUdpNode? udpNode) {
            UdpNode = udpNode;
        }
    }
}
