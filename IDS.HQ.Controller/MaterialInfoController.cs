using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IDS.HQ.Controller
{
    [Route("material-info")]
    [PropertiesAutowired]
    [ApiController]
    public class MaterialInfoController : DbBaseController<MaterialInfo>
    {
        public MaterialInfoAdapter _adapter { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<MaterialInfo> Adapter()
        {
            return _adapter;
        }

        /// <summary>
        /// 根据任务ID获取物料信息
        /// </summary>
        [HttpPost]
        [Route("get-by-taskid")]
        public ResponseEntity<MaterialInfo> GetByTaskId(RequestData<string> taskId)
        {
            var res = _adapter.GetByTaskId(taskId.data);
            if (!res.Success)
            {
                return ResponseEntity<MaterialInfo>.Error(res.Message);
            }
            return ResponseEntity<MaterialInfo>.Success(res.Data);
        }

        /// <summary>
        /// 根据料架ID获取物料信息列表
        /// </summary>
        [HttpPost]
        [Route("get-by-rackid")]
        public ResponseEntity<List<MaterialInfo>> GetByRackId(RequestData<string> rackId)
        {
            var res = _adapter.GetByRackId(rackId.data);
            if (!res.Success)
            {
                return ResponseEntity<List<MaterialInfo>>.Error(res.Message);
            }
            return ResponseEntity<List<MaterialInfo>>.Success(res.Data);
        }

        /// <summary>
        /// 根据PPID获取物料信息
        /// </summary>
        [HttpPost]
        [Route("get-by-ppid")]
        public ResponseEntity<MaterialInfo> GetByPPID(RequestData<string> ppid)
        {
            var res = _adapter.GetByPPID(ppid.data);
            if (!res.Success)
            {
                return ResponseEntity<MaterialInfo>.Error(res.Message);
            }
            return ResponseEntity<MaterialInfo>.Success(res.Data);
        }
    }
}