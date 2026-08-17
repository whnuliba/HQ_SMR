using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class RoleFactory
{
    public string Id { get; set; } = null!;
    [IdsColumn]
    public string RoleId { get; set; } = null!;
    [IdsColumn]
    public string FactoryId { get; set; } = null!;

}
