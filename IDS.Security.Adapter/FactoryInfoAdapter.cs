using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class FactoryInfoAdapter : SecBaseAdapter<FactoryInfo>
    {
        public IFactoryInfoService FactoryInfoService { set; get; }
        public override IDbBaseService<FactoryInfo> Service()
        {
            return FactoryInfoService;
        }
        public List<FactoryInfo> SelectByAll()
        {
            return FactoryInfoService.SelectByAll();
        }
        public List<RoleFactory> SelectByRoleId(String roleId)
        {
            return FactoryInfoService.SelectByRoleId(roleId);
        }

        public int BatchInsertRoleFactory(List<RoleFactory> factories)
        {
            return FactoryInfoService.BatchInsertRoleFactory(factories);
        }
    }
}
