using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 物料类型定义
/// </summary>
public partial class CwLogisticsMaterial : IdsBaseEntity
{
    public int? MaterialCode { get; set; }

    public string? MaterialName { get; set; }

    public string? MaterialNameEn { get; set; }

    public string? MaterialDesciption { get; set; }

    public string? MaterialDesciptionEn { get; set; }
}
