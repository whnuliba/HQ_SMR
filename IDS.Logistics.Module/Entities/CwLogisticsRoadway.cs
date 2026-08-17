using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 仓库巷道信息
/// </summary>
public partial class CwLogisticsRoadway : IdsBaseEntity
{
    public string? RoadwayCode { get; set; }

    public string? RoadwayDescription { get; set; }

    public string? WareId { get; set; }

    public string? Administrator { get; set; }
}
