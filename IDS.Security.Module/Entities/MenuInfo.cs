using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

/// <summary>
/// 菜单表
/// </summary>
public partial class MenuInfo :AuthBaseEntity
{
    [IdsColumn]
    public string? MenuRoute { get; set; }
    [IdsColumn]
    public string? MenuName { get; set; }
    [IdsColumn]
    public string? MenuCode { get; set; }
    [IdsColumn]
    public string Pid { get; set; } = null!;
    [IdsColumn]
    public int? Sort { get; set; }
    [IdsColumn]
    public int MenuType { get; set; }
    [IdsColumn]
    public string? TextIcon { get; set; }
    [IdsColumn]
    public string? MenuGroup { get; set; }
    [IdsColumn]
    public string? Href { get; set; }
    [IdsColumn]
    public string? Component { get; set; }
    [IdsColumn]
    public string? MenuNameEn { get; set; }
    [IdsColumn]
    public string? OrgId { get; set; }
    [IdsColumn]
    public string? Scope { get; set; }
    [IdsColumn]
    public string? Platform { get; set; }
    [IdsColumn]
    public string? Udf1 { get; set; }
    [IdsColumn]
    public string? Udf2 { get; set; }
    [IdsColumn]
    public string? Udf3 { get; set; }
    [IdsColumn]
    public string? Udf4 { get; set; }
    [IdsColumn]
    public string? Udf5 { get; set; }
    [IdsColumn]
    public string? Udf6 { get; set; }

    public virtual Organization? Org { get; set; }
}
