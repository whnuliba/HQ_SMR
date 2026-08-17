using Google.Protobuf.WellKnownTypes;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Fms.Service;
using IDS.Persistence;
using IDS.Schedule;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.ClearScript.V8;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Quartz.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Schedule
{
    [DisallowConcurrentExecution]
    public abstract class DispatcherJob : IJob
    {
        public static ConcurrentDictionary<string, V8ScriptEngine> _V8ScriptEngine = new ConcurrentDictionary<string, V8ScriptEngine>();
        public static ConcurrentDictionary<string, HttpRequestEntity> _HttpRequestEntity = new ConcurrentDictionary<string, HttpRequestEntity>();
        public static ConcurrentDictionary<string, string> _Parameters= new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> _Ticket = new ConcurrentDictionary<string, string>();
        public virtual ILogger<DispatcherJob> Logger { set; get; }
        public virtual IdsRedis RedisClient { set; get; }
        public virtual IDbContextFactory<ScheduleDbContext> DbContextFactory { get; set; }
        public ScheduleDbContext JobContext => DbContextFactory.CreateDbContext();
        private  ILog _Log = LogManager.GetLogger(typeof(DispatcherJob));
        public async Task Execute(IJobExecutionContext context) {
            var jobMap = context.JobDetail?.JobDataMap;
            if (!jobMap.ContainsKey("task"))
            {
                await Execute(context, null);
                return;
            }
            var taskValue = jobMap?.Get("task")?.ToString();
            ScheduleModule scheduleModule = null;
            if (!string.IsNullOrEmpty(taskValue)) { 
                scheduleModule = JsonConvert.DeserializeObject<ScheduleModule>(taskValue);
            }
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            //if (!_Parameters.TryGetValue(key, out string value)) {

            //}
            string value = string.Empty ;
            string ticket = string.Empty ;
            bool modified = true;
            if (!await IsChangedScheduleParameter(scheduleModule)) {
                modified = false;
                await Execute(context, scheduleModule);
                return;
            }
            using (var ctx = JobContext)
            {
                var job = ctx.Query<CwQrtzScheduleJob>(f => f.ScheduleGrpCode == scheduleModule.ScheduleGrpCode && f.ScheduleCode == scheduleModule.ScheduleCode).FirstOrDefault();
                value = job.Parameters;
                string  modifiedTicket = job.Ticket;
                if (_Ticket.TryGetValue(key, out ticket) && ticket == modifiedTicket)
                {
                    modified = false;
                }
                else {
                    _Ticket.AddOrUpdate(key, modifiedTicket, (k2, v2) => modifiedTicket);
                }
                _Parameters.AddOrUpdate(key, value, (k2, v2) => value);
              
            }
            if (modified && !string.IsNullOrEmpty(value) && TryV8ScriptEngineParseHandling(out V8ScriptEngine engine, value))
            {
                if (_V8ScriptEngine.TryGetValue(key, out V8ScriptEngine oengin)) {
                    //先释放资源
                    oengin.Dispose();
                }
                _V8ScriptEngine.AddOrUpdate(key, engine, (k2, v2) => engine);
                ParseApiArguments(scheduleModule);
            }
            await Execute(context, scheduleModule);
        }
        public virtual async Task<bool> IsChangedScheduleParameter(ScheduleModule scheduleModule) {
            string key = $"Job:{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            string ticket = await RedisClient.GetCache<string>(key);

            if (_Ticket.TryGetValue(key, out string localTicket) && ticket == localTicket)
            {
                return false;
            }      
            return true;
        }
        private bool TryV8ScriptEngineParseHandling(out V8ScriptEngine engine,string args) {
            engine = new V8ScriptEngine();
            try
            {
                engine.AddHostObject("log", _Log);
                engine.Execute(args);
                return true;
            }
            catch (Exception ex)
            {
                engine.Dispose();
                Logger.LogError("DispatcherJob Parse Error: " + ex.Message);
            }
            return false;
        }
        public abstract Task<object> OriginalData(ScheduleModule scheduleModule);
        private object ParseApiArguments(ScheduleModule scheduleModule)
        {
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && _V8ScriptEngine.TryGetValue(key, out V8ScriptEngine engine))
                {
                    var res = engine.Invoke("apiArguments");
                    if (res == null)
                        return null;
                    if (res is string)
                    {
                        var apis = JsonConvert.DeserializeObject<HttpRequestEntity>(res.ToString());
                        _HttpRequestEntity.AddOrUpdate(key, apis, (k2, v2) => apis);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DispatcherJob Parameters Parse Error: " + ex.Message);
            }
            return null;
        }
         public virtual async Task<object> ConvertMessage(ScheduleModule scheduleModule) {
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            try
            {
                var ori = await OriginalData(scheduleModule);
                if (!string.IsNullOrWhiteSpace(key) && (ori != null && (ori is not string || (ori is string && !string.IsNullOrWhiteSpace(ori.ToString())))) &&  _V8ScriptEngine.TryGetValue(key, out V8ScriptEngine engine))
                {
                    return engine.Invoke("parseRequestMsg", ori);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DispatcherJob Parameters Parse Error: " + ex.Message);
            }
            return null;    
        }

        public virtual async Task<IdsResult<object>> ConvertResponse(ScheduleModule scheduleModule, object param) {

            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && _V8ScriptEngine.TryGetValue(key, out V8ScriptEngine engine))
                {
                    var res = engine.Invoke("parseResponseMsg", param);
                    if (res == null)
                        return IdsResult<object>.ok(param);
                    if (res is string)
                    {
                        return JsonConvert.DeserializeObject<IdsResult<object>>(res.ToString());
                    }
                    else {
                        return JsonConvert.DeserializeObject<IdsResult<object>>(JsonConvert.SerializeObject(res));
                    }
                }
            }
            catch (Exception ex)
            {              
                Logger.LogError("DispatcherJob Parameters Parse Error: " + ex.Message);
                return IdsResult<object>.failure("DispatcherJob Parameters Parse Error: " + ex.Message);
            }
            return IdsResult<object>.failure();
        }
        public virtual async Task<object> ConvertMessage(ScheduleModule scheduleModule,object param)
        {
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && _V8ScriptEngine.TryGetValue(key, out V8ScriptEngine engine))
                {
                    return engine.Invoke("parseRequestMsg", param);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DispatcherJob Parameters Parse Error: " + ex.Message);
            }
            return null;
        }

        public virtual async Task<string> ToDispatcher(ScheduleModule scheduleModule,string content) {
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            if (!string.IsNullOrWhiteSpace(key) && _HttpRequestEntity.TryGetValue(key, out HttpRequestEntity requestEntity) 
                 && !string.IsNullOrWhiteSpace(requestEntity.Uri) && requestEntity.Enable)
            {
               var res =await HttpUtil.Post(requestEntity.Uri, content, requestEntity.Header, requestEntity.Timeout);
               return res;
            }
            return null;
        }

        public virtual async Task<IdsResult<object>> SendSync(ScheduleModule scheduleModule, string content)
        {
            string key = $"{scheduleModule.ScheduleGrpCode}:{scheduleModule.ScheduleCode}";
            if (!string.IsNullOrWhiteSpace(key) && _HttpRequestEntity.TryGetValue(key, out HttpRequestEntity requestEntity)
                 && !string.IsNullOrWhiteSpace(requestEntity.Uri) && requestEntity.Enable)
            {
                var res = await HttpUtil.Post(requestEntity.Uri, content, requestEntity.Header, requestEntity.Timeout);
                var respRes = await ConvertResponse(scheduleModule, res);
                return respRes;
            }
            return IdsResult<object>.failure();
        }

        public abstract Task Execute(IJobExecutionContext context, ScheduleModule scheduleJob);
    }
}
