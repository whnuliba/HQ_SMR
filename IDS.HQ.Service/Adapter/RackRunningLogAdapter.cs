using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Service.IService;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class RackRunningLogAdapter : DbBaseAdapter<RackRunningLog>
    {
        public IRackRunningLogService _service { get; set; }

        public override IDbBaseService<RackRunningLog> Service()
        {
            return _service;
        }

        public IdsResult<List<RackRunningLog>> GetByRackNo(string rackNo)
        {
            return _service.GetByRackNo(rackNo);
        }

        public IdsResult<RackRunningLog> GetByTaskId(string taskId)
        {
            return _service.GetByTaskId(taskId);
        }

        public IdsResult<List<RackRunningLog>> GetByLogLevel(string logLevel)
        {
            return _service.GetByLogLevel(logLevel);
        }

        public IdsResult<int> DeleteLogsBefore(DateTime dateTime)
        {
            return _service.DeleteLogsBefore(dateTime);
        }
    }
}