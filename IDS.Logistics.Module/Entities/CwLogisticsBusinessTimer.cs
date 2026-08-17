using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 寻路任务表定时器
/// </summary>
public partial class CwLogisticsBusinessTimer : IdsBaseEntity
{
    public string? JobService { get; set; }

    public int? UseState { get; set; }

    public int? Time { get; set; }

    public string? Cron { get; set; }

    public string? Mutex { get; set; }
}
