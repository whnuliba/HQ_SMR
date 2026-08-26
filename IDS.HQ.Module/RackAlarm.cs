using IDS.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Module
{
    public class RackAlarm : IdsBaseEntity
    {
        public string? RackNo { get; set; }
        public string? RackSide { get; set; }
        public string? Location { get; set; }
        public string? TaskId { get; set; }
        public string? PPID { get; set; }
        public int? AlarmType { get; set; }
        public string? Message { get; set; }
        public int? HandleState { get; set; }
    }
}
