using IDS.HQ.Module;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Job
{
    public class CleanMaterialInfoJob : BaseJob<CleanMaterialInfoJob>
    {

        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;

        public CleanMaterialInfoJob(IDbContextFactory<RackDbContext> dbContext, ILogger<CleanMaterialInfoJob> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30))
        {
            _dbContext = dbContext;
            _idsRedis = redis;
        }

        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {
            using (var ctx = _dbContext.CreateDbContext()) {
                var rhis = ctx.Query<MaterialInfoHis>(f => f.CreateTime < DateTime.Now.AddDays(-120)).Take(20).Select(f => f.Id).ToList();
                ctx.Delete<MaterialInfoHis>(f => rhis.Contains(f.Id));
                var chis = ctx.Query<MaterialInfoCancel>(f => f.CreateTime < DateTime.Now.AddDays(-120)).Take(20).Select(f => f.Id).ToList();
                ctx.Delete<MaterialInfoCancel>(f => chis.Contains(f.Id)); 
            }
        }
    }
}
