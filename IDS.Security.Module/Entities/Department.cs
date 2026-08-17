using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class Department:AuthBaseEntity
{

    public string? DeptName { get; set; }

    public string? DeptCode { get; set; }

    public string Pid { get; set; } = null!;

    public int? Sort { get; set; }

    public int DeptType { get; set; }

    public int? DeptGrade { get; set; }

    public string? OrgId { get; set; }

    public string? JobDsc { get; set; }

    public string? Leader { get; set; }

    public string? LeaderName { get; set; }

    public string? LeaderId { get; set; }

    public virtual ICollection<DepartmentRole> DepartmentRole { get; set; } = new List<DepartmentRole>();

    public virtual ICollection<DepartmentUser> DepartmentUser { get; set; } = new List<DepartmentUser>();

}
