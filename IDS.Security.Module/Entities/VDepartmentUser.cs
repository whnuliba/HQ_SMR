using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VDepartmentUser
{
    public string? Id { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; } = null!;

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string? UserName { get; set; }

    public string? RealName { get; set; }

    public string? Password { get; set; }

    public int? UseState { get; set; }

    public string? Email { get; set; }

    public int? Sex { get; set; }

    public string? OrgId { get; set; }

    public string? JobId { get; set; }

    public string? Mobile { get; set; }

    public string? DeptId { get; set; }

    public string? DeptCode { get; set; }

    public string? DeptName { get; set; }

    public int? DeptType { get; set; }

    public string? DeptLeader { get; set; }

    public string? DeptLeaderId { get; set; }

    public string? DeptLeaderName { get; set; }

    public string? UserLeader { get; set; }

    public string? UserLeaderId { get; set; }

    public string? UserLeaderName { get; set; }

    public string? OrgCode { get; set; }

    public string? OrgName { get; set; }

    public DateTime? AccountExpireTime { get; set; }
    public DateTime? PasswordExpireTime { get; set; }
    public string? Alias { get; set; }
    public string? Lock { get; set; }
    public string? ChangePassword { get; set; }
    public string? NameSpell { get; set; }
}
