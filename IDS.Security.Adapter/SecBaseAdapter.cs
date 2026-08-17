using IDS.Base;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    public abstract class SecBaseAdapter<T> : DbBaseAdapter<T> where T : BaseEntity // where IService : ISecBaseService<T>
    {

    }
}
