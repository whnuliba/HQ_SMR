using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VRoleAndGroup
{
    public string? Id { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleName { get; set; }

    public int? UseState { get; set; }

    public string? OrgId { get; set; }

    public string? Scope { get; set; }

    public int? RoleMaxUser { get; set; }

    public int? RoleType { get; set; }
}
