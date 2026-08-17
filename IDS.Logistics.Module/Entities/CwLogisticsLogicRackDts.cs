using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 逻辑货架明细信息
/// </summary>
public partial class CwLogisticsLogicRackDts : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public string? RackId { get; set; }

    public string? LogicId { get; set; }
}
