using IDS.Base.Utils;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using MySqlX.XDevAPI.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class FactoryInfoService : SecBaseService<FactoryInfo, AuthDbContext>, IFactoryInfoService
    {
        public List<FactoryInfo> SelectByAll() {
            using (var ctx = DbContext()) {

               return ctx.FactoryInfo.ToList();
            }
        }
        public List<RoleFactory> SelectByRoleId(String roleId)
        {
            using (var ctx = DbContext())
            {
                return ctx.RoleFactory.Where(f=>f.RoleId==roleId).ToList();
            }
        }
        public List<FactoryInfo> SelectFactoryByRole(List<String> list)
        {
            using (var ctx = DbContext())
            {
                var FactoryInfo = from f in ctx.FactoryInfo join r in ctx.RoleFactory on f.Id equals r.FactoryId
                         where list.Contains(r.RoleId) select f;
                //var c = ctx.FactoryInfo.Join(ctx.RoleFactory,f=>f.Id,r=>r.FactoryId,(f,r)=> f).ToList();
                return FactoryInfo.ToList();
            }
        }
        public int BatchInsertRoleFactory(List<RoleFactory> factories)
        {

            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope()) {
                    factories.ForEach(c=>c.Id  = BaseUtil.uuid());
                    if (factories.Count() == 0)
                        throw new BussinessException("参数不存在");
                    String roleId = factories[0].RoleId;
                    if (factories.Count() == 1 && "#".Equals(factories[0].FactoryId))
                    {
                        return ctx.Delete<RoleFactory>(f => f.RoleId == roleId);
                    }
                    ctx.Delete<RoleFactory>(f => f.RoleId == roleId);
                    ctx.AddRange(factories);
                    return 1;
                }          
            }
        }
    }
}
