using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public interface ITask : IEquatable<ITask>, IEqualityComparer<ITask>
    {
        string? LockName { get; set; }
        string? JobId { get; set; }
        string? JobName { get; set; }
        DateTime LastExecuted { get; set; }
        int MonthDay { get; set; }
        int WeekDay { get; set; }
        TimeSpan AtTime { get; set; }
        TimeSpan Interval { get; set; }
        bool Executing { get; set; }
        public ILog JobLogger { get; set; }
        void Execute();
    }
}
