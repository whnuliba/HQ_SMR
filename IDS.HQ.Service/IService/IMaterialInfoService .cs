using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.IService
{
    public interface IMaterialInfoService : IDbBaseService<MaterialInfo>
    {
        /// <summary>
        /// 根据任务ID获取物料信息
        /// </summary>
        public IdsResult<MaterialInfo> GetByTaskId(string taskId);

        /// <summary>
        /// 根据料架ID获取物料信息列表
        /// </summary>
        public IdsResult<List<MaterialInfo>> GetByRackId(string rackId);

        /// <summary>
        /// 根据PPID获取物料信息
        /// </summary>
        public IdsResult<MaterialInfo> GetByPPID(string ppid);
    }
}
