using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VUserOrgDepartment
{
    public string? OrgId { get; set; } = null!;

    public string? UserName { get; set; }

    public string UserId { get; set; } = null!;

    public string? RealName { get; set; }

    public int? UserUseState { get; set; }

    public int? UserState { get; set; }

    public string DeptId { get; set; } = null!;

    public string? OrgCode { get; set; }

    public string? OrgName { get; set; }

    public string? DeptCode { get; set; }

    public string? DeptName { get; set; }
}
