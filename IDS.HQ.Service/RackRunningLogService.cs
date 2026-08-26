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
    public class RackRunningLogService : DbBaseService<RackRunningLog>, IRackRunningLogService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public IdsResult<List<RackRunningLog>> GetByRackNo(string rackNo)
        {
            if (string.IsNullOrWhiteSpace(rackNo))
            {
                return IdsResult<List<RackRunningLog>>.failure("料架号不能为空");
            }

            using (var ctx = DbContext())
            {
                var logs = ctx.RackRunningLog
                    .Where(l => l.RackNo == rackNo)
                    .OrderByDescending(l => l.CreateTime)
                    .ToList();

                return IdsResult<List<RackRunningLog>>.ok(logs);
            }
        }

        public IdsResult<RackRunningLog> GetByTaskId(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                return IdsResult<RackRunningLog>.failure("任务ID不能为空");
            }

            using (var ctx = DbContext())
            {
                var log = ctx.RackRunningLog.FirstOrDefault(l => l.TaskId == taskId);
                if (log == null)
                {
                    return IdsResult<RackRunningLog>.failure($"未找到任务ID为 {taskId} 的运行日志");
                }
                return IdsResult<RackRunningLog>.ok(log);
            }
        }

        public IdsResult<List<RackRunningLog>> GetByLogLevel(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                return IdsResult<List<RackRunningLog>>.failure("日志级别不能为空");
            }

            using (var ctx = DbContext())
            {
                var logs = ctx.RackRunningLog
                    .Where(l => l.LogLevel == logLevel)
                    .OrderByDescending(l => l.CreateTime)
                    .ToList();

                return IdsResult<List<RackRunningLog>>.ok(logs);
            }
        }

        public IdsResult<int> DeleteLogsBefore(DateTime dateTime)
        {
            using (var ctx = DbContext())
            {
                var affectedRows = ctx.RackRunningLog
                    .Where(l => l.CreateTime < dateTime)
                    .ExecuteDelete();

                return IdsResult<int>.ok(affectedRows);
            }
        }

        public override Page<RackRunningLog> List(Page<RackRunningLog> page, Expression<Func<RackRunningLog, bool>> predicate)
        {
            var upload = page.requestData ?? new RackRunningLog();

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

            // 按日志级别查询
            if (!string.IsNullOrWhiteSpace(upload.LogLevel))
            {
                if (predicate == null)
                    predicate = f => f.LogLevel == upload.LogLevel;
                else
                    predicate = predicate.And(f => f.LogLevel == upload.LogLevel);
            }

            // 按报警类型查询
            if (upload.AlarmType.HasValue)
            {
                if (predicate == null)
                    predicate = f => f.AlarmType == upload.AlarmType;
                else
                    predicate = predicate.And(f => f.AlarmType == upload.AlarmType);
            }

            return base.List(page, predicate);
        }
    }
}