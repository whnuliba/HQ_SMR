using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public interface IDeviceConnection
    {
        string IP { set; get; }
        ushort Port { set; get; }
        void Connect(string ip, ushort port, string protocol="TCP");
        void Connect();
        bool IsConnected { get; }
        bool Closed { get; }
        bool Close(bool forceClose = true);
        bool Read(out byte[] data);
        bool ReadAsync(out byte[] data);
        bool Write(byte[] data);
        bool Send( byte[] data, IdsEndPoint endpoint);
        bool WriteAsync(byte[] data);
        Task SendAsync(byte[] data, IdsEndPoint endpoint);
    }
}
