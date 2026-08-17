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
using System.Transactions;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class JobInfoService : SecBaseService<JobInfo, AuthDbContext>, IJobInfoService
    {
        public IOrganizationService OrganizationService { get; set; }

        public IRoleInfoService RoleInfoService { get; set; }
        public override int save(JobInfo jobInfo,string[] properites=null)
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            jobInfo.OrgId = userOrgDepartmentView.OrgId;
            return base.save(jobInfo);
        }

        public int BatchInsert(List<JobRole> list)
            {


            using (var ctx = DbContext())
            {
                using (var ts = new TransactionScope())
                {


                    int i = ctx.Count<JobRole>(f => f.JobId == list[0].JobId);
                    if (i > 0)
                    {
                        i = ctx.Delete<JobRole>(f => f.JobId == list[0].JobId);
                        if (i <= 0)
                            throw new BussinessException("删除部门角色失败");
                    }
                    if (list.Count() == 1 && "#".Equals(list[0].RoleId))
                        return i;
                    ctx.AddRange(list);
                    ts.Complete();
                    return i;
                }
             }
            }
    public List<JobInfo> QueryAllJobByOrgId()
        {
            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            using (var ctx = DbContext()) {
                var orgId = userOrgDepartmentView.OrgId;
               return ctx.JobInfo.Where(f => f.Scope == "1" || (f.Scope == "0" && f.OrgId == orgId)).ToList();
            }
              
        }

    public List<JobInfo> SelectJobByNos(List<String> list)
        {
            if (list == null || list.Count() == 0)
                return new List<JobInfo>();
            using (var ctx = DbContext())
            {
                return ctx.JobInfo.Where(f => list.Contains(f.JobNo)).ToList();
            }
        }

    public List<JobInfo> SelectJobInfo(String jobNo)
        {

            String currusername = CurrentUser.GetUserInfo()?.UserName;
            if (string.IsNullOrEmpty(currusername)) throw new BussinessException("当前用户未登陆");
            VUserOrgDepartment userOrgDepartmentView = OrganizationService.QueryUserOrg(currusername);

            if (userOrgDepartmentView == null)
                throw new BussinessException("当前用户不存在某一个组织，请确认");
            String orgId = "";
    
            using (var ctx = DbContext())
            {
                if (IdsConstant.SUPER_ADMIN_ACCOUNT.Equals(currusername) || RoleInfoService.IsSupperAdmin(currusername))
                {
                    orgId = userOrgDepartmentView.OrgId;
                }

                var query = ctx.JobInfo.Where(f => f.JobNo == jobNo);
                if (!string.IsNullOrWhiteSpace(orgId)) {
                    query = query.Where(f => f.Scope == "1" && (f.Scope == "0" && f.OrgId == orgId));
                }
                return query.ToList();
            }
        }
    }
}
