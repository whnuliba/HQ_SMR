using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class SysParamter:IdsBaseEntity
{
    [IdsColumn]
    public string? ParamName { get; set; }
    [IdsColumn]
    public string? ParamCode { get; set; }
    [IdsColumn]
    public string? ParamDsc { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; }
    [IdsColumn]
    public string? Scope { get; set; }
}
