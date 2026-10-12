using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Controller
{
    [Route("wms-message")]
    [PropertiesAutowired]
    [ApiController]
    public class DispatchMessageController : DbBaseController<DispatchMessage>
    {
        public DispatchMessageAdapter _adapter { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<DispatchMessage> Adapter()
        {
            return _adapter;
        }

        [Route("UpdateStatus")]
        [HttpPost]
        public ResponseEntity<object> UpdateStatus(RequestData<DispatchMessage> message) {

            if (!RequestData<DispatchMessage>.isRequest(message))
                return ResponseEntity<object>.Error("请传入合法参数");
            var res = _adapter.UpdateStatus(message.data);
            if (!res.Success) return ResponseEntity<object>.Error(res.Message);
            return ResponseEntity<object>.Success("ok");
        }
    }
}
