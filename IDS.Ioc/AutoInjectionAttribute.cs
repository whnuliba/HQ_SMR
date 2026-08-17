using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace   IDS.Ioc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInjectionAttribute : Attribute
    {
        public AutoInjectionAttribute(bool preserveExistingDefaults = true)
        {

        }

        public AutoInjectionAttribute(ServiceLifetime serviceLifetime, bool preserveExistingDefaults = true)
        {
            Lifetime = serviceLifetime;
        }

        public AutoInjectionAttribute(ServiceLifetime serviceLifetime, string name, Type type, bool preserveExistingDefaults = true)
        {
            Lifetime = serviceLifetime;
            Name = name;
            NamedType = type;
        }
        public AutoInjectionAttribute(string name, Type type)
        {
            Name = name;
            NamedType = type;
        }
        public bool PreserveExistingDefaults { get; init; }
        public ServiceLifetime Lifetime { get; init; } = ServiceLifetime.Singleton;
        public string Name { get; init; }
        public Type NamedType { get; init; }

    }
}
