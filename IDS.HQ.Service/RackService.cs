using IDS.Base;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackService : DbBaseService<Rack>, IRackService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }
        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public override Page<Rack> List(Page<Rack> page, Expression<Func<Rack, bool>> predicate)
        {
            var upload = page.requestData ?? new Rack();
            if (!string.IsNullOrWhiteSpace(upload.RackNo))  //托盘编码批量
            {
                var trayNum = upload.RackNo.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.RackNo);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.RackNo));
            }
            if (!string.IsNullOrWhiteSpace(upload.IP))  //托盘编码批量
            {
                var trayNum = upload.IP.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.IP);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.IP));
            }
            return base.List(page, predicate);
        }
    }
}
