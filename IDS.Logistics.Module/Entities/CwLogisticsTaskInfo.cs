using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 任务表
/// </summary>
public partial class CwLogisticsTaskInfo : IdsBaseEntity
{
    public int? TaskNumber { get; set; }

    public string? RoadId { get; set; }

    public int? TaskState { get; set; }

    public int? SendState { get; set; }

    public int? CompleteState { get; set; }

    public DateTime? SendTime { get; set; }

    public DateTime? CompleteTime { get; set; }

    public string? CarrierId { get; set; }

    public string? CarrierCode { get; set; }

    public int? Priority { get; set; }

    public int? FromCmd1 { get; set; }

    public int? FromCmd2 { get; set; }

    public int? ToCmd1 { get; set; }

    public int? ToCmd2 { get; set; }

    public string? FromLoactionCode { get; set; }

    public string? ToLocationCode { get; set; }

    public int? Fork { get; set; }

    public string? TrakGroupId { get; set; }

    public int? TaskGroupState { get; set; }
}
