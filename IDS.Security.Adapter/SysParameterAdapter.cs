using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.Module;
using IDS.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]

    public class SysParameterAdapter : SecBaseAdapter<SysParamter>
    {
        public ISysParameterService SysParamterService { get; set; }
        public override IDbBaseService<SysParamter> Service()
        {
            return SysParamterService;
        }
        public SysParamter SelectByParamCode(String paramCode)
        {
            return SysParamterService.SelectByParamCode(paramCode);
        }
        public int DeleteParams(String id)
        {
            return SysParamterService.DeleteParams(id);
        }
        public List<SysParameterAndDts> GetSysParameterAndDtsByParamCode(String paramCode)
        {
            return SysParamterService.GetSysParameterAndDtsByParamCode(paramCode);
        }
        public List<SysParamDto> QueryParamsByCode(String paramCode)
        {
            return SysParamterService.QueryParamsByCode(paramCode);
        }
        public IdsResult<string> RefreshCache(String paramCode)
        {
            SysParamterService.RefreshCache(paramCode);
            return IdsResult<string>.ok();
        }
    }
}
