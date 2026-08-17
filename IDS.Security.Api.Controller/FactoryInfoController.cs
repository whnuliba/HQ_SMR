using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("base-factory")]
    [PropertiesAutowired]
    [ApiController]
    public class FactoryInfoController : DbBaseController<FactoryInfo>
    {
        public virtual FactoryInfoAdapter _FactoryInfoAdapter { set; get; }
        public virtual ILogger<FactoryInfoController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<FactoryInfo> Adapter()
        {
            return _FactoryInfoAdapter;
        }

        [Route("guest/items")]
        [HttpPost]
        public ResponseEntity<Page<FactoryInfo>> items(Page<FactoryInfo> data)
        {
            return ResponseEntity<Page<FactoryInfo>>.Success(_FactoryInfoAdapter.GetPages(data,null));
        }

        [Route("guest/all")]
        [HttpPost]
        public ResponseEntity<List<FactoryInfo>> items()
        {
            return ResponseEntity<List<FactoryInfo>>.Success(_FactoryInfoAdapter.SelectByAll());
        }

        [Route("guest/role-factory")]
        [HttpPost]
        public ResponseEntity<List<RoleFactory>> roleFactory(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<RoleFactory>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<RoleFactory>>.Success(_FactoryInfoAdapter.SelectByRoleId(data.data));
        }
        [Route("save-role-factory")]
        [HttpPost]
        public ResponseEntity<int> saveRoleFactory(RequestData<List<RoleFactory>> data)
        {
            if (!RequestData<List<RoleFactory>>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<int>.Success(_FactoryInfoAdapter.BatchInsertRoleFactory(data.data));
        }
    }


}
