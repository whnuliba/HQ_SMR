using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IDS.HQ.Controller
{

    [Route("task")]
    [PropertiesAutowired]
    [ApiController]
    public class RackTaskController : DbLongBaseController<RackTask>
    {
        public RackTaskAdapter  _adapter { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbLongBaseAdapter<RackTask> Adapter()
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
    }
}
