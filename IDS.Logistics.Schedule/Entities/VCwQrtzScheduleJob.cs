using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDS.Schedule;

public partial class VCwQrtzScheduleJob
{
    public string? Id { get; set; } = null!;

    public DateTime? CreateTime { get; set; }

    public string? CreateUser { get; set; }

    public DateTime? LastModifyTime { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string? ScheduleCode { get; set; } = null!;

    public string? ScheduleName { get; set; }

    public string? ScheduleGrpCode { get; set; } = null!;

    public string? Cron { get; set; }

    public string? JobClass { get; set; }

    public string? ScheduleType { get; set; }

    public string? BusinessCode { get; set; }

    public string? TriggerState { get; set; }

    public long? NextFireTime { get; set; }

    public long? PreFireTime { get; set; }

    public long? StartTime { get; set; }

    public long? EndTime { get; set; }

    [NotMapped]
    public string? NextFireTime1 { get; set; }
    [NotMapped]
    public string? PreFireTime1 { get; set; }
    [NotMapped]
    public string? StartTime1 { get; set; }
    [NotMapped]
    public string? EndTime1 { get; set; }

    public int? Interval { set; get; }
    public string? Parameters { get; set; }
    public string? Ticket { get; set; }
}
