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
    public class RoleInfoAdapter : SecBaseAdapter<RoleInfo>
    {
        public IRoleInfoService RoleInfoService { get; set; }   
        public override IDbBaseService<RoleInfo> Service()
        {
            return RoleInfoService;
        }
        public bool IsSupperAdmin(string username) { 
          return RoleInfoService.IsSupperAdmin(username);
        }
        public List<RoleInfo> QuerySubRoleByRoleId(string id) {
            return RoleInfoService.QuerySubRoleByRoleId(id);
        }
        public List<VFunctionInfo> QueryAllFuncView() {
            return RoleInfoService.QueryAllFuncView();
        }
        public int BatchInsert(List<RoleFunction> list) { 
            return RoleInfoService.BatchInsert(list);
        }
        public List<RoleFunction> SelectByRoleId(String roleId) {
            return RoleInfoService.SelectByRoleId(roleId);
        }
        public List<RoleInfo> QueryAllRoles() { 
           return RoleInfoService.QueryAllRoles();
        }
        public int DelRole(String id) { 
            return RoleInfoService.DelRole(id);
         }
        public List<String> QueryAllowAuthAll() { 
        return RoleInfoService.QueryAllowAuthAll();
        }
        public int DeleteAllowAuthByFuncIds(List<String> list) {
            return RoleInfoService.DeleteAllowAuthByFuncIds(list);
        }
        public int ReplaceAllowAuth(List<String> list) { 
            return RoleInfoService.ReplaceAllowAuth(list);
        }
        public List<VFunctionInfo> QueryAllFuncViewAll() {
            return RoleInfoService.QueryAllFuncViewAll();
        }
        public List<RoleInfo> QueryGrpRoles(String id) {
            return RoleInfoService.QueryGrpRoles(id);
        }
        public List<RoleInfo> QueryGrpRolesByJob(String id) {
            return RoleInfoService.QueryGrpRolesByJob(id);
        }
        public List<RoleInfo> QueryGrpRolesByDept(String id) {
            return RoleInfoService.QueryGrpRolesByDept(id);
        }
        public List<RoleInfo> QueryGrpRolesByUserGroup(String id) {
            return RoleInfoService.QueryGrpRolesByUserGroup(id);
        }
        public List<RoleInfo> QueryRolesNotContainsCurrId(String id) { 
           return RoleInfoService.QueryRolesNotContainsCurrId(id);
        }
        public int BatchInsertSubRole(List<SubRole> list) {
            return RoleInfoService.BatchInsertSubRole(list);
        }

        public List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(String username, String menuRoute) {
            return RoleInfoService.QueryAllFuncViewByUserNameAndRoute(username, menuRoute);
        }
        public List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(String username, String menuRoute, String appCode) {
            return RoleInfoService.QueryAllFuncViewByUserNameAndRoute(username, menuRoute, appCode);
        }
    }
}
