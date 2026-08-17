using IDS.Device.Communication;

namespace IDS.Extend.HYDevice
{
    public class HYClientConnection : ClientConnection
    {
        public HYClientConnection(string ip, ushort port) : base(ip, port)
        {
        }
    }
}
