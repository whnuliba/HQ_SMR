using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class MenuGrpInfoAdapter : SecBaseAdapter<MenuGrpInfo>
    {
        public IMenuGrpInfoService MenuGrpInfoService { get; set; }
        public override IDbBaseService<MenuGrpInfo> Service()
        {
            return MenuGrpInfoService;
        }
        public List<MenuGrpInfo> QueryMenuGroup() { 
             return MenuGrpInfoService.QueryMenuGroup();    
          }
    }
}
