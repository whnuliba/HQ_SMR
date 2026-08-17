using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.Adapter;
using IDS.Security.IService.DTO;
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
    [Route(Route.ROUTE_ROOT_SYS_P)]
    [PropertiesAutowired]
    [ApiController]
    public class SysParameterDtsController : DbBaseController<SysParameterDts>
    {
        public virtual SysParameterDtsAdapter SysParameterAdapter { set; get; }
        public virtual ILogger<SysParameterDtsController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<SysParameterDts> Adapter()
        {
            return SysParameterAdapter;
        }

        [Route(Route.ROUTE_ROOT_SYS_GET_P_CODE)]
        [HttpPost]
        public ResponseEntity<List<SysParameterDts>> queryParamsByParamCode(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<List<SysParameterDts>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<SysParameterDts>>.Success(SysParameterAdapter.QueryParamsByParamCode(data.data));
        }
        [Route(Route.ROUTE_ROOT_SYS_GET_PK_CODE)]
        [HttpPost]
        public ResponseEntity<SysParameterDts> queryParamsByParamCodeAndKey(RequestData<SysParamDto> data)
        {
            if (!RequestData<SysParamDto>.isRequest(data))
                return ResponseEntity<SysParameterDts>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<SysParameterDts>.Success(SysParameterAdapter.QueryParamsByParamCodeAndKey(data.data.paramCode, data.data.paramKey));
        }

    }
}
