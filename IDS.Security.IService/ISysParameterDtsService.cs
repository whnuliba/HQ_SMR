using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface ISysParameterDtsService : ISecBaseService<SysParameterDts>
    {
        List<SysParameterDts> QueryParamsByParamCode(string paramCode);
        SysParameterDts QueryParamsByParamCodeAndKey(string paramCode, string paramKey);
        int DeleteByParamId(string paramId);
    }
}
