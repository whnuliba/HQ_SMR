using IDS.Base;
using IDS.Common;
using IDS.Extension;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class UserGroupService : SecBaseService<UserGroup, AuthDbContext>, IUserGroupService
    {
        public IOrganizationService OrganizationService { get; set; }
        public IRoleInfoService RoleInfoService { get; set; }
        public int BatchInsert(List<UserGroupRole> list)
        {
            using (var ctx = DbContext()) {
                using (var ts = new TransactionScope()) {

                    int i = ctx.Count<UserGroupRole>(f => f.GroupId == list[0].GroupId);//userGroupMapper.queryCountByUserGrpId(list.get(0).getGroupId());
                    if (i > 0)
                    {
                        i = ctx.Delete<UserGroupRole>(f => f.GroupId == list[0].GroupId);
                        if (i <= 0)
                            throw new BussinessException("删除失败");
                    }
                    if (list.Count() == 1 && "#".Equals(list[0].GroupId))
                    {
                        ts.Complete();
                        return i;
                    }
                    ctx.AddRange(list);
                    ts.Complete();
                    return i;
                }
 
            }
     
        }
         public int BatchInsertUserGrp(List<UserGroupUser> list)
        {
            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope()) {
                    int i = ctx.Count<UserGroupUser>(f => f.GroupId == list[0].GroupId);//userGroupMapper.queryCountByUserGrpId(list.get(0).getGroupId());
                    if (i > 0)
                    {
                        i = ctx.Delete<UserGroupUser>(f => f.GroupId == list[0].GroupId);
                        if (i <= 0)
                            throw new BussinessException("删除失败");
                    }
                    if (list.Count() == 1 && "#".Equals(list[0].UserId))
                    {
                       
                        ts.Complete(); 
                        return i;
                    }
                    ctx.AddRange(list);
                    ts.Complete();
                    return i;
                }
            }

        }

            public List<UserInfo> QueryUserByUserGrpId(String groupId)
                {

                        using (var ctx = DbContext()) {

                            var c = from user in ctx.UserInfo join grp in ctx.UserGroupUser on user.Id equals grp.UserId
                                    where grp.GroupId == groupId select user;
                            return c.ToList();
                        }
                }


        public override Page<UserGroup> List(Page<UserGroup> page, Expression<Func<UserGroup, bool>> predicate)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (page.requestData == null)
                page.requestData= new UserGroup();
            if (userOrgDepartmentView != null && !string.IsNullOrEmpty(userOrgDepartmentView.OrgId)) {
                page.requestData.OrgId = userOrgDepartmentView.OrgId;
            }
            if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
            {
                 page.requestData.OrgId =null;
            }
            Expression<Func<UserGroup, bool>> where = null;
            if (!string.IsNullOrEmpty(page.requestData.GroupNo)){
                where = f=>f.GroupNo == page.requestData.GroupNo;
            }
            if (where != null && !string.IsNullOrEmpty(page.requestData.GroupName)) {

                where = where.And(f => f.GroupName == page.requestData.GroupName);
            }
            if (where != null && !string.IsNullOrEmpty(page.requestData.OrgId)) {
                where = where.And(f => f.OrgId == page.requestData.OrgId);
            }
            return base.List(page, predicate);
        }

        public override int save(UserGroup record, string?[] properites = null)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);
            record.OrgId = userOrgDepartmentView.OrgId;
            return base.save(record, properites);
        }

        public List<UserGroup> SelectByUserId(String userId) {
            using (var ctx = DbContext()) {

                var query = from grp in ctx.UserGroup
                            join ugu in ctx.UserGroupUser on grp.Id equals ugu.GroupId
                            where ugu.UserId == userId
                            select grp;
                return query.ToList();
            }
        }
    }
}
