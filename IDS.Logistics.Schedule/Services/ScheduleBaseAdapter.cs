using IDS.Base;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Fms.Adapter
{
    public abstract class ScheduleBaseAdapter<T> : DbBaseAdapter<T> where T : BaseEntity
    {
    }
}
