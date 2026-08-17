using IDS.Base;
using IDS.Persistence;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface ISecBaseService<T> :IDbBaseService<T> where T : BaseEntity
    {

    }
}
