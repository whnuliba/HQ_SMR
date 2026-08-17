using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 寻路任务表定时器
/// </summary>
public partial class CwLogisticsTaskTimer : IdsBaseEntity
{
    public string? TaskService { get; set; }

    public int? UseState { get; set; }

    public int? Time { get; set; }

    public string? Cron { get; set; }

    public string? Mutex { get; set; }
    public virtual ICollection<CwLogisticsTaskOption> CwLogisticsTaskOption { get; set; } = new List<CwLogisticsTaskOption>();

    public virtual ICollection<CwLogisticsTaskRoad> CwLogisticsTaskRoad { get; set; } = new List<CwLogisticsTaskRoad>();
}
