using IDS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.Module
{
    public class RackInfo:IdsBaseEntity
    {

        public string? RackNo { get; set; }
        public string? RackSide { get; set; }
        public int? Enable { get; set; }
        public int? Inductive { get; set; }
        public int? Location { get; set; }
        public int? Light { get; set; }
        public int? Loading { get; set; }
        public string? PPID { get; set; }
        public string? IP { get; set; }
        public int? Port { get; set; }
    }
}
