using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    public interface IScheduleService
    {

        Task<DateTime> Start(ScheduleModule scheduleJob);
        Task<bool> Stop(ScheduleModule scheduleJob);
        Task<DateTime>  Restart(ScheduleModule scheduleJob);
        Task<bool> Remove(ScheduleModule scheduleJob);
        /**
         * 暂停任务
         * scheduleJob
         */
        Task PauseJob(ScheduleModule scheduleJob);
        /**
         * 恢复任务
         * scheduleJob
         */
        Task ResumeJob(ScheduleModule scheduleJob);
          Task<bool> DeleteJob(ScheduleModule scheduleJob);

        /**
         * 创建任务
         * @param scheduleJob
         */
         Task<int> CreateJob(ScheduleModule scheduleJob);

        Task<int> UpdateCronAndJobClass(ScheduleModule scheduleJob);

        Task StartIntervalInSecond(ScheduleModule scheduleJob);
        Task StartIntervalInMilliSecond(ScheduleModule scheduleJob);
    }
}
