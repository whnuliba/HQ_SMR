using IDS.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.Security.Module;

/// <summary>
/// 用户定义表
/// </summary>
public partial class UserInfo :AuthBaseEntity
{
    [IdsColumn]
    public string? UserName { get; set; }
    [IdsColumn]
    public string? RealName { get; set; }
    [IdsColumn]
    public string? Password { get; set; }
    [IdsColumn]
    public int? UseState { get; set; }
    [IdsColumn]
    public string? Email { get; set; }
    [IdsColumn]
    public int? Sex { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; }
    [IdsColumn]
    public string? JobId { get; set; }
    [IdsColumn]
    public string? Mobile { get; set; }
    [IdsColumn]
    public string? Leader { get; set; }
    [IdsColumn]
    public string? LeaderName { get; set; }
    [IdsColumn]
    public string? LeaderId { get; set; }
    [IdsColumn]
    public DateTime? AccountExpireTime { get; set; }
    [IdsColumn]
    public DateTime? PasswordExpireTime { get; set; }
    [IdsColumn]
    public string? Alias { get; set; }
    [IdsColumn]
    public string? Lock { get; set; }
    [IdsColumn]
    public string? ChangePassword { get; set; }
    [IdsColumn]
    public string? NameSpell { get; set; }

    [NotMapped]
    public string? DeptId { get; set; }
    [NotMapped]
    public string? DeptCode { get; set; }
    [NotMapped]
    public string? DeptName { get; set; }
    [NotMapped]
    public string? OrgCode { get; set; }
    [NotMapped]
    public string? OrgName { get; set; }
    [NotMapped]
    public string? DeptLeader { get; set; }
    [NotMapped]
    public string? DeptLeaderName { get; set; }
    [NotMapped]
    public string? DeptLeaderId { get; set; }


    [NotMapped]
    public List<RoleInfo>? Roles { get; set; }

    public virtual DepartmentUser? DepartmentUser { get; set; }

    public virtual ICollection<UserRole> UserRole { get; set; } = new List<UserRole>();
}
