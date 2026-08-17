using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 物料信息明细
/// </summary>
public partial class CwLogisticsMaterialInfo : IdsBaseEntity
{
    public int? Barcode { get; set; }

    public int? MaterialState { get; set; }

    public int? MaterialPos { get; set; }

    public int? IsDummy { get; set; }

    public string? Attribute1 { get; set; }

    public string? Attribute2 { get; set; }

    public string? Attribute3 { get; set; }

    public string? Attribute4 { get; set; }

    public string? Attribute5 { get; set; }

    public string? Attribute6 { get; set; }

    public string? Attribute7 { get; set; }

    public string? Attribute8 { get; set; }

    public string? Attribute9 { get; set; }

    public string? Attribute10 { get; set; }

    public string? Attribute13 { get; set; }

    public string? Attribute11 { get; set; }

    public string? Attribute12 { get; set; }
}
