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
    [Route("taskhis")]
    [PropertiesAutowired]
    [ApiController]
    public class RackTaskHisController : DbBaseController<RackTaskHis>
    {
        public RackTaskHisAdapter adapter { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackTaskHis> Adapter()
        {
            return adapter;
        }
    }
}
