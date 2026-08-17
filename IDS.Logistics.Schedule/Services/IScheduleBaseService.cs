using IDS.Base;
using IDS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Fms.IService
{
    public interface IScheduleBaseService<T> : IDbBaseService<T> where T : BaseEntity
    {
    }
}
