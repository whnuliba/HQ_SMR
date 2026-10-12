using IDS.Common;
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
    public class DispatchMessageAdapter : DbBaseAdapter<DispatchMessage>
    {
        public IDispatchMessageService _service { get; set; }
        public override IDbBaseService<DispatchMessage> Service()
        {
            return _service;
        }

        public IdsResult<string> UpdateStatus(DispatchMessage message) {
            return _service.UpdateStatus(message);
        }
    }
}
