using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public interface IJobEngine
    {
        bool IsRunning { get; }
        bool AddJob(ITask job);
        bool RemoveJob(string? jobId);
        ITask GetJob(string? id);
        bool ExecuteJob(ITask job);
        bool ExecuteJob(string? jobId);
        void Start();
        void Stop();
    }
}
