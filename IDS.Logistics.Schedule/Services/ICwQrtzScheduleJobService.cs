using IDS.Base;
using IDS.Fms.IService;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace IDS.Schedule
{
    public interface ICwQrtzScheduleJobService : IScheduleBaseService<CwQrtzScheduleJob>
    {
        Task<DateTime> Start(string id);
        Task<bool> Stop(string id);
        Task<DateTime> Restart(string id);
        Task<bool> Remove(string id);
        /**
         * 暂停任务
         * scheduleJob
         */
        Task PauseJob(string id);
        /**
         * 恢复任务
         * scheduleJob
         */
        Task ResumeJob(string id);
        Task<bool> DeleteJob(string id);

        /**
         * 创建任务
         * @param scheduleJob
         */
        Task<int> CreateJob(CwQrtzScheduleJob scheduleJob);

        Task<int> UpdateCronAndJobClass(CwQrtzScheduleJob scheduleJob);
        Task<int> UpdateParameter(CwQrtzScheduleJob scheduleJob);
        Task StartIntervalInSecond(VCwQrtzScheduleJob scheduleJob);

        Task<Page<VCwQrtzScheduleJob>> All(Page<VCwQrtzScheduleJob> data, Expression<Func<VCwQrtzScheduleJob, bool>> predicate);
    }
}
