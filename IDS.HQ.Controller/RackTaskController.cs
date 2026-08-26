using Autofac.Core;
using IDS.Base;
using IDS.Base.Utils;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
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
        public RackTaskAdapter _adapter { get; set; }
        public IdsRedisLock IdsRedisLock { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RackTask> Adapter()
        {
            return _adapter;
        }

        [HttpPost]
        [Route("PutWay")]
        public async  Task<ResponseEntity<RackTask>> PutWay(RequestData<WmsPuywayRequest> data) {
            if (!RequestData<WmsPuywayRequest>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");

            if (data.data == null || string.IsNullOrWhiteSpace(data.data.RackId))
            {
                return ResponseEntity<RackTask>.Error("上传信息为空");
            }
            string lockStr = "HQ:COMMON:PUTWAY_TASK_LOCK:" + data.data.RackId;
            string value = BaseUtil.uuid();
            try
            {
                if (IdsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(60)))
                {
                    try
                    {
                        IdsResult<RackTask> res = _adapter.Putway(data.data);
                        if (res.Success)
                            return ResponseEntity<RackTask>.Success(res.Data);
                        else return ResponseEntity<RackTask>.Error(res.Message);
                    }
                    catch (Exception ex)
                    {
                        return ResponseEntity<RackTask>.Error(ex.Message);
                    }
                    finally
                    {
                       await IdsRedisLock.UnLock(lockStr, value);
                    }

                }
            }
            catch (Exception ex) {
                return ResponseEntity<RackTask>.Error(ex.Message);
            }
            return ResponseEntity<RackTask>.Error("");
        }
        [HttpPost]
        [Route("Outbound")]
        public async Task<ResponseEntity<RackTask>> Outbound(RequestData<RackTask> data) {
            if (!RequestData<RackTask>.isRequest(data))
                return ResponseEntity<RackTask>.Error("上传信息为空");



            if (data.data == null || string.IsNullOrWhiteSpace(data.data.RackNo))
            {
                return ResponseEntity<RackTask>.Error("上传信息为空");
            }
            string lockStr = "HQ:COMMON:OUTBOUND_TASK_LOCK:" + data.data.RackNo;
            string value = BaseUtil.uuid();
            try
            {
                if (IdsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(60)))
                {
                    try
                    {
                        IdsResult<RackTask> res = _adapter.Outbound(data.data);
                        if (res.Success)
                            return ResponseEntity<RackTask>.Success(res.Data);
                        else return ResponseEntity<RackTask>.Error(res.Message);
                    }
                    catch (Exception ex)
                    {
                        return ResponseEntity<RackTask>.Error(ex.Message);
                    }
                    finally
                    {
                        await IdsRedisLock.UnLock(lockStr, value);
                    }

                }
            }
            catch (Exception ex)
            {
                return ResponseEntity<RackTask>.Error(ex.Message);
            }
            return ResponseEntity<RackTask>.Error("");


           
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
