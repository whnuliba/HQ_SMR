using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 监控图
/// </summary>
public partial class CwLogisticsMonitor : IdsBaseEntity
{
    /// <summary>
    /// MONITOR_CODE
    /// </summary>
    public int? MonitorCode { get; set; }

    /// <summary>
    /// MONITOR_NAME
    /// </summary>
    public string? MonitorName { get; set; }

    /// <summary>
    /// GRAPH
    /// </summary>
    public string? Diagram { get; set; }

    public string? Parameters { get; set; }
}
