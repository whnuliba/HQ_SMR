using IDS.Common;
using IDS.Common.Utils;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Controller
{
    [PropertiesAutowired]
    [ApiController]
    public class WmsController
    {
        public RackTaskAdapter rackTaskAdapter { get; set; }    
        [HttpPost]
        [Route("UpRackCMD")]
        public WmsResponse UpRackCMD(WmsPuywayRequest wmsPuyway) {
            //做任务分发，处理上架和上架完成的业务转发
            IdsResult<RackTask> res = rackTaskAdapter.Putway(wmsPuyway);
            var wmsResponse = new WmsResponse();
            wmsResponse.Message = "上架无异常";
            wmsResponse.Code = 0;
            if (!res.Success) {
                wmsResponse.Code = 1;
                wmsResponse.Message = res.Message;
            }
            wmsResponse.RackId = wmsPuyway.RackId;
            wmsResponse.CellId = "";
            wmsResponse.SessionId = wmsPuyway.SessionId;
            wmsResponse.Timestamp = wmsPuyway.Timestamp;        
            return wmsResponse;   
        }
        [HttpPost]
        [Route("DownRackCMD")]
        public WmsResponse WmsOutbound(WmsOutBoundRequest wmsOutBound) {
            //处理出库需要检查
            var wmsResponse = new WmsResponse();
            wmsResponse.Message = "上架无异常";
            wmsResponse.Code = 0;
            wmsResponse.RackId = "";
            wmsResponse.CellId = "";
            wmsResponse.SessionId = wmsOutBound.SessionId;
            wmsResponse.Timestamp = wmsOutBound.Timestamp;
            if (wmsOutBound == null || wmsOutBound.RackClass == null)
            {
                wmsResponse.Code = 1;
                wmsResponse.Message ="下架信息为空，或者货架号为空";
                return wmsResponse;

            }
            var rackDic = wmsOutBound.RackClass.ToDictionary(f => f.RackId, f => f.CellList);
            var message = new StringBuilder();
            foreach (var rack in rackDic) {
                //检查数据完整性
                if (string.IsNullOrEmpty(rack.Key)) {
                    wmsResponse.Code = 1;
                    wmsResponse.Message = "存在货架编码为空的报文。";
                    return wmsResponse;
                }
                if (rack.Value == null || rack.Value.Count == 0) {
                    wmsResponse.Code = 1;
                    wmsResponse.Message = $"存在货架{rack.Key}储位为空的报文！";
                    return wmsResponse;
                }
            }
            //执行保存，每个循环是一个事务。即每个货架一个事务.后续看是否需要包到一个事务中
            string groupId = IdUtils.Id + "";
            foreach (var rack in rackDic) {
                var rackTask = new RackTask();
                rackTask.RackNo = rack.Key;
                rackTask.TaskGroupId = groupId;
                rackTask.ExtendId = wmsOutBound.SessionId;
                rackTask.Locations = string.Join(",", rack.Value);
                IdsResult<RackTask> res = rackTaskAdapter.Outbound(rackTask);
                message.Append(res.Message).Append(";");
            }
            wmsResponse.Message = message.ToString();
            return wmsResponse;
        }
    }
}
