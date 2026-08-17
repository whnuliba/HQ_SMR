using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class RoleGroupItem
{
    public string? Id { get; set; } = null!;

    public string? GroupId { get; set; } = null!;

    public string? RoleId { get; set; } = null!;

    public virtual RoleGroup Group { get; set; } = null!;

    public virtual RoleInfo Role { get; set; } = null!;
}
