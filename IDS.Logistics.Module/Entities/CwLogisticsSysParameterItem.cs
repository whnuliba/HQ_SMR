using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 调度参数
/// </summary>
public partial class CwLogisticsSysParameterItem : IdsBaseEntity
{
    public string? ItemCode { get; set; }

    public string? ItemName { get; set; }

    public string? ItemDescription { get; set; }

    public string? ItemNameEn { get; set; }

    public string? ItemDescriptionEn { get; set; }

    public string? ItemValue { get; set; }

    public string? ParamId { get; set; }
}
