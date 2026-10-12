using IDS.HQ.Module;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Controller
{
    [Route("wms-message-his")]
    [PropertiesAutowired]
    [ApiController]
    public class DispatchMessageHisController : DbBaseController<DispatchMessageHis>
    {
        public DispatchMessageHisAdapter _adapter { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<DispatchMessageHis> Adapter()
        {
            return _adapter;
        }
    }
}
