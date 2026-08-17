using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 调度参数
/// </summary>
public partial class CwLogisticsSysParameter : IdsBaseEntity
{

    public string? ParamCode { get; set; }

    public string? ParamName { get; set; }

    public string? ParamDescription { get; set; }

    public string? ParamNameEn { get; set; }

    public string? ParamDescriptionEn { get; set; }
}
