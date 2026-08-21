using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class RackCancelTaskAdapter : DbBaseAdapter<RackCancelTask>
    {
        public IRackCancelTaskService _rackCancelTask { set; get; }
        public override IDbBaseService<RackCancelTask> Service()
        {
           return _rackCancelTask;
        }
    }
}
