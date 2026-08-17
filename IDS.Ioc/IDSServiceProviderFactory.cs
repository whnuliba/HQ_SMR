using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Ioc
{
    public class IDSServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        public IDSServiceProviderFactory() { }
        public  ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            var serviceProviderRegistration = builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().ExternallyOwned();
            builder.RegisterType<IDSServiceScopeFactory>().As<IServiceScopeFactory>();
            foreach (ServiceDescriptor descriptor in services)
            {
                if (descriptor.ImplementationType != null)
                {
                    var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                    if (serviceTypeInfo.IsGenericTypeDefinition)
                    {
                        builder
                            .RegisterGeneric(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .IDSPropertiesAutowired(descriptor)
                            .ConfigureLifecycle(descriptor.Lifetime);
                    }
                    else
                    {
                        builder
                            .RegisterType(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .IDSPropertiesAutowired(descriptor)
                            .ConfigureLifecycle(descriptor.Lifetime);
                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var registration = RegistrationBuilder.ForDelegate(descriptor.ServiceType, (context, parameters) =>
                        {
                            IServiceProvider serviceProvider = null;
                            try {

                                serviceProvider = context.Resolve<IServiceProvider>();
                                return descriptor.ImplementationFactory(serviceProvider);
                            } catch(Exception ex) { 
                            
                            var error = ex.ToString();
                            }
                            return null;
                           // var serviceProvider = context.Resolve<IServiceProvider>();
                          //  return descriptor.ImplementationFactory(serviceProvider);
                        })
                        .ConfigureLifecycle(descriptor.Lifetime)
                        .CreateRegistration();

                    builder.RegisterComponent(registration);
                }
                else
                {
                    builder
                        .RegisterInstance(descriptor.ImplementationInstance)
                        .As(descriptor.ServiceType)
                        .IDSPropertiesAutowired(descriptor)
                        .ConfigureLifecycle(descriptor.Lifetime);
                }
            }
            return builder;
        }


       

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            //containerBuilder.RegisterCallback(PropertyInjector.InjectProperties);

            var container = containerBuilder.Build();
            var provider = new AutofacServiceProvider(container);
            ContainerUtils.CreateContainer(container);
            ContainerUtils.CreateAutofacServiceProvider(provider);
            return provider;
        }
    }
}
