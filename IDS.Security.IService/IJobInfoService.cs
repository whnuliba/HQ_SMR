using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IJobInfoService : ISecBaseService<JobInfo>
    {
        int BatchInsert(List<JobRole> list);
        List<JobInfo> QueryAllJobByOrgId();
        List<JobInfo> SelectJobByNos(List<String> list);
        List<JobInfo> SelectJobInfo(String jobNo);
    }
}
