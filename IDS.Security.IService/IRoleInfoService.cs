using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IRoleInfoService : ISecBaseService<RoleInfo>
    {
        bool IsSupperAdmin(string username);
        List<VFunctionInfo> QueryAllFuncView();
        int BatchInsert(List<RoleFunction> list);
        List<RoleFunction> SelectByRoleId(String roleId);
        List<RoleInfo> QueryAllRoles();
        int DelRole(String id);
        List<String> QueryAllowAuthAll();
        int DeleteAllowAuthByFuncIds(List<String> list);
        int ReplaceAllowAuth(List<String> list);
        List<VFunctionInfo> QueryAllFuncViewAll();
        List<RoleInfo> QueryGrpRoles(String id);
        public List<RoleInfo> QueryGrpRolesByJob(String id);
        List<RoleInfo> QueryGrpRolesByDept(String id);
        List<RoleInfo> QueryGrpRolesByUserGroup(String id);
        List<RoleInfo> QuerySubRoleByRoleId(string id);
        List<RoleInfo> QueryRolesNotContainsCurrId(string id);
        int BatchInsertSubRole(List<SubRole> list);

        List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(String username, String menuRoute);
        List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(String username, String menuRoute, String appCode);


    }
}
