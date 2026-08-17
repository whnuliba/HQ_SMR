using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 货架状态
/// </summary>
public partial class CwLogisticsRackState : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public int? LoadState { get; set; }

    public int? AutoState { get; set; }

    public int? FireState { get; set; }

    public int? UseState { get; set; }

    public int? CloseState { get; set; }

    public decimal? Temperature { get; set; }

    public int? FullLocked { get; set; }

    public int? ProcessCode { get; set; }

    public int? SampleState { get; set; }

    public DateTime? SampleOTime { get; set; }

    public DateTime? SampleITime { get; set; }

    public string? Sampler { get; set; }

    public DateTime? InTime { get; set; }

    public DateTime? PlanOutTime { get; set; }
}
