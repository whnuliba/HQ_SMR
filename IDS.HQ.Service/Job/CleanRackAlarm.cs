using IDS.Extend.HYDevice;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Job
{
    public class CleanRackAlarm : BaseJob<CleanRackAlarm>
    {

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;

        public CleanRackAlarm(IDbContextFactory<RackDbContext> dbContext,ILogger<CleanRackAlarm> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)) {
            _dbContext = dbContext;
            _idsRedis = redis;
        }
        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            using (var ctx = _dbContext.CreateDbContext())
            {
                var alarms = ctx.Query<RackAlarm>(f => f.HandleState != 0).OrderBy(f => f.CreateTime).Take(20).ToList(); //每次读取20条完成归档
                var ids = alarms.Select(f => f.Id).ToList();
                var alarmsHis = alarms.Select(f =>
                {
                    var alarmHis = new RackAlarmHis();
                    ObjectExtensions.CopyProperties(f, alarmHis);
                    return alarmHis;
                }).ToList();

                using (var transaction = await ctx.Database.BeginTransactionAsync())
                {
                    try
                    {
                        ctx.AddRange(alarmsHis);
                        ctx.RemoveRange(alarms);
                        await ctx.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "CleanRackAlarm failed");
                    }
                }
            }
        }
    }
}
