using IDS.Base;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.Module;
using IDS.Security.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("menuGrp")]
    [PropertiesAutowired]
    [ApiController]
    public class MenuGrpInfoController : DbBaseController<MenuGrpInfo>
    {
        public virtual MenuGrpInfoAdapter MenuGrpInfoAdapter { set; get; }
        public virtual ILogger<MenuGrpInfoController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<MenuGrpInfo> Adapter()
        {
            return MenuGrpInfoAdapter;
        }

        [Route("guest/items")]
        [HttpPost]
        public ResponseEntity<Page<MenuGrpInfo>> queryItems(Page<MenuGrpInfo> data)
        {
            return ResponseEntity<Page<MenuGrpInfo>>.Success(MenuGrpInfoAdapter.GetPages(data,null));
        }
        [Route("query-all-grp")]
        [HttpPost]
        public ResponseEntity<List<MenuGrpInfo>> queryMenuGroup()
        {
            return ResponseEntity<List<MenuGrpInfo>>.Success(MenuGrpInfoAdapter.QueryMenuGroup());
        }
    }
}
