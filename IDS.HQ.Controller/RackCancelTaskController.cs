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
    [Route("taskcancel")]
    [PropertiesAutowired]
    [ApiController]
    public class RackCancelTaskController : DbBaseController<RackCancelTask>
    {
        public RackCancelTaskAdapter rackCancelTask { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackCancelTask> Adapter()
        {
            return rackCancelTask;
        }
    }
}
