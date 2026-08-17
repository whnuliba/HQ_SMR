using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule.Ioc
{
    using Autofac;
    using Quartz.Spi;
    using Quartz;
    using System.Collections.Specialized;
    using JetBrains.Annotations;

    /// <summary>
    ///     Provides additional configuration to Quartz scheduler.
    /// </summary>
    /// <param name="componentContext"></param>
    /// <returns>Quartz configuration settings.</returns>
    public delegate NameValueCollection QuartzConfigurationProvider(IComponentContext componentContext);

    /// <summary>
    ///     Configures scheduler job scope.
    /// </summary>
    /// <remarks>
    ///     Used to override global container registrations at job scope.
    /// </remarks>
    /// <param name="containerBuilder">Autofac container builder.</param>
    /// <param name="scopeTag">Job scope tag.</param>
    public delegate void QuartzJobScopeConfigurator(ContainerBuilder containerBuilder, object scopeTag);
    [PublicAPI]
    public class CwQuartzAutofacFactoryModule : Module
    {
        /// <summary>
        ///     Default name for nested lifetime scope.
        /// </summary>
        public static readonly string LifetimeScopeName = "quartz.job";

        readonly string _lifetimeScopeTag;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CwQuartzAutofacFactoryModule" /> class with a default lifetime scope
        ///     name.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">lifetimeScopeName</exception>
        public CwQuartzAutofacFactoryModule()
            : this(LifetimeScopeName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CwQuartzAutofacFactoryModule" /> class.
        /// </summary>
        /// <param name="lifetimeScopeTag">Tag of the lifetime scope to wrap job resolution and execution.</param>
        /// <exception cref="System.ArgumentNullException">lifetimeScopeName</exception>
        public CwQuartzAutofacFactoryModule(string lifetimeScopeTag)
        {
            _lifetimeScopeTag = lifetimeScopeTag ?? throw new ArgumentNullException(nameof(lifetimeScopeTag));
        }

        /// <summary>
        ///     Provides custom configuration for Scheduler.
        ///     Returns <see cref="NameValueCollection" /> with custom Quartz settings.
        ///     <para>See http://quartz-scheduler.org/documentation/quartz-2.x/configuration/ for settings description.</para>
        ///     <seealso cref="StdSchedulerFactory" /> for some configuration property names.
        /// </summary>
        public QuartzConfigurationProvider? ConfigurationProvider { get; set; }

        /// <summary>
        ///     Allows to override job scope registrations.
        /// </summary>
        public QuartzJobScopeConfigurator? JobScopeConfigurator { get; set; }

        /// <summary>
        ///     Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        ///     Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">
        ///     The builder through which components can be
        ///     registered.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                    new CwAutofacJobFactory(c.Resolve<ILifetimeScope>(), _lifetimeScopeTag, JobScopeConfigurator))
                .AsSelf()
                .As<IJobFactory>()
                .SingleInstance();

            builder.Register<ISchedulerFactory>(c => {
                var cfgProvider = ConfigurationProvider;

                var autofacSchedulerFactory = cfgProvider != null
                    ? new CwAutofacSchedulerFactory(cfgProvider(c), c.Resolve<CwAutofacJobFactory>())
                    : new CwAutofacSchedulerFactory(c.Resolve<CwAutofacJobFactory>());
                return autofacSchedulerFactory;
            })
                .SingleInstance();

            builder.Register(c => {
                var factory = c.Resolve<ISchedulerFactory>();
                return factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
            })
                .As<IScheduler>()
                .SingleInstance();
        }
    }
}
