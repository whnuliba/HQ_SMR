using IDS.HQ.Module;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Job
{
    public class CleanRunningLog : BaseJob<CleanRunningLog>
    {

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;

        public CleanRunningLog(IDbContextFactory<RackDbContext> dbContext, ILogger<CleanRunningLog> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
        {
            _dbContext = dbContext;
            _idsRedis = redis;
        }

        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            //只保留一个月的日志数据
            using (var ctx = _dbContext.CreateDbContext())
            {
                ctx.Delete<RackRunningLog>(f => f.CreateTime < DateTime.Now.AddDays(-30));
            }
        }
    }
}
