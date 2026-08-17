using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Device.Communication
{
    public class DeviceCommand<E>
    {
        public string DeviceNo { get; set; }
        public byte [] Message { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }
        public IdsEndPoint IPEndPoint { get; set; }
        public string RackNo { get; set; }

        public DateTime ReceiveTime = DateTime.Now;

        public List<int> Locations = new List<int>();
        public string RackSide { get; set; }
        public E Extend { get; set; }
    }
}
