using IDS.Common;
using IDS.Common.Utils;
using IDS.Extend.HYDevice.Dispatch;
using IDS.Extension;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace IDS.HQ.Service.Job
{
    public class DispatchMessageJob : BaseJob<DispatchMessageJob>
    {
        private readonly IDbContextFactory<RackDbContext> _dbContext;
        private readonly IdsRedis _idsRedis;
        private string  _servicePath ;
        private string _token ;
        public DispatchMessageJob(IDbContextFactory<RackDbContext> dbContext, ILogger<DispatchMessageJob> logger, IdsRedis redis) : base(logger, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)) {
            _dbContext = dbContext;
            _idsRedis = redis;
            _servicePath = AppConfig.GetConfigInfo("AddressPool:WmsUrl");
            _token = AppConfig.GetConfigInfo("AddressPool:WmsToken");
        }
        protected override async Task ActionAsync(CancellationToken stoppingToken)
        {

           
            using (var ctx = _dbContext.CreateDbContext())
            {
                //获取需要重发的信息
                var accessLogs = ctx.Query<DispatchMessage>(f => f.Status == 0 && f.PostCount<=10).Take(20).ToList();
                foreach (var log in accessLogs) {
                    string respMessage = string.Empty;
                    int sendState = 1;
                    var start = new Stopwatch();
                    try
                    {

                        start.Start();
                        respMessage = await HttpUtil.Post(_servicePath, log.Message,null, 1000 * 30);
                        var responseData = TaskReturnWmsDispatchHandler.Instance.ParseResponse<ApiResponseData>(respMessage);
                        start.Stop();

                        if (responseData == null || responseData.ApiResHeader.ApiCode != "0x000000") // "APICode": "0x000000",--成功0x000000，失败0x000009
                        {
                            sendState = 1;
                        }
                        _logger.LogInformation($"发送WMS报文[LocationInfoChange]完成:{start.ElapsedMilliseconds},Request:{log.Message},返回:{respMessage}");
                    }
                    catch (Exception ex)
                    {
                        sendState = 0;
                        respMessage = ex.Message;
                        start.Stop();
                        string msg = $"发送WMS报文[LocationInfoChange]出现错误了时长:{start.ElapsedMilliseconds},Request:{log.Message},错误:{ex.Message}";
                        _logger.LogError(msg);
                        IdsMessageHandler<object>.ErrorMessage(msg);
                        //输出到控制台

                    }

                    //失败 就计数
                    if (sendState == 0)
                    {

                        ctx.DispatchMessage.Where(a => a.Id == log.Id)
                                             .ExecuteUpdate(setters => setters
                                                 .SetProperty(a => a.PostCount, v => v.PostCount ?? 0 + 1).SetProperty(f=>f.RespMessage,respMessage )
                                              );
                    }
                    if (sendState == 1)
                    {
                        using (var ts = ctx.Database.BeginTransaction()) {
                            var message = new DispatchMessageHis();
                            try {
                                // 成功直接写入到历史
                                log.PostCount = log.PostCount ?? 0 + 1;
                                ObjectExtensions.CopyProperties(log, message);
                                message.Id = IdUtils.Id + "";
                                message.ExtId = log.Id;
                                ctx.Insert(message);
                                //删除历史记录
                                ctx.Delete<DispatchMessage>(f => f.Id == log.Id);
                                ts.Commit();
                            } catch (Exception ex) {
                                string msg = $"迁移报文[LocationInfoChange]出现错误了Request:{JsonConvert.SerializeObject(message)},错误:{ex.Message}";
                                _logger.LogError(msg);
                                IdsMessageHandler<object>.ErrorMessage(msg);
                                ts.Rollback();
                            }
                         
                        }
      
                    }

                }
            }
        }
    }
}
