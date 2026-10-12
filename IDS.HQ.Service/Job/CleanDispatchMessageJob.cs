using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Job
{
    public class CleanDispatchMessageJob : BaseJob<CleanDispatchMessageJob>
    {

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;
        public CleanDispatchMessageJob(IDbContextFactory<RackDbContext> dbContext, ILogger<CleanDispatchMessageJob> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
        {
            _dbContext = dbContext;
            _idsRedis = redis;
        }
        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            //只保留一个月的日志数据
            using (var ctx = _dbContext.CreateDbContext()) {
                ctx.Delete<DispatchMessageHis>(f => f.CreateTime < DateTime.Now.AddDays(-30));
                ctx.Delete<DispatchMessage>(f => f.CreateTime < DateTime.Now.AddDays(-60));
            }
        }
    }
}
