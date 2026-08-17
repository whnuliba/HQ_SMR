using IDS.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public static class ScheduldConfigure
    {
        public static IServiceCollection AddQuartzServer(this IServiceCollection serviceCollection, ConfigurationManager configuration) {
            var quartzSettings = configuration.GetSection("Quartz").Get<QuartzOptions>();
            serviceCollection.AddSingleton(quartzSettings);
            NameValueCollection properties = null;
            if (quartzSettings != null)
            {
                properties = quartzSettings.ToNameValueCollection();
            }
            if (quartzSettings == null || quartzSettings.ToNameValueCollection().Count == 0) {
                properties = new NameValueCollection
                {
                    ["quartz.jobStore.type"] = AppConfig.GetConfigInfo("quartz.jobStore.type") ?? "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                    // "newtonsoft" and "json" are aliases for "Quartz.Simpl.JsonObjectSerializer, Quartz.Serialization.Json"
                    // you should prefer "newtonsoft" as it's more explicit from Quartz 3.10 onwards
                    ["quartz.serializer.type"] = AppConfig.GetConfigInfo("quartz.serializer.type") ?? "newtonsoft",
                    ["quartz.jobStore.misfireThreshold"] = AppConfig.GetConfigInfo("quartz.jobStore.misfireThreshold") ?? "60000",
                    ["quartz.scheduler.instanceName"] = AppConfig.GetConfigInfo("quartz.scheduler.instanceName") ?? "MyClusteredScheduler",
                    ["quartz.scheduler.instanceId"] = AppConfig.GetConfigInfo("quartz.scheduler.instanceId") ?? "AUTO",
                    ["quartz.jobStore.type"] = AppConfig.GetConfigInfo("quartz.jobStore.type") ?? "Quartz.Impl.AdoJobStore.JobStoreTX",
                    ["quartz.jobStore.driverDelegateType"] = AppConfig.GetConfigInfo("quartz.jobStore.driverDelegateType") ?? "Quartz.Impl.AdoJobStore.SqlServerDelegate",
                    ["quartz.jobStore.useProperties"] = AppConfig.GetConfigInfo("quartz.jobStore.useProperties") ?? "true",
                    ["quartz.jobStore.dataSource"] = AppConfig.GetConfigInfo("quartz.jobStore.dataSource") ?? "myDS",
                    ["quartz.jobStore.tablePrefix"] = AppConfig.GetConfigInfo("quartz.jobStore.tablePrefix") ?? "CW_QRTZ_",
                    ["quartz.jobStore.clustered"] = AppConfig.GetConfigInfo("quartz.jobStore.clustered") ?? "true",
                    ["quartz.jobStore.clusterCheckinInterval"] = AppConfig.GetConfigInfo("quartz.jobStore.clusterCheckinInterval") ?? "20000",
                    ["quartz.dataSource.myDS.provider"] = AppConfig.GetConfigInfo($"quartz.dataSource.{AppConfig.GetConfigInfo("quartz.jobStore.dataSource") ?? "myDS"}.provider") ?? "SqlServer",
                    ["quartz.dataSource.myDS.connectionString"] = AppConfig.GetConfigInfo("quartz.dataSource.myDS.connectionString")
                };
            }
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            serviceCollection.AddSingleton(schedulerFactory);
            serviceCollection.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.SchedulerId = AppConfig.GetConfigInfo("Quartz:SchedulerId")?? "MySchedulerId";
                q.SchedulerName = AppConfig.GetConfigInfo("Quartz:SchedulerName")?? "MySchedulerName";
                //q.UsePersistentStore(configure => {
                //    configure.UseSqlServer(configuration.GetConnectionString("QuartzConnectionString"));
                //});
            });
            serviceCollection.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });
            serviceCollection.AddSingleton<IScheduleService, ScheduleService>();


            return serviceCollection;
        }
    }
}
