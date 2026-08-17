using IDS.Base;
using IDS.Fms.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static LinqToDB.Common.Configuration;

namespace IDS.Schedule
{
    [AutoInjection]
    public class CwQrtzScheduleJobAdapter : ScheduleBaseAdapter<CwQrtzScheduleJob>
    {
        public ICwQrtzScheduleJobService CwCwQrtzScheduleJobService { get; set; }
        public override IDbBaseService<CwQrtzScheduleJob> Service()
        {
            return CwCwQrtzScheduleJobService;
        }

        public async Task<DateTime> Start(string id) { 
           return await CwCwQrtzScheduleJobService.Start(id);
        }
        public async Task<bool> Stop(string id)
        {
            return await CwCwQrtzScheduleJobService.Stop(id);
        }
        public async Task<DateTime> Restart(string id)
        {
            return await CwCwQrtzScheduleJobService.Restart(id);
        }
        public async Task<bool> Remove(string id)
        {
            return await CwCwQrtzScheduleJobService.Remove(id);
        }
        /**
         * 暂停任务
         * scheduleJob
         */
        public async Task PauseJob(string id)
        {
             await CwCwQrtzScheduleJobService.PauseJob(id);
        }
        /**
         * 恢复任务
         * scheduleJob
         */
        public async Task ResumeJob(string id)
        {
             await CwCwQrtzScheduleJobService.ResumeJob(id);
        }
        public async Task<bool> DeleteJob(string id)
        {
            return await CwCwQrtzScheduleJobService.DeleteJob(id);
        }

        /**
         * 创建任务
         * @param scheduleJob
         */
        public async Task<int> CreateJob(CwQrtzScheduleJob scheduleJob)
        {
            return await CwCwQrtzScheduleJobService.CreateJob(scheduleJob);
        }

        public async Task<int> UpdateCronAndJobClass(CwQrtzScheduleJob scheduleJob)
        {
            return await CwCwQrtzScheduleJobService.UpdateCronAndJobClass(scheduleJob);
        }

        public async Task StartIntervalInSecond(VCwQrtzScheduleJob scheduleJob)
        {
             await CwCwQrtzScheduleJobService.StartIntervalInSecond(scheduleJob);
        }

        public async Task<Page<VCwQrtzScheduleJob>> All(Page<VCwQrtzScheduleJob> data, Expression<Func<VCwQrtzScheduleJob, bool>> predicate) {
            return await CwCwQrtzScheduleJobService.All(data, predicate);
        }
        public async Task<int> UpdateParameter(CwQrtzScheduleJob scheduleJob) {
            return await CwCwQrtzScheduleJobService.UpdateParameter(scheduleJob);
        }
    }
}
