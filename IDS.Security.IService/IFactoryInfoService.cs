using IDS.Security.Module;
//using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IFactoryInfoService : ISecBaseService<FactoryInfo>
    {
        public List<FactoryInfo> SelectByAll();
        List<RoleFactory> SelectByRoleId(String roleId);
        int BatchInsertRoleFactory(List<RoleFactory> factories);

        List<FactoryInfo> SelectFactoryByRole(List<String> list);
    }
}
