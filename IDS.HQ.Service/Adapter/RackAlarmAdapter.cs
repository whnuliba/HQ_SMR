using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using System.Collections.Generic;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class RackAlarmAdapter : DbBaseAdapter<RackAlarm>
    {
        public IRackAlarmService _service { get; set; }

        public override IDbBaseService<RackAlarm> Service()
        {
            return _service;
        }

        public IdsResult<List<RackAlarm>> GetByRackNo(string rackNo)
        {
            return _service.GetByRackNo(rackNo);
        }

        public IdsResult<RackAlarm> GetByTaskId(string taskId)
        {
            return _service.GetByTaskId(taskId);
        }

        public IdsResult<List<RackAlarm>> GetByHandleState(int handleState)
        {
            return _service.GetByHandleState(handleState);
        }

        public IdsResult<bool> BatchUpdateHandleState(List<string> ids, int handleState)
        {
            return _service.BatchUpdateHandleState(ids, handleState);
        }
    }
}