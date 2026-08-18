using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.Module
{
    public class Rack : IdsLongBaseEntity
    {
        public string RackNo { get; set; }
        public string? RackSide { get; set; }
        public int? Enable { get; set; }
        public int? Inductive { get; set; }
        public int? Port { get; set; }
        public string? IP { get; set; }
        public int? ASideQty { get; set; }
        public int? BSideQty { get; set; }
        public string? LocalIP { set; get; }
        public int? LocalPort { set; get; }
    }
}
