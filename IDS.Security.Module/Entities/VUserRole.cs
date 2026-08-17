using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VUserRole
{
    public string? Id { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string CreateUser { get; set; } = null!;

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleName { get; set; }

    public int? UseState { get; set; }

    public string OrgId { get; set; } = null!;

    public string? Scope { get; set; }

    public int? RoleMaxUser { get; set; }

    public int? RoleType { get; set; }

    public string? RealName { get; set; }

    public int? UserUseState { get; set; }

    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public int? UserState { get; set; }
}
