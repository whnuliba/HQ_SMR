using IDS.Common;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class RackTaskAdapter : DbBaseAdapter<RackTask>
    {
        public IRackTaskService _service{ set; get; }
        public override IDbBaseService<RackTask> Service()
        {
            return _service;
        }

        public IdsResult<RackTask> Putway(RackTask rackTask) {
            return _service.Putway(rackTask);
        }
        public IdsResult<RackTask> Outbound(RackTask rackTask) {
            return _service.Outbound(rackTask);
        }
        public IdsResult<RackTask> CancelTask(RackTask rackTask) {
            return _service.CancelTask(rackTask);
        }
        public IdsResult<RackTask> ForceCompleteTask(RackTask rackTask) {
            return _service.ForceCompleteTask(rackTask);
        }
    }
}
