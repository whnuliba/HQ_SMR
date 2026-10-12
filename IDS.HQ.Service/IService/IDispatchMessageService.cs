using IDS.Common;
using IDS.HQ.Module;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Service.IService
{
    public interface IDispatchMessageService : IDbBaseService<DispatchMessage>
    {
        public IdsResult<string> UpdateStatus(DispatchMessage message);
    }
}
