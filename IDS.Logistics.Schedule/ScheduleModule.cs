using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public class ScheduleModule
    {
        public string? ScheduleCode { set; get; }

        public string? ScheduleName { set; get; }

        public string? ScheduleGrpCode { set; get; }

        public string? Cron { set; get; }

        public string? JobClass { set; get; }

        public string? ScheduleType { set; get; }

        public string? TriggerState { set; get; }

        public long NextFireTime { set; get; }

        public long PreFireTime { set; get; }

        public long StartTime { set; get; }

        public long EndTime { set; get; }

        public string? NextFireTimeStr { set; get; }

        public string? PreFireTimeStr { set; get; }

        public string? StartTimeStr { set; get; }

        public string? EndTimeStr { set; get; }

        public string? BusinessCode { set; get; }

        public int? Interval { set; get; }
    }
}
