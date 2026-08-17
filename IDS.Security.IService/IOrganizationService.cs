using IDS.Security.IService.POCO;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IOrganizationService : ISecBaseService<Organization>
    {
        VUserOrgDepartment QueryUserOrg(String username);
        List<VOrganization> SelectOrgViewBy(String pid);
        List<OrganizationTree> GetAllUserTree(String name);
        List<OrganizationTree> GetOrgTree();
    }
}
