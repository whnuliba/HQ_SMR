using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 任务选择传递
/// </summary>
public partial class CwLogisticsTaskOption : IdsBaseEntity
{
    public int? RoadIndex { get; set; }

    public string? ServiceName { get; set; }

    public string? TimerId { get; set; }
    public virtual CwLogisticsTaskTimer? Timer { get; set; }

}
