using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackAlarmService : DbBaseService<RackAlarm>, IRackAlarmService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public IdsResult<List<RackAlarm>> GetByRackNo(string rackNo)
        {
            if (string.IsNullOrWhiteSpace(rackNo))
            {
                return IdsResult<List<RackAlarm>>.failure("料架号不能为空");
            }

            using (var ctx = DbContext())
            {
                var alarms = ctx.RackAlarm
                    .Where(a => a.RackNo == rackNo)
                    .OrderByDescending(a => a.CreateTime)
                    .ToList();

                return IdsResult<List<RackAlarm>>.ok(alarms);
            }
        }

        public IdsResult<RackAlarm> GetByTaskId(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                return IdsResult<RackAlarm>.failure("任务ID不能为空");
            }

            using (var ctx = DbContext())
            {
                var alarm = ctx.RackAlarm.FirstOrDefault(a => a.TaskId == taskId);
                if (alarm == null)
                {
                    return IdsResult<RackAlarm>.failure($"未找到任务ID为 {taskId} 的报警记录");
                }
                return IdsResult<RackAlarm>.ok(alarm);
            }
        }

        public IdsResult<List<RackAlarm>> GetByHandleState(int handleState)
        {
            using (var ctx = DbContext())
            {
                var alarms = ctx.RackAlarm
                    .Where(a => a.HandleState == handleState)
                    .OrderByDescending(a => a.CreateTime)
                    .ToList();

                return IdsResult<List<RackAlarm>>.ok(alarms);
            }
        }

        public IdsResult<bool> BatchUpdateHandleState(List<string> ids, int handleState)
        {
            if (ids == null || ids.Count == 0)
            {
                return IdsResult<bool>.failure("ID列表不能为空");
            }

            using (var ctx = DbContext())
            {
                var now = DateTime.Now;
                var affectedRows = ctx.RackAlarm
                    .Where(a => ids.Contains(a.Id))
                    .ExecuteUpdate(setters => setters
                        .SetProperty(a => a.HandleState, handleState)
                        .SetProperty(a => a.LastModifyTime, now)
                    );

                return IdsResult<bool>.ok(affectedRows > 0);
            }
        }

        public override Page<RackAlarm> List(Page<RackAlarm> page, Expression<Func<RackAlarm, bool>> predicate)
        {
            var upload = page.requestData ?? new RackAlarm();

            // 按料架号批量查询
            if (!string.IsNullOrWhiteSpace(upload.RackNo))
            {
                var rackNos = upload.RackNo.Split(",").ToList();
                if (predicate == null)
                    predicate = f => rackNos.Contains(f.RackNo);
                else
                    predicate = predicate.And(f => rackNos.Contains(f.RackNo));
            }

            // 按任务ID批量查询
            if (!string.IsNullOrWhiteSpace(upload.TaskId))
            {
                var taskIds = upload.TaskId.Split(",").ToList();
                if (predicate == null)
                    predicate = f => taskIds.Contains(f.TaskId);
                else
                    predicate = predicate.And(f => taskIds.Contains(f.TaskId));
            }

            // 按报警类型查询
            if (upload.AlarmType.HasValue)
            {
                if (predicate == null)
                    predicate = f => f.AlarmType == upload.AlarmType;
                else
                    predicate = predicate.And(f => f.AlarmType == upload.AlarmType);
            }

            // 按处理状态查询
            if (upload.HandleState.HasValue)
            {
                if (predicate == null)
                    predicate = f => f.HandleState == upload.HandleState;
                else
                    predicate = predicate.And(f => f.HandleState == upload.HandleState);
            }

            return base.List(page, predicate);
        }
    }
}