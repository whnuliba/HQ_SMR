using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class UserGroup:AuthBaseEntity
{

    public string? GroupNo { get; set; } = null!;

    public string? GroupName { get; set; } = null!;

    public string? GroupDesc { get; set; }

    public string? OrgId { get; set; } = null!;

    public string? Scope { get; set; } = null!;

    public virtual ICollection<UserGroupRole> UserGroupRole { get; set; } = new List<UserGroupRole>();

    public virtual ICollection<UserGroupUser> UserGroupUser { get; set; } = new List<UserGroupUser>();
}
