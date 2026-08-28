using IDS.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IDS.HQ.Module
{
    public class RackRunningLog : IdsBaseEntity
    {
        public string? RackNo { get; set; }

        public string? RackSide { get; set; }

        public string? Location { get; set; }

        public string? TaskId { get; set; }

        public string? PPID { get; set; }

        public int? AlarmType { get; set; }

        public string? Message { get; set; }

        public string? LogLevel { get; set; }

        public int? HandleState { get; set; }
        public string? Class { get; set; }
    }
}
