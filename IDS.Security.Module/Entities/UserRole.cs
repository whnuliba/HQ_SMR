using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

/// <summary>
/// 用户角色表
/// </summary>
public partial class UserRole: AuthBaseEntity
{
    [IdsColumn]
    public string UserId { get; set; } = null!;
    [IdsColumn]
    public string RoleId { get; set; } = null!;

    /// <summary>
    /// 0 角色 1 角色组
    /// </summary>
    [IdsColumn]
    public int? RoleType { get; set; }

    public virtual UserInfo? User { get; set; } = null!;
}
