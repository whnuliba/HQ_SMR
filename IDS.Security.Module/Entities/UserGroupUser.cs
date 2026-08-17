using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class UserGroupUser
{
    public string? Id { get; set; } = null!;

    public string? GroupId { get; set; } = null!;

    public string? UserId { get; set; } = null!;

    public virtual UserGroup? Group { get; set; } = null!;
}
