using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

/// <summary>
/// 菜单表
/// </summary>
public partial class BizMenuInfo
{
    public string Id { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string? CreateUser { get; set; }

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public int Status { get; set; }

    public string? MenuRoute { get; set; }

    public string? MenuName { get; set; }

    public string? MenuCode { get; set; }

    public string Pid { get; set; } = null!;

    public int? Sort { get; set; }

    public int MenuType { get; set; }

    public string? TextIcon { get; set; }

    public string? MenuGroup { get; set; }

    public string? Href { get; set; }

    public string? Component { get; set; }

    public string? MenuNameEn { get; set; }

    public string? OrgId { get; set; }

    public string? Scope { get; set; }

    public string? Platform { get; set; }

    public string? Udf1 { get; set; }

    public string? Udf2 { get; set; }

    public string? Udf3 { get; set; }

    public string? Udf4 { get; set; }

    public string? Udf5 { get; set; }

    public string? Udf6 { get; set; }
}
