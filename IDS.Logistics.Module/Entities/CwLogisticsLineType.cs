using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 物流线类型信息
/// </summary>
public partial class CwLogisticsLineType : IdsBaseEntity
{
    public string? LineTypeCode { get; set; }

    public string? LineTypeDescription { get; set; }

    public int? LineTypeState { get; set; }
}
