using Amazon.Runtime.Internal;
using Autofac.Core;
using IDS.Base;
using IDS.Common;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.HQ.Service;
using IDS.HQ.Service.Adapter;
using IDS.Ioc;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IDS.HQ.Controller
{

    [Route("location")]
    [PropertiesAutowired]
    [ApiController]
    public class RackInfoController : DbLongBaseController<RackInfo>
    {
        public RackInfoAdapter _adapter { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbLongBaseAdapter<RackInfo> Adapter()
        {
            return _adapter;
        }
        /// <summary>
        /// 注册货架
        /// </summary>
        /// <param name="rackInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Registration")]
        public ResponseEntity<object> RegisterRackInfo(RequestData<RegisterRackInfoDto>   rackInfo)
        {
            if (!RequestData<RegisterRackInfoDto>.isRequest(rackInfo))
                return ResponseEntity<object>.Error("请传入合法参数");
            var res = _adapter.RegisterRackInfo(rackInfo.data);
            if (!res.Success) return ResponseEntity<object>.Error(res.Message);
            return ResponseEntity<object>.Success("ok");
        }
    }
}
