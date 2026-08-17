using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public abstract class BaseJob : ITask
    {
        protected BaseJob()
        {
            LastExecuted = DateTime.Now;
            Interval = TimeSpan.Zero;
            AtTime = TimeSpan.Zero;
            MonthDay = 0;
            WeekDay = -1;
            Executing = false;
        }

        #region IJob 成员

        public string? JobId { get; set; }

        public string? JobName { get; set; }

        public DateTime LastExecuted { get; set; }

        public int MonthDay { get; set; }

        public int WeekDay { get; set; }

        public TimeSpan AtTime { get; set; }

        public TimeSpan Interval { get; set; }

        public bool Executing { get; set; }

        public ILog JobLogger { get; set; }
        public virtual string? LockName { get; set; } = "#";

        public abstract void Execute();

        public bool Equals(ITask x, ITask y)
        {
            return x?.JobId == y?.JobId;
        }

        public int GetHashCode(ITask obj)
        {
            return JobId.GetHashCode();
        }

        public bool Equals(ITask other)
        {
            return other?.JobId == JobId;
        }

        #endregion
    }
}
