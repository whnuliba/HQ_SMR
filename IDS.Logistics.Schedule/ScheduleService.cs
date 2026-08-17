using IDS.Common;
using log4net;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Quartz.MisfireInstruction;

namespace IDS.Logistics.Schedule
{
    public class ScheduleService : IScheduleService
    {
        public ILog Log = LogManager.GetLogger(typeof(ScheduleService));
        public ISchedulerFactory SchedulerFactory { get; set; }
        public IScheduler Scheduler { get; set; }
        //Quartz.ISchedulerFactory", ImplementationInstance = Quartz.Impl.StdSchedulerFactory
        public ScheduleService(ISchedulerFactory _SchedulerFactory)
        {
            SchedulerFactory = _SchedulerFactory;
        }
        public async Task<int> CreateJob(ScheduleModule scheduleJob)
        {
            var scheduler = await SchedulerFactory.GetScheduler();

            throw new NotImplementedException();
        }

        public async Task<bool> DeleteJob(ScheduleModule scheduleJob)
        {

            try
            {

                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");
                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                JobKey jobKey = JobKey.Create(scheduleJob.ScheduleCode, grpName);
                scheduler.UnscheduleJob(new TriggerKey(scheduleJob.ScheduleCode, grpName));
                bool f = await scheduler.DeleteJob(jobKey);
                return f;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }

        public async Task PauseJob(ScheduleModule scheduleJob)
        {

            try
            {

                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");
                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                JobKey jobKey = JobKey.Create(scheduleJob.ScheduleCode, grpName);
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                scheduler.PauseTrigger(triggerKey);
                scheduler.PauseJob(jobKey);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async Task<bool> Remove(ScheduleModule scheduleJob)
        {

            try
            {
                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");
                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                scheduler.PauseTrigger(triggerKey);// 停止触发器
                scheduler.UnscheduleJob(triggerKey);// 移除触发器
                JobKey jobKey = new JobKey(scheduleJob.ScheduleCode, grpName);
                bool b = await scheduler.DeleteJob(jobKey);// 删除任务
                Log.Info(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz success!", scheduleJob.ScheduleCode,
                        scheduleJob.ScheduleName,
                        scheduleJob.Cron));
                return b;

            }
            catch (SchedulerException e)
            {
                Log.Error(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz fail! desc:{3}",
                        scheduleJob.ScheduleCode,
                        scheduleJob.ScheduleName,
                        scheduleJob.Cron, e.Message));
                return false;
            }
        }

        public async Task<DateTime> Restart(ScheduleModule scheduleJob)
        {
            if (await Remove(scheduleJob))
                return await Start(scheduleJob);
            throw null;
        }

        public async Task ResumeJob(ScheduleModule scheduleJob)
        {
            try
            {
                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");
                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                JobKey jobKey = JobKey.Create(scheduleJob.ScheduleCode, grpName);
                scheduler.ResumeTrigger(triggerKey);
                scheduler.ResumeJob(jobKey);
            }
            catch (Exception e)
            {
                throw new BussinessException("任务恢复失败" + e.Message);
            }
            //throw new NotImplementedException();
        }

        public async Task<DateTime> Start(ScheduleModule scheduleJob)
        {
            try
            {
                //判断JOB是否已经存在
                //**STATE_BLOCKED 4 阻塞
                //STATE_COMPLETE 2 完成
                //STATE_ERROR 3 错误
                //STATE_NONE -1 不存在
                //STATE_NORMAL 0 正常
                //STATE_PAUSED 1 暂停**
                /* WAITING:等待   PAUSED:暂停  ACQUIRED:正常执行  BLOCKED：阻塞  ERROR：错误 */
                //Trigger.TriggerState state = scheduler.getTriggerState(triggerKey);
                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");
                //Scheduler scheduler = schedulerBean.getScheduler();
                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                Type type = Type.GetType(scheduleJob.JobClass);
                IJobDetail jobDetail = JobBuilder.Create(type)
                        .WithDescription(scheduleJob.ScheduleName)
                        .WithIdentity(scheduleJob.ScheduleCode, grpName).Build();
                JobDataMap jobDataMap = jobDetail.JobDataMap;
                jobDataMap.Add("ScheduleCode", scheduleJob.ScheduleCode);
                jobDataMap.Add("ScheduleType", scheduleJob.ScheduleType);
                jobDataMap.Add("ScheduleGrpCode", scheduleJob.ScheduleGrpCode);
                jobDataMap.Add("BusinessCode", scheduleJob.BusinessCode);
                jobDataMap.Add("task", JsonConvert.SerializeObject(scheduleJob));
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(scheduleJob.Cron);
                var cronTrigger = TriggerBuilder.Create()
                        .WithDescription(scheduleJob.ScheduleName)
                        .WithIdentity(triggerKey)
                        .WithSchedule(cronScheduleBuilder)
                        .Build();
                JobKey jobKey = new JobKey(scheduleJob.ScheduleCode, grpName);
                DateTime date = DateTime.Now;
                if (!await scheduler.CheckExists(jobKey))
                {
                    var d = await scheduler.ScheduleJob(jobDetail, cronTrigger);
                    date = d.DateTime;
                }
                if (scheduler.IsShutdown)
                {
                    scheduler.Start();
                }
                Log.Info(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz success!", scheduleJob.ScheduleCode,
                        scheduleJob.ScheduleName,
                        scheduleJob.Cron));
                return date;
            }
            catch (SchedulerException e)
            {
                Log.Error(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz fail! desc:{3}",
                        scheduleJob.ScheduleCode,
                        scheduleJob.ScheduleName,
                        scheduleJob.Cron, e.Message));
            }
           return DateTime.Now;
        }

        public async Task StartIntervalInSecond(ScheduleModule scheduleJob)
        {
            try {

                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");

                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                JobKey jobKey = JobKey.Create(scheduleJob.ScheduleCode, grpName);

                Type type = Type.GetType(scheduleJob.JobClass);
                IJobDetail job = JobBuilder.Create(type)
                .WithIdentity(jobKey)
                .Build();
                JobDataMap jobDataMap = job.JobDataMap;
                jobDataMap.Add("ScheduleCode", scheduleJob.ScheduleCode);
                jobDataMap.Add("ScheduleType", scheduleJob.ScheduleType);
                jobDataMap.Add("ScheduleGrpCode", scheduleJob.ScheduleGrpCode);
                jobDataMap.Add("BusinessCode", scheduleJob.BusinessCode);
                jobDataMap.Add("task", JsonConvert.SerializeObject(scheduleJob));
                // Trigger the job to run now, and then every 40 seconds
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity(triggerKey)
                  .StartNow()
                  .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(scheduleJob.Interval ?? 30)
                   .RepeatForever())
                .Build();
                scheduler.ScheduleJob(job, trigger);
            } catch (Exception e) {

                Log.Error(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz fail! desc:{3}",
                       scheduleJob.ScheduleCode,
                       scheduleJob.ScheduleName,
                       scheduleJob.Cron, e.Message));
            }

        }

        public async Task<bool> Stop(ScheduleModule scheduleJob)
        {
            return await Remove(scheduleJob);
        }

        public async Task<int> UpdateCronAndJobClass(ScheduleModule scheduleJob)
        {
            throw new NotImplementedException();
        }

        public async Task StartIntervalInMilliSecond(ScheduleModule scheduleJob)
        {


            try {
                var scheduler = await SchedulerFactory.GetScheduler();
                if (scheduleJob == null)
                    throw new BussinessException("the timer  arguments  cannot is empty|定时器的信息不能为空");

                string grpName = "DEFAULT_GROUP";
                if (!string.IsNullOrEmpty(scheduleJob.ScheduleGrpCode))
                    grpName = scheduleJob.ScheduleGrpCode;
                TriggerKey triggerKey = new TriggerKey(scheduleJob.ScheduleCode, grpName);
                JobKey jobKey = JobKey.Create(scheduleJob.ScheduleCode, grpName);

                Type type = Type.GetType(scheduleJob.JobClass);
                IJobDetail job = JobBuilder.Create(type)
                .WithIdentity(jobKey)
                .Build();
                JobDataMap jobDataMap = job.JobDataMap;
                jobDataMap.Add("ScheduleCode", scheduleJob.ScheduleCode);
                jobDataMap.Add("ScheduleType", scheduleJob.ScheduleType);
                jobDataMap.Add("ScheduleGrpCode", scheduleJob.ScheduleGrpCode);
                jobDataMap.Add("BusinessCode", scheduleJob.BusinessCode);
                // Trigger the job to run now, and then every 40 seconds
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity(triggerKey)
                  .StartNow()
                  .WithSimpleSchedule(x => x
                   .WithInterval(TimeSpan.FromMilliseconds(scheduleJob.Interval ?? 100))
                   .RepeatForever())
                .Build();
                scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception e)
            {
                Log.Error(string.Format("taskNo={0},taskName={1},scheduleRule={2} load to quartz fail! desc:{3}",
                   scheduleJob.ScheduleCode,
                   scheduleJob.ScheduleName,
                   scheduleJob.Cron, e.Message));

            }
  
        }
    }
}
