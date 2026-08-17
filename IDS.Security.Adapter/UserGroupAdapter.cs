using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class UserGroupAdapter : SecBaseAdapter<UserGroup>
    {
        public IUserGroupService UserGroupService { get; set; }
        public override IDbBaseService<UserGroup> Service()
        {
            return UserGroupService;
        }

        public int BatchInsert(List<UserGroupRole> list) {
            return UserGroupService.BatchInsert(list);
        }

        public int BatchInsertUserGrp(List<UserGroupUser> list)
        {
            return UserGroupService.BatchInsertUserGrp(list);
        }

        public List<UserInfo> QueryUserByUserGrpId(String userId)
        {
            return UserGroupService.QueryUserByUserGrpId(userId);
        }
        public List<UserGroup> SelectByUserId(String userId)
        {
            return UserGroupService.SelectByUserId(userId);
        }
    }
}
