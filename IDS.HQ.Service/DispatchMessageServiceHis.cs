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
    public class DispatchMessageHisService : DbBaseService<DispatchMessageHis>, IDispatchMessageHisService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
        public override Page<DispatchMessageHis> List(Page<DispatchMessageHis> page, Expression<Func<DispatchMessageHis, bool>> predicate, Expression<Func<DispatchMessageHis, object>> orderby = null, Expression<Func<DispatchMessageHis, object>> orderBydesc = null)
        {
            var upload = page.requestData ?? new DispatchMessageHis();
            if (!string.IsNullOrWhiteSpace(upload.Message))  //托盘编码批量
            {
                if (predicate == null)
                    predicate = f => EF.Functions.Like(f.Message, $"%{upload.Message}%");
                else
                    predicate = predicate.And(f => EF.Functions.Like(f.Message, $"%{upload.Message}%"));
            }
            if (!string.IsNullOrWhiteSpace(upload.RespMessage))  //托盘编码批量
            {
                if (predicate == null)
                    predicate = f => EF.Functions.Like(f.RespMessage, $"%{upload.RespMessage}%");
                else
                    predicate = predicate.And(f => EF.Functions.Like(f.RespMessage, $"%{upload.RespMessage}%"));
            }
            if (upload.BeginTime != null)  //托盘编码批量
            {
                if (predicate == null)
                    predicate = f => f.CreateTime >= upload.BeginTime;
                else
                    predicate = predicate.And(f => f.CreateTime >= upload.BeginTime);
            }
            if (upload.EndTime != null)  //托盘编码批量
            {
                if (predicate == null)
                    predicate = f => f.CreateTime <= upload.EndTime;
                else
                    predicate = predicate.And(f => f.CreateTime <= upload.EndTime);
            }
            orderBydesc = f => f.CreateTime;
            return base.List(page, predicate, orderby, orderBydesc);
        }
    }
}
