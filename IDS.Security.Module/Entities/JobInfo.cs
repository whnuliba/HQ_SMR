using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class JobInfo : AuthBaseEntity
{
    [IdsColumn]
    public string? JobNo { get; set; } = null!;
    [IdsColumn]
    public string? JobName { get; set; } = null!;
    [IdsColumn]
    public string? JobType { get; set; }
    [IdsColumn]
    public string? JobDesc { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; }
    [IdsColumn]
    public string? Scope { get; set; } = null!;

    public virtual ICollection<JobRole> JobRole { get; set; } = new List<JobRole>();
}
