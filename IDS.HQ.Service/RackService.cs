using IDS.Base;
using IDS.Common;
using IDS.Extend.HYDevice;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB.Data;
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

        public IdsResult<List<Rack>> GetAllRackNode()
        {
            using (var ctx = DbContext()) {
                var racks = ctx.Rack.ToList();
                return IdsResult<List<Rack>>.ok(racks);
            }
        }

        public override Page<Rack> List(Page<Rack> page, Expression<Func<Rack, bool>> predicate, Expression<Func<Rack, object>> orderby = null, Expression<Func<Rack, object>> orderBydesc = null)
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

        public IdsResult<object> UpdateRackInfo(Rack rack)
        {
            using (var ctx = DbContext()) {
                var rack_db = ctx.Rack.AsNoTracking().Where(f => f.Id == rack.Id).First();
                using (var ts = ctx.Database.BeginTransaction()) {
                    try {
                        ctx.RackInfo.Where(f => f.RackNo == rack_db.RackNo)
                        .ExecuteUpdate(f => f.SetProperty(f => f.IP, rack.IP?? rack_db.IP)
                                        .SetProperty(f => f.Port, rack.Port?? rack_db.Port)
                                        .SetProperty(f => f.RackNo, rack.RackNo?? rack_db.RackNo));
                        ctx.Rack.Where(f => f.Id == rack.Id).ExecuteUpdate(f => f.SetProperty(f => f.IP, rack.IP ?? rack_db.IP)
                                            .SetProperty(f => f.Port, rack.Port ?? rack_db.Port)
                                            .SetProperty(f => f.RackNo, rack.RackNo ?? rack_db.RackNo));
                        //更新缓存
                        var node = new RackNode
                        {
                            No = rack_db.RackNo,
                            IP = rack_db.IP,
                            Port = (ushort)rack_db.Port,
                            Enabled = "Y",
                        };
                        SmartMaterialRackNode.Instance.RemoveNode(node);
                        var nodeNew = new RackNode
                        {
                            No = rack.RackNo,
                            IP = rack.IP,
                            Port = (ushort)rack.Port,
                            Enabled = "Y",
                        };
                        SmartMaterialRackNode.Instance.AddNode(nodeNew);
                        ctx.SaveChanges();
                        ts.Commit();

                    } catch (Exception ex) {

                        ts.Rollback();
                        return IdsResult<object>.failure(ex.Message);
                    }
                }
            }
            return IdsResult<object>.ok();
        }
    }
}
