using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.DTO
{
    public class SysParameterAndDts
    {
        public string? ParamName { set; get; }
        public string? ParamCode { set; get; }
        public string? ParamKey { set; get; }
        public string? ParamValue { set; get; }
    }
    public class SysParamDto
    {
        public string? paramCode { set; get; }
        public string? paramKey { set; get; }
        public string? paramValue { set; get; }
        public string? paramDsc { set; get; }
    }


}
