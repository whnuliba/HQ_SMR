using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.DTO
{
    public class DeviceInfoDto
    {
        public string DeviceNo { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Type { get ; set; }

        public string Address { get; set; }
        public string port { get; set; }
        public string RackNo { get; set; }
        public bool Success { get { 
              if(string.IsNullOrEmpty(Type) && Type=="0x00") return true; return false;
            }}
    }
}
