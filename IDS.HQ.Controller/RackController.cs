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

    [Route("rack-node")]
    [PropertiesAutowired]
    [ApiController]
    public class RackController : DbBaseController<Rack>
    {
        public RackAdapter _adapter { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<Rack> Adapter()
        {
            return _adapter;
        }
    }
}
