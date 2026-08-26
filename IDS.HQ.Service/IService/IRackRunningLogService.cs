using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System.Collections.Generic;

namespace IDS.HQ.Service.IService
{
    public interface IRackRunningLogService : IDbBaseService<RackRunningLog>
    {
        /// <summary>
        /// 根据料架号获取运行日志列表
        /// </summary>
        public IdsResult<List<RackRunningLog>> GetByRackNo(string rackNo);

        /// <summary>
        /// 根据任务ID获取运行日志
        /// </summary>
        public IdsResult<RackRunningLog> GetByTaskId(string taskId);

        /// <summary>
        /// 根据日志级别获取日志列表
        /// </summary>
        public IdsResult<List<RackRunningLog>> GetByLogLevel(string logLevel);

        /// <summary>
        /// 批量删除指定时间之前的日志
        /// </summary>
        public IdsResult<int> DeleteLogsBefore(DateTime dateTime);
    }
}