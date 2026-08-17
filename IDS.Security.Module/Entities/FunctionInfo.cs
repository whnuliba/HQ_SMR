using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

/// <summary>
/// 功能表
/// </summary>
public partial class FunctionInfo
{
    public string Id { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string CreateUser { get; set; } = null!;

    public DateTime? LastModifyDate { get; set; }

    public string? LastModifyUser { get; set; }

    public int Status { get; set; }

    public string FuncCode { get; set; } = null!;

    public string FuncName { get; set; } = null!;

    public int UseState { get; set; }

    public string? MenuGroup { get; set; }

    public string? Href { get; set; }

    public string? Component { get; set; }

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
