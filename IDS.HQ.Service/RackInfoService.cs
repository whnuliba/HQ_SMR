using Autofac;
using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Transactions;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackInfoService : DbLongBaseService<RackInfo>, IRackInfoService
    {
        public object obj_lock = new object();
        public IdsRedis RedisClient { get; set; }
        public IDbContextFactory<RackDbContext> DbContextFactory { get; set; }
        public object _obj_lock = new object();
        private string _rackNodeCacheKey = "HQ:HY:RACKNODE:"; //料架

        public override RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }

        public IdsResult<object> RegisterRackInfo(RegisterRackInfoDto rackInfo)
        {
            //需要判断货架信息
            IdsResult<object> check = CheckRegisterInfo(rackInfo);
            if (!check.Success) { return check; }

            lock (_obj_lock)
            {

                //检查当前货架是否已经存在
                using (var ctx = DbContext())
                {
                    var _rackinfo = ctx.Count<RackInfo>(f => f.RackNo == rackInfo.RackNo);
                    if (_rackinfo > 0)
                        return IdsResult<object>.failure($"当前货架已经注册：{JsonConvert.SerializeObject(rackInfo)}");
                    _rackinfo = ctx.Count<RackInfo>(f => f.IP == rackInfo.IP);
                    if (_rackinfo > 0)
                        return IdsResult<object>.failure($"当前货架已经注册：{JsonConvert.SerializeObject(rackInfo)}");

                }

                //报货架注册到redis
                var rackNode = new Rack() {

                    RackNo = rackInfo.RackNo,
                    RackSide = rackInfo.ASide,
                    IP = rackInfo.IP,
                    Port = rackInfo.Port,
                    Inductive = 1,
                    Enable = 1,
                    Id = IdUtils.Id,
                    ASideQty= rackInfo.ASideCount,
                    BSideQty = rackInfo.ASideCount,
                };
                rackNode.saveInit();
                //构造货架信息
                var rackinfos = new List<RackInfo>();
                for (int i = rackInfo.ASideStartIndex; i < rackInfo.ASideStartIndex + rackInfo.ASideCount; i++)
                {
                    long id = IdUtils.Id;
                    var rack = new RackInfo()
                    {
                        RackNo = rackInfo.RackNo,
                        RackSide = rackInfo.ASide,
                        IP = rackInfo.IP,
                        Port= rackInfo.Port,
                        Inductive = 1,
                        Location = i,
                        Enable = 1,
                        Id = id
                    };
                    rack.saveInit();
                    rackinfos.AddRange(rack);
                }

                for (int i = rackInfo.ASideStartIndex+ rackInfo.ASideCount; i < rackInfo.BSideStartIndex + rackInfo.BSideCount+ rackInfo.ASideCount; i++)
                {
                    long id = IdUtils.Id;
                    var rack = new RackInfo()
                    {
                        IP = rackInfo.IP,
                        Port = rackInfo.Port,
                        RackNo = rackInfo.RackNo,
                        RackSide = rackInfo.BSide,
                        Inductive = 1,
                        Location = i,
                        Enable = 1,
                        Id = id
                    };
                    rack.saveInit();
                    rackinfos.AddRange(rack);
                }
                using (var ctx = DbContext())
                {
                    using (var ts = new TransactionScope())
                    {
                        var options = new BulkCopyOptions
                        {
                            // 明确指定使用最高效的原生批量复制方式
                            BulkCopyType = BulkCopyType.ProviderSpecific,
                            // 可选：如果表有自增列，但你想插入自己的值
                            KeepIdentity = true,
                            // 可选：设置超时时间
                            //BulkCopyTimeout = 120
                        };
 
                        ctx.Insert(rackNode);
                        ctx.BulkCopy(options, rackinfos);
                        //节点写入到缓存
                        RedisClient.GetDatabase().HashSet(_rackNodeCacheKey, rackInfo.IP,JsonConvert.SerializeObject(rackNode));

                        var node = new RackNode
                        {
                            No = rackInfo.RackNo,
                            IP = rackInfo.IP,
                            Port = (ushort)rackInfo.Port,
                            Enabled = "Y",
                        };
                        SmartMaterialRackNode.Instance.AddNode(node);
                        ts.Complete();
                    }

                }

            }

            return IdsResult<object>.ok();
        }
        public IdsResult<object> CheckRegisterInfo(RegisterRackInfoDto rackInfo)
        {
            // 1. 校验字符串属性：不能为 null、空字符串或纯空格
            if (string.IsNullOrWhiteSpace(rackInfo.IP))
                return IdsResult<object>.failure("货架IP地址不能为空");
            // 1. 校验字符串属性：不能为 null、空字符串或纯空格
            if (string.IsNullOrWhiteSpace(rackInfo.RackNo))
                return IdsResult<object>.failure("货架编号不能为空");

            if (string.IsNullOrWhiteSpace(rackInfo.ASide))
                return IdsResult<object>.failure("A面标识不能为空");

            if (string.IsNullOrWhiteSpace(rackInfo.BSide))
                return IdsResult<object>.failure("B面标识不能为空");

            // 2. 校验整型属性：不能小于 0
            if (rackInfo.ASideCount < 0)
                return IdsResult<object>.failure("A面数量不能小于0");
            if (rackInfo.Port < 0)
                return IdsResult<object>.failure("端口不能小于0");
            if (rackInfo.Port < 1024)
                return IdsResult<object>.failure("端口不能小于1024");
            if (rackInfo.ASideStartIndex < 0)
                return IdsResult<object>.failure("A面起始索引不能小于0");

            if (rackInfo.BSideCount < 0)
                return IdsResult<object>.failure("B面数量不能小于0");

            if (rackInfo.BSideStartIndex < 0)
                return IdsResult<object>.failure("B面起始索引不能小于0");

            // 3. 额外的业务逻辑校验（可选）
            // 例如：如果某面数量 > 0，则起始索引必须合法；数量为0时，起始索引默认为0等
            if (rackInfo.ASideCount > 0 && rackInfo.ASideStartIndex < 1)
                return IdsResult<object>.failure("A面起始索引应大于等于1（当数量大于0时）");

            if (rackInfo.BSideCount > 0 && rackInfo.BSideStartIndex < 1)
                return IdsResult<object>.failure("B面起始索引应大于等于1（当数量大于0时）");

            // 所有校验通过
            return IdsResult<object>.ok();
        }
    }
}
