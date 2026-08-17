using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class JobInfoAdapter : SecBaseAdapter<JobInfo>
    {
        public IJobInfoService JobInfoService { get; set; }
        public override IDbBaseService<JobInfo> Service()
        {
            return JobInfoService;
        }

        public int BatchInsert(List<JobRole> list) { 
           return JobInfoService.BatchInsert(list);
        }
        public List<JobInfo> QueryAllJobByOrgId()
        {
            return JobInfoService.QueryAllJobByOrgId();
        }
        public List<JobInfo> SelectJobByNos(List<String> list)
        {
            return JobInfoService.SelectJobByNos(list);
        }
        public List<JobInfo> SelectJobInfo(String jobNo)
        {
            return JobInfoService.SelectJobInfo(jobNo);
        }
    }
}
