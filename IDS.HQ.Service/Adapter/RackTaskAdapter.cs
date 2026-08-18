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
    public class RackTaskAdapter : DbLongBaseAdapter<RackTask>
    {
        public IRackTaskService _service{ set; get; }
        public override IDbLongBaseService<RackTask> Service()
        {
            return _service;
        }

        public IdsResult<RackTask> Putway(RackTask rackTask) {
            return _service.Putway(rackTask);
        }
        public IdsResult<RackTask> Outbound(RackTask rackTask) {
            return _service.Outbound(rackTask);
        }
    }
}
