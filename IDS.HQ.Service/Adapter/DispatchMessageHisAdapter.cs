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
    public class DispatchMessageHisAdapter : DbBaseAdapter<DispatchMessageHis>
    {
        public IDispatchMessageHisService _service { get; set; }
        public override IDbBaseService<DispatchMessageHis> Service()
        {
            return _service;
        }
    }
}
