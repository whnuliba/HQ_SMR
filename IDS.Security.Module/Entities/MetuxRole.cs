using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class MetuxRole
{
    public string Id { get; set; } = null!;
    [IdsColumn]
    public string? RoleId { get; set; }
    [IdsColumn]
    public string? MutexRoleId { get; set; }

    public virtual RoleInfo? MutexRole { get; set; }

    public virtual RoleInfo? Role { get; set; }
}
