using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public class QuartzConfigure
    {
        public static void Configure()
        {

            var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            var props = new NameValueCollection
                {
                    { "quartz.jobStore.clustered", "true" },
                    { "quartz.jobStore.type", "quartz.impl.adojobstore.jobstoretx, quartz" },
                    // 其他配置...
                };

            //scheduler.SchedulerFactory.Initialize(props);

            //await scheduler.Start();

        }
    }
}
