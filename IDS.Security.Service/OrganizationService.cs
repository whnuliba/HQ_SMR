using Autofac;
using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class OrganizationService : SecBaseService<Organization, AuthDbContext>, IOrganizationService
    {
        //public IRoleInfoService RoleInfoService { get; set; }
        public VUserOrgDepartment QueryUserOrg(String username)
        {
            using (var ctx = DbContext())
            {
                return ctx.VUserOrgDepartment.Where(f => f.UserName == username).FirstOrDefault();
            }

        }

        public List<VOrganization> SelectOrgViewBy(String pid)
        {
            using (var ctx = DbContext())
            {
                return ctx.VOrganization.Where(f => f.Pid == pid).ToList();
            }
        }

        public List<OrganizationTree> GetAllUserTree(String name)
        {
            //通过name 找到人
            //通过name 找到组织
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView =QueryUserOrg(currusername);
            string orgId = userOrgDepartmentView.OrgId;
            List<VOrganization> organizationViews = new List<VOrganization>();
            using (var ctx = DbContext()) {

                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername))
                {
                    organizationViews = ctx.VOrganizationUser.Where(f => f.Code == name).ToList().Select(x => {

                        var role = new VOrganization();
                        ObjectExtensions.CopyProperties(x, role);
                        return role;
                    }).ToList();
                }
                else
                {
                    if (userOrgDepartmentView == null || string.IsNullOrWhiteSpace(userOrgDepartmentView.OrgId))
                        throw new BussinessException("当前用户不存在具体的组织，请联系管理员");
                    // organizationViews

                    var query = ctx.VOrganizationUser.Where(f => f.OrgId == userOrgDepartmentView.OrgId);
                    if (!string.IsNullOrWhiteSpace(name))
                        query = query.Where(f => f.Code == name);
                    organizationViews = query.ToList().Select(x => {

                        var role = new VOrganization();
                        ObjectExtensions.CopyProperties(x, role);
                        return role;
                    }).ToList();
                }
                if (organizationViews.Count() == 0)
                    throw new BussinessException("没有找到指定部门及部门组");
                if (string.IsNullOrWhiteSpace(name))
                    return OrganizationTree.createAllOrganizationTree(organizationViews);
                return OrganizationTree.createFilterOrganizationTree(organizationViews);


            }
        
        }

        public List<OrganizationTree> GetOrgTree()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = QueryUserOrg(currusername);
            string orgId = userOrgDepartmentView.OrgId;
            List<VOrganization> organizationViews = new List<VOrganization>();
            using (var ctx = DbContext()) {
                IRoleInfoService RoleInfoService = (IRoleInfoService)ContainerUtils.AutofacServiceProvider.GetService(typeof(RoleInfoService));
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {
                    organizationViews = ctx.VOrganization.ToList();
                }
                else
                {
                    organizationViews = ctx.VOrganization.Where(f => ctx.VUserOrgDepartment.Any(x=>x.OrgId==f.OrgId)).ToList();
                }
                if (organizationViews.Count() == 0)
                    throw new BussinessException("没有找到指定部门及部门组");
                return OrganizationTree.createAllOrganizationTree(organizationViews);
            }

          
        }

    }
}
