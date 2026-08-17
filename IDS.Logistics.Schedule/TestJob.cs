using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public class TestJob : IJob
    {
        public ILog Log = LogManager.GetLogger(typeof(TestJob));

        public Task Execute(IJobExecutionContext context)
        {
            Log.Info(">>>>>>>>>>>>12345678987654321");
            return Task.CompletedTask;
        }
    }
}
