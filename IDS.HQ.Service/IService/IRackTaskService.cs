using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Persistence;

namespace IDS.HQ.Service
{
    public interface IRackTaskService : IDbBaseService<RackTask>
    { 
        IdsResult<RackTask> Putway(WmsPuywayRequest data);
        IdsResult<RackTask> Outbound(RackTask rackTask);
        IdsResult<RackTask> CancelTask(RackTask rackTask);
        IdsResult<RackTask> ForceCompleteTask(RackTask rackTask);
        //重置下线的储位亮灯，人工出发，按需出发
        IdsResult<string> ResetDownLightOn(string id);
    }
}
