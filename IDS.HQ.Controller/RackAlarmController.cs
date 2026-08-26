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
    [Route("rack-alarm")]
    [PropertiesAutowired]
    [ApiController]
    public class RackAlarmController : DbBaseController<RackAlarm>
    {
        public RackAlarmAdapter _adapter { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackAlarm> Adapter()
        {
            return _adapter;
        }

        /// <summary>
        /// 根据料架号获取报警列表
        /// </summary>
        [HttpPost]
        [Route("get-by-rackno")]
        public ResponseEntity<List<RackAlarm>> GetByRackNo(RequestData<string> rackNo)
        {
            var res = _adapter.GetByRackNo(rackNo.data);
            if (!res.Success)
            {
                return ResponseEntity<List<RackAlarm>>.Error(res.Message);
            }
            return ResponseEntity<List<RackAlarm>>.Success(res.Data);
        }

        /// <summary>
        /// 根据任务ID获取报警信息
        /// </summary>
        [HttpPost]
        [Route("get-by-taskid")]
        public ResponseEntity<RackAlarm> GetByTaskId(RequestData<string> taskId)
        {
            var res = _adapter.GetByTaskId(taskId.data);
            if (!res.Success)
            {
                return ResponseEntity<RackAlarm>.Error(res.Message);
            }
            return ResponseEntity<RackAlarm>.Success(res.Data);
        }

        /// <summary>
        /// 根据处理状态获取报警列表
        /// </summary>
        [HttpPost]
        [Route("get-by-handle-state")]
        public ResponseEntity<List<RackAlarm>> GetByHandleState(RequestData<int> handleState)
        {
            var res = _adapter.GetByHandleState(handleState.data);
            if (!res.Success)
            {
                return ResponseEntity<List<RackAlarm>>.Error(res.Message);
            }
            return ResponseEntity<List<RackAlarm>>.Success(res.Data);
        }

        /// <summary>
        /// 批量更新报警处理状态
        /// </summary>
        [HttpPost]
        [Route("batch-update-handle-state")]
        public ResponseEntity<bool> BatchUpdateHandleState(RequestData<BatchUpdateHandleStateRequest> data)
        {
            var request = data.data;
            if (request == null || request.Ids == null || request.Ids.Count == 0)
            {
                return ResponseEntity<bool>.Error("ID列表不能为空");
            }

            var res = _adapter.BatchUpdateHandleState(request.Ids, request.HandleState);
            if (!res.Success)
            {
                return ResponseEntity<bool>.Error(res.Message);
            }
            return ResponseEntity<bool>.Success(res.Data);
        }
    }

    /// <summary>
    /// 批量更新处理状态请求模型
    /// </summary>
    public class BatchUpdateHandleStateRequest
    {
        public List<string> Ids { get; set; }
        public int HandleState { get; set; }
    }
}