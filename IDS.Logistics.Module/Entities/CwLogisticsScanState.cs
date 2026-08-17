using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 扫码状态
/// </summary>
public partial class CwLogisticsScanState : IdsBaseEntity
{
    public int? ScanCode { get; set; }

    public string? ScanName { get; set; }

    public string? ScanNameEn { get; set; }

    public string? ScanDesciption { get; set; }

    public string? ScanDesciptionEn { get; set; }
}
