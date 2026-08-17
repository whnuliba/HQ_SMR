using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VRoleFunction
{
    public string? Id { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleName { get; set; }

    public int? Upd { get; set; }

    public int? Edit { get; set; }

    public int? Del { get; set; }

    public int? Add { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public string? MenuId { get; set; }

    public string? FuncId { get; set; }
}
