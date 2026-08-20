using Autofac.Core;
using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static LinqToDB.Common.Configuration;

namespace IDS.HQ.Controller
{

    [Route("task")]
    [PropertiesAutowired]
    [ApiController]
    public class RackTaskController : DbBaseController<RackTask>
    {
        public RackTaskAdapter  _adapter { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackTask> Adapter()
        {
            return _adapter;
        }

        [HttpPost]
        [Route("PutWay")]
        public ResponseEntity<RackTask> PutWay(RequestData<RackTask> data) {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");
            IdsResult<RackTask> res = _adapter.Putway(data.data);
            if (res.Success)
                return ResponseEntity<RackTask>.Success(res.Data);
            else return ResponseEntity<RackTask>.Error(res.Message);
        }
        [HttpPost]
        [Route("Outbound")]
        public ResponseEntity<RackTask> Outbound(RequestData<RackTask> data) {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");
            IdsResult<RackTask> res = _adapter.Outbound(data.data);
            if (res.Success)
                return ResponseEntity<RackTask>.Success(res.Data);
            else return ResponseEntity<RackTask>.Error(res.Message);
        } 
        [HttpPost]
        [Route("CancelTask")]
        public ResponseEntity<RackTask> CancelTask(RequestData<RackTask> data)
        {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");
            IdsResult<RackTask> res = _adapter.CancelTask(data.data);
            if (res.Success)
                return ResponseEntity<RackTask>.Success(res.Data);
            else return ResponseEntity<RackTask>.Error(res.Message);
        }

        [HttpPost]
        [Route("ForceCompleteTask")]
        public ResponseEntity<RackTask> ForceCompleteTask(RequestData<RackTask> data)
        {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");
            IdsResult<RackTask> res = _adapter.ForceCompleteTask(data.data);
            if (res.Success)
                return ResponseEntity<RackTask>.Success(res.Data);
            else return ResponseEntity<RackTask>.Error(res.Message);
        }
    }
}
