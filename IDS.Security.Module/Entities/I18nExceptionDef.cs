using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class I18nExceptionDef
{
    public string? Id { get; set; } = null!;
    [IdsColumn]
    public string? En { get; set; }
    [IdsColumn]
    public string? ExceptionCode { get; set; } = null!;
    [IdsColumn]
    public string? Fre { get; set; }
    [IdsColumn]
    public string? Poland { get; set; }
    [IdsColumn]
    public string? Korea { get; set; }
    [IdsColumn]
    public string? Japan { get; set; }
    [IdsColumn]
    public string? Ger { get; set; }
    [IdsColumn]
    public string? Vn { get; set; }
    [IdsColumn]
    public string? Zh { get; set; }
    [IdsColumn]
    public string? Ln1 { get; set; }
    [IdsColumn]
    public string? Ln2 { get; set; }
    [IdsColumn]
    public string? Ln3 { get; set; }
    [IdsColumn]
    public DateTime? CreateTime { get; set; }
    [IdsColumn]
    public string? Ch { get; set; }
    [IdsColumn]
    public string? AppCode { get; set; }
}
