using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class VOrganizationUser
{
    public string? Id { get; set; } = null!;

    public string? Pid { get; set; } = null!;

    public int? Status { get; set; }

    public string CreateUser { get; set; } = null!;

    public string? Name { get; set; }

    public string? Code { get; set; }

    public int? Sort { get; set; }

    public int DeptType { get; set; }

    public int? Grade { get; set; }

    public string? OrgId { get; set; }

    public string? JobDsc { get; set; }
}
