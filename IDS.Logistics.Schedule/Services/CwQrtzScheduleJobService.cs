using IDS.Base;
using IDS.Base.Utils;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extension;
using IDS.Fms.IService;
using IDS.Fms.Service;
using IDS.Ioc;
using IDS.Logistics.Schedule;
using IDS.Logistics.Schedule.Services;
using IDS.Persistence;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace IDS.Schedule
{
    [AutoInjection]
    public class CwQrtzScheduleJobService : ScheduleBaseService<CwQrtzScheduleJob, ScheduleDbContext>, ICwQrtzScheduleJobService
    {
        public IScheduleService ScheduleService { get; set; }
        public virtual IdsRedis RedisClient { set; get; }
        public async Task<Page<VCwQrtzScheduleJob>> All(Page<VCwQrtzScheduleJob> page, Expression<Func<VCwQrtzScheduleJob, bool>> predicate)
        {
            using (var ctx = DbContext())
            {
                var req = page.requestData;
                var data = ctx.Query<VCwQrtzScheduleJob>(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
                var count = ctx.Count<VCwQrtzScheduleJob>(predicate);
                data.ForEach(c =>
                {
                    if (c.NextFireTime != null && c.NextFireTime>0) {
                        c.NextFireTime1 = (new DateTime((long)c.NextFireTime)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    if (c.PreFireTime != null && c.PreFireTime > 0)
                    {
                        c.PreFireTime1 = (new DateTime((long)c.PreFireTime)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    if (c.StartTime != null && c.StartTime > 0)
                    {
                        c.StartTime1 = (new DateTime((long)c.StartTime)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    if (c.EndTime != null && c.EndTime > 0)
                    {
                        c.EndTime1 = (new DateTime((long)c.EndTime)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                });

                Page<VCwQrtzScheduleJob> page1 = new Page<VCwQrtzScheduleJob>(count, data, page.pageSize, page.current);
                return page1;
            }
        }

        public async Task<int> CreateJob(CwQrtzScheduleJob scheduleJob)
        {
           return  save(scheduleJob,null);
        }

        public async Task<bool> DeleteJob(string id)
        {
            using (var ctx = DbContext()) { 
              var schedule = ctx.Query<CwQrtzScheduleJob>(f=>f.Id==id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                deleteById(id);
                return await ScheduleService.DeleteJob(scheduleModule);
            }
        }

        public async Task PauseJob(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                await ScheduleService.PauseJob(scheduleModule);
            }
        }

        public async Task<bool> Remove(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                return await ScheduleService.DeleteJob(scheduleModule);
            }
        }

        public async Task<DateTime> Restart(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                if (await Remove(id)) {
                  return  await ScheduleService.Start(scheduleModule);
                }
                return DateTime.Now;
            }
        }

        public async Task ResumeJob(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                await ScheduleService.ResumeJob(scheduleModule);
            }
        }

        public  override int save(CwQrtzScheduleJob record, string?[] properites = null)
        {
            if (!String.IsNullOrWhiteSpace(record.Id))
            {
                return update(record, properites);
            }
            using (var ctx = DbContext())
            {

                record.saveInit();
                if (string.IsNullOrWhiteSpace(record.Id))
                    record.Id = IdUtils.Id+"";
                return ctx.Insert<CwQrtzScheduleJob>(record);
            }
        }

        public async Task<DateTime> Start(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<VCwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                if (scheduleModule.Interval != null && scheduleModule.Interval > 0) {
                    await ScheduleService.StartIntervalInMilliSecond(scheduleModule);
                    return DateTime.Now;
                }
                return await ScheduleService.Start(scheduleModule);
            }
        }

        public async Task StartIntervalInSecond(VCwQrtzScheduleJob scheduleJob)
        {
            using (var ctx = DbContext())
            {
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(scheduleJob, scheduleModule);
                await ScheduleService.StartIntervalInSecond(scheduleModule);
            }
        }

        public async Task<bool> Stop(string id)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == id).FirstOrDefault();
                ScheduleModule scheduleModule = new ScheduleModule();
                ObjectExtensions.CopyProperties(schedule,scheduleModule);
                return await ScheduleService.Stop(scheduleModule);
            }
        }

        public async Task<int> UpdateCronAndJobClass(CwQrtzScheduleJob scheduleJob)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == scheduleJob.Id).FirstOrDefault();
                if(schedule==null)
                    throw new BussinessException("The schedule information of the database does not exist.");
                schedule.Cron = scheduleJob.Cron;
                schedule.JobClass = scheduleJob.JobClass;
                ScheduleModule scheduleModule = new ScheduleModule();
                if (!string.IsNullOrEmpty(scheduleJob.Parameters))
                    schedule.Parameters = scheduleJob.Parameters;
                schedule.Ticket = BaseUtil.uuid();
                string key = $"Job:{schedule.ScheduleGrpCode}:{schedule.ScheduleCode}";
                await RedisClient.SetCache(key, schedule.Ticket);

                ObjectExtensions.CopyProperties(schedule, scheduleModule);
                await ScheduleService.Remove(scheduleModule);
                return ctx.UpdateByPrimaryKeySelective(scheduleJob);
            }
        }

        public async Task<int> UpdateParameter(CwQrtzScheduleJob scheduleJob)
        {
            using (var ctx = DbContext())
            {
                var schedule = ctx.Query<CwQrtzScheduleJob>(f => f.Id == scheduleJob.Id).FirstOrDefault();
                if (schedule == null)
                    throw new BussinessException("The schedule information of the database does not exist.");
                ScheduleModule scheduleModule = new ScheduleModule();
                schedule.Parameters = scheduleJob.Parameters??"";
                schedule.Ticket = BaseUtil.uuid();
                string key = $"Job:{schedule.ScheduleGrpCode}:{schedule.ScheduleCode}";
                await RedisClient.SetCache(key, schedule.Ticket);
                ObjectExtensions.CopyProperties(schedule, scheduleModule);
                await ScheduleService.Remove(scheduleModule);
                return ctx.UpdateByPrimaryKeySelective(scheduleJob);
            }
        }
    }
}
