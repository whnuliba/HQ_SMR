using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VUserRoleFunction
{
    public DateTime? CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public string? FuncCode { get; set; }

    public string? FuncName { get; set; }

    public string? MenuNameEn { get; set; }

    public string? Id { get; set; } = null!;

    public int Status { get; set; }

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public string Pid { get; set; } = null!;

    public string? MenuRoute { get; set; }

    public int? Sort { get; set; }

    public int MenuType { get; set; }

    public string? MenuGroup { get; set; }

    public string? TextIcon { get; set; }

    public string? Component { get; set; }

    public string? Href { get; set; }

    public string? OrgId { get; set; }

    public string? Scope { get; set; }

    public string? Platform { get; set; }

    public string? Udf1 { get; set; }

    public string? Udf2 { get; set; }

    public string? Udf3 { get; set; }

    public string? Udf4 { get; set; }

    public string? Udf5 { get; set; }

    public string? Udf6 { get; set; }

    public string? RoleCode { get; set; }

    public string RoleId { get; set; } = null!;

    public string? RoleName { get; set; }

    public int? RoleStatus { get; set; }

    public string? RealName { get; set; }

    public int? UserUseState { get; set; }

    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public int? UserState { get; set; }

    public int? Del { get; set; }

    public int? Edit { get; set; }

    public int? Add { get; set; }

    public int? Upd { get; set; }
}
