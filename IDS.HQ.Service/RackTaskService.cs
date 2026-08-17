using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace IDS.HQ.Service
{
    [AutoInjection]
    public class RackTaskService : IRackTaskService<RackTask>
    {
        public object obj_lock = new object();
        public IdsRedis RedisClient { get; set; }
        private string _checkPutwayKey = "HQ:HY:PUTWAY:CHECK"; //料架号
        public  IDbContextFactory<RackDbContext> DbContextFactory { get; set; }
        public RackDbContext DbContext()
        {
            return DbContextFactory.CreateDbContext();
        }
        public IdsResult<RackTask> Putway(RackTask rackTask)
        {
            //做两个操作，1是确认当前是否已经完成绑定
            if (rackTask == null || string.IsNullOrWhiteSpace(rackTask.RackNo)) {
                return IdsResult<RackTask>.failure("上传的货架信息为空，或者货架号为空");
            }
            lock (obj_lock) {
                string token = RedisClient.GetDatabase().StringGet(_checkPutwayKey + rackTask.RackNo);
                if (!string.IsNullOrWhiteSpace(token)) {
                    return IdsResult<RackTask>.failure($"当前该货架{rackTask.RackNo}有正在上架但未绑定的任务,任务token:{token}");
                }
                using (var ctx = DbContext()) {

                
                    long id = IdUtils.Id;
                    rackTask.Id = id;
                    using (var ts = new TransactionScope())
                    {
                        try
                        {
                            rackTask.saveInit();
                            ctx.Insert(rackTask);
                            RedisClient.GetDatabase().StringSet(_checkPutwayKey + rackTask.RackNo, id + "");
                            ts.Complete();
                        }
                        catch (Exception ex) { 
                          return IdsResult<RackTask>.failure(ex.Message);
                        }
                  
                    }
                }

                //处理任务创建
                return IdsResult<RackTask>.failure("任务下达，等待货架上传");
            }
        }
    }
}
