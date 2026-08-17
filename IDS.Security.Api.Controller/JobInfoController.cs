using IDS.Base.Utils;
using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Common.Utils;

namespace IDS.Security.Api.Controller
{
    [Route("job")]
    [PropertiesAutowired]
    [ApiController]
    public class JobInfoController : DbBaseController<JobInfo>
    {
        public virtual JobInfoAdapter JobInfoAdapter { set; get; }
        public virtual ILogger<JobInfoController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<JobInfo> Adapter()
        {
            return JobInfoAdapter;
        }

        [Route("guest/items")]
        [HttpPost]
        public ResponseEntity<Page<JobInfo>> queryItems(Page<JobInfo> data)
        {
            return ResponseEntity<Page<JobInfo>>.Success(JobInfoAdapter.GetPages(data,null));
        }

        [Route("batch-job-role")]
        [HttpPost]
        public ResponseEntity<int> saveFuncByRole(RequestData<List<JobRole>> list)
        {
            if (!RequestData<List<JobRole>>.isRequest(list))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            list.data.ForEach(c=>{
                c.Id=BaseUtil.uuid();
            });
            return ResponseEntity<int>.Success(JobInfoAdapter.BatchInsert(list.data));
        }
        [Route("guest/all-job")]
        [HttpPost]
        public ResponseEntity<List<JobInfo>> queryAllJobByOrgId()
        {
            return ResponseEntity<List<JobInfo>>.Success(JobInfoAdapter.QueryAllJobByOrgId());
        }
        [Route("guest/filter-job")]
        [HttpPost]
        public ResponseEntity<List<JobInfo>> selectJobInfo(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<JobInfo>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<JobInfo>>.Success(JobInfoAdapter.SelectJobInfo(data.data));
        }
    }
}
