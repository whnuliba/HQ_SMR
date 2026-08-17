using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quartz.Logging.OperationName;

namespace IDS.Logistics.Schedule.Ioc
{
    using System.Collections.Specialized;
    using System.Reflection;
    using Autofac;
    using Autofac.Extras.Quartz;
    using IDS.Common;
    using IDS.Ioc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;

    public static class CwQuartzAutofacBootstrap
    {

        public static ContainerBuilder UseScanQuartzJobModules(this ContainerBuilder builder, IConfiguration configuration, string[] files)
        {


            // configure and register Quartz
            var quartzSettings = configuration.GetSection("Quartz").Get<QuartzOptions>();
            NameValueCollection properties = null;
            if (quartzSettings != null)
            {
                properties = quartzSettings.ToNameValueCollection();
            }
            if (quartzSettings == null || quartzSettings.ToNameValueCollection().Count == 0)
            {
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
            builder.RegisterModule(new CwQuartzAutofacFactoryModule
            {
                ConfigurationProvider = c => properties,
                
            });
            var path = AppDomain.CurrentDomain.BaseDirectory;
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                var fileName = $"{path}{file}";
                if (!System.IO.File.Exists(fileName))
                {
                    continue;
                }
                  assemblies.Add(Assembly.LoadFrom(fileName));
            }
            RegisterAssemblyByAttribute(builder, assemblies.ToArray());
            return builder;
        }

        public static IServiceCollection AddQuartzAutoFacServer(this IServiceCollection serviceCollection, ConfigurationManager configuration)
        {
            var quartzSettings = configuration.GetSection("Quartz").Get<QuartzOptions>();
            serviceCollection.AddSingleton(quartzSettings);
            serviceCollection.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.SchedulerId = AppConfig.GetConfigInfo("Quartz:SchedulerId") ?? "MySchedulerId";
                q.SchedulerName = AppConfig.GetConfigInfo("Quartz:SchedulerName") ?? "MySchedulerName";
            });
            serviceCollection.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });
            serviceCollection.AddSingleton<IScheduleService, ScheduleService>();
            return serviceCollection;
        }
        private static void RegisterAssemblyByAttribute(ContainerBuilder builder, params Assembly []  assembly)
        {
            var QuartzAutofacJobsModule = new CwQuartzAutofacJobsModule(assembly)
            {
                AutoWireProperties = true
            };
            builder.RegisterModule(QuartzAutofacJobsModule);
        }
    }
}
