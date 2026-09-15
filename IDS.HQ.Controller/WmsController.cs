using IDS.Base;
using IDS.Base.Utils;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice;
using IDS.Extend.HYDevice.DTO;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static LinqToDB.Common.Configuration;

namespace IDS.HQ.Controller
{
    [PropertiesAutowired]
    [ApiController]
    public class WmsController
    {
        public RackTaskAdapter rackTaskAdapter { get; set; }
        public RackInfoAdapter rackInfoAdapter { get; set; }
        public IdsRedisLock IdsRedisLock { get; set; }

        [HttpPost]
        [Route("UpRackCMD")]
        public async Task<WmsResponse> UpRackCMD(WmsPuywayRequest wmsPuyway) {
            //做任务分发，处理上架和上架完成的业务转发


            string lockStr = "HQ:COMMON:PUTWAY_TASK_LOCK:" + wmsPuyway.RackId;
            string value = BaseUtil.uuid();
            var wmsResponse = new WmsResponse();
            wmsResponse.RackId = wmsPuyway.RackId;
            wmsResponse.CellId = "";
            wmsResponse.SessionId = wmsPuyway.SessionId;
            wmsResponse.Timestamp = wmsPuyway.Timestamp??DateTime.UtcNow;
            if (wmsPuyway == null || string.IsNullOrWhiteSpace(wmsPuyway.RackId))
            {
                wmsResponse.Code = 1;
                wmsResponse.Message ="上传信息为空";
                return wmsResponse;
            }
            try
            {
                if (IdsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(60)))
                {
                    try
                    {
                        IdsResult<RackTask> res = rackTaskAdapter.Putway(wmsPuyway);
                       
                        wmsResponse.Message = "上架无异常";
                        wmsResponse.Code = 0;
                        if (!res.Success)
                        {
                            wmsResponse.Code = 1;
                            wmsResponse.Message = res.Message;
                        }

                        return wmsResponse;
                    }
                    catch (Exception ex)
                    {
                        wmsResponse.Code = 1;
                        wmsResponse.Message = ex.Message;
                    }
                    finally
                    {
                        await IdsRedisLock.UnLock(lockStr, value);
                    }

                }
            }
            catch (Exception ex)
            {
                wmsResponse.Code = 1;
                wmsResponse.Message = ex.Message;
            }
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
                rackTask.AfterLightColor = wmsOutBound.LightColor;
                IdsResult<RackTask> res = rackTaskAdapter.Outbound(rackTask);
                message.Append(res.Message).Append(";");
            }
            wmsResponse.Message = message.ToString();
            return wmsResponse;
        }

        [HttpPost]
        [Route("GetShelfStatus")]
        public RackInfoResponse GetRackStatus(RackInfoRequest request) {
            int code = 0;
            string message = string.Empty;
            if (request == null && string.IsNullOrEmpty(request.RackId)) {
                code = 1;
                message = "上传的参数信息为空";
            }
            var res = rackInfoAdapter.GetRackStatus(request.RackId);
            if (res == null || res.Count == 0) {
                code = 1;
                message = $"料架系统中对该货架{request.RackId}查无记录";
            }
            var cellList =res.Select(f =>
            {
                return new CellInfo
                {
                    CellId = f.Location,
                    Side = f.RackSide,
                    Status = f.Loading,
                    Ppid = f.PPID
                };
            }).ToList();

            var resp = new RackInfoResponse { 
              Code = code,
              Message = message,
              CellList = cellList,
              Timestamp = DateTime.UtcNow.ToString(),
              SessionId = request.SessionId,
              RackId = request.RackId
            };
            return resp;
        }

        [HttpPost]
        [Route("CancelAlarm")]
        public WmsCancelAlarmResponse AlarmLightDown(WmsCancelAlarm data)
        {
            var response = new WmsCancelAlarmResponse();
            response.Code = 0;
            response.SessionId = data.SessionId;
            response.Timestamp = data.Timestamp;
            if (data == null || string.IsNullOrEmpty(data.RackId) || string.IsNullOrEmpty(data.RackSide))
            {
                response.Code = 1;
                response.Message = ["上传参数不能为空"];

            }
            byte shelfSide = data.RackSide == "A" ? (byte)0 : (byte)1;
            int locMode = 0;
            List<int> addrs = new List<int>();
            if (string.IsNullOrWhiteSpace(data.CellId))
            {
                locMode = 2;
            }
            else {
                if (data.CellId.Contains(","))
                {
                    locMode = 1;
                    addrs.AddRange(data.CellId.Split(',').Select(f =>
                    {
                        if (int.TryParse(f, out int addr))
                        {
                            return addr;
                        }
                        return -1;
                    }).Where(f=>f!=-1));
                }
                else {
                    locMode = 0;
                    if (int.TryParse(data.CellId, out int addr))
                    {
                        addrs.Add(addr);
                    }
                }
            
            }
            var alarm = new RackAlarmInfo
            {
                Side = shelfSide,
                AlarmMode = 1,
                LocationMode = locMode,
                locations = addrs,
                RackNo= data.RackId
            };
            SmartMaterialRackNode.Instance.SendAlarmNotice(alarm, null, (session) => { 
               var res = session.HandlerResult;
                if (res.Success)
                {

                    response.Code = 0;
                }
                else
                {
                    response.Code = 1;
                    response.Message = ["超时未收到设备取消成功的反馈"];
                }
            });
            return response;
        }
    }
}
