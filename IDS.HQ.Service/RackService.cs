using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB;
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

        public override int delete(Rack record)
        {
            using (var ctx = DbContext())
            {
                var rack = ctx.Rack.Where(f => f.Id == record.Id).FirstOrDefault();
                if (rack == null)
                    return 0;
                var hasStore = ctx.RackInfo.Where(c => c.Loading == (int)LocationStates.LOADING).Count();
                if (hasStore > 0)
                    return 0;
                using (var ts = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        ctx.Rack.Where(f => f.Id == rack.Id).ExecuteDelete();
                        ctx.RackInfo.Where(f => f.RackNo == rack.RackNo).ExecuteDelete();
                        ts.Commit();
                    }
                    catch
                    {
                        ts.Rollback();
                        throw;
                    }
                }
            }
            return 1;
        }

        public override int deleteById(string id)
        {
            using (var ctx = DbContext()) {
                var rack = ctx.Rack.Where(f => f.Id == id).FirstOrDefault();
                if (rack == null)
                    return 0;
                var hasStore = ctx.RackInfo.Where(c => c.Loading == (int)LocationStates.LOADING).Count();
                if (hasStore > 0)
                    return 0;
                using (var ts = ctx.Database.BeginTransaction()) {
                    try {
                        ctx.Rack.Where(f => f.Id == id).ExecuteDelete();
                        ctx.RackInfo.Where(f => f.RackNo == rack.RackNo).ExecuteDelete();
                        ts.Commit();
                    } catch  {
                        ts.Rollback();
                        throw ;
                    }
                }
            }
            return 1;
        }

        public IdsResult<object> UpdateRackInfo(Rack rack)
        {
            using (var ctx = DbContext()) {
                var rack_db = ctx.Rack.AsNoTracking().Where(f => f.Id == rack.Id).First();
                using (var ts = ctx.Database.BeginTransaction()) {
                    try {
                        //若储位已经存在库存 不允许修改
                        var hasStore = ctx.RackInfo.Where(c => c.Loading == (int)LocationStates.LOADING).Count();
                        if (hasStore > 0)
                            return IdsResult<object>.failure("已经存在库存，不许修改货架信息");
                        ctx.RackInfo.Where(f => f.RackNo == rack_db.RackNo)
                        .ExecuteUpdate(f => f.SetProperty(f => f.IP, rack.IP?? rack_db.IP)
                                        .SetProperty(f => f.Port, rack.Port?? rack_db.Port)
                                        .SetProperty(f => f.RackNo, rack.RackNo?? rack_db.RackNo));
                        ctx.Rack.Where(f => f.Id == rack.Id).ExecuteUpdate(f => f.SetProperty(f => f.IP, rack.IP ?? rack_db.IP)
                                            .SetProperty(f => f.Port, rack.Port ?? rack_db.Port)
                  
                                            .SetProperty(f => f.RackNo, rack.RackNo ?? rack_db.RackNo));
                        //处理储位信息 
                        if (rack.ASideQty>0) {
                            int totalLocation = rack.ASideQty ?? 0 + rack.BSideQty ?? 0;
                            
                            var countA = ctx.RackInfo.Where(x=>x.RackSide=="A").Count();
                            if (countA < rack.ASideQty) {
                                ctx.RackInfo.Where(f => f.Location <= rack.ASideQty).ExecuteUpdate(f => f.SetProperty(f => f.RackSide, "A"));
                            }
                            var countB = ctx.RackInfo.Where(x => x.RackSide == "B").Count();

                            if (countB < rack.BSideQty){
                                int n = (rack.BSideQty??0 + rack.ASideQty??0)-(countA+ countB);
                                var rackinfos = new List<RackInfo>();
                                for (int x = countA + countB + 1; x <= n; n++) {
                                    long id = IdUtils.Id;
                                    var _rack = new RackInfo()
                                    {
                                        RackNo = rack.RackNo,
                                        RackSide = "B",
                                        IP = rack.IP,
                                        Port = rack.Port,
                                        Inductive = 1,
                                        Loading = (int)LocationStates.FREE,
                                        Location = x,
                                        Enable = 1,
                                        Id = id + ""
                                    };
                                    rackinfos.Add(_rack);
                                }
                                ctx.AddRange(rackinfos);
                            }

                            if (countA > rack.ASideQty)
                            {
                                ctx.RackInfo.Where(f => f.Location > rack.ASideQty).ExecuteUpdate(f => f.SetProperty(f => f.RackSide, "B"));

                                if (countB > rack.BSideQty) {
                                    ctx.RackInfo.Where(f => f.Location > (rack.ASideQty + rack.BSideQty)).ExecuteDelete();
                                }
                            }
                        }
                       
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
                        //ctx.SaveChanges();
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
