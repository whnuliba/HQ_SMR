using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VBizInfoItem
{
    public string? Id { get; set; } = null!;

    public string? CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? LastModifyTime { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string FieldCode { get; set; } = null!;

    public string FieldName { get; set; } = null!;

    public string? BizId { get; set; }

    public string BizCode { get; set; } = null!;

    public string BizName { get; set; } = null!;

    public string? UserName { get; set; }

    public string? RealName { get; set; }

    public string RoleCode { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string? OrgId { get; set; }

    public int? Scope { get; set; }

    public int? BizStatus { get; set; }

    public string? BizType { get; set; }
}
