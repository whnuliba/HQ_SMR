using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 载具信息
/// </summary>
public partial class CwLogisticsCarrierInfo : IdsBaseEntity
{

    public string? CarrierCode { get; set; }

    public string? LocationCode { get; set; }

    public int? ProcessCode { get; set; }

    public int? LocationType { get; set; }

    public int? LoadState { get; set; }

    public int? MaterialCode { get; set; }

    public int? ToolingId { get; set; }

    public string? Marking { get; set; }

    public int? CarrierCmd { get; set; }

    public int? MoveState { get; set; }
}
