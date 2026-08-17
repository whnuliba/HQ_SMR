using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 逻辑货架信息
/// </summary>
public partial class CwLogisticsLogicRack : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public int? LocationCmd1 { get; set; }

    public int? LocationCmd2 { get; set; }

    public int? X { get; set; }

    public int? Y { get; set; }

    public int? Z { get; set; }

    public int? Deep { get; set; }

    public int? Fork { get; set; }

    public string? BoxCode { get; set; }

    public string? RoadwayId { get; set; }

    public int? RackDirect { get; set; }

    public string? LocatonDirect { get; set; }

    public int? CarrierType { get; set; }
}
