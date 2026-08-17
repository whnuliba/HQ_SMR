using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class SequenceGenerator:IdsBaseEntity
{


    public string? Classification { get; set; }

    public string? Prefix { get; set; }

    public int? Increase { get; set; }
}
