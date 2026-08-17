using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public class HelloJob : IdsJob
    {
        
        public ILog Log = LogManager.GetLogger(typeof(HelloJob));
        public override async Task Execute(IJobExecutionContext context, ScheduleModule scheduleJob)
        {
            Log.Info(">>>>>>>>>>>>12345678987654321");
        }
    }
}
