using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    [DisallowConcurrentExecution]
    public abstract class IdsJob : IJob
    {
        public async Task Execute(IJobExecutionContext context) {
            var jobMap = context.JobDetail?.JobDataMap;
            if (!jobMap.ContainsKey("task"))
            {
                await Execute(context, null);
                return;
            }
            var taskValue = jobMap?.Get("task")?.ToString();
            ScheduleModule scheduleModule = null;
            if (!string.IsNullOrEmpty(taskValue)) { 
                scheduleModule = JsonConvert.DeserializeObject<ScheduleModule>(taskValue);
            }
           await Execute(context, scheduleModule);
        }
        public abstract Task Execute(IJobExecutionContext context, ScheduleModule scheduleJob);
    }
}
