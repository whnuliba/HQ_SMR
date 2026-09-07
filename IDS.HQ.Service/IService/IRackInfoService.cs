using IDS.Base;
using IDS.Common;
using IDS.Extend.HYDevice.DTO;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Persistence;

namespace IDS.HQ.Service
{
    public interface IRackInfoService : IDbBaseService<RackInfo>
    { 
        public IdsResult<object> RegisterRackInfo(RegisterRackInfoDto rackInfo);
        public List<RackInfo> GetRackStatus(string rackNo);
        public IdsResult<object> CancelCacheAlarm(LightMultiRequest request);

    }
}
