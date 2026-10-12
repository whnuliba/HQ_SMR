using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice.ReceiveHandler;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Ioc;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace IDS.Extend.HYDevice.Dispatch
{
    public class TaskReturnWmsDispatchHandler
    {
        private static readonly Lazy<TaskReturnWmsDispatchHandler> _instance = new Lazy<TaskReturnWmsDispatchHandler>(() => new TaskReturnWmsDispatchHandler());
        public static TaskReturnWmsDispatchHandler Instance => _instance.Value;
        public ILog Logger = LogManager.GetLogger(typeof(TaskReturnWmsDispatchHandler));

        private TaskReturnWmsDispatchHandler() { }

        public async Task SendLocationInfoChange(object req, object resp,int status=1,string bizStatus ="0", string io = "I") {
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using(var ctx = dbContext.CreateDbContext())
            {
                try {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var dispatchMessage = new DispatchMessageHis
                    {
                        Id = IdUtils.Id + "",
                        Message = req == null ? "" : req is string ? req?.ToString() : JsonSerializer.Serialize(req, options),
                        RespMessage = resp == null ? "" : resp is string ? resp?.ToString() : JsonSerializer.Serialize(resp, options),
                        CreateTime = DateTime.Now,
                        Status = status,
                        BizStatus = bizStatus,
                        IOStatus = io
                    };
                    await ctx.DispatchMessageHis.AddAsync(dispatchMessage);
                    await ctx.SaveChangesAsync();
                } catch (Exception ex) {
                    string msg = $"接口日志保存失败Request: {req},Response:{resp},{ex.Message}";
                    Logger.Error(msg);
                    IdsMessageHandler<object>.ErrorMessage(msg);

                }

            }
        }

        //架回调WMS接口：用于处理WMS业务数据
        public void SendLocationInfoChange<E>(List<E> datas,string taskId) where E : LocationInfoChangeData
        {
            string servicePath = AppConfig.GetConfigInfo("AddressPool:WmsUrl");
            string token = AppConfig.GetConfigInfo("AddressPool:WmsToken");
            var start = new Stopwatch();
            // 1. 构造请求
            var request = new ApiRequest<E>
            {
                ApiReqHeader = new ApiRequestHeader
                {
                    RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                    SourceSystem = "HY_SMR",
                    ServiceName = "LocationInfoChange",
                    AppVersion = "V1.0",
                    Token = token//
                },
                ApiReqData = datas
            };
            // 2. 序列化为JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string jsonRequest = JsonSerializer.Serialize(request, options);
            int sendState = 1;
            string respMessage = string.Empty ;
            try
            {
                start.Start();
                respMessage = HttpUtil.Post(servicePath, jsonRequest,null,1000*30 ).Result;
                var responseData = ParseResponse<ApiResponseData>(respMessage);
                start.Stop();

                if (responseData == null || responseData.ApiResHeader.ApiCode!= "0x000000") // "APICode": "0x000000",--成功0x000000，失败0x000009
                {
                    sendState = 1;
                }
                Logger.Info($"发送WMS报文[LocationInfoChange]完成:{start.ElapsedMilliseconds},Request:{jsonRequest},返回:{respMessage}");
            }
            catch (Exception ex) {
                sendState =0;
                respMessage = ex.Message;
                start.Stop();
                string msg = $"发送WMS报文[LocationInfoChange]出现错误了时长:{start.ElapsedMilliseconds},Request:{jsonRequest},错误:{ex.Message}";
                Logger.Error(msg);
                IdsMessageHandler<object>.ErrorMessage(msg);
                //输出到控制台

            }
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext()) {

                if (sendState == 0) {
                    var message = new DispatchMessage
                    {
                        RackId = string.Join(",", request.ApiReqData.Select(f => f.RackId).ToList()),
                        RequestId = request.ApiReqHeader.RequestId,
                        Message = jsonRequest,
                        RespMessage = respMessage,
                        Status = sendState,
                        BizStatus = request.ApiReqData[0].Status,
                        TaskId = taskId,
                        PostCount = 1,
                        IOStatus = "O"
                    };
                    message.saveInit();
                    message.Id = IdUtils.Id + "";
                    ctx.Insert(message);
                }
                if (sendState == 1)
                {
                    var message = new DispatchMessageHis
                    {
                        RackId = string.Join(",", request.ApiReqData.Select(f => f.RackId).ToList()),
                        RequestId = request.ApiReqHeader.RequestId,
                        Message = jsonRequest,
                        RespMessage = respMessage,
                        Status = sendState,
                        BizStatus = request.ApiReqData[0].Status,
                        TaskId = taskId,
                        PostCount = 1,
                        IOStatus = "O"
                    };
                    message.saveInit();
                    message.Id = IdUtils.Id + "";
                    ctx.Insert(message);
                }
            }

        }

        public async Task SendLocationInfoChangeAsync<E>(List<E> datas, string taskId) where E : LocationInfoChangeData
        {
            string servicePath = AppConfig.GetConfigInfo("AddressPool:WmsUrl");
            string token = AppConfig.GetConfigInfo("AddressPool:WmsToken");
            var start = new Stopwatch();
            // 1. 构造请求
            var request = new ApiRequest<E>
            {
                ApiReqHeader = new ApiRequestHeader
                {
                    RequestId = Guid.NewGuid().ToString("N").ToUpper(),
                    SourceSystem = "HY_SMR",
                    ServiceName = "LocationInfoChange",
                    AppVersion = "V1.0",
                    Token = token//
                },
                ApiReqData = datas
            };
            // 2. 序列化为JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string jsonRequest = JsonSerializer.Serialize(request, options);
            int sendState = 1;
            string respMessage = string.Empty;
            try
            {
                start.Start();
                respMessage = await HttpUtil.Post(servicePath, jsonRequest,null, 1000 * 30);
                var responseData = ParseResponse<ApiResponseData>(respMessage);
                start.Stop();

                if (responseData == null || responseData.ApiResHeader.ApiCode != "0x000000") // "APICode": "0x000000",--成功0x000000，失败0x000009
                {
                    sendState = 1;
                }
                Logger.Info($"发送WMS报文[LocationInfoChange]完成:{start.ElapsedMilliseconds},Request:{jsonRequest},返回:{respMessage}");
            }
            catch (Exception ex)
            {
                sendState = 0;
                respMessage = ex.Message;
                start.Stop();
                string msg = $"发送WMS报文[LocationInfoChange]出现错误了时长:{start.ElapsedMilliseconds},Request:{jsonRequest},错误:{ex.Message}";
                Logger.Error(msg);
                IdsMessageHandler<object>.ErrorMessage(msg);
                //输出到控制台

            }
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext())
            {

                if (sendState == 0)
                {
                    var message = new DispatchMessage
                    {
                        RackId = string.Join(",", request.ApiReqData.Select(f => f.RackId).ToList()),
                        RequestId = request.ApiReqHeader.RequestId,
                        Message = jsonRequest,
                        RespMessage = respMessage,
                        Status = sendState,
                        BizStatus = request.ApiReqData[0].Status,
                        TaskId = taskId,
                        PostCount = 1,
                        IOStatus = "O"
                    };
                    message.saveInit();
                    message.Id = IdUtils.Id + "";
                    ctx.Insert(message);
                }
                if (sendState == 1)
                {
                    var message = new DispatchMessageHis
                    {
                        RackId = string.Join(",", request.ApiReqData.Select(f => f.RackId).ToList()),
                        RequestId = request.ApiReqHeader.RequestId,
                        Message = jsonRequest,
                        RespMessage = respMessage,
                        Status = sendState,
                        BizStatus = request.ApiReqData[0].Status,
                        TaskId = taskId,
                        PostCount = 1,
                        IOStatus = "O"
                    };
                    message.saveInit();
                    message.Id = IdUtils.Id + "";
                    ctx.Insert(message);
                }
            }

        }
        /// <summary>
        /// 解析响应
        /// </summary>
        public ApiResponse<E> ParseResponse<E>(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ApiResponse<E>>(json, options);
        }

    }
}
