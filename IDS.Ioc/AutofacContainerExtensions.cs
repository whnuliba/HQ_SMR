using Autofac;
using Autofac.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Autofac.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IDS.Ioc
{
    public static class WindsorContainerExtensions
    {
        public static ILifetimeScope RegisterTransient<TService, TComponent>(this ILifetimeScope scope)
            where TComponent : TService
            where TService : class
        {
            if (scope.IsRegistered<TService>()) return scope;
            scope.BeginLifetimeScope(builder =>
            {
                builder.RegisterType<TComponent>().AsSelf().As<TService>().InstancePerDependency();
            });
            return scope;
        }

        public static ILifetimeScope RegisterTransient<TService, TComponent>(this ILifetimeScope scope,
            string serviceName)
            where TComponent : TService
            where TService : class
        {

            if (scope.IsRegisteredWithName<TService>(serviceName)) return scope;
            return scope.BeginLifetimeScope(builder =>
           {
               builder.RegisterType<TComponent>().AsSelf().As<TService>().Named<TService>(serviceName).InstancePerDependency();

           });
        }

        internal static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceLifetime lifecycleKind)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Singleton:
                    registrationBuilder.SingleInstance();
                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }

            return registrationBuilder;
        }

        internal static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> IDSPropertiesAutowired<
            TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceDescriptor descriptor)
        {
            if (descriptor?.ServiceType?.GetCustomAttributes(typeof(PropertiesAutowiredAttribute), false)?.Length > 0)
            {
                registrationBuilder.PropertiesAutowired();
            }
            return registrationBuilder;
        }
        public static IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> ConfigurePreserveExistingDefaults<TLimit, TActivatorData, TSingleRegistrationStyle>(
              this IRegistrationBuilder<TLimit, TActivatorData, TSingleRegistrationStyle> registration, AutoInjectionAttribute autoInjection) where TSingleRegistrationStyle : SingleRegistrationStyle
        {
            return autoInjection.PreserveExistingDefaults ? registration.PreserveExistingDefaults() : registration;
        }


        public static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureIDSLifecycleNamed<
            TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            AutoInjectionAttribute autoInjection)
        {
            registrationBuilder = registrationBuilder.ConfigureLifecycle(autoInjection.Lifetime);
            if (string.IsNullOrEmpty(autoInjection.Name)) return registrationBuilder;

            if (autoInjection.NamedType is null)
                throw new ArgumentException(nameof(autoInjection.NamedType));
            return registrationBuilder.Keyed(autoInjection.Name, autoInjection.NamedType);
        }

    }
}
