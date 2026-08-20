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
    public class RackAdapter : DbBaseAdapter<Rack>
    {
        public IRackService _service { set; get; }
        public override IDbBaseService<Rack> Service()
        {
            return _service;
        }
    }
}
