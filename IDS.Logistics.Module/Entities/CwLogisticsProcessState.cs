using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 物流工艺状态
/// </summary>
public partial class CwLogisticsProcessState : IdsBaseEntity
{

    public int? ProcessCode { get; set; }

    public string? ProcessName { get; set; }

    public string? ProcessNameEn { get; set; }
}
