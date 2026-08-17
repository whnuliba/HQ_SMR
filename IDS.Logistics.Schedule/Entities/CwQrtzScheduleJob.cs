using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Schedule;

public partial class CwQrtzScheduleJob : IdsBaseEntity
{
    [IdsColumn]
    public string? ScheduleCode { get; set; } = null!;
    [IdsColumn]
    public string? ScheduleName { get; set; }
    [IdsColumn]
    public string? ScheduleGrpCode { get; set; } = null!;
    [IdsColumn]
    public string? Cron { get; set; }
    [IdsColumn]
    public string? JobClass { get; set; }
    [IdsColumn]
    public string? ScheduleType { get; set; }
    [IdsColumn]
    public string? BusinessCode { get; set; }
    [IdsColumn]
    public int? Interval { set; get; }
    [IdsColumn]
    public string? Parameters { get; set; }
    [IdsColumn]
    public string? Ticket { get; set; }
}
