using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class JobRole
{
    public string Id { get; set; } = null!;
    [IdsColumn]
    public string? JobId { get; set; }
    [IdsColumn]
    public string? RoleId { get; set; }
    [IdsColumn]
    public int? RoleType { get; set; }

    public virtual JobInfo? Job { get; set; }

    public virtual RoleInfo? Role { get; set; }
}
