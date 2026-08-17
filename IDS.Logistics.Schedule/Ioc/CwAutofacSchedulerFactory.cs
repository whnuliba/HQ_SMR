using Quartz.Core;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace IDS.Logistics.Schedule.Ioc
{
    [PublicAPI]
    public class CwAutofacSchedulerFactory : StdSchedulerFactory
    {
        readonly CwAutofacJobFactory _jobFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Quartz.Impl.StdSchedulerFactory" /> class.
        /// </summary>
        /// <param name="jobFactory">Job factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jobFactory" /> is <see langword="null" />.</exception>
        public CwAutofacSchedulerFactory(CwAutofacJobFactory jobFactory)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Quartz.Impl.StdSchedulerFactory" /> class.
        /// </summary>
        /// <param name="props">The properties.</param>
        /// <param name="jobFactory">Job factory</param>
        /// <exception cref="ArgumentNullException"><paramref name="jobFactory" /> is <see langword="null" />.</exception>
        public CwAutofacSchedulerFactory(NameValueCollection props, CwAutofacJobFactory jobFactory)
            : base(props)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }

        /// <summary>
        ///     Instantiates the scheduler.
        /// </summary>
        /// <param name="rsrcs">The resources.</param>
        /// <param name="qs">The scheduler.</param>
        /// <returns>Scheduler.</returns>
        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            var scheduler = base.Instantiate(rsrcs, qs);
            scheduler.JobFactory = _jobFactory;
            return scheduler;
        }
    }
}
