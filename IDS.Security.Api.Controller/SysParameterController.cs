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
    [Route(Route.ROUTE_ROOT_SYS)]
    [PropertiesAutowired]
    [ApiController]
    public class SysParameterController : DbBaseController<SysParamter>
    {
        public virtual SysParameterAdapter SysParameterAdapter { set; get; }
        public virtual ILogger<SysParameterController> Logger { set; get; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<SysParamter> Adapter()
        {
            return SysParameterAdapter;
        }

        [Route(Route.ROUTE_ROOT_SYS_DELETE)]
        [HttpPost]
        public ResponseEntity<int> deleteParams(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<int>.Success(SysParameterAdapter.DeleteParams(data.data));
        }



        [Route(Route.ROUTE_ROOT_SYS_GET_PARAMETER_AND_DTS)]
        [HttpPost]
        public ResponseEntity<List<SysParameterAndDts>> getSysParameterDts(RequestData<SysParameterAndDts> data)
        {
            if (!RequestData<SysParameterAndDts>.isRequest(data))
                return ResponseEntity<List<SysParameterAndDts>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<SysParameterAndDts>>.Success(SysParameterAdapter.GetSysParameterAndDtsByParamCode(data.data.ParamCode));
        }
        [Route(Route.ROUTE_ROOT_SYS_GET_PARAMETER_BY_CODE)]
        [HttpPost]
        public ResponseEntity<List<SysParamDto>> getSysParameterByCode(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<List<SysParamDto>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            return ResponseEntity<List<SysParamDto>>.Success(SysParameterAdapter.QueryParamsByCode(data.data));
        }
        [Route(Route.ROUTE_ROOT_SYS_REFRESH_CACHE_BY_CODE)]
        [HttpPost]
        public ResponseEntity<string> refreshCache(RequestData<String> data)
        {
            if (!RequestData<String>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            IdsResult<string> result = SysParameterAdapter.RefreshCache(data.data);
            if (result.Success)
                return ResponseEntity<string>.Success("ok");
            return ResponseEntity<string>.Error(result.Message);
        }

    }
}
