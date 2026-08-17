using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class UserGroupRole
{
    public string? Id { get; set; } = null!;

    public string? RoleId { get; set; } = null!;

    public string? GroupId { get; set; } = null!;

    public int? RoleType { get; set; }

    public virtual UserGroup? Group { get; set; } = null!;
}
