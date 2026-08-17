using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 线体组信息
/// </summary>
public partial class CwLogisticsLineGroup : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public int? LocationCmd1 { get; set; }

    public int? LocationCmd2 { get; set; }

    public int? Deep { get; set; }

    public int? Fork { get; set; }

    public string? BoxCode { get; set; }

    public string? RoadwayId { get; set; }

    public int? RoadwayDirect { get; set; }

    public string? LineDirect { get; set; }

    public int? CarrierType { get; set; }

    public string? LineTypeId { get; set; }
}
