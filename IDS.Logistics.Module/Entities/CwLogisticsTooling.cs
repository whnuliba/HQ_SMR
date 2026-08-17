using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 工装载具定义
/// </summary>
public partial class CwLogisticsTooling : IdsBaseEntity
{

    public string? ToolingCode { get; set; }

    public int? ToolingCmd { get; set; }

    public string? ToolingDescription { get; set; }

    public int? UseState { get; set; }
}
