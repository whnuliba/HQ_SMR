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
using System.Linq.Expressions;
using System.Text;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class DispatchMessageService : DbBaseService<DispatchMessage>, IDispatchMessageService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public override Page<DispatchMessage> List(Page<DispatchMessage> page, Expression<Func<DispatchMessage, bool>> predicate, Expression<Func<DispatchMessage, object>> orderby = null, Expression<Func<DispatchMessage, object>> orderBydesc = null)
        {
            var upload = page.requestData ?? new DispatchMessage();
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
            if (upload.BeginTime!=null)  //托盘编码批量
            {
                if (predicate == null)
                    predicate = f =>f.CreateTime>= upload.BeginTime;
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
            orderBydesc = f=>f.CreateTime;
           return base.List(page, predicate, orderby, orderBydesc);

        }

        public IdsResult<string> UpdateStatus(DispatchMessage message)
        {
            using (var ctx = DbContext()) {
             int i =   ctx.DispatchMessage.Where(a => a.Id == message.Id)
                                              .ExecuteUpdate(setters => setters
                                                  .SetProperty(a => a.Status, v => message.Status).SetProperty(f=>f.PostCount, message.PostCount??0)
                                               );
                if (i > 0) {
                    return IdsResult<string>.ok();
                }
                return  IdsResult<string>.failure("状态变更失败");
            }
        }
    }
}
