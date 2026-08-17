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
    public class RoleGroupAdapter : SecBaseAdapter<RoleGroup>
    {
        public IRoleGroupService RoleGroupService { get; set; }
        public override IDbBaseService<RoleGroup> Service()
        {
            return RoleGroupService;
        }
        public int BatchInsert(List<RoleGroupItem> list) { 
             return RoleGroupService.BatchInsert(list);
        }
    }
}
