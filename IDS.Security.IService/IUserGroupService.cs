using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IUserGroupService : ISecBaseService<UserGroup>
    {
        int BatchInsert(List<UserGroupRole> list);

        int BatchInsertUserGrp(List<UserGroupUser> list);

        List<UserInfo> QueryUserByUserGrpId(String userId);

        List<UserGroup> SelectByUserId(String userId);
    }
}
