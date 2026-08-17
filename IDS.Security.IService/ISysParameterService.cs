using IDS.Common;
using IDS.Security.IService.DTO;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface ISysParameterService : ISecBaseService<SysParamter>
    {
        SysParamter SelectByParamCode(String paramCode);
        int DeleteParams(String id);
        List<SysParameterAndDts> GetSysParameterAndDtsByParamCode(String paramCode);
        List<SysParamDto> QueryParamsByCode(String paramCode);
        IdsResult<string> RefreshCache(String paramCode);
    }
}
