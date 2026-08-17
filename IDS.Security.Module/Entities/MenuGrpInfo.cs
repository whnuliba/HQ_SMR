using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class MenuGrpInfo:AuthBaseEntity
{

    public string? GroupCode { get; set; } = null!;
    [IdsColumn]
    public string? GroupName { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; } = null!;
    [IdsColumn]
    public string? Scope { get; set; } = null!;
    [IdsColumn]
    public string? Desc { get; set; }
    [IdsColumn]
    public string? Platform { get; set; }
    [IdsColumn]
    public string? Udf1 { get; set; }
    [IdsColumn]
    public string? Udf2 { get; set; }
    [IdsColumn]
    public string? Udf3 { get; set; }
    [IdsColumn]
    public string? Udf4 { get; set; }
    [IdsColumn]
    public string? Udf5 { get; set; }
    [IdsColumn]
    public string? Udf6 { get; set; }

    //public virtual ICollection<MenuInfo>? MenuInfo { get; set; } = new List<MenuInfo>();

    public virtual Organization? Org { get; set; } = null!;

}
