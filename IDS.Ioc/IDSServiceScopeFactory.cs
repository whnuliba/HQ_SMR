using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace IDS.Ioc
{
    internal class IDSServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ILifetimeScope _lifetimeScope;
        public IDSServiceScopeFactory(ILifetimeScope lifetimeScope)
        {
           
            _lifetimeScope = lifetimeScope;
        }
        public IServiceScope CreateScope()
        {
            return new IDSServiceScope(_lifetimeScope.BeginLifetimeScope());
        }
    }
}
