using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System.Collections.Generic;

namespace IDS.HQ.Service.IService
{
    public interface IRackAlarmService : IDbBaseService<RackAlarm>
    {
        /// <summary>
        /// 根据料架号获取报警列表
        /// </summary>
        public IdsResult<List<RackAlarm>> GetByRackNo(string rackNo);

        /// <summary>
        /// 根据任务ID获取报警信息
        /// </summary>
        public IdsResult<RackAlarm> GetByTaskId(string taskId);

        /// <summary>
        /// 根据处理状态获取报警列表
        /// </summary>
        public IdsResult<List<RackAlarm>> GetByHandleState(int handleState);

        /// <summary>
        /// 批量更新报警处理状态
        /// </summary>
        public IdsResult<bool> BatchUpdateHandleState(List<string> ids, int handleState);
    }
}