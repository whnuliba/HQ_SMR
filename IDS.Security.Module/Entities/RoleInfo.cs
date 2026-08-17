using IDS.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDS.Security.Module;

/// <summary>
/// 角色表
/// </summary>
public partial class RoleInfo :AuthBaseEntity
{
    [IdsColumn]
    public string? RoleCode { get; set; }
    [IdsColumn]
    public string? RoleName { get; set; }
    [IdsColumn]
    public int? UseState { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; } = null!;
    [IdsColumn]
    public string? Scope { get; set; }

    /// <summary>
    /// 0表示无限制
    /// </summary>
    [IdsColumn]
    public int? RoleMaxUser { get; set; }
    [IdsColumn]
    public int? RoleType { get; set; }

    [NotMapped]
    public string? UserId { get; set; }

    public virtual ICollection<DepartmentRole> DepartmentRole { get; set; } = new List<DepartmentRole>();

    public virtual ICollection<JobRole> JobRole { get; set; } = new List<JobRole>();

    public virtual ICollection<MetuxRole> MetuxRoleMutexRole { get; set; } = new List<MetuxRole>();

    public virtual ICollection<MetuxRole> MetuxRoleRole { get; set; } = new List<MetuxRole>();

    public virtual ICollection<RoleFunction> RoleFunction { get; set; } = new List<RoleFunction>();

    public virtual ICollection<RoleGroupItem> RoleGroupItem { get; set; } = new List<RoleGroupItem>();

    public virtual ICollection<SubRole> SubRoleRole { get; set; } = new List<SubRole>();

    public virtual ICollection<SubRole> SubRoleSubRoleNavigation { get; set; } = new List<SubRole>();
}
