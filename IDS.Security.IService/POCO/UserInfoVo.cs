using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.POCO
{
    public class UserInfoVo : VDepartmentUser
    {
        public VUserRole UserRole {set;get;}
        public List<VUserRole> Roles { set; get; }
        public List<String> Factory { set; get; }
        public List<FactoryInfo>  FactoryInfo { set; get; }
        public List<string> UserGroup { set; get; }
        public List<string> RoleList { set; get; }
    }
}
