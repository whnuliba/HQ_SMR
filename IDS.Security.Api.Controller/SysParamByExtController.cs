using IDS.Base;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.Adapter;
using IDS.Security.IService.DTO;
using IDS.Security.Module;
using IDS.Security.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    [Route("sys-params")]
    [PropertiesAutowired]
    [ApiController]
    public class SysParamByExtController: ControllerBase
    {
        public virtual SysParameterDtsAdapter SysParameterAdapter { set; get; }
        [Route(Route.ROUTE_ROOT_SYS_GET_P_CODE)]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<List<SysParameterDts>> queryParamsByParamCode(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                throw new BussinessException("参数不能为空");
            return ResponseEntity<List<SysParameterDts>>.Success(SysParameterAdapter.QueryParamsByParamCode(data.data));
        }
        [Route(Route.ROUTE_ROOT_SYS_GET_PK_CODE)]
        [HttpPost]
        [Anonymous]
        public ResponseEntity<SysParameterDts> queryParamsByParamCodeAndKey(RequestData<SysParamDto> data)
        {
            if (!RequestData<SysParamDto>.isRequest(data))
                throw new BussinessException("参数不能为空");
            return ResponseEntity<SysParameterDts>.Success(SysParameterAdapter.QueryParamsByParamCodeAndKey(data.data.paramCode, data.data.paramKey));
        }
    }
}
