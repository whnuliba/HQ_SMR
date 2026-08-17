using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class BizInfo
{
    public string Id { get; set; } = null!;

    public string? CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? LastModifyUser { get; set; }

    public DateTime? LastModifyTime { get; set; }
    [IdsColumn]
    public int? Status { get; set; }
    [IdsColumn]
    public string? BizComment { get; set; }
    [IdsColumn]
    public string RoleId { get; set; } = null!;
    [IdsColumn]
    public string RoleCode { get; set; } = null!;
    [IdsColumn]
    public string BizCode { get; set; } = null!;
    [IdsColumn]
    public string BizName { get; set; } = null!;
    [IdsColumn]
    public string? OrgId { get; set; }
    [IdsColumn]
    public int? Scope { get; set; }
    [IdsColumn]
    public string? BizType { get; set; }
}
