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
    public class RackTaskHisAdapter : DbBaseAdapter<RackTaskHis>
    {
        public IRackTaskHisService _rackRackTaskHis { set; get; }
        public override IDbBaseService<RackTaskHis> Service()
        {
            return _rackRackTaskHis;
        }
    
    }
}
