using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.DTO
{
    public class InductiveShelfTask
    {
        public string RackNo { get; set; }
        public int Address { get; set; }
        public string Side { get; set; }
        public int Operation { get; set; } //0下架 1 上架
        public int Cancel { get; set; }
    }
}
