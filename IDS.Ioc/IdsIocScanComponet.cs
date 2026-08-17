using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Ioc
{
    public static class IdsIocScanComponet
    {

        public static ContainerBuilder UseScanModules(this ContainerBuilder builder, IConfiguration configuration,string[] files)
        {

            //builder.RegisterModule(new PcsModule(configuration, logger));

           // var files = new string[] { "IDS.Fms.Adapter.dll", "IDS.Fms.Service.dll", "IDS.Formation.Adapter.dll", "IDS.Formation.Service.dll", "IDS.Formation.IPC.dll" };
            var path = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var file in files)
            {
                var fileName = $"{path}{file}";
                if (!System.IO.File.Exists(fileName))
                {
                    //logger.LogWarning($"加载程序集[{fileName}]失败,文件不存在!");
                    continue;
                }
                RegisterAssemblyByAttribute(builder, Assembly.LoadFrom(fileName));
            }
            //var listConfig = configuration.GetSection("Autofac").AsEnumerable().OrderBy(f => f.Key).Select(f => f.Value).ToList();
            //foreach (var item in listConfig)
            //{
            //    if (string.IsNullOrEmpty(item)) continue;
            //    var config = new ConfigurationBuilder();
            //    config.AddJsonFile(item);
            //    try
            //    {
            //        var module = new PcsConfigurationModule(config.Build());
            //        builder.RegisterModule(module);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //    }
            //}
            return builder;
        }

        //public static ILifetimeScope AutofacConfig(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new PcsModule(Configuration));
        //    var list = Configuration.GetSection("autofac").AsEnumerable().OrderBy(f => f.Key).Select(f => f.Value).ToList();
        //    foreach (var item in list)
        //    {
        //        if (string.IsNullOrEmpty(item)) continue;
        //        var config = new ConfigurationBuilder();
        //        config.AddJsonFile(item);
        //        try
        //        {
        //            var module = new PcsConfigurationModule(config.Build());
        //            builder.RegisterModule(module);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex);
        //        }
        //    }
        //    return builder;
        //}


        private static void RegisterAssemblyByAttribute(ContainerBuilder builder, Assembly assembly)
        {
            foreach (var item in assembly.DefinedTypes)
            {
                if (item.IsAbstract || !item.IsPublic) continue;
                var customAttribute = item.GetCustomAttribute<AutoInjectionAttribute>();
                if (customAttribute == null) continue;
                var type = item.AsType();
                builder.RegisterType(type).AsSelf()
                    .AsImplementedInterfaces().ConfigurePreserveExistingDefaults(customAttribute)
                    .AutoActivate()
                    .PropertiesAutowired()
                    .ConfigureIDSLifecycleNamed(customAttribute);
                if (!string.IsNullOrEmpty(customAttribute.Name) && customAttribute.NamedType != null)
                {
                    IdsContainerUtils.InitIocNamed(customAttribute.Name, customAttribute.NamedType);
                }
            }

        }
    }
}
