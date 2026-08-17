using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service;
using IDS.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IDS.HQ.Controller
{

    [Route("task")]
    [PropertiesAutowired]
    [ApiController]
    public class RackTaskController : ControllerBase
    {
        public IRackTaskService<RackTask> RackTaskService { get; set; }
        [HttpPost]
        [Route("PutWay")]
        public ResponseEntity<RackTask> PutWay(RequestData<RackTask> data) {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");
            IdsResult<RackTask> res = RackTaskService.Putway(data.data);
            if (res.Success)
                return ResponseEntity<RackTask>.Success(res.Data);
            else return ResponseEntity<RackTask>.Error(res.Message);
        }
    }
}
