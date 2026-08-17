using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public enum ErrorCode
    {
        [Description("数据更新异常")]
        DATA_UPDATA_FAIL=500,
        [Description("存在工艺流程明细，无法删除")]
        PROCESS_FLOW_DTS_IS_NOT_NULL = 500,
        [Description("缺少参数或值为空")]
        PARAMETER_NULL = 502,

    }
}
