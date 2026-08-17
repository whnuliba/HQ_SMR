using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class RoleGroup :AuthBaseEntity
{

    [IdsColumn]
    public string? GroupNo { get; set; }
    [IdsColumn]
    public string? GroupName { get; set; }
    [IdsColumn]
    public string? GroupDesc { get; set; }

    public string? OrgId { get; set; }
    [IdsColumn]
    public string? Scope { get; set; }
    [IdsColumn]
    public int? UseState { get; set; }
    [IdsColumn]
    public int? RoleMaxUser { get; set; }
    [IdsColumn]
    public int? RoleType { get; set; }

    public virtual ICollection<RoleGroupItem> RoleGroupItem { get; set; } = new List<RoleGroupItem>();
}
