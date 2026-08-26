using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace IDS.HQ.Controller
{
    [Route("rack-running-log")]
    [PropertiesAutowired]
    [ApiController]
    public class RackRunningLogController : DbBaseController<RackRunningLog>
    {
        public RackRunningLogAdapter _adapter { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackRunningLog> Adapter()
        {
            return _adapter;
        }

        /// <summary>
        /// 根据料架号获取运行日志列表
        /// </summary>
        [HttpPost]
        [Route("get-by-rackno")]
        public ResponseEntity<List<RackRunningLog>> GetByRackNo(RequestData<string> rackNo)
        {
            var res = _adapter.GetByRackNo(rackNo.data);
            if (!res.Success)
            {
                return ResponseEntity<List<RackRunningLog>>.Error(res.Message);
            }
            return ResponseEntity<List<RackRunningLog>>.Success(res.Data);
        }

        /// <summary>
        /// 根据任务ID获取运行日志
        /// </summary>
        [HttpPost]
        [Route("get-by-taskid")]
        public ResponseEntity<RackRunningLog> GetByTaskId(RequestData<string> taskId)
        {
            var res = _adapter.GetByTaskId(taskId.data);
            if (!res.Success)
            {
                return ResponseEntity<RackRunningLog>.Error(res.Message);
            }
            return ResponseEntity<RackRunningLog>.Success(res.Data);
        }

        /// <summary>
        /// 根据日志级别获取日志列表
        /// </summary>
        [HttpPost]
        [Route("get-by-log-level")]
        public ResponseEntity<List<RackRunningLog>> GetByLogLevel([FromQuery] string logLevel)
        {
            var res = _adapter.GetByLogLevel(logLevel);
            if (!res.Success)
            {
                return ResponseEntity<List<RackRunningLog>>.Error(res.Message);
            }
            return ResponseEntity<List<RackRunningLog>>.Success(res.Data);
        }

        /// <summary>
        /// 批量删除指定时间之前的日志
        /// </summary>
        [HttpDelete]
        [Route("delete-logs-before")]
        public ResponseEntity<int> DeleteLogsBefore([FromQuery] DateTime dateTime)
        {
            var res = _adapter.DeleteLogsBefore(dateTime);
            if (!res.Success)
            {
                return ResponseEntity<int>.Error(res.Message);
            }
            return ResponseEntity<int>.Success(res.Data);
        }
    }
}