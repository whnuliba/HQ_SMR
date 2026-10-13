using IDS.Base;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IDS.HQ.Service.IService
{
    [AutoInjection]
    public class RackTaskHisService : DbBaseService<RackTaskHis>, IRackTaskHisService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override IDSContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
        public override Page<RackTaskHis> List(Page<RackTaskHis> page, Expression<Func<RackTaskHis, bool>> predicate, Expression<Func<RackTaskHis, object>> orderby = null, Expression<Func<RackTaskHis, object>> orderBydesc = null)
        {

            var upload = page.requestData ?? new RackTaskHis();
            if (!string.IsNullOrWhiteSpace(upload.RackNo))  //托盘编码批量
            {
                var trayNum = upload.RackNo.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.RackNo);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.RackNo));
            }
            if (!string.IsNullOrWhiteSpace(upload.PPID))  //托盘编码批量
            {
                var trayNum = upload.PPID.Split(",").ToList();
                if (predicate == null)
                    predicate = f => trayNum.Contains(f.PPID);
                else
                    predicate = predicate.And(f => trayNum.Contains(f.PPID));
            }
            if (upload.TaskType != null)
            {
                if (predicate == null)
                    predicate = f => f.TaskType == upload.TaskType;
                else
                    predicate = predicate.And(f => f.TaskType == upload.TaskType);
            }
            if (upload.TaskState != null)
            {

                if (predicate == null)
                    predicate = f => f.TaskState == upload.TaskState;
                else
                    predicate = predicate.And(f => f.TaskState == upload.TaskState);
            }
            orderBydesc = f => f.CreateTime;
            return base.List(page, predicate,null, orderBydesc);
        }
    }
}
