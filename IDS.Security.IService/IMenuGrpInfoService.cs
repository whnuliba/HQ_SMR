using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface IMenuGrpInfoService : ISecBaseService<MenuGrpInfo>
    {
        List<MenuGrpInfo> QueryMenuGroup();
    }
}
