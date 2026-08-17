using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 仓库定义信息
/// </summary>
public partial class CwLogisticsWare : IdsBaseEntity
{
    public string? LocationCode { get; set; }

    public string? LocationDescription { get; set; }

    public string? WareTypeId { get; set; }

    public string? Administrator { get; set; }
}
