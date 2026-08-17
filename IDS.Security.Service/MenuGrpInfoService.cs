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

namespace IDS.Security.Service
{
    [AutoInjection]
    public class MenuGrpInfoService : SecBaseService<MenuGrpInfo, AuthDbContext>, IMenuGrpInfoService
    {
        public IOrganizationService OrganizationService { get; set; }
        public IRoleInfoService RoleInfoService { get; set; }
        public List<MenuGrpInfo> QueryMenuGroup()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            String orgId = userOrgDepartmentView.OrgId;
            if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
            {
                orgId = null;
            }
            using (var ctx = DbContext())
            {
                var query = ctx.MenuGrpInfo.AsQueryable();
                if (!string.IsNullOrWhiteSpace(orgId))
                {
                    query = query.Where( f => f.Status==1 && (f.Scope == "1" || (f.Scope == "0" && f.OrgId == orgId)));
                }
                return query.ToList();
            }
        }
        public override int save(MenuGrpInfo record, string?[] properites = null)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);
            if (userOrgDepartmentView == null)
                throw new BussinessException("没有找到用户指定的组织");
            record.OrgId = userOrgDepartmentView.OrgId;
            return base.save(record, properites);
        }

        public override int deleteById(string id)
        {
            using (var ctx = DbContext()) {                 
                //若已经存在菜单引用，则不可以删除
               var list = ctx.Query<MenuInfo>(f=>f.Pid==id).ToList();
                if (list.Any()) {
                    throw new BussinessException("01:An existing menu cannot be deleted directly|已经存在菜单无法直接删除");
                }
                else {
                    var group = ctx.Query<MenuGrpInfo>(f => f.Id == id).FirstOrDefault();
                    if (group==null)
                    {
                        throw new BussinessException("The menu group does not exist or has been deleted|菜单组已经不存在或已经删除");
                    }
                    list = ctx.Query<MenuInfo>(f => f.MenuGroup == id).ToList();
                    if (list.Count>0)
                    {
                        throw new BussinessException("02:An existing menu cannot be deleted directly|已经存在菜单无法直接删除");
                    }
                }
                return base.deleteById(id);
            }
    
        }
    }
}
