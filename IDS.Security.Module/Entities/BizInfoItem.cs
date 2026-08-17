using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class BizInfoItem
{
    public string Id { get; set; } = null!;

    public string? CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? LastModifyTime { get; set; }

    public string? LastModifyUser { get; set; }
    [IdsColumn]
    public int? Status { get; set; }
    [IdsColumn]
    public string FieldCode { get; set; } = null!;
    [IdsColumn]
    public string FieldName { get; set; } = null!;
    [IdsColumn]
    public string? BizId { get; set; }
}
