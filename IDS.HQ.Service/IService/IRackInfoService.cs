using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Persistence;

namespace IDS.HQ.Service
{
    public interface IRackInfoService : IDbBaseService<RackInfo>
    { 
        public IdsResult<object> RegisterRackInfo(RegisterRackInfoDto rackInfo);
    }
}
