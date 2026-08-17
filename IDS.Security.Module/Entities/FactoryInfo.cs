using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class FactoryInfo:IdsBaseEntity
{
    [IdsColumn]
    public string? FactoryNo { get; set; } = null!;
    [IdsColumn]
    public string? FactoryName { get; set; }
    [IdsColumn]
    public string? Remark { get; set; }
    [IdsColumn]
    public string? FactoryDesc { get; set; }
    [IdsColumn]
    public string? Udf1 { get; set; }
    [IdsColumn]
    public string? Udf2 { get; set; }
    [IdsColumn]
    public string? Udf3 { get; set; }

}
