using Google.Protobuf.Collections;
using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using IDS.Extension;
using System.Transactions;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class RoleInfoService : SecBaseService<RoleInfo, AuthDbContext>, IRoleInfoService
    {
        public IOrganizationService OrganizationService { get; set; }
        public bool IsSupperAdmin(string username)
        {
            using (var ctx = DbContext()) {

                List<VUserRole> roleInfos = ctx.VUserRole.Where(f => f.UserName == username).ToList();
                List<String> roleStrList = roleInfos.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                    return true;
                return false;
            }


        }

        public List<VFunctionInfo> QueryAllFuncView()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");


            using (var ctx = DbContext()) {

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername)) {
                   return  ctx.VFunctionInfo.Join(ctx.AllowAuthorized, e => e.Id, o => o.FuncId, (e, o) => e).ToList();
                }
                var cUserRole = ctx.VUserRole.Where(f => f.UserName == currusername).ToList();
                List<String> roleStrList = cUserRole.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    return ctx.VFunctionInfo.Join(ctx.AllowAuthorized, e => e.Id, o => o.FuncId, (e, o) => e).ToList();
                }
                var roles = from vf in ctx.VFunctionInfo
                            join
                            auth in ctx.AllowAuthorized on vf.Id equals auth.FuncId
                            where ((vf.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == currusername select vd.OrgId).Contains(vf.OrgId)) || vf.Scope == "1")
                            select vf;

                return roles.ToList();
            }

        }

        public int BatchInsert(List<RoleFunction> list)
        {
            //获取当前用户下的功能
            using (var ctx = DbContext()) {
                List<VFunctionInfo> userFuncList = QueryAllFuncView();
                if (userFuncList.Count() == 0)
                    throw new BussinessException("当前角色或者本用户未授权");
                List<String> delRoleFuncIds = new List<String>();
                List<RoleFunction> listArr = new List<RoleFunction>();
                List<String> notDelRoleFuncIds = list.Select(c => c.FuncId).ToList();
                List<String> userFuncIds = userFuncList.Select(c => c.Id).ToList();
                List<RoleFunction> roleFuncAll =ctx.RoleFunction.Where(f=>f.RoleId== list[0].RoleId).ToList();

                List<RoleFunction> userUseFuncList = new List<RoleFunction>();
                roleFuncAll.ForEach(c=>{
                    if (userFuncIds.Contains(c.FuncId))
                    {
                        userUseFuncList.Add(c);
                        if (!notDelRoleFuncIds.Contains(c.FuncId))
                        {
                            delRoleFuncIds.Add(c.Id);
                        }
                    }
                });
                Dictionary<string, string> funcIdMap = new Dictionary<string, string>();// userUseFuncList.ToDictionary(f=>f.FuncId,f=>f.Id);

                userUseFuncList.ForEach(f =>
                {
                    if (funcIdMap.ContainsKey(f.FuncId))
                    {
                        funcIdMap[f.FuncId] = f.Id;
                    }
                    else {
                        funcIdMap.Add(f.FuncId, f.Id);
                    }
                });
                //获取需要新增的功能
                list.ForEach(c=>{
                    if (!funcIdMap.ContainsKey(c.FuncId))
                    {
                        listArr.Add(c);
                    }
                });

                //获取需要删除的功能
                using (var ts = new TransactionScope()) {

                    int i = 0;
                    if (delRoleFuncIds.Count() > 0)
                    {

                        List<List<string>> deleteFuncIds = delRoleFuncIds.Partition<string>(100);

                        foreach (List<string> funcIds in deleteFuncIds)
                        {
                            i += ctx.Delete<RoleFunction>(f => funcIds.Contains(f.Id));
                        }
                    }

                    if (notDelRoleFuncIds.Count() == 1 && "#".Equals(notDelRoleFuncIds[0]))
                       { 
                        ts.Complete();
                        return i;
                    }
                    if (listArr.Count() > 0)
                    {
                        List<List<RoleFunction>> insertLists = listArr.Partition(100);
                        foreach (List<RoleFunction> ll in insertLists)
                        {
                            ctx.AddRange(ll);
                        }
                    }
                    ts.Complete();
                }

            }
            return 0;
        }


        public List<RoleFunction> SelectByRoleId(String roleId)
        {
            using (var ctx = DbContext()) {
                return ctx.RoleFunction.Where(f => f.RoleId == roleId).ToList();
            }
        }

        public List<RoleInfo> QueryAllRoles()
        {

          List<String> adminRoles = new List<String>(){
           IdsConstant.ADMIN_ROLE.Substring(5),
           IdsConstant.SUPER_ADMIN_ROLE.Substring(5)
          };
         String currusername = CurrentUser.GetUserInfo()?.UserName;
         if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
         using (var ctx = DbContext()) {
              VUserOrgDepartment userOrgDepartmentView =  OrganizationService.QueryUserOrg(currusername);

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var roleQuery = ctx.VRoleAndGroup.ToList();
                    return roleQuery.Select(item => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(item, role);
                        return role;
                    }).ToList();
                }

                var roleStrList = ctx.VUserRole.Where(f => f.UserName == currusername).Select(f=>f.RoleCode ).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    var roleQuery = ctx.VRoleAndGroup.ToList();
                    return roleQuery.Select(item => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(item, role);
                        return role;
                    }).ToList();
                }
                var roleQuery1 = ctx.VRoleAndGroup.Where(f=> !adminRoles.Contains(f.RoleCode) && (f.Scope=="1" || (f.Scope=="0" && f.OrgId== userOrgDepartmentView.OrgId))).ToList();
                return roleQuery1.Select(item => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(item, role);
                    return role;
                }).ToList();
            }
        }
        public int DelRole(String id)
        {
            using (var ctx = DbContext()) {
                using (var ts = new TransactionScope()) {
                    try {
                        ctx.Delete<RoleFunction>(f => f.RoleId == id);
                        ctx.Delete<UserRole>(f => f.RoleId == id);
                        ctx.Delete<RoleInfo>(f => f.Id == id);
                        ts.Complete();
                        return 1;
                    } catch (Exception ex) {
                        throw ex;
                    }
                
                }
            }
        }
        public List<String> QueryAllowAuthAll()
        {
            using (var ctx = DbContext()) { 
              return ctx.AllowAuthorized.Select(c=>c.FuncId).ToList();
            }
        }
        public int DeleteAllowAuthByFuncIds(List<String> list)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope()) {

                    int i =  ctx.Delete<AllowAuthorized>(c => list.Contains(c.FuncId));
                    ts.Complete();
                    return i;
                }
            }
        }

        public int ReplaceAllowAuth(List<String> list)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {
                    List<String> funcList = QueryAllowAuthAll();
                    List<String> delList = funcList.Where(c=>!list.Contains(c)).ToList();
                    List<String> alreadyList = funcList.Where(c=>list.Contains(c)).ToList();
                    List<AllowAuthorized> insertList = list.Where(c=>!alreadyList.Contains(c)).Select(c=>new AllowAuthorized { 
                        FuncId =c 
                    }).ToList();
                    int i = 0;
                    if (delList.Count() > 0)
                    {
                        List<List<String>> delLists = delList.Partition(100);
                        foreach (List<String> s in delLists)
                        {
                            i += ctx.Delete<AllowAuthorized>(f => s.Contains(f.FuncId));
                        }
                        if (i <= 0)
                        {
                            throw new BussinessException("高级授权变更失败");
                        }
                    }
                    if (insertList.Count() == 0)
                    {
                        ts.Complete();
                        return 0;
                    }
                    List<List<AllowAuthorized>> insertLists = insertList.Partition(100);
                    foreach (List<AllowAuthorized> s1  in insertLists)
                    {
                         ctx.AddRange(s1);
                    }
                    ts.Complete();
                    return i;
                }
            }
      
        }
        public List<VFunctionInfo> QueryAllFuncViewAll()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            using (var ctx = DbContext()) {
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    return ctx.VFunctionInfo.ToList();
                }
                List<RoleInfo> roleInfos =ctx.VUserRole.Where(f=>f.UserName== currusername).ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                }).ToList();

                 var roleStrList = roleInfos.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    return ctx.VFunctionInfo.ToList();
                }
                return ctx.VFunctionInfo.Where(f=>f.Scope=="1" || (f.Scope == "0" && ctx.VUserOrgDepartment.Where(x=>x.UserName== currusername).Select(x=>x.OrgId).Contains(f.OrgId))).ToList();

            }
     
        }
        public List<RoleInfo> QueryGrpRoles(String id)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);
            using (var ctx = DbContext()) {
                if (userOrgDepartmentView == null)
                    throw new BussinessException("当前用户不存在某一个组织，请确认");
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var roles = from role in ctx.RoleInfo
                            join roleItem in ctx.RoleGroupItem on role.Id equals roleItem.RoleId
                            join grp in ctx.RoleGroup on roleItem.GroupId equals grp.Id
                            where grp.Id == id
                            select role;
                    return roles.ToList();
                }

                 var roleInfos = ctx.VUserRole.Where(f=>f.UserName== currusername).ToList().Select(f=> {
                     var role = new RoleInfo();
                     ObjectExtensions.CopyProperties(f, role);
                     return role;
                 });
                var roleStrList = roleInfos.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    var roles = from role in ctx.RoleInfo
                                join roleItem in ctx.RoleGroupItem on role.Id equals roleItem.RoleId
                                join grp in ctx.RoleGroup on roleItem.GroupId equals grp.Id
                                where grp.Id == id
                                select role;
                    return roles.ToList();
                }


                var roles1 = from role in ctx.RoleInfo
                            join roleItem in ctx.RoleGroupItem on role.Id equals roleItem.RoleId
                            join grp in ctx.RoleGroup on roleItem.GroupId equals grp.Id
                            where grp.Id == id && (grp.Scope=="1" || (grp.Scope == "0" && grp.OrgId== userOrgDepartmentView.OrgId))
                            select role;
                return roles1.ToList();

            }

        }


        public List<RoleInfo> QueryGrpRolesByJob(String id)
        {
   
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            using (var ctx = DbContext()) {
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var roles = from role in ctx.VRoleAndGroup  
                                join jobrole in ctx.JobRole on role.Id equals jobrole.RoleId
                                join job in ctx.JobInfo on jobrole.JobId equals job.Id
                                where job.Id==id select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }
                var roleInfos = ctx.VUserRole.Where(f => f.UserName == currusername).ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                });

                List<String> roleStrList = roleInfos.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    var roles = from role in ctx.VRoleAndGroup
                                join jobrole in ctx.JobRole on role.Id equals jobrole.RoleId
                                join job in ctx.JobInfo on jobrole.JobId equals job.Id
                                where job.Id == id
                                select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }

                var roles1 = from role in ctx.VRoleAndGroup
                            join jobrole in ctx.JobRole on role.Id equals jobrole.RoleId
                            join job in ctx.JobInfo on jobrole.JobId equals job.Id
                            where job.Id == id && (job.Scope == "1" || (job.Scope == "0" && job.OrgId == userOrgDepartmentView.OrgId))
                            select role;

                return roles1.ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                }).ToList();
            }
    
        }


        public List<RoleInfo> QueryGrpRolesByDept(String id)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            using (var ctx = DbContext()) {
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var roles = from role in ctx.VRoleAndGroup
                                join jobrole in ctx.DepartmentRole on role.Id equals jobrole.RoleId
                                join job in ctx.Department on jobrole.DeptId equals job.Id
                                where job.Id == id
                                select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }
                var roleInfos = ctx.VUserRole.Where(f => f.UserName == currusername).ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                });

                var roleStrList = roleInfos.Select(c => c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    var roles = from role in ctx.VRoleAndGroup
                                join jobrole in ctx.DepartmentRole on role.Id equals jobrole.RoleId
                                join job in ctx.Department on jobrole.DeptId equals job.Id
                                where job.Id == id
                                select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }


                var roles1 = from role in ctx.VRoleAndGroup
                            join jobrole in ctx.DepartmentRole on role.Id equals jobrole.RoleId
                            join job in ctx.Department on jobrole.DeptId equals job.Id
                            where job.Id == id && job.OrgId == userOrgDepartmentView.OrgId
                             select role;

                return roles1.ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                }).ToList();
            }

        }
        public List<RoleInfo> QueryGrpRolesByUserGroup(String id) {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            using (var ctx = DbContext()) {
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var roles = from role in ctx.VRoleAndGroup
                                join jobrole in ctx.UserGroupRole on role.Id equals jobrole.RoleId
                                join job in ctx.UserGroup on jobrole.GroupId equals job.Id
                                where job.Id == id
                                select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }
                var roleInfos = ctx.VUserRole.Where(f => f.UserName == currusername).ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                });

                var roleStrList = roleInfos.Select(c => c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    var roles = from role in ctx.VRoleAndGroup
                                join jobrole in ctx.UserGroupRole on role.Id equals jobrole.RoleId
                                join job in ctx.UserGroup on jobrole.GroupId equals job.Id
                                where job.Id == id
                                select role;

                    return roles.ToList().Select(f => {
                        var role = new RoleInfo();
                        ObjectExtensions.CopyProperties(f, role);
                        return role;
                    }).ToList();
                }

                var roles1 = from role in ctx.VRoleAndGroup
                            join jobrole in ctx.UserGroupRole on role.Id equals jobrole.RoleId
                            join job in ctx.UserGroup on jobrole.GroupId equals job.Id
                             where job.Id == id && (job.Scope == "1" || (job.Scope == "0" && job.OrgId == userOrgDepartmentView.OrgId))
                             select role;

                return roles1.ToList().Select(f => {
                    var role = new RoleInfo();
                    ObjectExtensions.CopyProperties(f, role);
                    return role;
                }).ToList();
            }

     }

        public List<RoleInfo> QuerySubRoleByRoleId(String id)
        {
            using (var ctx = DbContext()) {
                var c = from role in ctx.RoleInfo join sub in ctx.SubRole on role.Id equals sub.SubRoleId
                        where sub.RoleId == id select role;
                return c.ToList();
            }
        }

        public List<RoleInfo> QueryRolesNotContainsCurrId(String id) {

            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");

            using (var ctx = DbContext()) {
                List<String> adminRoles = new List<String>(){ IdsConstant.ADMIN_ROLE.Substring(5),IdsConstant.SUPER_ADMIN_ROLE.Substring(5)};

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    var query = ctx.RoleInfo.Where(f=>f.Id!=id && !adminRoles.Contains(f.RoleCode));
       
                    return query.ToList();
                }

                List<RoleInfo> roleInfos =ctx.VUserRole.Where(f=>f.UserName==currusername).Select(vr => new RoleInfo {
                    Id = vr.Id,
                    RoleCode = vr.RoleCode,
                    RoleName = vr.RoleName,
                    RoleType = vr.RoleType,
                    OrgId=vr.OrgId,
                    Scope=vr.Scope,
                    Status=vr.Status,
                    CreateDate=vr.CreateDate,
                    LastModifyDate =vr.LastModifyDate,
                    CreateUser=vr.CreateUser,
                    LastModifyUser  =vr.LastModifyUser,
                    UseState=vr.UseState,
                    RoleMaxUser=vr.RoleMaxUser,

                }).ToList();
                List<String> roleStrList = roleInfos.Select(c=>c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {

                    var query = ctx.RoleInfo.Where(f => f.Id != id && !adminRoles.Contains(f.RoleCode));
                    return query.ToList();
                }

                var query1 = ctx.RoleInfo.Where(f => f.Id != id && f.OrgId== userOrgDepartmentView .OrgId && !adminRoles.Contains(f.RoleCode));
                return query1.ToList();
            }
        }


        public int BatchInsertSubRole(List<SubRole> list)
        {

            using (var ctx = DbContext()) {
                using (var ts = new TransactionScope()) {

                    int i = ctx.Count<SubRole>(f => f.RoleId == list[0].RoleId);// roleInfoMapper.querySubCountByRoleId(list.get(0).getRoleId());
                    if (i > 0)
                    {
                        i = ctx.Delete<SubRole>(f => f.RoleId == list[0].RoleId);
                        if (i <= 0)
                            throw new BussinessException("删除失败");
                    }
                    if (list.Count() == 1 && "#".Equals(list[0].SubRoleId))
                    {
                        ts.Complete();
                        return 1;
                    }
                    ctx.AddRange(list);
                    ts.Complete();
                    return i;
                }
            
            }

        }
        public override int save(RoleInfo record, string?[] properites = null)
        {
            using (var ctx = DbContext()) {
                String currusername = CurrentUser.GetUserInfo()?.UserName;
                if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
                VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

                if (userOrgDepartmentView == null)
                    throw new BussinessException("当前用户不存在某一个组织，请确认");
                record.OrgId = userOrgDepartmentView.OrgId;
                record.RoleType =0;
            }
            return base.save(record, properites);
        }
        public override Page<RoleInfo> List(Page<RoleInfo> page, Expression<Func<RoleInfo, bool>> predicate)
        {
            using (var ctx = DbContext())
            {

                if (page.requestData == null)
                    page.requestData = new RoleInfo();
                List<String> adminRoles = new List<String>(){
                 IdsConstant.ADMIN_ROLE.Substring(5),
                IdsConstant.SUPER_ADMIN_ROLE.Substring(5)

              };
                bool isAdmin = false;
                String currusername = CurrentUser.GetUserInfo()?.UserName;
                if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
                VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

                if (userOrgDepartmentView == null)
                    throw new BussinessException("当前用户不存在某一个组织，请确认");
                page.requestData.OrgId = (userOrgDepartmentView == null ? null : userOrgDepartmentView.OrgId);

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    isAdmin = true;
                }
                var roleInfos = ctx.VUserRole.Where(f => f.UserName == currusername).Select(vr => new RoleInfo
                {
                    Id = vr.Id,
                    RoleCode = vr.RoleCode,
                    RoleName = vr.RoleName,
                    RoleType = vr.RoleType,
                    OrgId = vr.OrgId,
                    Scope = vr.Scope,
                    Status = vr.Status,
                    CreateDate = vr.CreateDate,
                    LastModifyDate = vr.LastModifyDate,
                    CreateUser = vr.CreateUser,
                    LastModifyUser = vr.LastModifyUser,
                    UseState = vr.UseState,
                    RoleMaxUser = vr.RoleMaxUser,
                }).ToList();
                List<String> roleStrList = roleInfos.Select(c => c.RoleCode).ToList();
                if (roleStrList.Contains(IdsConstant.SUPER_ADMIN_ROLE.Substring(5)))
                {
                    isAdmin = true;
                }
                if (!isAdmin)
                {
                    predicate = f => ((f.OrgId == userOrgDepartmentView.OrgId && f.Scope == "0") || f.Scope == "1") && !adminRoles.Contains(f.RoleCode);
                }
                var req = page.requestData;
                var data = ctx.Query<RoleInfo>(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
                var count = ctx.Count<RoleInfo>(predicate);
                Page<RoleInfo> page1 = new Page<RoleInfo>(count, data, page.pageSize, page.current);
                return page1;
            }
        }

        public List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(string username, string menuRoute)
        {
            using (var ctx = DbContext()) {
                var dms = from menu in ctx.VUserRoleFunction
                          join auth in ctx.AllowAuthorized on menu.Id equals auth.FuncId
                          where menu.Status == 1 && menu.MenuType==1 && menu.UserName == username && menu.MenuRoute == menuRoute
                          && ((menu.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == username select vd.OrgId).ToList().Contains(menu.OrgId)) || menu.Scope == "1")
                          select new VFunctionInfo
                          {
                              Id = menu.Id,
                              CreateDate = menu.CreateDate ?? DateTime.Now,
                              CreateUser = menu.CreateUser,
                              LastModifyDate = menu.LastModifyDate,
                              LastModifyUser = menu.LastModifyUser,
                              Status = menu.Status,
                              MenuRoute = menu.MenuRoute,
                              FuncName = menu.FuncName,
                              MenuNameEn = menu.MenuNameEn,
                              FuncCode = menu.FuncCode,
                              Pid = menu.Pid,
                              Sort = menu.Sort,
                              MenuType = menu.MenuType,
                              TextIcon = menu.TextIcon,
                              MenuGroup = menu.MenuGroup,
                              Href = menu.Href,
                              Component = menu.Component,
                              OrgId = menu.OrgId,
                              Platform = menu.Platform,
                              Udf1 = menu.Udf1,
                              Udf2 = menu.Udf2,
                              Udf3 = menu.Udf3,
                              Udf4 = menu.Udf4,
                              Udf5 = menu.Udf5,
                              Udf6 = menu.Udf6

                          };
                return dms.ToList();


            }
        }

        public List<VFunctionInfo> QueryAllFuncViewByUserNameAndRoute(string username, string menuRoute, string appCode)
        {
            using (var ctx = DbContext())
            {
                var dms = from menu in ctx.VUserRoleFunction
                          join auth in ctx.AllowAuthorized on menu.Id equals auth.FuncId
                          where menu.Status == 1 && menu.MenuType == 1 && menu.MenuGroup== appCode && menu.UserName == username && menu.MenuRoute == menuRoute
                          && ((menu.Scope == "0" && (from vd in ctx.VUserOrgDepartment where vd.UserName == username select vd.OrgId).ToList().Contains(menu.OrgId)) || menu.Scope == "1")
                          select new VFunctionInfo
                          {
                              Id = menu.Id,
                              CreateDate = menu.CreateDate ?? DateTime.Now,
                              CreateUser = menu.CreateUser,
                              LastModifyDate = menu.LastModifyDate,
                              LastModifyUser = menu.LastModifyUser,
                              Status = menu.Status,
                              MenuRoute = menu.MenuRoute,
                              FuncName = menu.FuncName,
                              MenuNameEn = menu.MenuNameEn,
                              FuncCode = menu.FuncCode,
                              Pid = menu.Pid,
                              Sort = menu.Sort,
                              MenuType = menu.MenuType,
                              TextIcon = menu.TextIcon,
                              MenuGroup = menu.MenuGroup,
                              Href = menu.Href,
                              Component = menu.Component,
                              OrgId = menu.OrgId,
                              Platform = menu.Platform,
                              Udf1 = menu.Udf1,
                              Udf2 = menu.Udf2,
                              Udf3 = menu.Udf3,
                              Udf4 = menu.Udf4,
                              Udf5 = menu.Udf5,
                              Udf6 = menu.Udf6

                          };
                return dms.ToList();

            }
        }
    }
}
