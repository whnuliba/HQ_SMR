using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 托盘信息
/// </summary>
public partial class CwLogisticsTrayInfo : IdsBaseEntity
{
    public string? TrayCode { get; set; }

    public string? TrayIndex { get; set; }

    public int? ProcessCode { get; set; }

    public int? LocationType { get; set; }

    public int? LoadState { get; set; }

    public int? MaterialCode { get; set; }

    public int? CarrierCmd { get; set; }

    public int? CarrierId { get; set; }

    public int? MoveState { get; set; }
}
