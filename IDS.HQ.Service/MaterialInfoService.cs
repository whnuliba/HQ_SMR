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
    public class MaterialInfoService : DbBaseService<MaterialInfo>, IMaterialInfoService
    {
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public IdsResult<MaterialInfo> GetByTaskId(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                return IdsResult<MaterialInfo>.failure("任务ID不能为空");
            }

            using (var ctx = DbContext())
            {
                var material = ctx.MaterialInfo.FirstOrDefault(m => m.TaskId == taskId);
                if (material == null)
                {
                    return IdsResult<MaterialInfo>.failure($"未找到任务ID为 {taskId} 的物料信息");
                }
                return IdsResult<MaterialInfo>.ok(material);
            }
        }

        public IdsResult<List<MaterialInfo>> GetByRackId(string rackId)
        {
            if (string.IsNullOrWhiteSpace(rackId))
            {
                return IdsResult<List<MaterialInfo>>.failure("料架ID不能为空");
            }

            using (var ctx = DbContext())
            {
                var materials = ctx.MaterialInfo
                    .Where(m => m.RackId == rackId)
                    .OrderByDescending(m => m.CreateTime)
                    .ToList();

                return IdsResult<List<MaterialInfo>>.ok(materials);
            }
        }

        public IdsResult<MaterialInfo> GetByPPID(string ppid)
        {
            if (string.IsNullOrWhiteSpace(ppid))
            {
                return IdsResult<MaterialInfo>.failure("PPID不能为空");
            }

            using (var ctx = DbContext())
            {
                var material = ctx.MaterialInfo.FirstOrDefault(m => m.PPID == ppid);
                if (material == null)
                {
                    return IdsResult<MaterialInfo>.failure($"未找到PPID为 {ppid} 的物料信息");
                }
                return IdsResult<MaterialInfo>.ok(material);
            }
        }

        public override Page<MaterialInfo> List(Page<MaterialInfo> page, Expression<Func<MaterialInfo, bool>> predicate)
        {
            var upload = page.requestData ?? new MaterialInfo();

            // 按任务ID批量查询
            if (!string.IsNullOrWhiteSpace(upload.TaskId))
            {
                var taskIds = upload.TaskId.Split(",").ToList();
                if (predicate == null)
                    predicate = f => taskIds.Contains(f.TaskId);
                else
                    predicate = predicate.And(f => taskIds.Contains(f.TaskId));
            }

            // 按料架ID批量查询
            if (!string.IsNullOrWhiteSpace(upload.RackId))
            {
                var rackIds = upload.RackId.Split(",").ToList();
                if (predicate == null)
                    predicate = f => rackIds.Contains(f.RackId);
                else
                    predicate = predicate.And(f => rackIds.Contains(f.RackId));
            }

            // 按PPID批量查询
            if (!string.IsNullOrWhiteSpace(upload.PPID))
            {
                var ppids = upload.PPID.Split(",").ToList();
                if (predicate == null)
                    predicate = f => ppids.Contains(f.PPID);
                else
                    predicate = predicate.And(f => ppids.Contains(f.PPID));
            }

            // 按供应商编码查询
            if (!string.IsNullOrWhiteSpace(upload.VendorCode))
            {
                if (predicate == null)
                    predicate = f => f.VendorCode.Contains(upload.VendorCode);
                else
                    predicate = predicate.And(f => f.VendorCode.Contains(upload.VendorCode));
            }

            return base.List(page, predicate);
        }
    }
}
