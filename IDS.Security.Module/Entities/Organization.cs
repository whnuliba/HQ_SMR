using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class Organization:AuthBaseEntity
{
    [IdsColumn]
    public string? OrgName { get; set; }
    [IdsColumn]
    public string? OrgCode { get; set; }
    [IdsColumn]
    public int Grade { get; set; }
    [IdsColumn]
    public int? Sort { get; set; }

    public virtual ICollection<MenuGrpInfo> MenuGrpInfo { get; set; } = new List<MenuGrpInfo>();

    public virtual ICollection<MenuInfo> MenuInfo { get; set; } = new List<MenuInfo>();

}
