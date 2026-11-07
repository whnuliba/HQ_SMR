using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
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
        [HttpPost]
        [Route("all-racknodes")]
        public ResponseEntity<List<Rack>> GetAllRackNode() { 
           var res = _adapter.GetAllRackNode();
            return ResponseEntity<List<Rack>>.Success(res.Data);
        }

        
        [HttpPost]
        [Route("UpdateRack")]
        public ResponseEntity<string> UpdateRackInfo(RequestData<Rack> data)
        {
            if (!RequestData<Rack>.isRequest(data))
                return ResponseEntity<string>.Error("上传信息为空");
            var res = _adapter.UpdateRackInfo(data.data);
            if(res.Success)
              return ResponseEntity<string>.Success("");
            return ResponseEntity<string>.Error(res.Message);
        }
    }
}
