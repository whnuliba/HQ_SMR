using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Module.DTO
{
    public class RegisterRackInfoDto
    {
        public string RackNo { set; get; }
        public string ASide { set; get; }
        public int ASideCount { set; get; }
        public int ASideStartIndex { set; get; }
        public string BSide { set; get; }
        public int BSideCount { set; get; }
        public int BSideStartIndex { set; get; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
}
