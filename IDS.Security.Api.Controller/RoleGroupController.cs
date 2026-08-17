using IDS.Base.Utils;
using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("roleGrp")]
    [PropertiesAutowired]
    [ApiController]
    public class RoleGroupController : DbBaseController<RoleGroup>
    {
        public virtual RoleGroupAdapter RoleGroupAdapter { set; get; }
        public virtual ILogger<RoleGroupController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<RoleGroup> Adapter()
        {
            return RoleGroupAdapter;
        }


        [Route("guest/items")]
        [HttpPost]
        public ResponseEntity<Page<RoleGroup>> queryItems(Page<RoleGroup> data)
        {
            return ResponseEntity<Page<RoleGroup>>.Success(RoleGroupAdapter.GetPages(data));
        }
        [Route("batch_save_roles")]
        [HttpPost]
        public ResponseEntity<string> batchInsert(ResponseEntity<List<RoleGroupItem>> data)
        {
            if (data == null || data.data == null || data.data.Count() == 0)
                return ResponseEntity<string>.Error("参数不能为空");
            if (data.data.Count() > 4)
                throw new BussinessException("用户组添加最多不能超过4个");
            data.data.ForEach(c=>{
                c.Id= BaseUtil.uuid();
            });
            RoleGroupAdapter.BatchInsert(data.data);
            return ResponseEntity<string>.Success("OK");
        }
    }
}
