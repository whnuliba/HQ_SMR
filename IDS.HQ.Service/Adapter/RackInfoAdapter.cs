using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Ioc;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.Adapter
{
    [AutoInjection]
    public class RackInfoAdapter : DbLongBaseAdapter<RackInfo>
    {
        public IRackInfoService _service { set; get; }
        public override IDbLongBaseService<RackInfo> Service()
        {
            return _service;
        }
        public IdsResult<object> RegisterRackInfo(RegisterRackInfoDto rackInfo) { 
           return _service.RegisterRackInfo(rackInfo);
        }
    }
}
