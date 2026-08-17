using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class RoleGroupService : SecBaseService<RoleGroup, AuthDbContext>, IRoleGroupService
    {
        public int BatchInsert(List<RoleGroupItem> list) {
            if (list == null || list.Count() == 0) {
                throw new BussinessException("参数不能为空");
            }
            using (var ctx = DbContext()) {

                int i = ctx.RoleGroupItem.Count(f => f.GroupId == list[0].GroupId);
                if (i > 0)
                {
                    ctx.Delete<RoleGroupItem>(f => f.GroupId == list[0].GroupId);
                }
                if (list.Count() == 1 && "#".Equals(list[0].RoleId))
                    return i;
                ctx.AddRange(list);
                return i;
            }
        
        }
        public override int update(RoleGroup record, string?[] properites = null) {
            string[] props = { "GroupNo", "GroupName", "GroupDesc", "Scope", "UseState", "RoleMaxUser", "RoleMaxUser" };
            return base.update(record, props);
        }
    }
}
