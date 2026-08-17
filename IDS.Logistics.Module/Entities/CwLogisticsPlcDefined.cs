using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// PLC位置定义
/// </summary>
public partial class CwLogisticsPlcDefined:IdsBaseEntity
{
    public string? IpAddr { get; set; }

    public int? IpPort { get; set; }

    public int? DbNum { get; set; }

    public int? StartAddress { get; set; }

    public int? Offset { get; set; }

    public string? Protocol { get; set; }

    public string? BusinessType { get; set; }

    public string? Parameter { get; set; }

    public string? PlcVersion { get; set; }

    public int? AreaCode { get; set; }

    public string? AreaName { get; set; }

    public int? LocationType { get; set; }
}
