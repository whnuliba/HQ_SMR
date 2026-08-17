using HPSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class IdsUpdClient
    {
        public IClient? UdpClient { set; get; }
        public SocketOperation? SocketOperation { set; get; }
        public IdsUpdClient(IClient? idpClient) {
            UdpClient = idpClient;
        }
        public IdsUpdClient(IClient? idpClient, SocketOperation? socketOperation)
        {
            UdpClient = idpClient;
            SocketOperation = socketOperation;
        }
    }
}
