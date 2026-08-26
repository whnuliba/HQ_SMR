using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using System.Collections.Generic;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class MaterialInfoAdapter : DbBaseAdapter<MaterialInfo>
    {
        public IMaterialInfoService _service { get; set; }

        public override IDbBaseService<MaterialInfo> Service()
        {
            return _service;
        }

        public IdsResult<MaterialInfo> GetByTaskId(string taskId)
        {
            return _service.GetByTaskId(taskId);
        }

        public IdsResult<List<MaterialInfo>> GetByRackId(string rackId)
        {
            return _service.GetByRackId(rackId);
        }

        public IdsResult<MaterialInfo> GetByPPID(string ppid)
        {
            return _service.GetByPPID(ppid);
        }
    }
}