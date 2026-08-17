using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IRoleGroupService : ISecBaseService<RoleGroup>
    {
        public int BatchInsert(List<RoleGroupItem> list);
    }
}
