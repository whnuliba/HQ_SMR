using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.IService.POCO;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class OrganizationAdapter : SecBaseAdapter<Organization>
    {
        public IOrganizationService OrganizationService { get; set; }
        public override IDbBaseService<Organization> Service()
        {
            return OrganizationService;
        }
        public VUserOrgDepartment QueryUserOrg(String username) {
            return OrganizationService.QueryUserOrg(username);
        }
        public List<VOrganization> SelectOrgViewBy(String pid)
        {
            return OrganizationService.SelectOrgViewBy(pid);
        }
        public List<OrganizationTree> GetAllUserTree(String name)
        {
            return OrganizationService.GetAllUserTree(name);
        }
        public List<OrganizationTree> GetOrgTree() { 
        return OrganizationService.GetOrgTree(); 
       }
    }
}
