using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 任务路径
/// </summary>
public partial class CwLogisticsTaskRoad : IdsBaseEntity
{
    public string? RoadId { get; set; }

    public int? RoadIndex { get; set; }

    public string? TimerId { get; set; }
    public virtual CwLogisticsTaskTimer? Timer { get; set; }

}
