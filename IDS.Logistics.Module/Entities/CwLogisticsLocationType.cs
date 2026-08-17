using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

public partial class CwLogisticsLocationType : IdsBaseEntity
{
    public int? LocationType { get; set; }

    public string? LocationName { get; set; }

    public string? LocationNameEn { get; set; }

    public string? LocationDesciption { get; set; }

    public string? LocationDesciptionEn { get; set; }
}
