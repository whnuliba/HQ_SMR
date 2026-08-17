using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class SysParameterDts:IdsBaseEntity
{
    [IdsColumn]
    public string? ParamId { get; set; }
    [IdsColumn]
    public string? ParamKey { get; set; }
    [IdsColumn]
    public string? ParamValue { get; set; }
    [IdsColumn]
    public string? ParamDsc { get; set; }
}
