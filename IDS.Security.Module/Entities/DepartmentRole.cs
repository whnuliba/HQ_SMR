using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class DepartmentRole
{
    public string Id { get; set; } = null!;

    public string? DeptId { get; set; }

    public string? RoleId { get; set; }

    public int? RoleType { get; set; }

    public virtual Department? Dept { get; set; }

    public virtual RoleInfo? Role { get; set; }

}
