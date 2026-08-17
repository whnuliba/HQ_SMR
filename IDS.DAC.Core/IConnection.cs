using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.DAC.Core
{
    public interface IConnection
    {
        IDeviceAccess GetAccess();
        IDeviceAccess GetAccess(string tag);
    }
}
