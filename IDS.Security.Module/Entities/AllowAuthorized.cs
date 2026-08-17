using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class AllowAuthorized
{
    public string FuncId { get; set; } = null!;

    public int? AuthSate { get; set; }
}
