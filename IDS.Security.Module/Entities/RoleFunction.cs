using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class RoleFunction : AuthBaseEntity
{

    public string? RoleId { get; set; }

    public string? FuncId { get; set; }

    public int? Edit { get; set; }

    public int? Add { get; set; }

    public int? Del { get; set; }

    public int? Upd { get; set; }

    public string? MenuId { get; set; }

    public virtual RoleInfo? Role { get; set; }
}
