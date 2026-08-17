using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class DepartmentUser: AuthBaseEntity
{
    [IdsColumn]

    public string UserId { get; set; } = null!;
    [IdsColumn]
    public string DeptId { get; set; } = null!;
    [IdsColumn]
    public int? MainJob { get; set; }

    public virtual Department Dept { get; set; } = null!;

    public virtual UserInfo User { get; set; } = null!;

}
