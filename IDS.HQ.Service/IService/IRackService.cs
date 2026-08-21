using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.IService
{
    public interface IRackService : IDbBaseService<Rack>
    {
        public IdsResult<List<Rack>> GetAllRackNode();
    }
}
