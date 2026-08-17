using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 仓库类型定义信息
/// </summary>
public partial class CwLogisticsWareType : IdsBaseEntity
{
    public string? WareTypeCode { get; set; }

    public string? WareTypeDescription { get; set; }

    public int? WareTypeState { get; set; }
}
