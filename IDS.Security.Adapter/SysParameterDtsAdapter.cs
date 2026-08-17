using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Adapter
{
    [AutoInjection]
    public class SysParameterDtsAdapter : SecBaseAdapter<SysParameterDts>
    {
        public ISysParameterDtsService SysParameterDtsService { get; set; }
        public override IDbBaseService<SysParameterDts> Service()
        {
            return SysParameterDtsService;
        }
        public  List<SysParameterDts> QueryParamsByParamCode(string paramCode) {
            return SysParameterDtsService.QueryParamsByParamCode(paramCode);
        }
        public SysParameterDts QueryParamsByParamCodeAndKey(string paramCode, string paramKey){
            return SysParameterDtsService.QueryParamsByParamCodeAndKey(paramCode, paramKey);
        }
        public int DeleteByParamId(string paramId) {
            return SysParameterDtsService.DeleteByParamId(paramId);
        }
    }
}
