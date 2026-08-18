using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;

namespace IDS.HQ.Service
{
    public interface IRackTaskService : IDbLongBaseService<RackTask>
    { 
        IdsResult<RackTask> Putway(RackTask data);
        IdsResult<RackTask> Outbound(RackTask rackTask);

    }
}
