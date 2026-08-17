using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class SubRole
{
    public string? Id { get; set; } = null!;

    public string? RoleId { get; set; }

    public string? SubRoleId { get; set; }

    public virtual RoleInfo? Role { get; set; }

    public virtual RoleInfo? SubRoleNavigation { get; set; }
}
