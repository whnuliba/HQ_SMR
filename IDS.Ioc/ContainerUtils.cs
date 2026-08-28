using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Ioc
{
    public class ContainerUtils
    {
        public static IContainer Container { get; private set; }
        public static AutofacServiceProvider AutofacServiceProvider { get; private set; }
        internal static void CreateContainer(IContainer container) {
            Container = container;
        }
        internal static void CreateAutofacServiceProvider(AutofacServiceProvider provider)
        {
            AutofacServiceProvider = provider;
        }
        public static E GetRequiredService<E>()
        {
            return AutofacServiceProvider.GetRequiredService<E>();
        }
        public static E GetService<E>(Type type)
        {
            return (E)AutofacServiceProvider.GetService(type);
        }
        public static E GetService<E>()
        {
            return AutofacServiceProvider.GetService<E>();
        }
    }
}
