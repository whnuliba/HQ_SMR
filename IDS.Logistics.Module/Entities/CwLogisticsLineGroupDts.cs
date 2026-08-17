using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 线体组明细
/// </summary>
public partial class CwLogisticsLineGroupDts : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public string? LocationId { get; set; }

    public string? GroupId { get; set; }
}
